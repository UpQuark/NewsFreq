using System.Web.Mvc;

namespace DataNervesUI.Areas.NewsLibrary
{
    public class NewsLibraryAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "NewsLibrary";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "NewsLibrary_default",
                "NewsLibrary/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
