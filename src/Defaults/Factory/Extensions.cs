using Scriban;

namespace Rocket.Surgery.Airframe.Defaults.Factory;

public static class Extensions
{
    public static string Render<T>(this T model, string template) => Template.Parse(template).Render(model);
}