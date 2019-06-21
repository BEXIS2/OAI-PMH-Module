using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BExIS.Modules.OAIPMH.UI.Models
{
    public class Set
    {
        public string Name { get; set; }
        public string Spec { get; set; }
        public string Description { get; set; }
        public List<Metadata> AdditionalDescriptions { get; set; }
    }
}