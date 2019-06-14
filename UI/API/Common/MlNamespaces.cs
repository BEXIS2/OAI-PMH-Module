using System.Xml.Linq;

namespace BExIS.Modules.OAIPMH.UI.API.Common
{
    public class MlNamespaces
    {
        /* OAI */
        public static XNamespace oaiNs = "http://www.openarchives.org/OAI/2.0/";
        public static XNamespace oaiXsi = "http://www.w3.org/2001/XMLSchema-instance";

        public static XNamespace oaiSchemaLocation = "http://www.openarchives.org/OAI/2.0/ " +
                                                     "http://www.openarchives.org/OAI/2.0/OAI-PMH.xsd";

        /* Dublin Core */
        public static XNamespace oaiDc = "http://www.openarchives.org/OAI/2.0/oai_dc/";
        public static XNamespace dcNs = "http://purl.org/dc/elements/1.1/";
        public static XNamespace dcXsi = "http://www.w3.org/2001/XMLSchema-instance";

        public static XNamespace dcSchemaLocation = "http://www.openarchives.org/OAI/2.0/oai_dc/ " +
                                                    "http://www.openarchives.org/OAI/2.0/oai_dc.xsd";

        /* Provenance */
        public static XNamespace provNs = "http://www.openarchives.org/OAI/2.0/provenance";
        public static XNamespace provXsi = "http://www.w3.org/2001/XMLSchema-instance";

        public static XNamespace provSchemaLocation = "http://www.openarchives.org/OAI/2.0/provenance " +
                                                      "http://www.openarchives.org/OAI/2.0/provenance.xsd";

        /* TODO: Add format here */
        /* Pan Simple */

        public static XNamespace psNs = "urn:pangaea.de:dataportals";
        public static XNamespace psSchemaLocation = "https://ws.pangaea.de/schemas/pansimple/pansimple.xsd";
        public static XNamespace psXsi = "urn:pangaea.de:dataportals http://ws.pangaea.de/schemas/pansimple/pansimple.xsd";
    }
}