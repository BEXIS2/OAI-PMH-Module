using BExIS.Dlm.Services.Data;
using BExIS.Modules.OAIPMH.UI.API.Common;
using BExIS.Modules.OAIPMH.UI.Models;
using BExIS.Xml.Helpers;
using System;
using System.Configuration;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using Vaiona.Utils.Cfg;
using System.Linq;
using BExIS.Modules.OAIPMH.UI.Helper;

namespace BExIS.Modules.OAIPMH.UI.App_Start
{
    public static class OaiPmhConfig
    {
        public static void Register()
        {
            XDocument config;

            try
            {
                OAIHelper helper = new OAIHelper();

                string filepath = Path.Combine(AppConfiguration.GetModuleWorkspacePath("OAIPMH"), "OAIPMH.Settings.xml");
                if (!File.Exists(filepath)) throw new NullReferenceException("OAIPMH.Settings.xml is missing in the workspace.");

                using (var reader = XmlReader.Create(filepath))
                {
                    config = XDocument.Load(reader);

                    if (config != null)
                    {
                        Properties props = new Properties();

                        /* Set default settings */
                        /* Identify properties */
                        string repositoryName = getValue("RepositoryName", config);
                        if (string.IsNullOrEmpty(repositoryName)) repositoryName = AppConfiguration.ApplicationName;
                        props["RepositoryName"] = new Property() { Key = "RepositoryName", Value = AppConfiguration.ApplicationName, Section = "ip" };

                        props["BaseURL"] = new Property() { Key = "BaseURL", Value = getValue("BaseURL", config), Section = "ip" };
                        props["ProtocolVersion"] = new Property() { Key = "ProtocolVersion", Value = "2.0", Section = "ip" };

                        //ToDo SQLDATE CHECK
                        props["EarliestDatestamp"] = new Property() { Key = "EarliestDatestamp", Value = "" };

                        props["DeletedRecord"] = new Property() { Key = "DeletedRecord", Value = getValue("DeletedRecord", config), Section = "ip" };
                        props["Granularity"] = new Property() { Key = "Granularity", Value = getValue("Granularity", config), Section = "ip" };

                        //send admin email
                        string adminEmail = getValue("AdminEmails", config);
                        if (string.IsNullOrEmpty(adminEmail)) adminEmail = ConfigurationManager.AppSettings["SystemEmail"].ToString();
                        props["AdminEmails"] = new Property() { Key = "AdminEmails", Value = ConfigurationManager.AppSettings["SystemEmail"].ToString(), Section = "ip" };

                        props["Compression"] = new Property() { Key = "Compression", Value = getValue("Compression", config), Section = "ip" };
                        props["Description"] = new Property() { Key = "Description", Value = getValue("Description", config), Section = "ip" };
                        /* Data provider properties */
                        props["SupportSets"] = new Property() { Key = "SupportSets", Value = getValue("SupportSets", config), Section = "dpp" };
                        props["ResumeListSets"] = new Property() { Key = "ResumeListSets", Value = getValue("ResumeListSets", config), Section = "dpp" };
                        props["MaxSetsInList"] = new Property() { Key = "MaxSetsInList", Value = getValue("MaxSetsInList", config), Section = "dpp" };
                        props["ResumeListIdentifiers"] = new Property() { Key = "ResumeListIdentifiers", Value = getValue("ResumeListIdentifiers", config), Section = "dpp" };
                        props["MaxIdentifiersInList"] = new Property() { Key = "MaxIdentifiersInList", Value = getValue("MaxIdentifiersInList", config), Section = "dpp" };
                        props["ResumeListRecords"] = new Property() { Key = "ResumeListRecords", Value = getValue("ResumeListRecords", config), Section = "dpp" };
                        props["MaxRecordsInList"] = new Property() { Key = "MaxRecordsInList", Value = getValue("MaxRecordsInList", config), Section = "dpp" };
                        props["ExpirationTimeSpan"] = new Property() { Key = "ExpirationTimeSpan", Value = getValue("ExpirationTimeSpan", config), Section = "dpp" };
                        props["LoadAbout"] = new Property() { Key = "LoadAbout", Value = "True", Section = getValue("LoadAbout", config) };
                    }
                    else
                    {
                        throw new NullReferenceException("OAIPMH.Settings.xml is missing in the workspace.");
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private static string getValue(string key, XDocument config)
        {
            XElement tmp = XmlUtility.GetXElementByAttribute("entry", "key", key, config);
            if (tmp != null && tmp.HasAttributes && tmp.Attribute("value") != null)
            {
                XAttribute attr = tmp.Attribute("value");
                return attr.Value;
            }

            return "";
        }
    }
}