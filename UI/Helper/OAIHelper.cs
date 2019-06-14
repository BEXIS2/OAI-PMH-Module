using BExIS.Dim.Entities.Mapping;
using BExIS.Dim.Helpers.Mapping;
using BExIS.Dlm.Entities.Data;
using BExIS.Dlm.Services.Data;
using BExIS.Modules.OAIPMH.UI.API.MdFormats;
using BExIS.Modules.OAIPMH.UI.Models;
using BExIS.Xml.Helpers;
using System;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Vaiona.Utils.Cfg;

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
                header.OAI_Set = ""; // ToDo what is a oai_set?
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

                metadata.Title = MappingUtils.GetValuesFromMetadata(Convert.ToInt64(Key.Title), LinkElementType.Key, mdId, xMetadata).FirstOrDefault();
                metadata.Description = MappingUtils.GetValuesFromMetadata(Convert.ToInt64(Key.Description), LinkElementType.Key, mdId, xMetadata).FirstOrDefault();

                // string to date
                string tmp = MappingUtils.GetValuesFromMetadata(Convert.ToInt64(Key.DataLastModified), LinkElementType.Key, mdId, xMetadata).FirstOrDefault();
                DateTime dt;
                if (DateTime.TryParse(tmp, out dt))
                    metadata.Date = dt;

                metadata.Subject = MappingUtils.GetValuesFromMetadata(Convert.ToInt64(Key.Subject), LinkElementType.Key, mdId, xMetadata).FirstOrDefault();
                metadata.Publisher = MappingUtils.GetValuesFromMetadata(Convert.ToInt64(Key.Publisher), LinkElementType.Key, mdId, xMetadata).FirstOrDefault();
                metadata.Contributor = MappingUtils.GetValuesFromMetadata(Convert.ToInt64(Key.Contributor), LinkElementType.Key, mdId, xMetadata).FirstOrDefault();
                metadata.Type = MappingUtils.GetValuesFromMetadata(Convert.ToInt64(Key.Type), LinkElementType.Key, mdId, xMetadata).FirstOrDefault();
                metadata.Format = MappingUtils.GetValuesFromMetadata(Convert.ToInt64(Key.Format), LinkElementType.Key, mdId, xMetadata).FirstOrDefault();
                metadata.Identifier = MappingUtils.GetValuesFromMetadata(Convert.ToInt64(Key.Identifier), LinkElementType.Key, mdId, xMetadata).FirstOrDefault();
                metadata.Source = MappingUtils.GetValuesFromMetadata(Convert.ToInt64(Key.Source), LinkElementType.Key, mdId, xMetadata).FirstOrDefault();
                metadata.Language = MappingUtils.GetValuesFromMetadata(Convert.ToInt64(Key.Language), LinkElementType.Key, mdId, xMetadata).FirstOrDefault();
                metadata.Relation = MappingUtils.GetValuesFromMetadata(Convert.ToInt64(Key.Relation), LinkElementType.Key, mdId, xMetadata).FirstOrDefault();
                metadata.Coverage = MappingUtils.GetValuesFromMetadata(Convert.ToInt64(Key.Coverage), LinkElementType.Key, mdId, xMetadata).FirstOrDefault();
                metadata.Rights = MappingUtils.GetValuesFromMetadata(Convert.ToInt64(Key.Rights), LinkElementType.Key, mdId, xMetadata).FirstOrDefault();

                //relinks
                metadata.ParentIdentifier = MappingUtils.GetValuesFromMetadata(Convert.ToInt64(Key.Relation), LinkElementType.Key, mdId, xMetadata).FirstOrDefault();
                metadata.AdditionalContent = MappingUtils.GetValuesFromMetadata(Convert.ToInt64(Key.Description), LinkElementType.Key, mdId, xMetadata).FirstOrDefault();
                //metadata.DataCenter = MappingUtils.GetValuesFromMetadata(Convert.ToInt64(Key.Publisher), LinkElementType.Key, mdId, xMetadata).FirstOrDefault();
                metadata.PrincipalInvestigator = MappingUtils.GetValuesFromMetadata(Convert.ToInt64(Key.Contributor), LinkElementType.Key, mdId, xMetadata).FirstOrDefault();

                //set metadata prefix
                var formatNum = FormatList.Prefix2Int(metadataPrefix);
                metadata.MdFormat = formatNum;

                // linkTypes
                metadata.DataCenter = AppConfiguration.ApplicationName;

                metadata.AddtionalData.Add(Metadata.METADATA_URL.ToString(), "metadata url");
                metadata.AddtionalData.Add(Metadata.DATA_URL.ToString(), "data url");

                metadata.CoverageComplex = new Coverage();
                metadata.CoverageComplex.Location = MappingUtils.GetValuesFromMetadata(Convert.ToInt64(Key.Coverage), LinkElementType.Key, mdId, xMetadata).FirstOrDefault();

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

            string ServerName = Properties.baseURL;

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
    }
}