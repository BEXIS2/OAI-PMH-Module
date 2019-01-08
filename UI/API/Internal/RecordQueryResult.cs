using BExIS.Modules.OAIPMH.UI.Models;
using System.Collections.Generic;
using System.Linq;

namespace BExIS.Modules.OAIPMH.UI.API.Internal
{
    public class RecordQueryResult
    {

        public RecordQueryResult()
        { }

        public RecordQueryResult(Header header, Metadata metadata)
        {
            this.Header = header;
            this.Metadata = metadata;
        }
        public RecordQueryResult(Header header, Metadata metadata, IList<Metadata> about)
        {
            this.Header = header;
            this.Metadata = metadata;
            this.About = about == null ? null : about.ToList();
        }
        public RecordQueryResult(Header header, Metadata metadata, IEnumerable<Metadata> about)
        {
            this.Header = header;
            this.Metadata = metadata;
            this.About = about == null ? null : about.ToList();
        }

        public Header Header { get; set; }
        public Metadata Metadata { get; set; }
        public List<Metadata> About { get; set; }


        //ToDo Store To Database

        //public static void AddRecordToDatabase(
        //    RecordQueryResult record,
        //    OaiPmhContext context,
        //    OAIDataProvider dp,
        //    string metadataPrefix,
        //    DateTime harvestDate,
        //    bool addProvenance,
        //    bool createNewIdentifier,
        //    string identifierBase,
        //    bool isHarvestDateTime)
        //{
        //    if (addProvenance)
        //    {
        //        record.About.Add(Provenance.NewMeta(harvestDate,
        //            isHarvestDateTime,
        //            createNewIdentifier,
        //            dp.BaseURL,
        //            record.Header.OAI_Identifier,
        //            record.Header.Datestamp.HasValue ? record.Header.Datestamp.Value : DateTime.MinValue,
        //            record.Header.IsDatestampDateTime,
        //            FormatList.GetNamespace(metadataPrefix)));
        //    }

        //    /* add header */
        //    Header.AddRecHeaderToDatabase(
        //        context,
        //        record.Header,
        //        dp,
        //        createNewIdentifier,
        //        identifierBase);

        //    /* add metadata */
        //    DbQueries.AddRecMetadataToDatabase(
        //        context,
        //        record.Header.HeaderId,
        //        record.Metadata);

        //    /* add about */
        //    DbQueries.AddRecAboutToDatabase(
        //        context,
        //        record.Header.HeaderId,
        //        record.About);
        //}
    }
}