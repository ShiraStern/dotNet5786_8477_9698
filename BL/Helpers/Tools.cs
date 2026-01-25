using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace Helpers
{
    internal static class Tools
    {
        private const string ApiKey = "696e41fdcca33758143655vuc857b7b";
        private const string BaseUrl = "https://geocode.maps.co/search";

        // ===============================
        // בדיקה האם כתובת קיימת (Async)
        // Checks if an address exists
        // ===============================
        internal static async Task<bool> IsValidAddressAsync(string address)
        {
            try
            {
                string url =
                    $"{BaseUrl}?q={Uri.EscapeDataString(address)}&api_key={ApiKey}";

                using HttpClient client = new();

                string response =
                    await client.GetStringAsync(url);

                using JsonDocument doc = JsonDocument.Parse(response);

                return doc.RootElement.GetArrayLength() > 0;
            }
            catch
            {
                return false;
            }
        }

        // =========================================
        // קבלת קואורדינטות של כתובת (Async)
        // Get coordinates from address
        // =========================================
        internal static async Task<(double lat, double lon)> GetCoordinatesAsync(string address)
        {
            try
            {
                string url =
                    $"{BaseUrl}?q={Uri.EscapeDataString(address)}&api_key={ApiKey}";

                using HttpClient client = new();

                string response =
                    await client.GetStringAsync(url);

                using JsonDocument doc = JsonDocument.Parse(response);

                if (doc.RootElement.GetArrayLength() == 0)
                    throw new Exception("Address not found");

                var item = doc.RootElement[0];

                double lat =
                    double.Parse(item.GetProperty("lat").GetString()!);

                double lon =
                    double.Parse(item.GetProperty("lon").GetString()!);

                return (lat, lon);
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to get coordinates", ex);
            }
        }

        // =========================================
        // חישוב מרחק אווירי (לוקאלי – בלי רשת)
        // Air distance calculation (No internet)
        // =========================================
        internal static double CalculateAirDistance(
            double lat1, double lon1,
            double lat2, double lon2)
        {
            const double R = 6371;

            double dLat = ToRadians(lat2 - lat1);
            double dLon = ToRadians(lon2 - lon1);

            lat1 = ToRadians(lat1);
            lat2 = ToRadians(lat2);

            double a =
                Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Sin(dLon / 2) * Math.Sin(dLon / 2) *
                Math.Cos(lat1) * Math.Cos(lat2);

            double c = 2 * Math.Asin(Math.Sqrt(a));

            return R * c;
        }

        private static double ToRadians(double angle)
        {
            return angle * Math.PI / 180;
        }
    }
}
