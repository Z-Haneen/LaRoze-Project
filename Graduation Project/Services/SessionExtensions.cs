using Microsoft.AspNetCore.Http;
using System.Text.Json;

namespace Graduation_Project.Services
{
    public static class AppSessionExtensions
    {
        // Removed the conflicting methods that duplicate Microsoft's functionality
        
        public static void SetObject<T>(this ISession session, string key, T value)
        {
            session.SetString(key, JsonSerializer.Serialize(value));
        }

        public static T GetObject<T>(this ISession session, string key)
        {
            var value = session.GetString(key);
            return value == null ? default : JsonSerializer.Deserialize<T>(value);
        }

        public static void SetInt(this ISession session, string key, int value)
        {
            session.SetString(key, value.ToString());
        }

        public static int? GetInt(this ISession session, string key)
        {
            var value = session.GetString(key);
            return string.IsNullOrEmpty(value) ? null : int.Parse(value);
        }

        public static bool HasKey(this ISession session, string key)
        {
            return !string.IsNullOrEmpty(session.GetString(key));
        }
    }
}

