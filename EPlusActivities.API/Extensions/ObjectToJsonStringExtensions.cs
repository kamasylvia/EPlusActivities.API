using System;
using System.Text.Json;

namespace EPlusActivities.API.Extensions
{
    public static class ObjectToJsonStringExtensions
    {
        public static string ToString(this object obj) =>
            JsonSerializer
                .Serialize(obj,
                new JsonSerializerOptions { WriteIndented = true });
    }
}
