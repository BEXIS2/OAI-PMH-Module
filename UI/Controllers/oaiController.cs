using BExIS.Modules.OAIPMH.UI.API;
using System.Net.Http;
using System.Web.Http;

namespace BExIS.Modules.OAIPMH.UI.Controllers
{
    public class oaiController : ApiController
    {
        public HttpResponseMessage Identify()
        {
            return Common.XDocResponse(DataProvider.CheckAttributes("Identify"));
        }

        public HttpResponseMessage CheckAttributes(
            string verb,
            string from,
            string until,
            string metadataPrefix,
            string set,
            string resumptionToken,
            string identifier)
        {
            return Common.XDocResponse(DataProvider.CheckAttributes(
                        verb,
                        from,
                        until,
                        metadataPrefix,
                        set,
                        resumptionToken,
                        identifier));
        }

        // GET api/oai
        [HttpGet]
        public HttpResponseMessage Get()
        {
            return Identify();
        }

        // GET api/oai
        /// <summary>
        ///
        /// </summary>
        /// <param name="verb"></param>
        /// <param name="from"></param>
        /// <param name="until"></param>
        /// <param name="metadataPrefix"></param>
        /// <param name="set"></param>
        /// <param name="resumptionToken"></param>
        /// <param name="identifier"></param>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage Get(
            string verb,
            string from = null,
            string until = null,
            string metadataPrefix = null,
            string set = null,
            string resumptionToken = null,
            string identifier = null)
        {
            return CheckAttributes(
                        verb,
                        from,
                        until,
                        metadataPrefix,
                        set,
                        resumptionToken,
                        identifier);
        }

        // POST api/oai
        [HttpPost]
        public HttpResponseMessage Post()
        {
            return Identify();
        }

        // POST api/oai
        [HttpPost]
        public HttpResponseMessage Post(
            string verb,
            string from = null,
            string until = null,
            string metadataPrefix = null,
            string set = null,
            string resumptionToken = null,
            string identifier = null)
        {
            return CheckAttributes(
                        verb,
                        from,
                        until,
                        metadataPrefix,
                        set,
                        resumptionToken,
                        identifier);
        }
    }
}