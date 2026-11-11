using System.Collections.Immutable;
using System.Composition;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Editing;
using Microsoft.CodeAnalysis.Formatting;
using Microsoft.CodeAnalysis.Operations;
using Microsoft.CodeAnalysis.Simplification;
using Rocket.Surgery.Airframe.Analyzers;

namespace Rocket.Surgery.Airframe.CodeFixes.Performance;

/// <summary>
/// Represents a code fix for <see cref="Descriptions.RSA3006"/>.
/// </summary>
/// <remarks>
/// Rewrites <c>Observable.FromEvent</c> / <c>Observable.FromEventPattern</c> to an <c>Observable.Create</c> that adds the
/// handler on subscribe and removes it on dispose. No fix is offered when the rewrite cannot be proven equivalent:
/// scheduler overloads (the scheduler controls where the handler is added and removed), add/remove handlers that are not
/// lambdas, conversions whose shape is not recognized, and reflection overloads whose event cannot be resolved statically.
/// </remarks>
[Shared]
[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(Rsa3006Fix))]
public class Rsa3006Fix : CodeFixProvider
{
    private const string Title = "Use Observable.Create";

    private static readonly SymbolDisplayFormat NullableFullyQualifiedFormat =
        SymbolDisplayFormat.FullyQualifiedFormat.AddMiscellaneousOptions(SymbolDisplayMiscellaneousOptions.IncludeNullableReferenceTypeModifier);

    /// <inheritdoc/>
    public sealed override ImmutableArray<string> FixableDiagnosticIds { get; } = [RSA3006.Id];

    /// <inheritdoc/>
    public sealed override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

    /// <inheritdoc/>
    public override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
        var semanticModel = await context.Document.GetSemanticModelAsync(context.CancellationToken).ConfigureAwait(false);

        if (root is null || semanticModel is null)
        {
            return;
        }

        foreach (var diagnostic in context.Diagnostics.Where(diagnostic => diagnostic.Id == RSA3006.Id))
        {
            var invocation = root
               .FindNode(diagnostic.Location.SourceSpan, getInnermostNodeForTie: true)
               .FirstAncestorOrSelf<InvocationExpressionSyntax>();

            if (invocation is null)
            {
                continue;
            }

            var replacement = new ReplacementBuilder(invocation, semanticModel, context.CancellationToken).Build();
            if (replacement is null)
            {
                continue;
            }

            context.RegisterCodeFix(
                CodeAction.Create(
                    title: Title,
                    createChangedDocument: cancellationToken => ApplyAsync(context.Document, invocation, replacement, cancellationToken),
                    equivalenceKey: nameof(Rsa3006Fix)),
                diagnostic);
        }
    }

    private static async Task<Document> ApplyAsync(
        Document document,
        InvocationExpressionSyntax invocation,
        ExpressionSyntax replacement,
        CancellationToken cancellationToken)
    {
        var editor = await DocumentEditor.CreateAsync(document, cancellationToken).ConfigureAwait(false);
        editor.ReplaceNode(invocation, replacement);

        var changed = editor.GetChangedDocument();
        changed = await ImportAdder.AddImportsAsync(changed, Simplifier.AddImportsAnnotation, cancellationToken: cancellationToken).ConfigureAwait(false);
        changed = await Simplifier.ReduceAsync(changed, Simplifier.Annotation, cancellationToken: cancellationToken).ConfigureAwait(false);
        return await Formatter.FormatAsync(changed, Formatter.Annotation, cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Builds the <c>Observable.Create</c> replacement for one invocation, or <see langword="null"/> when it cannot.
    /// </summary>
    private sealed class ReplacementBuilder
    {
        private readonly InvocationExpressionSyntax _invocation;
        private readonly SemanticModel _semanticModel;
        private readonly CancellationToken _cancellationToken;
        private readonly HashSet<string> _taken;
        private readonly HashSet<string> _generated = new(StringComparer.Ordinal);
        private readonly bool _allowsShadowing;
        private readonly SymbolDisplayFormat _format;
        private readonly string _indentation;

        public ReplacementBuilder(InvocationExpressionSyntax invocation, SemanticModel semanticModel, CancellationToken cancellationToken)
        {
            _invocation = invocation;
            _semanticModel = semanticModel;
            _cancellationToken = cancellationToken;

            // Every identifier in the enclosing member is off limits for names the rewrite introduces into the scope of
            // the user's add/remove code, so the moved code can never bind to a generated name by accident.
            var scope = invocation.Ancestors().FirstOrDefault(node => node is MemberDeclarationSyntax and not GlobalStatementSyntax) ??
                        invocation.SyntaxTree.GetRoot(cancellationToken);

            // The parameters of the lambdas passed to FromEvent are replaced by the rewrite, so their names are free.
            var replacedParameters = new HashSet<string>(
                invocation.ArgumentList.Arguments
                   .Select(argument => argument.Expression)
                   .OfType<LambdaExpressionSyntax>()
                   .SelectMany(lambda => lambda switch
                    {
                        SimpleLambdaExpressionSyntax simple => [simple.Parameter],
                        ParenthesizedLambdaExpressionSyntax parenthesized => parenthesized.ParameterList.Parameters,
                        var _ => Enumerable.Empty<ParameterSyntax>(),
                    })
                   .Select(parameter => parameter.Identifier.ValueText),
                StringComparer.Ordinal);

            _taken = new HashSet<string>(
                scope
                   .DescendantTokens()
                   .Where(token => token.IsKind(SyntaxKind.IdentifierToken))
                   .Where(token => !invocation.Span.Contains(token.Span) || !replacedParameters.Contains(token.ValueText))
                   .Select(token => token.ValueText),
                StringComparer.Ordinal);

            var text = invocation.SyntaxTree.GetText(cancellationToken);
            var line = text.Lines.GetLineFromPosition(invocation.SpanStart).ToString();
            _indentation = line.Substring(0, line.Length - line.TrimStart().Length);

            _allowsShadowing = invocation.SyntaxTree.Options is CSharpParseOptions { LanguageVersion: >= LanguageVersion.CSharp8 };
            _format = semanticModel.GetNullableContext(invocation.SpanStart).AnnotationsEnabled()
                ? NullableFullyQualifiedFormat
                : SymbolDisplayFormat.FullyQualifiedFormat;
        }

        private bool IsPattern { get; set; }

        private ITypeSymbol ElementType { get; set; } = null!;

        private string Observer { get; set; } = null!;

        public ExpressionSyntax? Build()
        {
            if (!ObservableFromEventSymbols.TryGetFromEventMethod(_invocation, _semanticModel, out var method) ||
                ObservableFromEventSymbols.HasScheduler(method) ||
                method.ReturnType is not INamedTypeSymbol { TypeArguments.Length: 1 } observableType ||
                _semanticModel.GetOperation(_invocation, _cancellationToken) is not IInvocationOperation operation)
            {
                return null;
            }

            IsPattern = method.Name == ObservableFromEventSymbols.FromEventPattern;
            ElementType = observableType.TypeArguments[0];
            Observer = Reserve("observer");

            var statements = new List<string>();
            string? removeLambda;

            if (ObservableFromEventSymbols.IsReflectionOverload(method))
            {
                if (!TryResolveEventAccess(operation, out var eventAccess, out var delegateType) ||
                    !TryCreateSignatureHandler(delegateType, statements, out var handlerName))
                {
                    return null;
                }

                statements.Add($"{eventAccess} += {handlerName};");
                removeLambda = $"() => {eventAccess} -= {handlerName}";
            }
            else
            {
                var addHandler = GetArgument(operation, "addHandler");
                var removeHandler = GetArgument(operation, "removeHandler");
                var delegateType = method.Parameters.FirstOrDefault(parameter => parameter.Name == "addHandler")?.Type is INamedTypeSymbol { TypeArguments.Length: 1 } action
                    ? action.TypeArguments[0] as INamedTypeSymbol
                    : null;

                if (addHandler is null || removeHandler is null || delegateType is null)
                {
                    return null;
                }

                var conversion = GetArgument(operation, "conversion");
                string? handlerName;
                bool isLocalFunction;

                if (conversion is not null)
                {
                    if (!TryCreateConversionHandler(conversion, delegateType, statements, out handlerName, out isLocalFunction))
                    {
                        return null;
                    }
                }
                else
                {
                    if (!TryCreateSignatureHandler(delegateType, statements, out handlerName))
                    {
                        return null;
                    }

                    isLocalFunction = true;
                }

                if (isLocalFunction && (!OnlyUsedAsEventOperand(addHandler) || !OnlyUsedAsEventOperand(removeHandler)))
                {
                    // A local function is a method group; outside `+=`/`-=` it may convert differently from the original
                    // delegate type, so hand the user's code a local of exactly that type instead.
                    var alias = Reserve("handler");
                    statements.Add($"{Render(delegateType)} {alias} = {handlerName};");
                    handlerName = alias;
                }

                if (!TryInlineAddHandler(addHandler, handlerName, statements) ||
                    (removeLambda = InlineRemoveHandler(removeHandler, handlerName)) is null)
                {
                    return null;
                }
            }

            var text = new StringBuilder()
               .Append("global::System.Reactive.Linq.Observable.Create<").Append(Render(ElementType)).Append(">(")
               .Append(Observer).Append(" =>\n")
               .Append(_indentation).Append("{\n");

            // The formatter indents the lambda body relative to its braces but leaves the braces where they are, so the
            // braces start at the indentation of the line the original call was on.
            var bodyIndentation = _indentation + "    ";
            foreach (var statement in statements)
            {
                text.Append(bodyIndentation).Append(statement).Append('\n');
            }

            text
               .Append(bodyIndentation).Append("return global::System.Reactive.Disposables.Disposable.Create(").Append(removeLambda).Append(");\n")
               .Append(_indentation).Append("})");

            var expression = ParseExpression(text.ToString());
            if (expression.ContainsDiagnostics)
            {
                return null;
            }

            return AnnotateGeneratedNames(expression)
               .WithTriviaFrom(_invocation)
               .WithAdditionalAnnotations(Formatter.Annotation);
        }

        private static ExpressionSyntax? GetArgument(IInvocationOperation operation, string parameterName) =>
            operation.Arguments.FirstOrDefault(argument => argument.Parameter?.Name == parameterName)?.Syntax is ArgumentSyntax argument
                ? argument.Expression
                : null;

        private static bool TryGetSingleParameterLambda(ExpressionSyntax expression, out LambdaExpressionSyntax lambda, out ParameterSyntax parameter)
        {
            (lambda, parameter) = expression switch
            {
                SimpleLambdaExpressionSyntax simple => (simple, simple.Parameter),
                ParenthesizedLambdaExpressionSyntax { ParameterList.Parameters.Count: 1 } parenthesized =>
                    ((LambdaExpressionSyntax)parenthesized, parenthesized.ParameterList.Parameters[0]),
                var _ => (null!, null!),
            };

            return lambda is not null && lambda.AsyncKeyword.IsKind(SyntaxKind.None);
        }

        /// <summary>
        /// Whether <paramref name="body"/> has a <c>return</c> of its own (one not inside a nested function).
        /// </summary>
        private static bool HasOwnReturn(SyntaxNode body) =>
            body
               .DescendantNodes(node => node == body || node is not (AnonymousFunctionExpressionSyntax or LocalFunctionStatementSyntax))
               .OfType<ReturnStatementSyntax>()
               .Any();

        private static ExpressionSyntax AnnotateGeneratedNames(ExpressionSyntax expression)
        {
            var roots = expression
               .DescendantNodes()
               .OfType<AliasQualifiedNameSyntax>()
               .Where(alias => alias.Alias.Identifier.IsKind(SyntaxKind.GlobalKeyword))
               .Select(alias =>
                {
                    SyntaxNode top = alias;
                    while ((top.Parent is QualifiedNameSyntax qualified && qualified.Left == top) ||
                           (top.Parent is MemberAccessExpressionSyntax memberAccess && memberAccess.Expression == top))
                    {
                        top = top.Parent;
                    }

                    return top;
                })
               .ToList();

            return expression.ReplaceNodes(
                roots,
                (_, rewritten) => rewritten.WithAdditionalAnnotations(Simplifier.Annotation, Simplifier.AddImportsAnnotation));
        }

        private static string Text(SyntaxNode node) => node.ToFullString().Trim();

        private static string RefKindPrefix(IParameterSymbol parameter) => parameter.RefKind switch
        {
            RefKind.Ref => "ref ",
            RefKind.Out => "out ",
            RefKind.In => "in ",
            var _ => string.Empty,
        };

        private static bool IsStableReceiver(ExpressionSyntax expression) => expression switch
        {
            ThisExpressionSyntax or BaseExpressionSyntax or IdentifierNameSyntax => true,
            MemberAccessExpressionSyntax { RawKind: (int)SyntaxKind.SimpleMemberAccessExpression } memberAccess => IsStableReceiver(memberAccess.Expression),
            ParenthesizedExpressionSyntax parenthesized => IsStableReceiver(parenthesized.Expression),
            var _ => false,
        };

        private string Render(ITypeSymbol type) => type.ToDisplayString(_format);

        /// <summary>
        /// Reserves a name visible to the user's moved add/remove code: it must not clash with anything in the member.
        /// </summary>
        private string Reserve(string preferred) => Reserve(preferred, isolated: false);

        /// <summary>
        /// Reserves a name. An <paramref name="isolated"/> name (a generated handler's parameter) is only visible inside
        /// the generated handler, so from C# 8 on it may shadow outer names and only has to avoid the generated ones.
        /// </summary>
        private string Reserve(string preferred, bool isolated)
        {
            var checkOuterScope = !isolated || !_allowsShadowing;
            var name = preferred;
            for (var suffix = 1;
                 _generated.Contains(name) ||
                 (checkOuterScope && (_taken.Contains(name) || _semanticModel.LookupSymbols(_invocation.SpanStart, name: name).Length > 0));
                 suffix++)
            {
                name = preferred + suffix;
            }

            _generated.Add(name);
            return name;
        }

        /// <summary>
        /// Builds a handler from the delegate's signature: <c>void Handler(params) =&gt; observer.OnNext(payload);</c>.
        /// </summary>
        private bool TryCreateSignatureHandler(INamedTypeSymbol delegateType, List<string> statements, out string handlerName)
        {
            handlerName = null!;

            if (delegateType.DelegateInvokeMethod is not { ReturnsVoid: true } invoke)
            {
                return false;
            }

            var isAction = delegateType is { Name: "Action" } && delegateType.ContainingNamespace?.ToDisplayString() == "System";
            var names = invoke.Parameters
               .Select((parameter, index) =>
                    Reserve(
                        isAction ? (invoke.Parameters.Length == 1 ? "args" : $"arg{index + 1}") : parameter.Name,
                        isolated: true))
               .ToList();

            var payload = IsPattern ? CreatePatternPayload(invoke, names) : CreateEventPayload(invoke, names);
            if (payload is null)
            {
                return false;
            }

            handlerName = Reserve("Handler");
            var parameters = string.Join(", ", invoke.Parameters.Select((parameter, index) => $"{RefKindPrefix(parameter)}{Render(parameter.Type)} {names[index]}"));
            statements.Add($"void {handlerName}({parameters}) => {Observer}.OnNext({payload});");
            return true;
        }

        private string? CreateEventPayload(IMethodSymbol invoke, IReadOnlyList<string> names)
        {
            if (invoke.Parameters.Length == 0)
            {
                return ElementType is { Name: "Unit" } && ElementType.ContainingNamespace?.ToDisplayString() == "System.Reactive"
                    ? "global::System.Reactive.Unit.Default"
                    : null;
            }

            return invoke.Parameters.Length == 1 && invoke.Parameters[0].RefKind == RefKind.None
                ? ConvertTo(names[0], invoke.Parameters[0].Type, ElementType)
                : null;
        }

        private string? CreatePatternPayload(IMethodSymbol invoke, IReadOnlyList<string> names)
        {
            if (invoke.Parameters.Length != 2 ||
                invoke.Parameters.Any(parameter => parameter.RefKind != RefKind.None) ||
                ElementType is not INamedTypeSymbol { Name: "EventPattern", TypeArguments.Length: 1 or 2 } pattern)
            {
                return null;
            }

            var senderType = pattern.TypeArguments.Length == 2
                ? pattern.TypeArguments[0]
                : _semanticModel.Compilation.GetSpecialType(SpecialType.System_Object);
            var argsType = pattern.TypeArguments[pattern.TypeArguments.Length - 1];

            var sender = ConvertTo(names[0], invoke.Parameters[0].Type, senderType);
            var args = ConvertTo(names[1], invoke.Parameters[1].Type, argsType);

            return sender is null || args is null ? null : $"new {Render(pattern)}({sender}, {args})";
        }

        /// <summary>
        /// Renders <paramref name="name"/> converted to <paramref name="target"/>: as is when implicit, cast when explicit.
        /// </summary>
        private string? ConvertTo(string name, ITypeSymbol source, ITypeSymbol target)
        {
            var conversion = _semanticModel.Compilation.ClassifyConversion(source, target);
            if (conversion.IsImplicit)
            {
                return name;
            }

            return conversion.Exists ? $"({Render(target)}){name}" : null;
        }

        /// <summary>
        /// Builds the handler from the user's own conversion lambda by replacing calls to its handler parameter with
        /// <c>observer.OnNext(...)</c>. That keeps whatever mapping the conversion performed.
        /// </summary>
        private bool TryCreateConversionHandler(
            ExpressionSyntax conversion,
            INamedTypeSymbol delegateType,
            List<string> statements,
            out string handlerName,
            out bool isLocalFunction)
        {
            handlerName = null!;
            isLocalFunction = false;

            if (!TryGetSingleParameterLambda(conversion, out var lambda, out var parameterSyntax) ||
                _semanticModel.GetDeclaredSymbol(parameterSyntax, _cancellationToken) is not { } parameter)
            {
                return false;
            }

            var replacements = new Dictionary<SyntaxNode, Func<SyntaxNode, SyntaxNode?>>();
            foreach (var reference in References(lambda.Body, parameter))
            {
                var invocation = reference.Parent switch
                {
                    InvocationExpressionSyntax direct when direct.Expression == reference => direct,
                    MemberAccessExpressionSyntax { Name.Identifier.ValueText: "Invoke", Parent: InvocationExpressionSyntax viaInvoke } memberAccess
                        when memberAccess.Expression == reference => viaInvoke,
                    var _ => null,
                };

                if (invocation is not null)
                {
                    replacements[invocation] = rewritten => CreateOnNext(((InvocationExpressionSyntax)rewritten).ArgumentList);
                }
                else if (!IsPattern)
                {
                    // `Action<TEventArgs>` used as a value; `observer.OnNext` is the same method group.
                    replacements[reference] = rewritten => ParseExpression($"{Observer}.OnNext").WithTriviaFrom(rewritten);
                }
                else
                {
                    return false;
                }
            }

            var failed = false;
            var body = lambda.Body.ReplaceNodes(
                replacements.Keys,
                (original, rewritten) =>
                {
                    var replaced = replacements[original](rewritten);
                    failed |= replaced is null;
                    return replaced ?? rewritten;
                });

            if (failed)
            {
                return false;
            }

            if (body is ExpressionSyntax expressionBody)
            {
                handlerName = Reserve("handler");
                statements.Add($"{Render(delegateType)} {handlerName} = {Text(expressionBody)};");
                return true;
            }

            if (body is not BlockSyntax { Statements: { Count: > 0 } blockStatements } ||
                blockStatements.Last() is not ReturnStatementSyntax { Expression: { } returned } ||
                HasOwnReturn(SyntaxFactory.Block(blockStatements.RemoveAt(blockStatements.Count - 1))))
            {
                return false;
            }

            if (blockStatements.Count == 2 &&
                blockStatements[0] is LocalFunctionStatementSyntax localFunction &&
                returned is IdentifierNameSyntax { Identifier.ValueText: var returnedName } &&
                returnedName == localFunction.Identifier.ValueText)
            {
                statements.Add(Text(localFunction));
                handlerName = returnedName;
                isLocalFunction = true;
                return true;
            }

            statements.AddRange(blockStatements.Take(blockStatements.Count - 1).Select(Text));
            handlerName = Reserve("handler");
            statements.Add($"{Render(delegateType)} {handlerName} = {Text(returned)};");
            return true;
        }

        private SyntaxNode? CreateOnNext(ArgumentListSyntax arguments)
        {
            string? payload;
            if (IsPattern)
            {
                payload = arguments.Arguments.Count == 2
                    ? $"new {Render(ElementType)}({Text(arguments.Arguments[0])}, {Text(arguments.Arguments[1])})"
                    : null;
            }
            else
            {
                payload = arguments.Arguments.Count == 1 ? Text(arguments.Arguments[0]) : null;
            }

            return payload is null ? null : ParseExpression($"{Observer}.OnNext({payload})");
        }

        private IEnumerable<IdentifierNameSyntax> References(SyntaxNode body, ISymbol symbol) =>
            body
               .DescendantNodesAndSelf()
               .OfType<IdentifierNameSyntax>()
               .Where(identifier =>
                    identifier.Identifier.ValueText == symbol.Name &&
                    SymbolEqualityComparer.Default.Equals(_semanticModel.GetSymbolInfo(identifier, _cancellationToken).Symbol, symbol))
               .ToList();

        private bool OnlyUsedAsEventOperand(ExpressionSyntax handler)
        {
            if (!TryGetSingleParameterLambda(handler, out var lambda, out var parameterSyntax) ||
                _semanticModel.GetDeclaredSymbol(parameterSyntax, _cancellationToken) is not { } parameter)
            {
                return true;
            }

            return References(lambda.Body, parameter).All(reference =>
                reference.Parent is AssignmentExpressionSyntax assignment &&
                assignment.Right == reference &&
                assignment.Kind() is SyntaxKind.AddAssignmentExpression or SyntaxKind.SubtractAssignmentExpression &&
                _semanticModel.GetSymbolInfo(assignment.Left, _cancellationToken).Symbol is IEventSymbol);
        }

        private SyntaxNode? SubstituteParameter(ExpressionSyntax handler, string handlerName)
        {
            if (!TryGetSingleParameterLambda(handler, out var lambda, out var parameterSyntax) ||
                _semanticModel.GetDeclaredSymbol(parameterSyntax, _cancellationToken) is not { } parameter)
            {
                return null;
            }

            return lambda.Body.ReplaceNodes(
                References(lambda.Body, parameter),
                (_, rewritten) => IdentifierName(handlerName).WithTriviaFrom(rewritten));
        }

        private bool TryInlineAddHandler(ExpressionSyntax addHandler, string handlerName, List<string> statements)
        {
            switch (SubstituteParameter(addHandler, handlerName))
            {
                case ExpressionSyntax expression:
                    statements.Add($"{Text(expression)};");
                    return true;
                case BlockSyntax block when !HasOwnReturn(block):
                    statements.AddRange(block.Statements.Select(Text));
                    return true;
                default:
                    return false;
            }
        }

        private string? InlineRemoveHandler(ExpressionSyntax removeHandler, string handlerName) =>
            SubstituteParameter(removeHandler, handlerName) switch
            {
                ExpressionSyntax expression => $"() => {Text(expression)}",
                BlockSyntax block => $"() => {Text(block)}",
                var _ => null,
            };

        /// <summary>
        /// Resolves the event a reflection overload names, as <c>target.Event</c> or <c>Type.Event</c>.
        /// </summary>
        private bool TryResolveEventAccess(IInvocationOperation operation, out string eventAccess, out INamedTypeSymbol delegateType)
        {
            eventAccess = null!;
            delegateType = null!;

            if (GetArgument(operation, "eventName") is not { } eventNameArgument ||
                _semanticModel.GetConstantValue(eventNameArgument, _cancellationToken) is not { HasValue: true, Value: string eventName })
            {
                return false;
            }

            string receiver;
            ITypeSymbol? receiverType;
            bool isStatic;

            if (GetArgument(operation, "target") is { } target)
            {
                // The receiver is evaluated twice (add and remove) after the rewrite, so only accept side-effect free ones.
                if (!IsStableReceiver(target))
                {
                    return false;
                }

                receiver = Text(target);
                receiverType = _semanticModel.GetTypeInfo(target, _cancellationToken).Type;
                isStatic = false;
            }
            else if (GetArgument(operation, "type") is TypeOfExpressionSyntax typeOf)
            {
                receiverType = _semanticModel.GetTypeInfo(typeOf.Type, _cancellationToken).Type;
                receiver = receiverType is null ? string.Empty : Render(receiverType);
                isStatic = true;
            }
            else
            {
                return false;
            }

            for (var type = receiverType; type is not null; type = type.BaseType)
            {
                var @event = type.GetMembers(eventName).OfType<IEventSymbol>().FirstOrDefault(candidate => candidate.IsStatic == isStatic);
                if (@event is null)
                {
                    continue;
                }

                if (@event.Type is not INamedTypeSymbol eventType || !_semanticModel.IsAccessible(_invocation.SpanStart, @event))
                {
                    return false;
                }

                eventAccess = $"{receiver}.{@event.Name}";
                delegateType = eventType;
                return true;
            }

            return false;
        }
    }
}
