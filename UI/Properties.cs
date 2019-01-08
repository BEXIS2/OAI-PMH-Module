using BExIS.Modules.OAIPMH.UI.Models;
using System;
using System.Collections.Concurrent;

namespace BExIS.Modules.OAIPMH.UI
{
    public class Properties
    {
        private static ConcurrentDictionary<string, Property> properties = new ConcurrentDictionary<string, Property>();


        #region /* Identify properties -------------------------------------------------------------*/

        public static string repositoryName
        {
            get { return GetStringProperty("RepositoryName"); }
            set { SetProperty("RepositoryName", value); }
        }
        public static string baseURL
        {
            get { return GetStringProperty("BaseURL"); }
            set { SetProperty("BaseURL", value); }
        }
        public static string protocolVersion
        {
            get { return GetStringProperty("ProtocolVersion"); }
            set { SetProperty("ProtocolVersion", value); }
        }
        public static string earliestDatestamp
        {
            get { return GetStringProperty("EarliestDatestamp"); }
            set { SetProperty("EarliestDatestamp", value); }
        }
        public static string deletedRecord
        {
            get { return GetStringProperty("DeletedRecord"); }
            set { SetProperty("DeletedRecord", value); }
        }
        public static string granularity
        {
            get { return GetStringProperty("Granularity"); }
            set { SetProperty("Granularity", value); }
        }
        public static string adminEmails
        {
            get { return GetStringProperty("AdminEmails"); }
            set { SetProperty("AdminEmails", value); }
        }
        public static string compression
        {
            get { return GetStringProperty("Compression"); }
            set { SetProperty("Compression", value); }
        }
        public static string description
        {
            get { return GetStringProperty("Description"); }
            set { SetProperty("Description", value); }
        }

        #endregion

        #region /* Other dp properties -------------------------------------------------------------*/

        public static bool supportSets
        {
            get { return GetBoolProperty("SupportSets"); }
            set { SetProperty("SupportSets", value); }
        }
        public static bool resumeListSets
        {
            get { return GetBoolProperty("ResumeListSets"); }
            set { SetProperty("ResumeListSets", value); }
        }
        public static int maxSetsInList
        {
            get { return GetIntProperty("MaxSetsInList"); }
            set { SetProperty("MaxSetsInList", value); }
        }
        public static bool resumeListIdentifiers
        {
            get { return GetBoolProperty("ResumeListIdentifiers"); }
            set { SetProperty("ResumeListIdentifiers", value); }
        }
        public static int maxIdentifiersInList
        {
            get { return GetIntProperty("MaxIdentifiersInList"); }
            set { SetProperty("MaxIdentifiersInList", value); }
        }
        public static bool resumeListRecords
        {
            get { return GetBoolProperty("ResumeListRecords"); }
            set { SetProperty("ResumeListRecords", value); }
        }
        public static int maxRecordsInList
        {
            get { return GetIntProperty("MaxRecordsInList"); }
            set { SetProperty("MaxRecordsInList", value); }
        }
        public static TimeSpan expirationTimeSpan
        {
            get { return GetTimeSpanProperty("ExpirationTimeSpan"); }
            set { SetProperty("ExpirationTimeSpan", value); }
        }
        public static bool loadAbout
        {
            get { return GetBoolProperty("LoadAbout"); }
            set { SetProperty("LoadAbout", value); }
        }

        #endregion

        #region Getters and setter for different types

        /* string ----------------------------------------------------------------------------------*/
        private static string GetStringProperty(string name)
        {
            return GetStringProperty(name, null);
        }
        private static string GetStringProperty(string name, string defaultReturn)
        {
            Property property = null;
            if (properties.TryGetValue(name, out property))
            {
                return property.Value;
            }
            return defaultReturn;
        }

        /* bool ----------------------------------------------------------------------------------*/
        private static bool GetBoolProperty(string name)
        {
            return GetBoolProperty(name, false);
        }
        private static bool GetBoolProperty(string name, bool defaultReturn)
        {
            bool bValue = false;
            Property property = null;
            if (properties.TryGetValue(name, out property) &&
                !string.IsNullOrEmpty(property.Value) &&
                bool.TryParse(property.Value, out bValue))
            {
                return bValue;
            }
            return defaultReturn;
        }

        /* int ----------------------------------------------------------------------------------*/
        private static int GetIntProperty(string name)
        {
            return GetIntProperty(name, 0);
        }
        private static int GetIntProperty(string name, int defaultReturn)
        {
            int iValue = 0;
            Property property = null;
            if (properties.TryGetValue(name, out property) &&
                !string.IsNullOrEmpty(property.Value) &&
                int.TryParse(property.Value, out iValue))
            {
                return iValue;
            }
            return defaultReturn;
        }

        /* TimeSpan --------------------------------------------------------------------------------*/
        private static TimeSpan GetTimeSpanProperty(string name)
        {
            return GetTimeSpanProperty(name, TimeSpan.MinValue);
        }
        private static TimeSpan GetTimeSpanProperty(string name, TimeSpan defaultReturn)
        {
            TimeSpan tsValue = TimeSpan.MinValue;
            Property property = null;
            if (properties.TryGetValue(name, out property) &&
                !string.IsNullOrEmpty(property.Value) &&
                TimeSpan.TryParse(property.Value, out tsValue))
            {
                return tsValue;
            }
            return defaultReturn;
        }

        public static void SetProperty(string name, object value)
        {
            Property property = null;
            if (properties.TryGetValue(name, out property))
            {
                property.Value = value.ToString();
                properties.AddOrUpdate(name, property, (key, oldValue) => property);
            }
        }

        #endregion


        public Property this[string key]
        {
            get
            {
                return properties[key];
            }
            set
            {
                properties[key] = value;
            }
        }
    }
}