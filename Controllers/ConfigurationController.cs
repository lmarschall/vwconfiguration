using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using VWConfiguration.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace VWConfiguration.Controllers
{
    public class ConfigurationController : Controller
    {

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Country()
        {
            // create new Selection and save to Manager
            Selection newSelection = Manager.AddNewSelection();

            // get all avaible Countries from API
            List<ConfigCountry> countries = Supplier.RequestCountries();

            // send Countries to View in SelectionList
            ViewBag.Countries = new SelectList(countries, "ConfigCountryCode", "ConfigCountryName");

            // save Countries in Selection
            newSelection.SelectionCountries = countries;

            return View(newSelection);
        }

        public IActionResult Brand([Bind("SelectionId, SelectionCountryCode")] Selection userSelection)
        {
            // get Selection from Manager with SelectionId
            Selection editSelection = Manager.GetSelectionById(userSelection.SelectionId);

            // get all avaible Brands from API
            List<ConfigBrand> brands = Supplier.RequestBrands(userSelection.SelectionCountryCode);

            // send Brands to View in SelectionList
            ViewBag.Brands = new SelectList(brands, "ConfigBrandId", "ConfigBrandName");

            // save Brands in Selection
            editSelection.SelectionBrands = brands;

            // save CountryCode in Selection
            editSelection.SelectionCountryCode = userSelection.SelectionCountryCode;

            return View(editSelection);
        }

        public IActionResult Model([Bind("SelectionId, SelectionBrandCode")] Selection userSelection)
        {
            // get Selection from Manager with SelectionId
            Selection editSelection = Manager.GetSelectionById(userSelection.SelectionId);

            // get all avaiable Models from API
            List<ConfigModel> models = Supplier.RequestModels(editSelection.SelectionCountryCode, userSelection.SelectionBrandCode);

            //Dictionary<ConfigModel, Image> choose = new Dictionary<ConfigModel, Image>();

            //foreach(ConfigModel item in models)
            //{
            //    choose.Add(item, Supplier.GetModelImage(editSelection.SelectionBrandCode, item.ConfigModelId));
            //}


            // send Models to View with SelectionList
            ViewBag.Models = new SelectList(models, "ConfigModelId", "ConfigModelName");
            //ViewBag.Choose = choose;

            // save Models in Selection
            editSelection.SelectionModels = models;

            // save BrandCode in Selection
            editSelection.SelectionBrandCode = userSelection.SelectionBrandCode;

            return View(editSelection);
        }

        public IActionResult Type([Bind("SelectionId, SelectionModelCode")] Selection userSelection)
        {
            // get Selection from Manager with SelectionId
            Selection editSelection = Manager.GetSelectionById(userSelection.SelectionId);

            // save ModelCode in Selection
            editSelection.SelectionModelCode = userSelection.SelectionModelCode;

            // get all avaiable Models from API
            List<ConfigType> types = Supplier.RequestTypes(editSelection.SelectionCountryCode, editSelection.SelectionModelCode);

            //Dictionary<ConfigType, Image> choose = new Dictionary<ConfigType, Image>();

            //foreach (ConfigType item in types)
            //{
            //    choose.Add(item, Supplier.GetTypeImage(editSelection.SelectionBrandCode, editSelection.SelectionModelCode, item.ConfigTypeId, editSelection.SelectionCountryCode));
            //}

            // send Models to View with SelectionList
            ViewBag.Types = new SelectList(types, "ConfigTypeId", "ConfigTypeName");
            //ViewBag.Choose = choose;

            // save Models in Selection
            editSelection.SelectionTypes = types;

            return View(editSelection);
        }

        public IActionResult Result(int id, [Bind("SelectionId, SelectionTypeCode")] Selection userSelection)
        {
            // get Selection from Manager with SelectionId
            Selection editSelection = Manager.GetSelectionById(userSelection.SelectionId);

            // save ModelCode of Selection
            editSelection.SelectionTypeCode = userSelection.SelectionTypeCode;

            // take information from UserSelection and create new Config
            Config selectionConfig = Manager.AssembleConfig(editSelection);

            return View(selectionConfig);
        }

        public IActionResult Configure(string configurationId)
        {
            if (configurationId == null) configurationId = "493ec8ac-04de-46fb-bbc7-333ca07d08a3";

            Config conf = Manager.GetConfigById(configurationId);
            ViewBag.ShowMessage = false;
            ViewBag.Message = "";

            ViewBag.Images = Supplier.GetImages(configurationId);
            ViewBag.Options = Supplier.GetOptions(configurationId);
            ViewBag.Choices = Supplier.GatherChoices(configurationId);

            //Supplier.GatherChoices(confId);

            return View(conf);
        }

        public IActionResult AddOption(string configurationId, string optionId)
        {
            Config conf = Manager.GetConfigById(configurationId);

            ViewBag.Message = "";

            // Try to add option to Configuration
            try
            {
                List<ConfigurationOption> resultOptions = Supplier.AddOption(configurationId, optionId);
                ViewBag.Message = "Option added to Configuration!";
            }
            catch
            {
                ViewBag.Message = "Could not add Option to Configuration!";
            }

            ViewBag.Images = Supplier.GetImages(configurationId);
            ViewBag.Options = Supplier.GetOptions(configurationId);
            ViewBag.Choices = Supplier.GatherChoices(configurationId);

            return View("Configure", conf);
        }

        public IActionResult RemoveOption(string configurationId, string optionId)
        {
            Config conf = Manager.GetConfigById(configurationId);
            ViewBag.Message = "";

            // Try to remove Optionf rom Configuration
            try
            {
                List<ConfigurationOption> resultOptions = Supplier.RemoveOption(configurationId, optionId);
                ViewBag.Message = "Option removed from Configuration!";
            }
            catch
            {
                ViewBag.Message = "Could not remove Option from Configuration!";
            }
            

            ViewBag.Images = Supplier.GetImages(configurationId);
            ViewBag.Options = Supplier.GetOptions(configurationId);
            ViewBag.Choices = Supplier.GatherChoices(configurationId);

            return View("Configure", conf);
        }

        public IActionResult CheckConfiguration(string configurationId)
        {
            Config conf = Manager.GetConfigById(configurationId);
            ViewBag.Message = "";

            // get result if Configuration is builable and distinct
            bool[] result = Supplier.CheckConfiguration(configurationId);

            if(result[0] == true && result[1] == true)
            {
                ViewBag.Message = "Configuration is buildable and distinct!";
            }

            if (result[0] == false && result[1] == true)
            {
                ViewBag.Message = "Configuration is not buildable, but distinct!";
            }

            if (result[0] == true && result[1] == false)
            {
                ViewBag.Message = "Configuration is buildable, but not distinct!";
            }

            if (result[0] == false && result[1] == false)
            {
                ViewBag.Message = "Configuration is not buildable and distinct!";
            }

            ViewBag.Images = Supplier.GetImages(configurationId);
            ViewBag.Options = Supplier.GetOptions(configurationId);
            ViewBag.Choices = Supplier.GatherChoices(configurationId);

            return View("Configure", conf);
        }

        public IActionResult CompleteConfiguration(string configurationId)
        {
            Config conf = Manager.GetConfigById(configurationId);
            ViewBag.Message = "";

            // Try to resolve Configuration
            try
            {
                VWConfiguration.Models.Configuration completedConfiguration = Supplier.ResolveConfiguration(conf.ConfigCountry.ConfigCountryCode, conf.ConfigBrand.ConfigBrandId, conf.ConfigModel.ConfigModelId, conf.ConfigType.ConfigTypeId);
                configurationId = Supplier.CreateConfiguration(completedConfiguration);
                conf.ConfigString = configurationId;
                ViewBag.Message = "Configuration succesful completed!";
            }
            catch
            {
                ViewBag.Message = "Configuration could not be completed!";
            }

            ViewBag.Images = Supplier.GetImages(configurationId);
            ViewBag.Options = Supplier.GetOptions(configurationId);
            ViewBag.Choices = Supplier.GatherChoices(configurationId);

            return View("Configure", conf);
        }

        public IActionResult LastConfigs()
        {
            return View(Manager.GetAllConfigs());
        }
    }
}