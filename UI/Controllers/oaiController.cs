using BExIS.Modules.OAIPMH.UI.API;
using BExIS.Modules.OAIPMH.UI.API.Common;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using System.Xml.Linq;

namespace BExIS.Modules.OAIPMH.UI.Controllers
{
    public class oaiController : ApiController
    {
        public HttpResponseMessage Identify(bool unexpectedParameters)
        {
            return Common.XDocResponse(DataProvider.CheckAttributes("Identify", null, null, null, null, null, null, unexpectedParameters));
        }

        public HttpResponseMessage CheckAttributes(
            string verb,
            string from,
            string until,
            string metadataPrefix,
            string set,
            string resumptionToken,
            string identifier,
            bool unexpectedParameters)
        {
            return Common.XDocResponse(DataProvider.CheckAttributes(
                        verb,
                        from,
                        until,
                        metadataPrefix,
                        set,
                        resumptionToken,
                        identifier,
                        unexpectedParameters));
        }

        private bool unexpectedParametersExist()
        {
            IEnumerable<KeyValuePair<string, string>> dic = this.Request.GetQueryNameValuePairs();

            List<string> tmp = new List<string>();
            tmp.Add("verb");
            tmp.Add("from");
            tmp.Add("until");
            tmp.Add("metadataPrefix");
            tmp.Add("set");
            tmp.Add("resumptionToken");
            tmp.Add("identifier");

            foreach (KeyValuePair<string, string> d in dic)
            {
                if (!tmp.Contains(d.Key)) return true;
            }

            return false;
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
            [FromUri] string verb = null,
            [FromUri] string from = null,
            [FromUri] string until = null,
            [FromUri] string metadataPrefix = null,
            [FromUri] string set = null,
            [FromUri] string resumptionToken = null,
            [FromUri] string identifier = null)

        {
            bool unexpectedParameters = unexpectedParametersExist();

            if (verb == null &&
                    from == null &&
                    until == null &&
                    metadataPrefix == null &&
                    set == null &&
                    resumptionToken == null &&
                    identifier == null)
            {
                return Identify(unexpectedParameters);
            }
            else
            {
                return CheckAttributes(
                            verb,
                            from,
                            until,
                            metadataPrefix,
                            set,
                            resumptionToken,
                            identifier,
                            unexpectedParameters);
            }
        }

        // POST api/oai
        [HttpPost]
        public HttpResponseMessage Post(
            [FromUri] string verb = null,
            [FromUri] string from = null,
            [FromUri] string until = null,
            [FromUri] string metadataPrefix = null,
            [FromUri] string set = null,
            [FromUri] string resumptionToken = null,
            [FromUri] string identifier = null)

        {
            bool unexpectedParameters = unexpectedParametersExist();

            if (verb == null &&
                    from == null &&
                    until == null &&
                    metadataPrefix == null &&
                    set == null &&
                    resumptionToken == null &&
                    identifier == null)
            {
                return Identify(unexpectedParameters);
            }
            else
            {
                return CheckAttributes(
                            verb,
                            from,
                            until,
                            metadataPrefix,
                            set,
                            resumptionToken,
                            identifier,
                            unexpectedParameters);
            }
        }
    }
}