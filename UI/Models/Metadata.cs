namespace BExIS.Modules.OAIPMH.UI.Models
{
    using System;

    //[Table("Metadata")]
    public partial class Metadata
    {
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
    }
}