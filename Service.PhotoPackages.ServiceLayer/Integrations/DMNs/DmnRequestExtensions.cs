using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Service.PhotoPackages.ServiceLayer.Integrations.DMNs
{
    public static class DmnRequestExtensions
    {
        public static Dictionary<string, dynamic> ToDictionary<T>(this T request) where T : IDmnRequest
        {
            return request.GetType()
                .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .ToDictionary(prop => prop.Name, prop => prop.GetValue(request, null));
        }
    }
}