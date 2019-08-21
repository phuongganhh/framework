using Framework.Attributes;
using System.Web;
using System.Web.Mvc;

namespace TrainingMVC
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new Cors());
            //filters.Add(new AuthorizeAttr());
            filters.Add(new HandleErrorAttribute());
        }
    }
}
