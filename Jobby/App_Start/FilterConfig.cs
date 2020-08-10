using System.Web;
using System.Web.Mvc;

namespace Jobby
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new OutputCacheAttribute
            {
                VaryByParam = "*",
                Duration = 0,
                NoStore = true,
            });
            filters.Add(new HandleErrorAttribute());
        }
    }
}
