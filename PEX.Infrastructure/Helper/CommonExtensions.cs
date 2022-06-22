using System.Collections.ObjectModel;

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace PEX.Infrastructure.Helper;
public static class CommonExtensions
{
    /// <summary>
    /// Check object is Null ...
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public static bool IsNull(this object source)
    {
        return source == null;
    }
    /// <summary>
    /// Check collection Has Childs ...
    /// </summary>
    /// <param name="source"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static bool HasChild<T>(this IList<T> source)
    {
        return !source.IsNull() && source.Any();
    }
    /// <summary>
    /// Check collection Has Childs ...
    /// </summary>
    /// <param name="source"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static bool HasChild<T>(this ICollection<T> source)
    {
        return !source.IsNull() && source.Any();
    }
    /// <summary>
    /// Check collection Has Childs ...
    /// </summary>
    /// <param name="source"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static bool HasChild<T>(this IEnumerable<T> source)
    {
        return !source.IsNull() && source.Any();
    }

    /// <summary>
    /// Check an string is Null or Empty ...
    /// </summary>
    /// <param name="sourse"></param>
    /// <returns></returns>
    public static bool IsNullOrEmpty(this string sourse)
    {
        return string.IsNullOrEmpty(sourse);
    }

    /// <summary>
    /// Convert From Json String ...
    /// </summary>
    /// <param name="value"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static T? FromJSON<T>(this string value)
    {
        if (value!.IsNullOrEmpty())
        {
            throw new ArgumentNullException(paramName: nameof(value), message: "value must not be null");
        }
        return JsonConvert.DeserializeObject<T>(value);
    }

    /// <summary>
    /// Convert object to Json ...
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static string ToJSON(this object value)
    {
        //
        if (value.IsNull())
        {
            return string.Empty;
        }

        //
        var setting = new JsonSerializerSettings
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            PreserveReferencesHandling = PreserveReferencesHandling.None
        };

        //
        return JsonConvert.SerializeObject(value, Newtonsoft.Json.Formatting.None, setting);
    }

    /// <summary>
    /// Convert to Json With Support of Camel Casing
    /// </summary>
    /// <param name="value"></param>
    /// <param name="camelCase"></param>
    /// <returns></returns>
    public static string ToJSON(this object value, bool camelCase)
    {
        //
        if (value.IsNull())
        {
            return string.Empty;
        }

        //
        var setting = new JsonSerializerSettings
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            PreserveReferencesHandling = PreserveReferencesHandling.None,
        };

        //
        if (camelCase)
        {
            setting.ContractResolver = new CamelCasePropertyNamesContractResolver();
        }

        //
        return JsonConvert.SerializeObject(value, Newtonsoft.Json.Formatting.None, setting);
    }

    /// <summary>
    /// Convert XML String to Json Object ...
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public static string ToJSON(this string source)
    {
        //
        var result = string.Empty;
        try
        {
            //
            result = XMLToJSONHelper.ToJSON(source);

            //
            // Handle Removing Additional Values ...
            result = result.Replace("s:", "", StringComparison.OrdinalIgnoreCase).Replace("a:", "", StringComparison.OrdinalIgnoreCase);
        }
        catch
        {
            // 
        }

        //
        return result;
    }
    /// <summary>
    /// Check specific type is Collection Type or not
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public static bool IsCollectionType(this Type source)
    {
        //
        if (source.IsNull() || !source.IsGenericType)
        {
            return false;
        }

        //
        var genericTypeDefinition = source.GetGenericTypeDefinition();
        var result = genericTypeDefinition == typeof(List<>) ||
            genericTypeDefinition == typeof(HashSet<>) ||
            genericTypeDefinition == typeof(Collection<>) ||
            genericTypeDefinition == typeof(ICollection<>) ||
            genericTypeDefinition == typeof(IEnumerable<>);

        //
        return result;
    }

    public static T UpdateData<T, TD>(
        this T source,
        TD updateWith,
        ICollection<string>? propertyWhiteList = null,
        ICollection<string>? propertyBlackList = null,
        ICollection<KeyValuePair<string, Func<TD, object>>>? propertyValueProviders = null,
        bool updateWithNullOrEmptyValues = false,
        bool throwExceptionOnFails = false) where T : class where TD : class
    {
        //
        var hasWhiteList = propertyWhiteList!.HasChild<string>();
        var hasBlackList = propertyBlackList!.HasChild<string>();
        var hasValueProvider = propertyValueProviders!.HasChild<KeyValuePair<string, Func<TD, object>>>();


        if (updateWith is null)
        {
            throw new ArgumentNullException(paramName: nameof(updateWith), message: "updateWith must not be null");
        }

        //
        var fullPropertyList = updateWith.GetType().GetProperties().Select(prop => prop.Name);
        var mustSetProps = (hasWhiteList ? propertyWhiteList : fullPropertyList);

        //
        if (hasBlackList)
        {
            mustSetProps = mustSetProps!.Where(propName => !propertyBlackList!.Contains(propName));
        }

        //
        mustSetProps!.ToList().ForEach(propName =>
        {
            //
            try
            {
                //
                var sourceProp = source.GetType().GetProperty(propName);

                var sourceValue = sourceProp?.GetValue(source);


                //
                var updateWithProp = updateWith.GetType().GetProperty(propName);
                var updateWithValue = updateWithProp?.GetValue(updateWith);


                //
                var valueProviderValue = hasValueProvider ? propertyValueProviders!
                    .FirstOrDefault(p => string.Equals(p.Key, propName, StringComparison.OrdinalIgnoreCase)).Value : null;

                object? updateVal;
                if (!valueProviderValue!.IsNull())
                {
                    updateVal = valueProviderValue!.Invoke(updateWith);
                }
                else if (updateWithValue!.IsNull() && updateWithNullOrEmptyValues)
                {
                    updateVal = sourceValue;
                }
                else
                {
                    updateVal = updateWithValue;
                }

                var updateValType = updateVal!.IsNull() ? null : updateVal!.GetType();

                //
                Type t = Nullable.GetUnderlyingType(sourceProp!.PropertyType) ?? sourceProp.PropertyType;
                object? safeValue;
                if (updateVal == null)
                {
                    safeValue = null;
                }
                else if (updateValType!.IsCollectionType())
                {
                    safeValue = Convert.ChangeType(updateVal, t, CultureInfo.InvariantCulture);
                }
                else
                {
                    safeValue = updateVal;
                }

                //
                sourceProp.SetValue(source, safeValue);
            }
            catch (Exception)
            {
                //
                if (throwExceptionOnFails)
                {
                    throw;
                }
            }
        });

        //
        return source;
    }
}
