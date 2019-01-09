using BExIS.Modules.OAIPMH.UI.API.Common;
using BExIS.Modules.OAIPMH.UI.Models;
using System;

namespace BExIS.Modules.OAIPMH.UI.App_Start
{
    public static class OaiPmhConfig
    {
        public static void Register()
        {

            Properties props = new Properties();

            /* Set default settings */
            /* Identify properties */
            props["RepositoryName"] = new Property() { Key = "RepositoryName", Value = "Test repository", Section = "ip" };
            props["BaseURL"] = new Property() { Key = "BaseURL", Value = "http://localhost:44345/api/oai", Section = "ip" };
            props["ProtocolVersion"] = new Property() { Key = "ProtocolVersion", Value = "2.0", Section = "ip" };

            //ToDo SQLDATE CHECK
            props["EarliestDatestamp"] = new Property() { Key = "EarliestDatestamp", Value = "" };//SqlDateTime.MinValue.Value.ToString(Enums.Granularity.DateTime), Section = "ip" };

            props["DeletedRecord"] = new Property() { Key = "DeletedRecord", Value = Enums.DeletedRecords.No, Section = "ip" };
            props["Granularity"] = new Property() { Key = "Granularity", Value = Enums.Granularity.DateTime, Section = "ip" };
            props["AdminEmails"] = new Property() { Key = "AdminEmails", Value = "test@domain.com", Section = "ip" };
            props["Compression"] = new Property() { Key = "Compression", Value = null, Section = "ip" };
            props["Description"] = new Property() { Key = "Description", Value = null, Section = "ip" };
            /* Data provider properties */
            props["SupportSets"] = new Property() { Key = "SupportSets", Value = "False", Section = "dpp" };
            props["ResumeListSets"] = new Property() { Key = "ResumeListSets", Value = "False", Section = "dpp" };
            props["MaxSetsInList"] = new Property() { Key = "MaxSetsInList", Value = "10", Section = "dpp" };
            props["ResumeListIdentifiers"] = new Property() { Key = "ResumeListIdentifiers", Value = "True", Section = "dpp" };
            props["MaxIdentifiersInList"] = new Property() { Key = "MaxIdentifiersInList", Value = "100", Section = "dpp" };
            props["ResumeListRecords"] = new Property() { Key = "ResumeListRecords", Value = "True", Section = "dpp" };
            props["MaxRecordsInList"] = new Property() { Key = "MaxRecordsInList", Value = "10", Section = "dpp" };
            props["ExpirationTimeSpan"] = new Property() { Key = "ExpirationTimeSpan", Value = new TimeSpan(1, 0, 0, 0).ToString(), Section = "dpp" };
            props["LoadAbout"] = new Property() { Key = "LoadAbout", Value = "True", Section = "dpp" };

        }

    }
}