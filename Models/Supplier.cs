using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace VWConfiguration.Models
{
    public static class Supplier
    {
        private static string ClientId;
        private static string ClientSecret;
        private static string TokenString;
        private static DateTime TokenCreateDate;

        static Supplier()
        {
            ClientId = "1df1f03b-dbca-4d58-a610-3ff06f206238@apps_vw-dilab_com";
            ClientSecret = "90121a80ee1492c0221312e919bc3db47ed64dde77eec688aba24b0fbebfe747";
            TokenString = "";
            TokenCreateDate = DateTime.MinValue;
        }

        private static bool CheckTokenExpired()
        {
            DateTime now = DateTime.Now;
            TimeSpan tokenLife = now - TokenCreateDate;
            
            if(tokenLife.TotalHours >= 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private static void RequestNewToken()
        {
            HttpWebRequest httpRequest = HttpWebRequest.CreateHttp("https://identity.vwgroup.io/oidc/v1/token?grant_type=client_credentials&client_id=" + ClientId + "&client_secret=" + ClientSecret);
            httpRequest.ContentType = "application/x-www-form-urlencoded";
            httpRequest.Method = "POST";

            HttpWebResponse httpResponse = httpRequest.GetResponse() as HttpWebResponse;
            StreamReader streamReader = new StreamReader(httpResponse.GetResponseStream());

            Dictionary<string, string> roots = new Dictionary<string, string>();
            roots = JsonConvert.DeserializeObject<Dictionary<string, string>>(streamReader.ReadToEnd());

            TokenString = roots["access_token"];
            TokenCreateDate = DateTime.Now;
        }

        public static List<ConfigCountry> RequestCountries()
        {
            if(CheckTokenExpired())
            {
                RequestNewToken();
            }

            HttpWebRequest httpRequest = HttpWebRequest.CreateHttp("https://api.productdata.vwgroup.com/v2/countries");
            httpRequest.Accept = "application/json";
            httpRequest.Method = "GET";
            httpRequest.Headers.Add("Authorization", "bearer " + TokenString);

            HttpWebResponse httpResponse = httpRequest.GetResponse() as HttpWebResponse;
            //var headers = httpResponse.Headers;
            //var etag = headers["ETag"];
            StreamReader streamReader = new StreamReader(httpResponse.GetResponseStream());

            Dictionary<string, object> roots = new Dictionary<string, object>();
            //string response = streamReader.ReadToEnd();
            roots = JsonConvert.DeserializeObject<Dictionary<string, object>>(streamReader.ReadToEnd());

            List<ConfigCountry> countries = JsonConvert.DeserializeObject<List<ConfigCountry>>(roots["data"].ToString());

            return countries;
        }

        public static List<ConfigBrand> RequestBrands(string countryCode)
        {
            if (CheckTokenExpired())
            {
                RequestNewToken();
            }

            HttpWebRequest httpRequest = HttpWebRequest.CreateHttp("https://api.productdata.vwgroup.com/v2/catalog/" + countryCode + "/brands");
            httpRequest.Accept = "application/json";
            httpRequest.Method = "GET";
            httpRequest.Headers.Add("Authorization", "bearer " + TokenString);

            HttpWebResponse httpResponse = httpRequest.GetResponse() as HttpWebResponse;
            StreamReader streamReader = new StreamReader(httpResponse.GetResponseStream());

            Dictionary<string, object> roots = new Dictionary<string, object>();
            //string response = streamReader.ReadToEnd();
            roots = JsonConvert.DeserializeObject<Dictionary<string, object>>(streamReader.ReadToEnd());

            List<ConfigBrand> brands = JsonConvert.DeserializeObject<List<ConfigBrand>>(roots["data"].ToString());

            return brands;
        }

        public static List<ConfigModel> RequestModels(string countryCode, string brandId)
        {
            if (CheckTokenExpired())
            {
                RequestNewToken();
            }

            HttpWebRequest httpRequest = HttpWebRequest.CreateHttp("https://api.productdata.vwgroup.com/v2/catalog/" + countryCode + "/brands/" + brandId+ "/models");
            httpRequest.Accept = "application/json";
            httpRequest.Method = "GET";
            httpRequest.Headers.Add("Authorization", "bearer " + TokenString);

            HttpWebResponse httpResponse = httpRequest.GetResponse() as HttpWebResponse;
            StreamReader streamReader = new StreamReader(httpResponse.GetResponseStream());

            Dictionary<string, object> roots = new Dictionary<string, object>();
            roots = JsonConvert.DeserializeObject<Dictionary<string, object>>(streamReader.ReadToEnd());

            List<ConfigModel> models = JsonConvert.DeserializeObject<List<ConfigModel>>(roots["data"].ToString());

            return models;
        }
        public static List<ConfigType> RequestTypes(string countryCode, string modelId)
        {
            if (CheckTokenExpired())
            {
                RequestNewToken();
            }

            HttpWebRequest httpRequest = HttpWebRequest.CreateHttp("https://api.productdata.vwgroup.com/v2/catalog/" + countryCode + "/models/" + modelId + "/types");
            httpRequest.Accept = "application/json";
            httpRequest.Method = "GET";
            httpRequest.Headers.Add("Authorization", "bearer " + TokenString);

            HttpWebResponse httpResponse = httpRequest.GetResponse() as HttpWebResponse;
            StreamReader streamReader = new StreamReader(httpResponse.GetResponseStream());

            //string response = streamReader.ReadToEnd();
            Dictionary<string, object> roots = new Dictionary<string, object>();
            roots = JsonConvert.DeserializeObject<Dictionary<string, object>>(streamReader.ReadToEnd());

            List<ConfigType> types = JsonConvert.DeserializeObject<List<ConfigType>>(roots["data"].ToString());

            return types;
        }

        public static string CreateConfiguration(string brandId, string modelId, string typeId = "", string countryCode = "")
        {
            if (CheckTokenExpired())
            {
                RequestNewToken();
            }

            Configuration postConfig = new Configuration();
            postConfig.BrandId = brandId;
            postConfig.ModelId = modelId;

            if(typeId != "" && countryCode != "")
            {
                postConfig.ConfigOptions = GetTypeOptions(countryCode, typeId);
            }

            string postData = JsonConvert.SerializeObject(postConfig);
            byte[] data = Encoding.ASCII.GetBytes(postData);

            HttpWebRequest httpRequest = HttpWebRequest.CreateHttp("https://api.productdata.vwgroup.com/v2/configurations");
            httpRequest.Accept = "application/json";
            httpRequest.ContentType = "application/json";
            httpRequest.ContentLength = data.Length;
            httpRequest.Method = "POST";
            httpRequest.Headers.Add("Authorization", "bearer " + TokenString);

            using (var stream = httpRequest.GetRequestStream())
            {
                stream.Write(data, 0, data.Length);
            }


            HttpWebResponse httpResponse = httpRequest.GetResponse() as HttpWebResponse;
            StreamReader streamReader = new StreamReader(httpResponse.GetResponseStream());

            Dictionary<string, object> roots = new Dictionary<string, object>();
            string response = streamReader.ReadToEnd();
            roots = JsonConvert.DeserializeObject<Dictionary<string, object>>(response);

            //List<Model> models = JsonConvert.DeserializeObject<List<Model>>(roots["data"].ToString());

            return roots["id"].ToString();
        }

        public static string CreateConfiguration(Configuration postConfig)
        {
            if (CheckTokenExpired())
            {
                RequestNewToken();
            }

            string postData = JsonConvert.SerializeObject(postConfig);
            byte[] data = Encoding.ASCII.GetBytes(postData);

            HttpWebRequest httpRequest = HttpWebRequest.CreateHttp("https://api.productdata.vwgroup.com/v2/configurations");
            httpRequest.Accept = "application/json";
            httpRequest.ContentType = "application/json";
            httpRequest.ContentLength = data.Length;
            httpRequest.Method = "POST";
            httpRequest.Headers.Add("Authorization", "bearer " + TokenString);

            using (var stream = httpRequest.GetRequestStream())
            {
                stream.Write(data, 0, data.Length);
            }


            HttpWebResponse httpResponse = httpRequest.GetResponse() as HttpWebResponse;
            StreamReader streamReader = new StreamReader(httpResponse.GetResponseStream());

            Dictionary<string, object> roots = new Dictionary<string, object>();
            string response = streamReader.ReadToEnd();
            roots = JsonConvert.DeserializeObject<Dictionary<string, object>>(response);

            //List<Model> models = JsonConvert.DeserializeObject<List<Model>>(roots["data"].ToString());

            return roots["id"].ToString();
        }

        public static Image GetModelImage(string brandId, string modelId)
        {
            Image returnImage = new Image();
            string configId = CreateConfiguration(brandId, modelId);
            int selectIndex = 3;
            List<Image> images = GetImages(configId);

            if(images.Count > 0) returnImage = images[selectIndex];

            return returnImage;
        }

        public static Image GetTypeImage(string brandId, string modelId, string typeId, string countryCode)
        {
            Image returnImage = new Image();
            string configId = CreateConfiguration(brandId, modelId, typeId, countryCode);
            int selectIndex = 3;
            List<Image> images = GetImages(configId);

            if (images.Count > 0) returnImage = images[selectIndex];

            return returnImage;
        }

        public static Configuration ResolveConfiguration(string countryCode, string brandId, string modelId, string typeId)
        {
            if (CheckTokenExpired())
            {
                RequestNewToken();
            }

            Configuration postConfig = new Configuration();
            postConfig.BrandId = brandId;
            postConfig.ModelId = modelId;
            //postConfig.ConfigOptions = GetTypeOptions(countryCode, typeId);
            postConfig.ConfigOptions = new List<ConfigurationOption>();

            string postData = JsonConvert.SerializeObject(postConfig);
            byte[] data = Encoding.ASCII.GetBytes(postData);

            HttpWebRequest httpRequest = HttpWebRequest.CreateHttp("https://api.productdata.vwgroup.com/v2/operation/"+ countryCode + "/resolve");
            httpRequest.Accept = "application/json";
            httpRequest.ContentType = "application/json";
            httpRequest.ContentLength = data.Length;
            httpRequest.Method = "POST";
            httpRequest.Headers.Add("Authorization", "bearer " + TokenString);

            using (var stream = httpRequest.GetRequestStream())
            {
                stream.Write(data, 0, data.Length);
            }


            HttpWebResponse httpResponse = httpRequest.GetResponse() as HttpWebResponse;
            StreamReader streamReader = new StreamReader(httpResponse.GetResponseStream());

            Configuration config = JsonConvert.DeserializeObject<Configuration>(streamReader.ReadToEnd());

            //List<Model> models = JsonConvert.DeserializeObject<List<Model>>(roots["data"].ToString());

            return config;
        }

        public static List<ConfigurationOption> AddOption(string configurationId, string optionId)
        {
            if (CheckTokenExpired())
            {
                RequestNewToken();
            }

            ConfigurationOption addOption = new ConfigurationOption();
            addOption.OptionId = optionId;

            string postData = JsonConvert.SerializeObject(addOption);
            byte[] data = Encoding.ASCII.GetBytes(postData);

            HttpWebRequest httpRequest = HttpWebRequest.CreateHttp("https://api.productdata.vwgroup.com/v2/configurations/" + configurationId + "/options");
            httpRequest.Accept = "application/json";
            httpRequest.ContentType = "application/json";
            httpRequest.ContentLength = data.Length;
            httpRequest.Method = "POST";
            httpRequest.Headers.Add("Authorization", "bearer " + TokenString);

            using (var stream = httpRequest.GetRequestStream())
            {
                stream.Write(data, 0, data.Length);
            }

            HttpWebResponse httpResponse = httpRequest.GetResponse() as HttpWebResponse;
            StreamReader streamReader = new StreamReader(httpResponse.GetResponseStream());
            string respone = streamReader.ReadToEnd();

            Dictionary<string, object> roots = new Dictionary<string, object>();
            roots = JsonConvert.DeserializeObject<Dictionary<string, object>>(respone);

            List<ConfigurationOption> options = JsonConvert.DeserializeObject<List<ConfigurationOption>>(roots["data"].ToString());

            //List<Model> models = JsonConvert.DeserializeObject<List<Model>>(roots["data"].ToString());

            return options;
        }

        public static List<ConfigurationOption> RemoveOption(string configurationId, string optionId)
        {
            if (CheckTokenExpired())
            {
                RequestNewToken();
            }

            //ConfigurationOption addOption = new ConfigurationOption();
            //addOption.OptionId = optionId;

            //string postData = JsonConvert.SerializeObject(addOption);
            //byte[] data = Encoding.ASCII.GetBytes(postData);

            HttpWebRequest httpRequest = HttpWebRequest.CreateHttp("https://api.productdata.vwgroup.com/v2/configurations/" + configurationId + "/options/" + optionId);
            httpRequest.Accept = "application/json";
            httpRequest.ContentType = "application/json";
            //httpRequest.ContentLength = data.Length;
            httpRequest.Method = "DELETE";
            httpRequest.Headers.Add("Authorization", "bearer " + TokenString);

            //using (var stream = httpRequest.GetRequestStream())
            //{
            //    stream.Write(data, 0, data.Length);
            //}

            HttpWebResponse httpResponse = httpRequest.GetResponse() as HttpWebResponse;
            StreamReader streamReader = new StreamReader(httpResponse.GetResponseStream());
            string respone = streamReader.ReadToEnd();

            Dictionary<string, object> roots = new Dictionary<string, object>();
            roots = JsonConvert.DeserializeObject<Dictionary<string, object>>(respone);

            List<ConfigurationOption> options = new List<ConfigurationOption>();
                
            if(roots["data"] != null) options= JsonConvert.DeserializeObject<List<ConfigurationOption>>(roots["data"].ToString());

            //List<Model> models = JsonConvert.DeserializeObject<List<Model>>(roots["data"].ToString());

            return options;
        }

        public static List<Image> GetImages(string configurationId)
        {
            if (CheckTokenExpired())
            {
                RequestNewToken();
            }

            HttpWebRequest httpRequest = HttpWebRequest.CreateHttp("https://api.productdata.vwgroup.com/v2/configurations/" + configurationId + "/images");
            httpRequest.Accept = "application/json";
            httpRequest.Method = "GET";
            httpRequest.Headers.Add("Authorization", "bearer " + TokenString);

            List<Image> images = new List<Image>();

            try
            {
                HttpWebResponse httpResponse = httpRequest.GetResponse() as HttpWebResponse;
                StreamReader streamReader = new StreamReader(httpResponse.GetResponseStream());

                Dictionary<string, object> roots = new Dictionary<string, object>();
                string respone = streamReader.ReadToEnd();
                roots = JsonConvert.DeserializeObject<Dictionary<string, object>>(respone);

                if (roots["data"] != null)
                {
                    images = JsonConvert.DeserializeObject<List<Image>>(roots["data"].ToString());

                    // Make Image Links https
                    foreach (Image item in images)
                    {
                        item.ImageUrl.Replace("http", "https");
                    }
                }
            }
            catch
            {
                ;
            }

            return images;
        }

        public static List<ConfigurationChoice> GatherChoices(string configurationId)
        {
            if (CheckTokenExpired())
            {
                RequestNewToken();
            }

            HttpWebRequest httpRequest = HttpWebRequest.CreateHttp("https://api.productdata.vwgroup.com/v2/configurations/" + configurationId + "/choices");
            httpRequest.Accept = "application/json";
            httpRequest.Method = "GET";
            httpRequest.Headers.Add("Authorization", "bearer " + TokenString);

            HttpWebResponse httpResponse = httpRequest.GetResponse() as HttpWebResponse;
            StreamReader streamReader = new StreamReader(httpResponse.GetResponseStream());

            //string response = streamReader.ReadToEnd();
            Dictionary<string, object> roots = new Dictionary<string, object>();
            roots = JsonConvert.DeserializeObject<Dictionary<string, object>>(streamReader.ReadToEnd());

            List<ConfigurationChoice> choices = JsonConvert.DeserializeObject<List<ConfigurationChoice>>(roots["data"].ToString());

            return choices;
        }

        public static List<ConfigurationOption> GetOptions(string configurationId)
        {
            if (CheckTokenExpired())
            {
                RequestNewToken();
            }

            HttpWebRequest httpRequest = HttpWebRequest.CreateHttp("https://api.productdata.vwgroup.com/v2/configurations/" + configurationId + "/options");
            httpRequest.Accept = "application/json";
            httpRequest.Method = "GET";
            httpRequest.Headers.Add("Authorization", "bearer " + TokenString);

            HttpWebResponse httpResponse = httpRequest.GetResponse() as HttpWebResponse;
            StreamReader streamReader = new StreamReader(httpResponse.GetResponseStream());

            //string response = streamReader.ReadToEnd();
            Dictionary<string, object> roots = new Dictionary<string, object>();
            roots = JsonConvert.DeserializeObject<Dictionary<string, object>>(streamReader.ReadToEnd());

            List<ConfigurationOption> options = JsonConvert.DeserializeObject<List<ConfigurationOption>>(roots["data"].ToString());

            return options;
        }

        public static List<ConfigurationOption> GetTypeOptions(string countryCode, string typeId)
        {
            if (CheckTokenExpired())
            {
                RequestNewToken();
            }

            HttpWebRequest httpRequest = HttpWebRequest.CreateHttp("https://api.productdata.vwgroup.com/v2/catalog/" + countryCode + "/types/" + typeId);
            httpRequest.Accept = "application/json";
            httpRequest.Method = "GET";
            httpRequest.Headers.Add("Authorization", "bearer " + TokenString);

            HttpWebResponse httpResponse = httpRequest.GetResponse() as HttpWebResponse;
            StreamReader streamReader = new StreamReader(httpResponse.GetResponseStream());

            //string response = streamReader.ReadToEnd();
            //Dictionary<string, object> roots = new Dictionary<string, object>();
            //roots = JsonConvert.DeserializeObject<Dictionary<string, object>>(streamReader.ReadToEnd());

            ConfigType type = JsonConvert.DeserializeObject<ConfigType>(streamReader.ReadToEnd());

            List<ConfigurationOption> options = type.ConfigTypeOptions;

            return options;
        }

        public static bool[] CheckConfiguration(string configurationId)
        {
            if (CheckTokenExpired())
            {
                RequestNewToken();
            }

            HttpWebRequest httpRequest = HttpWebRequest.CreateHttp("https://api.productdata.vwgroup.com/v2/configurations/" + configurationId + "/check");
            httpRequest.Accept = "application/json";
            httpRequest.Method = "GET";
            httpRequest.Headers.Add("Authorization", "bearer " + TokenString);

            HttpWebResponse httpResponse = httpRequest.GetResponse() as HttpWebResponse;
            StreamReader streamReader = new StreamReader(httpResponse.GetResponseStream());

            //string response = streamReader.ReadToEnd();
            Dictionary<string, object> roots = new Dictionary<string, object>();
            roots = JsonConvert.DeserializeObject<Dictionary<string, object>>(streamReader.ReadToEnd());

            bool buildable = (bool) roots["buildable"];
            bool distinct = (bool) roots["distinct"];


            return new bool[]{ buildable, distinct};
        }
    }
}
