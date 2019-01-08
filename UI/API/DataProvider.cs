using BExIS.Modules.OAIPMH.UI.API.Common;
using BExIS.Modules.OAIPMH.UI.API.Internal;
using BExIS.Modules.OAIPMH.UI.API.MdFormats;
using BExIS.Modules.OAIPMH.UI.Helper;
using BExIS.Modules.OAIPMH.UI.Models;
using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace BExIS.Modules.OAIPMH.UI.API
{
    public class DataProvider
    {
        public static XDocument CheckAttributes(
            string verb,
            string from = null,
            string until = null,
            string metadataPrefix = null,
            string set = null,
            string resumptionToken = null,
            string identifier = null)
        {
            bool isVerb = !String.IsNullOrEmpty(verb);
            bool isFrom = !String.IsNullOrEmpty(from);
            bool isUntil = !String.IsNullOrEmpty(until);
            bool isPrefixOk = !String.IsNullOrEmpty(metadataPrefix);
            bool isSet = !String.IsNullOrEmpty(set);
            bool isResumption = !String.IsNullOrEmpty(resumptionToken);
            bool isIdentifier = !String.IsNullOrEmpty(identifier);


            if (!isVerb)
            {
                XElement request = new XElement("request", Properties.baseURL);
                return CreateXml(new XElement[] { request, MlErrors.badVerbMissing });
            }

            List<XElement> errors = new List<XElement>();
            switch (verb)
            {
                case "GetRecord":
                    {
                        if (isFrom) errors.Add(MlErrors.badFromArgumentNotAllowed);
                        if (isUntil) errors.Add(MlErrors.badUntilArgumentNotAllowed);
                        if (isSet) errors.Add(MlErrors.badSetArgumentNotAllowed);
                        if (isResumption) errors.Add(MlErrors.badResumptionArgumentNotAllowed);

                        return GetRecord(identifier, metadataPrefix, errors, Properties.loadAbout);
                    }
                case "Identify":
                    {
                        if (isFrom) errors.Add(MlErrors.badFromArgumentNotAllowed);
                        if (isUntil) errors.Add(MlErrors.badUntilArgumentNotAllowed);
                        if (isPrefixOk) errors.Add(MlErrors.badMetadataArgumentNotAllowed);
                        if (isSet) errors.Add(MlErrors.badSetArgumentNotAllowed);
                        if (isResumption) errors.Add(MlErrors.badResumptionArgumentNotAllowed);
                        if (isIdentifier) errors.Add(MlErrors.badIdentifierArgumentNotAllowed);

                        return Identify(errors);
                    }
                //case "ListIdentifiers":
                //case "ListRecords":
                //    {
                //        if (isIdentifier) errors.Add(MlErrors.badIdentifierArgumentNotAllowed);

                //        return ListIdentifiersOrRecords(verb, from, until, metadataPrefix,
                //            set, resumptionToken, false, errors, null);
                //    }
                //case "ListMetadataFormats":
                //    {
                //        if (isFrom) errors.Add(MlErrors.badFromArgumentNotAllowed);
                //        if (isUntil) errors.Add(MlErrors.badUntilArgumentNotAllowed);
                //        if (isPrefixOk) errors.Add(MlErrors.badMetadataArgumentNotAllowed);
                //        if (isSet) errors.Add(MlErrors.badSetArgumentNotAllowed);
                //        if (isResumption) errors.Add(MlErrors.badResumptionArgumentNotAllowed);

                //        return ListMetadataFormats(identifier, errors);
                //    }
                //case "ListSets":
                //    {
                //        if (isFrom) errors.Add(MlErrors.badFromArgumentNotAllowed);
                //        if (isUntil) errors.Add(MlErrors.badUntilArgumentNotAllowed);
                //        if (isPrefixOk) errors.Add(MlErrors.badMetadataArgumentNotAllowed);
                //        if (isSet) errors.Add(MlErrors.badSetArgumentNotAllowed);
                //        if (isIdentifier) errors.Add(MlErrors.badIdentifierArgumentNotAllowed);

                //        return ListSets(resumptionToken, false, errors);
                //    }
                default:
                    {
                        XElement request = new XElement("request", Properties.baseURL);
                        return CreateXml(new XElement[] { request, MlErrors.badVerb });
                    }
            }
        }

        public static XDocument Identify(List<XElement> errorList)
        {
            XElement request = new XElement("request",
                                            new XAttribute("verb", "Identify"),
                                            Properties.baseURL);

            if (errorList.Count == 0)
            {
                XElement identify = new XElement("Identify",
                    new XElement("repositoryName", Properties.repositoryName),
                    new XElement("baseURL", Properties.baseURL),
                    new XElement("protocolVersion", Properties.protocolVersion),
                    new XElement("adminEmail", Properties.adminEmails),
                    new XElement("earliestDatestamp", Properties.earliestDatestamp),
                    new XElement("deletedRecord", Properties.deletedRecord),
                    new XElement("granularity", Properties.granularity.Replace("'", "")),
                    new XElement("compression", Properties.compression),
                    Properties.description != null ? Properties.description : null);

                return CreateXml(new XElement[] { request, identify });
            }

            errorList.Insert(0, request);
            return CreateXml(errorList.ToArray());
        }

        public static XDocument GetRecord(string identifier, string metadataPrefix, List<XElement> errorList, bool? loadAbout)
        {
            List<XElement> errors = errorList;

            bool isIdentifier = !String.IsNullOrEmpty(identifier);
            if (!isIdentifier)
            {
                errors.Add(MlErrors.badIdentifierArgument);
            }

            bool isPrefixOk = !String.IsNullOrEmpty(metadataPrefix);
            if (!isPrefixOk)
            {
                errors.Add(MlErrors.badMetadataArgument);
            }
            else if (FormatList.Prefix2Int(metadataPrefix) == 0)
            {
                errors.Add(MlErrors.cannotDisseminateFormat);
                isPrefixOk = false;
            }

            bool isAbout = loadAbout.HasValue ? loadAbout.Value : Properties.loadAbout;

            RecordQueryResult record = null;
            if (isIdentifier && isPrefixOk)
            {
                OAIHelper helper = new OAIHelper();

                long datasetId = helper.ConvertToId(identifier);
                Header header = helper.GetHeader(datasetId);

                if (header == null)
                {
                    errors.Add(MlErrors.idDoesNotExist);
                }
                else
                {

                    record = new RecordQueryResult(header, helper.GetMetadata(datasetId, metadataPrefix));

                    // ToDo Check About in the documentaion of oai-pmh
                    record.About = null;

                    if (record == null || record.Metadata == null)
                    {
                        errors.Add(MlErrors.cannotDisseminateRecordFormat);
                    }
                }
            }


            XElement request = new XElement("request",
                new XAttribute("verb", "GetRecord"),
                isIdentifier ? new XAttribute("identifier", identifier) : null,
                isPrefixOk ? new XAttribute("metadataPrefix", metadataPrefix) : null,
                Properties.baseURL);

            if (errors.Count > 0)
            {
                errors.Insert(0, request); /* add request on the first position, that it will be diplayed before errors */
                return CreateXml(errors.ToArray());
            }


            XElement theRecord = new XElement("GetRecord",
                new XElement("record",
                    MlEncode.HeaderItem(record.Header, Properties.granularity),
                    MlEncode.Metadata(record.Metadata, Properties.granularity)),
                    isAbout ? MlEncode.About(record.About, Properties.granularity) : null);


            return CreateXml(new XElement[] { request, theRecord });

        }

        /// <summary>
        /// Creates response xml document
        /// </summary>
        /// <param name="oaiElements">First oai element should be request and second should be either an error 
        /// or element with the same name as the verb.</param>
        /// <returns>Complete response aml document</returns>
        private static XDocument CreateXml(XElement[] oaiElements)
        {
            foreach (XElement xe in oaiElements)
            {
                SetDefaultXNamespace(xe, MlNamespaces.oaiNs);
            }

            return new XDocument(new XDeclaration("1.0", "utf-8", "no"),
                new XElement(MlNamespaces.oaiNs + "OAI-PMH",
                    new XAttribute(XNamespace.Xmlns + "xsi", MlNamespaces.oaiXsi),
                    new XAttribute(MlNamespaces.oaiXsi + "schemaLocation", MlNamespaces.oaiSchemaLocation),
                        new XElement(MlNamespaces.oaiNs + "responseDate",
                            DateTime.Now.ToUniversalTime().ToString(Enums.Granularity.DateTime)),
                            oaiElements));
        }

        public static void SetDefaultXNamespace(XElement xelem, XNamespace xmlns)
        {
            if (xelem.Name.NamespaceName == string.Empty)
                xelem.Name = xmlns + xelem.Name.LocalName;

            foreach (var xe in xelem.Elements())
                SetDefaultXNamespace(xe, xmlns);
        }
    }
}