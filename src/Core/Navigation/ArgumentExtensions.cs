using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using JetBrains.Annotations;

namespace Rocket.Surgery.Airframe.Navigation;

/// <summary>
/// Extension methods for Navigation or Dialog parameters.
/// </summary>
/// <remarks>Taken from https://github.com/PrismLibrary/Prism/blob/5b296e1d5cac8e43e8457d0241e219405b57ed47/src/Prism.Core/Common/ParametersBase.cs#L1C1-L199C2.</remarks>
[EditorBrowsable(EditorBrowsableState.Never)]
public static class ArgumentExtensions
{
    /// <summary>
    /// Searches <paramref name="parameters"/> for <paramref name="key"/>.
    /// </summary>
    /// <typeparam name="T">The type of the parameter to return.</typeparam>
    /// <param name="parameters">A collection of parameters to search.</param>
    /// <param name="key">The key of the parameter to find.</param>
    /// <returns>A matching value of <typeparamref name="T"/> if it exists.</returns>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static T? GetValue<T>(this IEnumerable<KeyValuePair<string, object>> parameters, string key) =>
        (T?)GetValue(parameters, key, typeof(T));

    /// <summary>
    /// Searches <paramref name="parameters"/> for value referenced by <paramref name="key"/>.
    /// </summary>
    /// <param name="parameters">A collection of parameters to search.</param>
    /// <param name="key">The key of the parameter to find.</param>
    /// <param name="type">The type of the parameter to return.</param>
    /// <returns>A matching value of <paramref name="type"/> if it exists.</returns>
    /// <exception cref="InvalidCastException">Unable to convert the value of Type.</exception>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static object? GetValue(this IEnumerable<KeyValuePair<string, object>> parameters, string key, Type type)
    {
        foreach (var kvp in parameters)
        {
            if (string.Compare(kvp.Key, key, StringComparison.Ordinal) == 0)
            {
                if (TryGetValueInternal(kvp, type, out var value))
                {
                    return value;
                }

                throw new InvalidCastException($"Unable to convert the value of Type '{kvp.Value.GetType().FullName}' to '{type.FullName}' for the key '{key}' ");
            }
        }

        return GetDefault(type);
    }

    /// <summary>
    /// Searches <paramref name="parameters"/> for value referenced by <paramref name="key"/>.
    /// </summary>
    /// <typeparam name="T">The type of the parameter to return.</typeparam>
    /// <param name="parameters">A collection of parameters to search.</param>
    /// <param name="key">The key of the parameter to find.</param>
    /// <param name="value">The value of parameter to return.</param>
    /// <returns>Success if value is found; otherwise returns <c>false</c>.</returns>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static bool TryGetValue<T>(this IEnumerable<KeyValuePair<string, object>> parameters, string key, out T value)
    {
        var type = typeof(T);

        foreach (var kvp in parameters)
        {
            if (string.Compare(kvp.Key, key, StringComparison.Ordinal) == 0)
            {
                var success = TryGetValueInternal(kvp, typeof(T), out var valueAsObject);
                value = (T)valueAsObject;
                return success;
            }
        }

        value = default;
        return false;
    }

    /// <summary>
    /// Searches <paramref name="parameters"/> for value referenced by <paramref name="key"/>.
    /// </summary>
    /// <typeparam name="T">The type of the parameter to return.</typeparam>
    /// <param name="parameters">A collection of parameters to search.</param>
    /// <param name="key">The key of the parameter to find.</param>
    /// <returns>An IEnumerable{T} of all the values referenced by key.</returns>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static IEnumerable<T> GetValues<T>(this IEnumerable<KeyValuePair<string, object>> parameters, string key)
    {
        List<T> values = new();
        var type = typeof(T);

        foreach (var kvp in parameters)
        {
            if (string.Compare(kvp.Key, key, StringComparison.Ordinal) == 0)
            {
                TryGetValueInternal(kvp, type, out var value);
                values.Add((T)value);
            }
        }

        return values.AsEnumerable();
    }

    /// <summary>
    /// Checks to see if key exists in parameter collection.
    /// </summary>
    /// <param name="parameters">IEnumerable to search.</param>
    /// <param name="key">The key to search the <paramref name="parameters"/> for existence.</param>
    /// <returns><c>true</c> if key exists; <c>false</c> otherwise.</returns>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static bool ContainsKey(this IEnumerable<KeyValuePair<string, object>> parameters, string key) =>
        parameters.Any(x => string.Compare(x.Key, key, StringComparison.Ordinal) == 0);

    private static bool TryGetValueInternal(KeyValuePair<string, object?> kvp, Type type, out object value)
    {
        value = GetDefault(type);
        var success = false;
        if (kvp.Value == null)
        {
            success = true;
        }
        else if (kvp.Value.GetType() == type)
        {
            success = true;
            value = kvp.Value;
        }
        else if (type.IsInstanceOfType(kvp.Value))
        {
            success = true;
            value = kvp.Value;
        }
        else if (type.IsEnum)
        {
            var valueAsString = kvp.Value.ToString();
            if (Enum.IsDefined(type, valueAsString))
            {
                success = true;
                value = Enum.Parse(type, valueAsString);
            }
            else if (int.TryParse(valueAsString, out var numericValue))
            {
                success = true;
                value = Enum.ToObject(type, numericValue);
            }
        }

        if (!success && type.GetInterface("System.IConvertible") != null)
        {
            success = true;
            value = Convert.ChangeType(kvp.Value, type);
        }

        return success;
    }

    [CanBeNull]
    private static object? GetDefault(Type type)
    {
        if (type.IsValueType)
        {
            return Activator.CreateInstance(type);
        }

        return null;
    }
}