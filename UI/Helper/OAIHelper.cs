using BExIS.Dim.Entities.Mapping;
using BExIS.Dim.Helpers.Mapping;
using BExIS.Dlm.Entities.Data;
using BExIS.Dlm.Services.Data;
using BExIS.Modules.OAIPMH.UI.API.MdFormats;
using BExIS.Modules.OAIPMH.UI.Models;
using BExIS.Xml.Helpers;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Xml.Linq;
using Vaiona.Utils.Cfg;
using Vaiona.Model.MTnt;
using System.Collections.Generic;
using BExIS.Dlm.Entities.DataStructure;

namespace BExIS.Modules.OAIPMH.UI.Helper
{
    public class OAIHelper
    {
        // get header
        public Header GetHeader(long id)
        {
            if (id <= 0) return null;

            DatasetManager datasetManager = new DatasetManager();

            try
            {
                DatasetVersion datasetVersion = datasetManager.GetDatasetLatestVersion(id);
                DateTime lastModified = datasetVersion.Timestamp;

                Header header = new Header();
                header.OAI_Identifier = ConvertToOaiId(id);
                header.OAI_Set = AppConfiguration.ApplicationName; // ToDo what is a oai_set?
                header.Datestamp = lastModified;//Date Last Modified;
                header.Deleted = false;
                header.OAIDataProviderId = -1;

                return header;
            }
            finally
            {
                datasetManager.Dispose();
            }
        }

        /// <summary>
        /// get metadata informations the latest version of a dataset
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Metadata GetMetadata(long id, string metadataPrefix)
        {
            DatasetManager datasetManager = new DatasetManager();
            try
            {
                DatasetVersion datasetVersion = datasetManager.GetDatasetLatestVersion(id);

                Metadata metadata = new Metadata();
                long mdId = datasetVersion.Dataset.MetadataStructure.Id;
                XDocument xMetadata = XmlUtility.ToXDocument(datasetVersion.Metadata);

                metadata.Title = string.Join("][", MappingUtils.GetValuesFromMetadata(Convert.ToInt64(Key.Title), LinkElementType.Key, mdId, xMetadata));
                metadata.Description = string.Join("][", MappingUtils.GetValuesFromMetadata(Convert.ToInt64(Key.Description), LinkElementType.Key, mdId, xMetadata));

                metadata.Creator = string.Join("][", MappingUtils.GetValuesFromMetadata(Convert.ToInt64(Key.Author), LinkElementType.Key, mdId, xMetadata));

                // string to date
                string date = MappingUtils.GetValuesFromMetadata(Convert.ToInt64(Key.DataLastModified), LinkElementType.Key, mdId, xMetadata).FirstOrDefault();
                DateTime dt;
                if (DateTime.TryParse(date, out dt))
                    metadata.Date = dt;
                else // if date is not set in the metadata, lassd motified date of the version is loaded
                {
                    metadata.Date = datasetVersion.Timestamp;
                }

                metadata.Subject = string.Join("][", MappingUtils.GetValuesFromMetadata(Convert.ToInt64(Key.Subject), LinkElementType.Key, mdId, xMetadata));

                // if the publisher is not set in the metadata, its loaded from the tenant.
                var publishers = string.Join("][", MappingUtils.GetValuesFromMetadata(Convert.ToInt64(Key.Publisher), LinkElementType.Key, mdId, xMetadata));
                if (!publishers.Any())
                    metadata.Publisher = AppConfiguration.ApplicationName;

                metadata.Contributor = string.Join("][", MappingUtils.GetValuesFromMetadata(Convert.ToInt64(Key.Contributor), LinkElementType.Key, mdId, xMetadata));

                //if type is not in the metadata default it is "dataset"
                var types = string.Join("][", MappingUtils.GetValuesFromMetadata(Convert.ToInt64(Key.Type), LinkElementType.Key, mdId, xMetadata));
                if (!types.Any()) types = getEntityName(id);
                metadata.Type = types;

                //if language is not in the metadata default it is "english"
                var formats = string.Join("][", MappingUtils.GetValuesFromMetadata(Convert.ToInt64(Key.Format), LinkElementType.Key, mdId, xMetadata));
                if (!formats.Any()) formats = string.Join("][", getFormats(id));
                metadata.Format = formats;

                metadata.Identifier = string.Join("][", MappingUtils.GetValuesFromMetadata(Convert.ToInt64(Key.Identifier), LinkElementType.Key, mdId, xMetadata));
                metadata.Source = string.Join("][", MappingUtils.GetValuesFromMetadata(Convert.ToInt64(Key.Source), LinkElementType.Key, mdId, xMetadata));

                //if language is not in the metadata default it is "english"
                var language = string.Join("][", MappingUtils.GetValuesFromMetadata(Convert.ToInt64(Key.Language), LinkElementType.Key, mdId, xMetadata));
                if (language.Any()) metadata.Language = "English";

                metadata.Relation = string.Join("][", MappingUtils.GetValuesFromMetadata(Convert.ToInt64(Key.Relation), LinkElementType.Key, mdId, xMetadata));
                metadata.Coverage = string.Join("][", MappingUtils.GetValuesFromMetadata(Convert.ToInt64(Key.Coverage), LinkElementType.Key, mdId, xMetadata));
                metadata.Rights = string.Join("][", MappingUtils.GetValuesFromMetadata(Convert.ToInt64(Key.Rights), LinkElementType.Key, mdId, xMetadata));

                //relinks
                metadata.ParentIdentifier = string.Join("][", MappingUtils.GetValuesFromMetadata(Convert.ToInt64(Key.Relation), LinkElementType.Key, mdId, xMetadata));
                metadata.AdditionalContent = string.Join("][", MappingUtils.GetValuesFromMetadata(Convert.ToInt64(Key.Description), LinkElementType.Key, mdId, xMetadata));
                metadata.PrincipalInvestigator = string.Join("][", MappingUtils.GetValuesFromMetadata(Convert.ToInt64(Key.Contributor), LinkElementType.Key, mdId, xMetadata));

                //set metadata prefix
                var formatNum = FormatList.Prefix2Int(metadataPrefix);
                metadata.MdFormat = formatNum;

                // linkTypes
                metadata.DataCenter = AppConfiguration.ApplicationName;

                //ToDo add version to the path
                string server = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);
                string url = server + "/ddm/data/showdata/" + id;
                metadata.AddtionalData.Add(Metadata.METADATA_URL.ToString(), url);
                metadata.AddtionalData.Add(Metadata.DATA_URL.ToString(), url);

                metadata.CoverageComplex = new Coverage();
                metadata.CoverageComplex.NorthBoundLatitude = Convert.ToInt32(MappingUtils.GetValuesFromMetadata(Convert.ToInt64(Key.NorthBoundLatitude), LinkElementType.Key, mdId, xMetadata).FirstOrDefault());
                metadata.CoverageComplex.WestBoundLongitude = Convert.ToInt32(MappingUtils.GetValuesFromMetadata(Convert.ToInt64(Key.WestBoundLongitude), LinkElementType.Key, mdId, xMetadata).FirstOrDefault());
                metadata.CoverageComplex.SouthBoundLatitude = Convert.ToInt32(MappingUtils.GetValuesFromMetadata(Convert.ToInt64(Key.SouthBoundLatitude), LinkElementType.Key, mdId, xMetadata).FirstOrDefault());
                metadata.CoverageComplex.EastBoundLongitude = Convert.ToInt32(MappingUtils.GetValuesFromMetadata(Convert.ToInt64(Key.EastBoundLongitude), LinkElementType.Key, mdId, xMetadata).FirstOrDefault());
                metadata.CoverageComplex.Location = string.Join("][", MappingUtils.GetValuesFromMetadata(Convert.ToInt64(Key.Coverage), LinkElementType.Key, mdId, xMetadata));
                metadata.CoverageComplex.MinElevation = MappingUtils.GetValuesFromMetadata(Convert.ToInt64(Key.MinEvelation), LinkElementType.Key, mdId, xMetadata).FirstOrDefault();
                metadata.CoverageComplex.MaxElevation = MappingUtils.GetValuesFromMetadata(Convert.ToInt64(Key.MaxEvelation), LinkElementType.Key, mdId, xMetadata).FirstOrDefault();

                metadata.Keyword = string.Join("][", MappingUtils.GetValuesFromMetadata(Convert.ToInt64(Key.Keyword), LinkElementType.Key, mdId, xMetadata));

                var parameters = string.Join("][", MappingUtils.GetValuesFromMetadata(Convert.ToInt64(Key.Parameter), LinkElementType.Key, mdId, xMetadata));
                if (string.IsNullOrEmpty(parameters)) parameters = string.Join("][", getVariableNames(id));
                metadata.Parameter = parameters;

                metadata.Method = string.Join("][", MappingUtils.GetValuesFromMetadata(Convert.ToInt64(Key.Method), LinkElementType.Key, mdId, xMetadata));
                metadata.Sensor = string.Join("][", MappingUtils.GetValuesFromMetadata(Convert.ToInt64(Key.Sensor), LinkElementType.Key, mdId, xMetadata));
                metadata.Feature = string.Join("][", MappingUtils.GetValuesFromMetadata(Convert.ToInt64(Key.Feature), LinkElementType.Key, mdId, xMetadata));
                metadata.Taxonomy = string.Join("][", MappingUtils.GetValuesFromMetadata(Convert.ToInt64(Key.Taxonomy), LinkElementType.Key, mdId, xMetadata));
                metadata.Platform = string.Join("][", MappingUtils.GetValuesFromMetadata(Convert.ToInt64(Key.Platform), LinkElementType.Key, mdId, xMetadata));
                metadata.Project = string.Join("][", MappingUtils.GetValuesFromMetadata(Convert.ToInt64(Key.ProjectTitle), LinkElementType.Key, mdId, xMetadata));
                metadata.Habitat = string.Join("][", MappingUtils.GetValuesFromMetadata(Convert.ToInt64(Key.Habitat), LinkElementType.Key, mdId, xMetadata));
                metadata.Stratigraphy = string.Join("][", MappingUtils.GetValuesFromMetadata(Convert.ToInt64(Key.Stratigraphy), LinkElementType.Key, mdId, xMetadata));

                return metadata;
            }
            finally
            {
                datasetManager.Dispose();
            }
        }

        //helper

        public string ConvertToOaiId(long id)
        {
            if (id <= 0) return "";

            //Todo thinking abaout a good id for oai pmh
            //https://localhost:44345/api/oai
            var tmp = Properties.baseURL.Split('/');
            string ServerName = tmp[2];

            if (ServerName.Contains("www.")) ServerName.Replace("www.", "");

            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("oai:{0}:{1}", ServerName, id);

            return sb.ToString();
        }

        public long ConvertToId(string OaiId)
        {
            if (string.IsNullOrEmpty(OaiId)) return -1;

            var list = OaiId.Split(':');
            var last = list[list.Length - 1];

            long id;
            if (long.TryParse(last, out id))
            {
                return id;
            }

            return -1;
        }

        public string EarliestDatestamp()
        {
            DatasetManager datasetManager = new DatasetManager();

            try
            {
                var firstdsv = datasetManager.DatasetVersionRepo.Get().OrderBy(dsv => dsv.Timestamp).FirstOrDefault();
                string format = API.Common.Enums.Granularity.DateTime.Replace("'", "");

                if (firstdsv != null) return firstdsv.Timestamp.ToString(format);

                return "";
            }
            finally
            {
                datasetManager.Dispose();
            }
        }

        public List<Set> GetAllSets()
        {
            //ToDo Add More Sets

            List<Set> tmp = new List<Set>();

            Set s = new Set();
            s.Name = AppConfiguration.ApplicationName;
            s.Spec = "";
            s.Description = AppConfiguration.ApplicationVersion;

            tmp.Add(s);

            return tmp;
        }

        private string getEntityName(long datasetid)
        {
            XmlDatasetHelper xmlDatasetHelper = new XmlDatasetHelper();
            return xmlDatasetHelper.GetEntityType(datasetid);
        }

        private List<string> getFormats(long datasetid)
        {
            List<string> formats = new List<string>();
            bool isStructured = true;

            if (isStructured)
            {
                formats.Add("text/plain");
                formats.Add("text/tab-separated-values");
                formats.Add("text/comma-separated-values");
                formats.Add("application/vnd.openxmlformats - officedocument.spreadsheetml.sheet");
            }
            else
            {
                //ToDo add unsrtuctured Formats here
            }

            return formats;
        }

        private List<string> getVariableNames(long datasetId)
        {
            List<string> vars = new List<string>();
            DatasetManager datasetManager = new DatasetManager();
            try
            {
                var dataset = datasetManager.GetDataset(datasetId);

                if (dataset != null && dataset.DataStructure != null && dataset.DataStructure.Self is StructuredDataStructure)
                {
                    StructuredDataStructure sds = (StructuredDataStructure)dataset.DataStructure.Self;
                    if (sds != null) sds.Variables.ToList().ForEach(v => vars.Add(v.Label));
                }

                return vars;
            }
            catch (Exception ex)
            {
                return new List<string>();
            }
            finally
            {
            }
        }
    }
}