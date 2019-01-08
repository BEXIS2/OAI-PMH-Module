namespace BExIS.Modules.OAIPMH.UI.API.Common
{
    public class Enums
    {
        /* What to do with duplicates when harvesting */
        public enum DeDuplication
        {
            AddDuplicate,

            UpdateOriginal,

            Skip
        }

        /* Support for deleted records */
        public class DeletedRecords
        {
            /* No information about deletion is kept */
            public const string No = "no";

            /* Informations about deletion are maintained with no time limit */
            public const string Persistent = "persistent";

            /* No guaratee that a list of deletions is maintained persistently or consistently */
            public const string Transient = "transient";
        }

        public class Granularity
        {
            /* UTC year format */
            public const string Year = "yyyy";

            /* UTC year and month format */
            public const string YearAndMonth = "yyyy'-'MM";

            /* UTC date format */
            public const string Date = "yyyy'-'MM'-'dd";

            /* UTC date time format */
            public const string DateTime = "yyyy'-'MM'-'dd'T'HH':'mm':'ss'Z'";
        }

        public class ObjectType
        {
            public static byte OAIRecord = 0;
            public static byte OAISet = 1;
        }

        public class MetadataType
        {
            public static byte Metadata = 0;
            public static byte About = 1;
        }

        /* List of supported metadata formats 2^* values */
        public enum MetadataFormats
        {
            None = 0,
            DublinCore = 1,
            Provenance = 2
        }
    }
}