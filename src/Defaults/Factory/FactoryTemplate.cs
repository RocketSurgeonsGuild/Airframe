namespace Rocket.Surgery.Airframe.Defaults.Factory;

// public class FactoryTemplate
// {
//     // lang=csharp
//     private const string Template =
//         """
//         public partial class {{ className }}
//         {
//             public static {{ className }} {{ factoryMethod }}({{ parameters }}) => new({{ parameters }});
//         }
//         """;
// }
public partial class Thing
{
    public static Thing Create() => new Thing();
}