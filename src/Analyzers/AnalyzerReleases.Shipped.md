; Based on published Rocket.Surgery.Airframe.CodeAnalysis packages from NuGet.org.
; Version 0.12.0 contained no analyzer assemblies; the first bundled rules shipped in 0.12.1.
; See ../CodeAnalysis/docs/releases.md for the package audit.

## Release 0.12.2

### New Rules

Rule ID | Category | Severity | Notes
--------|----------|----------|--------------------
RSA0001 | Nullability | Info | RSA0001_MutablePublicPropertySetter
RSA0002 | Nullability | Warning | RSA0002_NullableCollectionReturn
RSA0003 | Nullability | Warning | RSA0003_NullableTaskReturn
RSA0004 | Nullability | Info | RSA0004_NullForgivingOperatorDensity
RSA0005 | Nullability | Info | RSA0005_RedundantNullConditionalAccess
RSA0006 | Nullability | Info | RSA0006_NullableBooleanTriState
RSA0007 | Nullability | Info | RSA0007_DefaultLiteralForReferenceType
RSA0008 | Nullability | Warning | RSA0008_NullableContextDisabled
RSA0009 | Nullability | Disabled | RSA0009_NullableReturnOnAbstraction
RSA1010 | Usage | Warning | RSA1010_BindDynamicDataOnMainThread
RSA2014 | Design | Warning | RSA2014_MissingAbstractionSummary
RSA2015 | Design | Warning | RSA2015_MissingInheritdoc
RSA3003 | Performance | Info | RSA3003_Closure
RSA3004 | Performance | Warning | RSA3004_AutoRefreshOnTaskPool
RSA3005 | Performance | Warning | RSA3005_AutoRefreshAlreadyApplied

## Release 0.12.1

### New Rules

Rule ID | Category | Severity | Notes
--------|----------|----------|--------------------
RSA1001 | Usage | Warning | RSA1001_UseExpressionOverride
RSA1002 | Usage | Error | RSA1002_ConstantExpressions
RSA1003 | Usage | Error | RSA1003_OutParameterAssignment
RSA1004 | Usage | Error | RSA1004_WhenAnyValueClosure
RSA1005 | Usage | Info | RSA1005_SchedulerNotProvided
RSA1006 | Usage | Info | RSA1006_MultipleUsesOfSubscribeOn
RSA1007 | Usage | Warning | RSA1007_UseInvokeFunction
RSA2001 | Design | Warning | RSA2001_ConstructorsFirst
RSA2002 | Design | Warning | RSA2002_PrivateMembersLast
RSA2003 | Design | Warning | RSA2003_MemberOrderByKind
RSA2004 | Design | Warning | RSA2004_MemberOrderByAccess
RSA2005 | Design | Warning | RSA2005_StaticBeforeInstance
RSA2006 | Design | Warning | RSA2006_ConstantsBeforeFields
RSA2007 | Design | Warning | RSA2007_ReadonlyBeforeMutable
RSA2008 | Design | Warning | RSA2008_SingleTypePerFile
RSA2009 | Design | Warning | RSA2009_FileNameMatchesType
RSA2010 | Design | Warning | RSA2010_NoRegions
RSA2011 | Design | Warning | RSA2011_ExplicitAccessibility
RSA2012 | Design | Warning | RSA2012_ConditionalCompilationSpansMembers
RSA2013 | Design | Warning | RSA2013_LineTooLong
RSA3001 | Performance | Warning | RSA3001_SubscriptionNotDisposed
RSA3002 | Performance | Warning | RSA3002_LambdaExpressionStatic
