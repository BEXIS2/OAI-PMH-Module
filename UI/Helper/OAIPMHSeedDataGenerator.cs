using BExIS.Security.Entities.Authorization;
using BExIS.Security.Entities.Objects;
using BExIS.Security.Services.Authorization;
using BExIS.Security.Services.Objects;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BExIS.Modules.OAIPMH.UI.Helpers
{
    public class OAIPMHSeedDataGenerator : IDisposable
    {
        public void GenerateSeedData()
        {
            FeatureManager featureManager = new FeatureManager();
            OperationManager operationManager = new OperationManager();

            try
            {
                #region SECURITY

                //Features
                List<Feature> features = featureManager.FeatureRepository.Get().ToList();

                Feature OAIPMH = features.FirstOrDefault(f => f.Name.Equals("OAI-PMH"));
                if (OAIPMH == null) OAIPMH = featureManager.Create("OAI-PMH", "Open Archives Initiative Protocol for Metadata Harvesting ");

                Feature AdminFeature = features.FirstOrDefault(f =>
                    f.Name.Equals("Admin") &&
                    f.Parent != null &&
                    f.Parent.Id.Equals(OAIPMH.Id));

                if (AdminFeature == null) AdminFeature = featureManager.Create("Admin", "Settings and customization of the protocol", OAIPMH);

                Feature ApiFeature = features.FirstOrDefault(f =>
                    f.Name.Equals("API") &&
                    f.Parent != null &&
                    f.Parent.Id.Equals(OAIPMH.Id));

                if (ApiFeature == null) ApiFeature = featureManager.Create("Api", "Api of the OAI PMH", OAIPMH);

                //Operations
                operationManager.Create("OAIPMH", "Admin", "*", OAIPMH);
                operationManager.Create("OAIPMH", "Home", "*", OAIPMH);
                operationManager.Create("API", "oai", "*", OAIPMH);

                var featurePermissionManager = new FeaturePermissionManager();

                if (!featurePermissionManager.Exists(null, ApiFeature.Id, PermissionType.Grant))
                    featurePermissionManager.Create(null, ApiFeature.Id, PermissionType.Grant);

                #endregion SECURITY
            }
            finally
            {
                featureManager.Dispose();
                operationManager.Dispose();
            }
        }

        public void Dispose()
        {
            //throw new NotImplementedException();
        }
    }
}