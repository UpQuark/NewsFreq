using System.Web.Mvc;

namespace DataNervesUI.Areas.Census
{
    public class CensusAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Census";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "Census_default",
                "Census/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
