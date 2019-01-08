namespace BExIS.Modules.OAIPMH.UI.Models
{
    using System;

    //[Table("Header")]
    public partial class Header
    {
        //[Key]
        //[Required]
        //[DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int HeaderId { get; set; }
        public Nullable<System.DateTime> EnterDate { get; set; }
        public string OAI_Identifier { get; set; }
        public string OAI_Set { get; set; }
        public Nullable<System.DateTime> Datestamp { get; set; }
        //[Required]
        public bool Deleted { get; set; }
        public string FilePath { get; set; }

        //[ForeignKey("OAIDataProviderId")]
        public virtual OAIDataProvider OAIDataProvider { get; set; }
        public Nullable<int> OAIDataProviderId { get; set; }


        //ToDo check storing of IsDatestampDateTime
        public bool IsDatestampDateTime { get; set; }


    }
}