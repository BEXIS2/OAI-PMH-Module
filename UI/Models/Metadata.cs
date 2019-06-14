namespace BExIS.Modules.OAIPMH.UI.Models
{
    using System;
    using System.Collections.Generic;

    //[Table("Metadata")]
    public partial class Metadata
    {
        public static string METADATA_URL = "METADATA_URL";
        public static string DATA_URL = "DATA_URL";

        //[Key]
        //[Required]
        //[DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int MetadataId { get; set; }

        //[Required]
        public int MdFormat { get; set; }

        public string Title { get; set; }
        public string Creator { get; set; }
        public string Subject { get; set; }
        public string Description { get; set; }
        public string Contributor { get; set; }
        public string Publisher { get; set; }
        public Nullable<System.DateTime> Date { get; set; }
        public string Type { get; set; }
        public string Format { get; set; }
        public string Identifier { get; set; }
        public string Source { get; set; }
        public string Language { get; set; }
        public string Relation { get; set; }
        public string Coverage { get; set; }
        public string Rights { get; set; }
        public Nullable<int> AdditionalInt1 { get; set; }
        public Nullable<System.DateTime> AdditionalDateTime1 { get; set; }
        public Nullable<bool> AdditionalBool1 { get; set; }
        public Nullable<bool> AdditionalBool2 { get; set; }
        public Nullable<bool> AdditionalBool3 { get; set; }

        // PanSimple Extentions

        public string Keyword { get; set; }
        public string Parameter { get; set; }
        public string Method { get; set; }
        public string Sensor { get; set; }
        public string Feature { get; set; }
        public string Taxonomy { get; set; }
        public string Platform { get; set; }
        public string Project { get; set; }
        public string Habitat { get; set; }
        public string Stratigraphy { get; set; }

        public string PangaeaTechKeyword { get; set; }

        //Coverage
        public Coverage CoverageComplex { get; set; }

        //relinks
        // elemente die in dublin core vorhanden aber nochmal unter einen anderen name
        // angelegt werden

        public string ParentIdentifier { get; set; } //dc:releation
        public string AdditionalContent { get; set; } //dc:description
        public string DataCenter { get; set; } //dc:publisher
        public string PrincipalInvestigator { get; set; } //dc:contributer

        public Dictionary<string, object> AddtionalData { get; set; }

        public Metadata()
        {
            CoverageComplex = new Coverage();
            AddtionalData = new Dictionary<string, object>();
        }
    }

    public class Coverage
    {
        public double NorthBoundLatitude { get; set; }
        public double WestBoundLongitude { get; set; }
        public double SouthBoundLatitude { get; set; }
        public double EastBoundLongitude { get; set; }
        public string Location { get; set; }
        public string MinElevation { get; set; }
        public string MaxElevation { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
    }
}