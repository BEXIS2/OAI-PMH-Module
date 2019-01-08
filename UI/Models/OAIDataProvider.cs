namespace BExIS.Modules.OAIPMH.UI.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Threading;

    [Table("OAIDataProvider")]
    public partial class OAIDataProvider
    {
        public OAIDataProvider()
        {
            this.Headers = new HashSet<Header>();
        }

        [Key]
        [Required]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int OAIDataProviderId { get; set; }
        public string BaseURL { get; set; }
        public string Granularity { get; set; }
        public Nullable<System.DateTime> LastHarvesting { get; set; }
        public string RepositoryName { get; set; }
        public string ProtocolVersion { get; set; }
        public string AdminEmail { get; set; }
        public Nullable<System.DateTime> EarliestDatestamp { get; set; }
        public string DeletedRecord { get; set; }
        public string Compression { get; set; }

        /* Properties for file harvesting */
        public string Function { get; set; }
        public string FirstSource { get; set; }
        public string SecondSource { get; set; }

        public virtual ICollection<Header> Headers { get; set; }
    }

    public class DataProviderProperties
    {
        public int OAIDataProviderId { get; set; }
        public string BaseURL { get; set; }
        public string RepositoryName { get; set; }
        public string From { get; set; }
        public string Until { get; set; }
        public bool Exclude { get; set; }
        public bool FullHarvestDelete { get; set; }
        public bool HarvestDeleteFiles { get; set; }
        public string MetadataPrefix { get; set; }
        public DataProviderHarvestStats Stats { get; set; }
    }

    public class DataProviderHarvestStats
    {
        public int OAIDataProviderId { get; set; }
        public int RatioDone { get; set; }
        public int RatioAll { get; set; }
        public string Status { get; set; }
        public string Message { get; set; }
    }

    public class DataProviderIntern
    {
        public OAIDataProvider DataProvider { get; set; }
        public DataProviderProperties HarvestOptions { get; set; }
        public CancellationTokenSource TokenSource { get; set; }
    }
}