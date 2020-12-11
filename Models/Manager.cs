using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VWConfiguration.Models
{
    public static class Manager
    {
        private static long maxSelectionId;
        private static List<Selection> allSelections = new List<Selection>();
        private static List<Config> allConfigs = new List<Config>();

        static Manager()
        {
            maxSelectionId = 1;
        }

        public static Selection AddNewSelection()
        {
            Selection newSelection = new Selection();
            newSelection.SelectionId = maxSelectionId++;

            allSelections.Add(newSelection);

            return newSelection;
        }

        public static Selection GetSelectionById(long selectionId)
        {
            return allSelections.Where(selection => selection.SelectionId == selectionId).Single();
        }

        public static Config AssembleConfig(Selection userSelection)
        {
            Config newConfig = new Config();

            newConfig.ConfigId = userSelection.SelectionId;
            newConfig.ConfigCountry = userSelection.SelectionCountries.Where(country => country.ConfigCountryCode == userSelection.SelectionCountryCode).Single();
            newConfig.ConfigBrand = userSelection.SelectionBrands.Where(brand => brand.ConfigBrandId == userSelection.SelectionBrandCode).Single();
            newConfig.ConfigModel = userSelection.SelectionModels.Where(model => model.ConfigModelId == userSelection.SelectionModelCode).Single();
            newConfig.ConfigType = userSelection.SelectionTypes.Where(type => type.ConfigTypeId == userSelection.SelectionTypeCode).Single();
            newConfig.ConfigString = Supplier.CreateConfiguration(newConfig.ConfigBrand.ConfigBrandId, newConfig.ConfigModel.ConfigModelId, newConfig.ConfigType.ConfigTypeId, newConfig.ConfigCountry.ConfigCountryCode);

            allConfigs.Add(newConfig);

            return newConfig;
        }

        public static Config GetConfigById(string configurationId)
        {
            return allConfigs.Where(config => config.ConfigString == configurationId).Single();
        }

        public static List<Config> GetAllConfigs()
        {
            return allConfigs;
        }
    }
}
