using System.Web;
using System.Web.Mvc;

namespace Monday_Tradeskola
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
