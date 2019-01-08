namespace BExIS.Modules.OAIPMH.UI.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("Property")]
    public partial class Property
    {
        [Key]
        [Required]
        public string Key { get; set; }
        [Required]
        public string Value { get; set; }
        public string Section { get; set; }
    }
}