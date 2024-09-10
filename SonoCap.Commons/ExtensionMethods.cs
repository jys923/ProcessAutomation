using System.Text.Json;

namespace SonoCap.Commons
{
    public static class ExtensionMethods
    {
        public static string ToFormattedString<T>(this T obj)
        {
            var properties = obj.GetType().GetProperties()
                .Where(p => p.CanRead && !p.GetIndexParameters().Any())
                .Select(p => new { Name = p.Name, Value = p.GetValue(obj, null) });

            return string.Join(", ", properties.Select(p => $"{p.Name}: {p.Value}"));
        }

        public static string ToJson<T>(this T obj)
        {
            try
            {
                return JsonSerializer.Serialize(obj);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error converting to JSON: {ex.Message}");
                return string.Empty;
            }
        }
    }
}

