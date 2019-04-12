using Microsoft.AspNetCore.Http;

namespace IntradayDashboard.WebApi.Helpers
{
    public static class Extensions
    {
        public static void AddApplicationError(this HttpResponse response, string message)
        {
            response.Headers.Add("Application-Error", message);
            response.Headers.Add("Access-Control-Expose-Headers", "Application-Error");
            response.Headers.Add("Access-Control-Allow-Origin", "*");
        }

        public static CacheInfo GetCacheDb () {
            return new CacheInfo() {
                HostAndPort = "localhost:6375",
                DatabaseId = 1,
                InstanceName = "master"
            };
        }

        public class CacheInfo
        {
            public string  HostAndPort { get; set; } 
            public int DatabaseId { get; set; }

            public string  InstanceName { get; set; }
        }
    }
}