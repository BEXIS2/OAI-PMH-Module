using System;

namespace BExIS.Modules.OAIPMH.UI.API.Common
{
    public class ResumptionToken
    {
        public string Verb { get; set; }
        public DateTime? From { get; set; }
        public DateTime? Until { get; set; }
        public string MetadataPrefix { get; set; }
        public string Set { get; set; }

        public DateTime? ExpirationDate { get; set; }
        public int? CompleteListSize { get; set; }
        public int? Cursor { get; set; }
    }
}