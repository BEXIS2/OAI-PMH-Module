/*     This file is part of OAI-PMH-.Net.
*
*      OAI-PMH-.Net is free software: you can redistribute it and/or modify
*      it under the terms of the GNU General Public License as published by
*      the Free Software Foundation, either version 3 of the License, or
*      (at your option) any later version.
*
*      OAI-PMH-.Net is distributed in the hope that it will be useful,
*      but WITHOUT ANY WARRANTY; without even the implied warranty of
*      MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
*      GNU General Public License for more details.
*
*      You should have received a copy of the GNU General Public License
*      along with OAI-PMH-.Net.  If not, see <http://www.gnu.org/licenses/>.
*----------------------------------------------------------------------------*/

using BExIS.Modules.OAIPMH.UI.API.Common;
using BExIS.Modules.OAIPMH.UI.Models;
using System.Xml.Linq;
using System;

namespace BExIS.Modules.OAIPMH.UI.API.MdFormats
{
    public static class PanSimple
    {
        public static int MdFormat = (byte)Enums.MetadataFormats.PanSimple;

        public static Metadata Decode(XElement metadata)
        {
            //XElement root = metadata.Element(MlNamespaces.oaiDc + "dc");

            return new Metadata()
            {
                //Title = MlDecode.Element(root, MlNamespaces.dcNs + "title"),
                //Creator = MlDecode.Element(root, MlNamespaces.dcNs + "creator"),
                //Subject = MlDecode.Element(root, MlNamespaces.dcNs + "subject"),
                //Description = MlDecode.Element(root, MlNamespaces.dcNs + "description"),
                //Publisher = MlDecode.Element(root, MlNamespaces.dcNs + "publisher"),
                //Contributor = MlDecode.Element(root, MlNamespaces.dcNs + "contributor"),
                //Date = MlDecode.SafeDateTime(root.Element(MlNamespaces.dcNs + "date")),
                //Type = MlDecode.Element(root, MlNamespaces.dcNs + "type"),
                //Format = MlDecode.Element(root, MlNamespaces.dcNs + "format"),
                //Identifier = MlDecode.Element(root, MlNamespaces.dcNs + "identifier"),
                //Source = MlDecode.Element(root, MlNamespaces.dcNs + "source"),
                //Language = MlDecode.Element(root, MlNamespaces.dcNs + "language"),
                //Relation = MlDecode.Element(root, MlNamespaces.dcNs + "relation"),
                //Coverage = MlDecode.Element(root, MlNamespaces.dcNs + "coverage"),
                //Rights = MlDecode.Element(root, MlNamespaces.dcNs + "rights"),
                MdFormat = (byte)Enums.MetadataFormats.PanSimple
            };
            //throw new NotImplementedException();
        }

        public static XElement Encode(Metadata panSimple, string granularity)
        {
            XElement Element = new XElement(MlNamespaces.psNs + "dataset",
                   /*Namespaces*/
                   new XAttribute(XNamespace.Xmlns + "dc", MlNamespaces.dcNs),
                   new XAttribute(XNamespace.Xmlns + "xsi", MlNamespaces.psXsi),
                   new XAttribute(MlNamespaces.psSchemaLocation + "schemaLocation", MlNamespaces.psSchemaLocation),
                   /* content */
                   /*Subject*/
                   MlEncode.Element(MlNamespaces.dcNs + "title", panSimple.Title),
                   MlEncode.Element(MlNamespaces.dcNs + "creator", panSimple.Creator),
                   MlEncode.Element(MlNamespaces.dcNs + "subject", panSimple.Subject),
                   !panSimple.Date.HasValue ? null
                          : new XElement(MlNamespaces.dcNs + "date",
                              panSimple.Date.Value.ToUniversalTime().ToString(granularity)),
                   MlEncode.Element(MlNamespaces.dcNs + "type", panSimple.Type),
                   MlEncode.Element(MlNamespaces.dcNs + "format", panSimple.Format),
                   MlEncode.Element(MlNamespaces.dcNs + "identifier", panSimple.Identifier),
                   MlEncode.Element(MlNamespaces.dcNs + "source", panSimple.Source),
                   MlEncode.Element(MlNamespaces.dcNs + "language", panSimple.Language),
                   MlEncode.Element(MlNamespaces.dcNs + "coverage", panSimple.Coverage),
                   MlEncode.Element(MlNamespaces.dcNs + "rights", panSimple.Rights),

                   /*PanSimple Extentions*/
                   MlEncode.Element(MlNamespaces.psNs + "keyword", panSimple.Keyword),
                   MlEncode.Element(MlNamespaces.psNs + "parameter", panSimple.Parameter),
                   MlEncode.Element(MlNamespaces.psNs + "method", panSimple.Method),
                   MlEncode.Element(MlNamespaces.psNs + "sensor", panSimple.Sensor),
                   MlEncode.Element(MlNamespaces.psNs + "feature", panSimple.Feature),
                   MlEncode.Element(MlNamespaces.psNs + "taxonomy", panSimple.Taxonomy),
                   MlEncode.Element(MlNamespaces.psNs + "platform", panSimple.Platform),
                   MlEncode.Element(MlNamespaces.psNs + "project", panSimple.Project),
                   MlEncode.Element(MlNamespaces.psNs + "habitat", panSimple.Habitat),
                   MlEncode.Element(MlNamespaces.psNs + "stratigraphy", panSimple.Stratigraphy),
                   MlEncode.Element(MlNamespaces.psNs + "pangaea-tech-keyword", panSimple.PangaeaTechKeyword),

                   /*Links*/

                   new XElement(MlNamespaces.psNs + "linkage",
                       new XAttribute("type", "metadata"),
                       new XAttribute("accessRestricted", "false"),
                       panSimple.AddtionalData[Metadata.METADATA_URL].ToString()),

                   new XElement(MlNamespaces.psNs + "linkage",
                       new XAttribute("type", "data"),
                       new XAttribute("accessRestricted", "false"),
                       panSimple.AddtionalData[Metadata.DATA_URL].ToString()),

                   /*relinks*/
                   MlEncode.Element(MlNamespaces.psNs + "parentIdentifer", panSimple.Relation),
                   MlEncode.Element(MlNamespaces.psNs + "additionalContent", panSimple.Description),
                   MlEncode.Element(MlNamespaces.psNs + "dataCenter", panSimple.Publisher),
                   MlEncode.Element(MlNamespaces.psNs + "pricipalInvestigator", panSimple.Contributor),

                  /*Converage*/
                  new XElement(MlNamespaces.psNs + "coverage",
                   MlEncode.Element(MlNamespaces.psNs + "northBoundLatitude",
                       panSimple.CoverageComplex.NorthBoundLatitude == 0 ? null : panSimple.CoverageComplex.NorthBoundLatitude.ToString()),
                   MlEncode.Element(MlNamespaces.psNs + "westBoundLongitude",
                       panSimple.CoverageComplex.WestBoundLongitude == 0 ? null : panSimple.CoverageComplex.WestBoundLongitude.ToString()),
                   MlEncode.Element(MlNamespaces.psNs + "southhBoundLatitude",
                       panSimple.CoverageComplex.SouthBoundLatitude == 0 ? null : panSimple.CoverageComplex.SouthBoundLatitude.ToString()),
                   MlEncode.Element(MlNamespaces.psNs + "eastBoundLongitude",
                       panSimple.CoverageComplex.EastBoundLongitude == 0 ? null : panSimple.CoverageComplex.EastBoundLongitude.ToString()),

                   MlEncode.Element(MlNamespaces.psNs + "location", panSimple.CoverageComplex.Location),
                   MlEncode.Element(MlNamespaces.psNs + "minEvelation", panSimple.CoverageComplex.MinElevation),
                   MlEncode.Element(MlNamespaces.psNs + "maxEvelation", panSimple.CoverageComplex.MaxElevation),
                   MlEncode.Element(MlNamespaces.psNs + "StartDate", panSimple.CoverageComplex.StartDate),
                   MlEncode.Element(MlNamespaces.psNs + "EndData", panSimple.CoverageComplex.EndDate))
                  );

            XAttribute attr = new XAttribute(XNamespace.Xmlns + "ps", MlNamespaces.psNs);

            Element.Add(attr);

            return Element;
        }
    }
}