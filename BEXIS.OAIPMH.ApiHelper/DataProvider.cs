using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace BExIS.OAIPMH.API
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
                case "ListIdentifiers":
                case "ListRecords":
                    {
                        if (isIdentifier) errors.Add(MlErrors.badIdentifierArgumentNotAllowed);

                        return ListIdentifiersOrRecords(verb, from, until, metadataPrefix,
                            set, resumptionToken, false, errors, null);
                    }
                case "ListMetadataFormats":
                    {
                        if (isFrom) errors.Add(MlErrors.badFromArgumentNotAllowed);
                        if (isUntil) errors.Add(MlErrors.badUntilArgumentNotAllowed);
                        if (isPrefixOk) errors.Add(MlErrors.badMetadataArgumentNotAllowed);
                        if (isSet) errors.Add(MlErrors.badSetArgumentNotAllowed);
                        if (isResumption) errors.Add(MlErrors.badResumptionArgumentNotAllowed);

                        return ListMetadataFormats(identifier, errors);
                    }
                case "ListSets":
                    {
                        if (isFrom) errors.Add(MlErrors.badFromArgumentNotAllowed);
                        if (isUntil) errors.Add(MlErrors.badUntilArgumentNotAllowed);
                        if (isPrefixOk) errors.Add(MlErrors.badMetadataArgumentNotAllowed);
                        if (isSet) errors.Add(MlErrors.badSetArgumentNotAllowed);
                        if (isIdentifier) errors.Add(MlErrors.badIdentifierArgumentNotAllowed);

                        return ListSets(resumptionToken, false, errors);
                    }
                default:
                    {
                        XElement request = new XElement("request", Properties.baseURL);
                        return CreateXml(new XElement[] { request, MlErrors.badVerb });
                    }
            }
        }
    }
}