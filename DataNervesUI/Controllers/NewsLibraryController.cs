using System.Web.Http;
using NewsLibrarySearch.API;

namespace DataNervesUI.Controllers
{
    public class NewsLibraryController : ApiController
    {
        public int GetCount(NlQuery query)
        {
            query.SendQuery();
            return query.Count;
        }

        public int PostCount(NlQuery query)
        {
            query.SendQuery();
            return query.Count;
        }
    }
}