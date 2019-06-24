using BExIS.Dlm.Entities.Data;
using BExIS.Dlm.Services.Data;
using BExIS.Modules.OAIPMH.UI.API.Common;
using BExIS.Modules.OAIPMH.UI.API.Internal;
using BExIS.Modules.OAIPMH.UI.API.MdFormats;
using BExIS.Modules.OAIPMH.UI.Helper;
using BExIS.Modules.OAIPMH.UI.Models;
using BExIS.Security.Services.Authorization;
using BExIS.Security.Services.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Vaiona.Model.MTnt;
using Vaiona.Utils.Cfg;
using static BExIS.Modules.OAIPMH.UI.API.Common.Enums;

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
            string identifier = null,
            bool unexpectedParameters = false)
        {
            bool isVerb = !String.IsNullOrEmpty(verb);
            bool isFrom = !String.IsNullOrEmpty(from);
            bool isUntil = !String.IsNullOrEmpty(until);
            bool isPrefixOk = !String.IsNullOrEmpty(metadataPrefix);
            bool isSet = !String.IsNullOrEmpty(set);
            bool isResumption = !String.IsNullOrEmpty(resumptionToken);
            bool isIdentifier = !String.IsNullOrEmpty(identifier);

            if (unexpectedParameters)
            {
                XElement request = new XElement("request", Properties.baseURL);
                return CreateXml(new XElement[] { request, MlErrors.badVerb });
            }

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

        public static XDocument Identify(List<XElement> errorList)
        {
            XElement request = new XElement("request",
                                            new XAttribute("verb", "Identify"),
                                            Properties.baseURL);

            OAIHelper oaiHelper = new OAIHelper();
            string earliestDatestamp = oaiHelper.EarliestDatestamp();

            if (errorList.Count == 0)
            {
                XElement identify = new XElement("Identify",
                    new XElement("repositoryName", Properties.repositoryName),
                    new XElement("baseURL", Properties.baseURL),
                    new XElement("protocolVersion", Properties.protocolVersion),
                    new XElement("adminEmail", Properties.adminEmails),
                    new XElement("earliestDatestamp", earliestDatestamp),
                    new XElement("deletedRecord", Properties.deletedRecord),
                    new XElement("granularity", Properties.granularity),
                    new XElement("compression", Properties.compression),
                Properties.description != null ? Properties.description : null);

                return CreateXml(new XElement[] { request, identify });
            }

            errorList.Insert(0, request);
            return CreateXml(errorList.ToArray());
        }

        public static XDocument GetRecord(string identifier, string metadataPrefix, List<XElement> errorList, bool? loadAbout, Tenant tenant = null)
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

        #region ListIdentifiers / ListRecords

        public static XDocument ListIdentifiersOrRecords(
            string verb,
            string from,
            string until,
            string metadataPrefix,
            string set,
            string resumptionToken,
            bool isRoundtrip,
            List<XElement> errorList,
            bool? loadAbout)
        {
            List<XElement> errors = errorList;
            DateTime? fromDate = DateTime.MinValue;
            DateTime? untilDate = DateTime.MaxValue;
            /* VERB */
            bool isRecord = false;
            if (String.IsNullOrEmpty(verb) || !(verb == "ListIdentifiers" || verb == "ListRecords"))
            {
                errors.Add(MlErrors.badVerbArgument);
            }
            else
            {
                isRecord = verb == "ListRecords";
            }
            /* FROM */
            bool isFrom = !String.IsNullOrEmpty(from);
            fromDate = MlDecode.SafeDateTime(from);
            if (isFrom && fromDate == null)
            {
                errors.Add(MlErrors.badFromArgument);
            }
            /* UNTIL */
            bool isUntil = !String.IsNullOrEmpty(until);
            untilDate = MlDecode.SafeDateTime(until);
            if (isUntil && untilDate == null)
            {
                errors.Add(MlErrors.badUntilArgument);
            }
            if (isFrom && isUntil && fromDate > untilDate)
            {
                errors.Add(MlErrors.badFromAndUntilArgument);
            }

            // if both dates exist, they should be in the same format
            if (!string.IsNullOrEmpty(from) && !string.IsNullOrEmpty(until))
            {
                if (from.Count() != until.Count())
                {
                    errors.Add(MlErrors.badFromAndUntilFormatArgument);
                }
            }

            if (fromDate == null) fromDate = new DateTime(1900, 1, 1);
            if (until == null) untilDate = DateTime.Now;

            if (untilDate != null) untilDate = ((DateTime)untilDate).AddMilliseconds(999);

            /* METADATA PREFIX */
            bool isPrefixOk = !String.IsNullOrEmpty(metadataPrefix);
            /* SETS */
            bool isSet = !String.IsNullOrEmpty(set);
            if (isSet && !Properties.supportSets)
            {
                errors.Add(MlErrors.noSetHierarchy);
            }
            /* RESUMPTION TOKEN */
            bool isResumption = !String.IsNullOrEmpty(resumptionToken);
            if (isResumption && !isRoundtrip)
            {
                if (isFrom || isUntil || isPrefixOk || isSet)
                {
                    errors.Add(MlErrors.badResumptionArgumentOnly);
                }

                if (!(Properties.resumptionTokens.ContainsKey(resumptionToken) &&
                    Properties.resumptionTokens[resumptionToken].Verb == verb &&
                    Properties.resumptionTokens[resumptionToken].ExpirationDate >= DateTime.UtcNow))
                {
                    errors.Insert(0, MlErrors.badResumptionArgument);
                }

                if (errors.Count == 0)
                {
                    return ListIdentifiersOrRecords(
                        verb,
                        Properties.resumptionTokens[resumptionToken].From.HasValue ?
                        Properties.resumptionTokens[resumptionToken].From.Value.ToUniversalTime().ToString(Properties.granularity) : null,
                        Properties.resumptionTokens[resumptionToken].Until.HasValue ?
                        Properties.resumptionTokens[resumptionToken].Until.Value.ToUniversalTime().ToString(Properties.granularity) : null,
                        Properties.resumptionTokens[resumptionToken].MetadataPrefix,
                        Properties.resumptionTokens[resumptionToken].Set,
                        resumptionToken,
                        true,
                        errors,
                        loadAbout);
                }
            }

            if (!isPrefixOk) /* Check if the only required attribute is included in the request */
            {
                errors.Add(MlErrors.badMetadataArgument);
            }
            else if (FormatList.Prefix2Int(metadataPrefix) == 0)
            {
                errors.Add(MlErrors.cannotDisseminateFormat);
            }

            bool isAbout = loadAbout.HasValue ? loadAbout.Value : Properties.loadAbout;

            XElement request = new XElement("request",
                new XAttribute("verb", verb),
                isFrom ? new XAttribute("from", from) : null,
                isUntil ? new XAttribute("until", until) : null,
                isPrefixOk ? new XAttribute("metadataPrefix", metadataPrefix) : null,
                isSet ? new XAttribute("set", set) : null,
                isResumption ? new XAttribute("resumptionToken", resumptionToken) : null,
                Properties.baseURL);

            if (errors.Count > 0)
            {
                errors.Insert(0, request); /* add request on the first position, that it will be diplayed before errors */
                return CreateXml(errors.ToArray());
            }

            var records = new List<RecordQueryResult>();
            List<string> sets = Common.Helper.GetAllSets(set);
            var formatNum = FormatList.Prefix2Int(metadataPrefix);

            EntityManager entityManager = new EntityManager();
            EntityPermissionManager entityPermissionManager = new EntityPermissionManager();
            DatasetManager datasetManager = new DatasetManager();
            OAIHelper oaiHelper = new OAIHelper();

            try
            {
                //1. Get list of all datasetids which shoudl be harvested -
                // ToDo use also the existing parameters like from date
                long? entityTypeId = entityManager.FindByName(typeof(Dataset).Name)?.Id;
                entityTypeId = entityTypeId.HasValue ? entityTypeId.Value : -1;

                // get all datasetids with the last modify date
                List<long> dsvIds = datasetManager.GetDatasetVersionLatestIds();
                // ToDo to get all datasets with the last modfied date, the datasetversionrepo of the dataset manager is used, but its many wrong because of session problem in the past
                List<long> datasetIds = datasetManager.GetDatasetLatestIds();
                datasetIds = datasetManager.DatasetVersionRepo.Query(dsv =>
                    dsvIds.Contains(dsv.Id) &&
                    dsv.Timestamp >= fromDate &&
                    dsv.Timestamp <= untilDate
                    ).Select(dsv => dsv.Dataset.Id).ToList();

                //2. Generate a list of headers
                var recordsQuery = new List<Header>();

                foreach (long id in datasetIds)
                {
                    if (entityPermissionManager.Exists(null, entityTypeId.Value, id))
                    {
                        recordsQuery.Add(oaiHelper.GetHeader(id));
                    }
                }

                if (isSet)
                {
                    recordsQuery = recordsQuery.Where(h => h.OAI_Set.Equals(AppConfiguration.ApplicationName)).ToList();
                }

                int recordsCount = recordsQuery.Count();

                if (recordsCount == 0)
                {
                    return CreateXml(new XElement[] { request, MlErrors.noRecordsMatch });
                }
                else if (isRoundtrip)
                {
                    Properties.resumptionTokens[resumptionToken].CompleteListSize = recordsCount;
                    recordsQuery = recordsQuery.AsEnumerable().Skip(
                        Properties.resumptionTokens[resumptionToken].Cursor.Value).Take(
                        isRecord ? Properties.maxRecordsInList : Properties.maxIdentifiersInList).ToList();
                }
                else if ((isRecord ? Properties.resumeListRecords : Properties.resumeListIdentifiers) &&
                    (isRecord ? recordsCount > Properties.maxRecordsInList
                    : recordsCount > Properties.maxIdentifiersInList))
                {
                    resumptionToken = Common.Helper.CreateGuid();
                    isResumption = true;
                    Properties.resumptionTokens.Add(resumptionToken,
                        new ResumptionToken()
                        {
                            Verb = verb,
                            From = isFrom ? fromDate : null,
                            Until = isUntil ? untilDate : null,
                            MetadataPrefix = metadataPrefix,
                            Set = set,
                            ExpirationDate = DateTime.UtcNow.Add(Properties.expirationTimeSpan),
                            CompleteListSize = recordsCount,
                            Cursor = 0
                        });

                    recordsQuery = recordsQuery.AsEnumerable().Take(
                        isRecord ? Properties.maxRecordsInList : Properties.maxIdentifiersInList).ToList();
                }

                /* get data from database */
                //var recGroup = (from rec in recordsQuery
                //                join omd in context.ObjectMetadata on rec.HeaderId equals omd.ObjectId
                //                join mdt in context.Metadata on omd.MetadataId equals mdt.MetadataId
                //                group new { OmdMetaType = omd.MetadataType, OaiMetaData = mdt } by rec into grp
                //                select grp).ToList();

                /* distribute data into logical units */

                foreach (var header in recordsQuery)
                {
                    long id = oaiHelper.ConvertToId(header.OAI_Identifier);
                    //ToDo add about to the RecordQueryResult object, currently its only null
                    records.Add(new RecordQueryResult(header, oaiHelper.GetMetadata(id, metadataPrefix), null));
                }
            }
            finally
            {
                datasetManager.Dispose();
                entityPermissionManager.Dispose();
            }

            bool isCompleted = isResumption ?
                Properties.resumptionTokens[resumptionToken].Cursor + records.Count >=
                Properties.resumptionTokens[resumptionToken].CompleteListSize :
                false;

            XElement list = new XElement(verb,
                isRecord ?
                    GetListRecords(records, isAbout) :
                    GetListIdentifiers(records),
                isResumption ? /* add resumption token or not */
                    MlEncode.ResumptionToken(Properties.resumptionTokens[resumptionToken], resumptionToken, isCompleted)
                    : null);

            if (isResumption)
            {
                if (isCompleted)
                {
                    Properties.resumptionTokens.Remove(resumptionToken);
                }
                else
                {
                    Properties.resumptionTokens[resumptionToken].Cursor =
                        Properties.resumptionTokens[resumptionToken].Cursor + records.Count;
                }
            }

            return CreateXml(new XElement[] { request, list });
        }

        private static XElement[] GetListIdentifiers(List<RecordQueryResult> records)
        {
            return (from rec in records
                    select MlEncode.HeaderItem(rec.Header, Properties.granularity)).ToArray();
        }

        private static XElement[] GetListRecords(List<RecordQueryResult> records, bool isAbout)
        {
            return (from rec in records
                    select new XElement("record",
                    MlEncode.HeaderItem(rec.Header, Properties.granularity),
                    MlEncode.Metadata(rec.Metadata, Properties.granularity),
                    isAbout ? MlEncode.About(rec.About, Properties.granularity) : null
                   )).ToArray();
        }

        #endregion ListIdentifiers / ListRecords

        #region metadatalist

        public static XDocument ListMetadataFormats(string identifier, List<XElement> errorList)
        {
            List<XElement> errors = errorList;

            bool isIdentifier = !String.IsNullOrEmpty(identifier);

            XElement request = new XElement("request",
                new XAttribute("verb", "ListMetadataFormats"),
                isIdentifier ? new XAttribute("identifier", identifier) : null,
                Properties.baseURL);

            List<OAIMetadataFormat> metadataFormats = new List<OAIMetadataFormat>();

            metadataFormats = FormatList.List.Where(m => m.IsForList == true).ToList();

            if (errors.Count > 0)
            {
                errors.Insert(0, request); /* add request on the first position, that it will be diplayed before errors */
                return CreateXml(errors.ToArray());
            }

            XElement listMetadataFormats = new XElement("ListMetadataFormats",
                from mf in metadataFormats
                where mf.IsForList
                select new XElement("metadataFormat",
                    new XElement("metadataPrefix", mf.Prefix),
                    new XElement("schema", mf.Schema),
                    new XElement("metadataNamespace", mf.Namespace)));

            return CreateXml(new XElement[] { request, listMetadataFormats });
        }

        #endregion metadatalist

        #region listSets

        public static XDocument ListSets(string resumptionToken, bool isRoundtrip, List<XElement> errorList)
        {
            List<XElement> errors = errorList;

            if (!Properties.supportSets)
            {
                errors.Add(MlErrors.noSetHierarchy);
            }

            bool isResumption = !String.IsNullOrEmpty(resumptionToken);
            if (isResumption && !isRoundtrip)
            {
                if (!(Properties.resumptionTokens.ContainsKey(resumptionToken) &&
                    Properties.resumptionTokens[resumptionToken].Verb == "ListSets" &&
                    Properties.resumptionTokens[resumptionToken].ExpirationDate >= DateTime.UtcNow))
                {
                    errors.Insert(0, MlErrors.badResumptionArgument);
                }

                if (errors.Count == 0)
                {
                    return ListSets(resumptionToken, true, new List<XElement>());
                }
            }

            XElement request = new XElement("request",
                new XAttribute("verb", "ListSets"),
                isResumption ? new XAttribute("resumptionToken", resumptionToken) : null,
                Properties.baseURL);

            if (errors.Count > 0)
            {
                errors.Insert(0, request); /* add request on the first position, that it will be diplayed before errors */
                return CreateXml(errors.ToArray());
            }
            OAIHelper oaiHelper = new OAIHelper();
            var sets = oaiHelper.GetAllSets();

            bool isCompleted = isResumption ?
                Properties.resumptionTokens[resumptionToken].Cursor + sets.Count ==
                Properties.resumptionTokens[resumptionToken].CompleteListSize :
                false;

            XElement list = new XElement("ListSets",
                from s in sets
                select new XElement("set",
                    new XElement("setSpec", s.Spec),
                    new XElement("setName", s.Name),
                    String.IsNullOrEmpty(s.Description) ? null
                        : new XElement("setDescription", s.Description),
                    MlEncode.SetDescription(s.AdditionalDescriptions, Properties.granularity)),
                isResumption ? /* add resumption token or not */
                    MlEncode.ResumptionToken(Properties.resumptionTokens[resumptionToken], resumptionToken, isCompleted)
                    : null);

            if (isResumption)
            {
                if (isCompleted)
                {
                    Properties.resumptionTokens.Remove(resumptionToken);
                }
                else
                {
                    Properties.resumptionTokens[resumptionToken].Cursor =
                        Properties.resumptionTokens[resumptionToken].Cursor + sets.Count;
                }
            }

            return CreateXml(new XElement[] { request, list });
        }

        #endregion listSets

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