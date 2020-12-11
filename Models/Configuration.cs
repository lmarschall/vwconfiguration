using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace VWConfiguration.Models
{
    public class Configuration
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }
        [JsonProperty(PropertyName = "brand_id")]
        public string BrandId { get; set; }
        [JsonProperty(PropertyName = "model_id")]
        public string ModelId { get; set; }
        [JsonProperty(PropertyName = "options")]
        public List<ConfigurationOption> ConfigOptions { get; set; }
    }

    public class ConfigurationOption
    {
        [Key]
        [JsonProperty(PropertyName = "id")]
        public string OptionId { get; set; }
        [JsonProperty(PropertyName = "category")]
        public string OptionCategory { get; set; }
        [JsonProperty(PropertyName = "classification")]
        public string OptionClassification { get; set; }
        [JsonProperty(PropertyName = "code")]
        public string OptionCode { get; set; }
        [JsonProperty(PropertyName = "description")]
        public string OptionDescription { get; set; }
    }

    public class ConfigurationChoice
    {
        [Key]
        [JsonProperty(PropertyName = "id")]
        public string ChoiceId { get; set; }
        [JsonProperty(PropertyName = "description")]
        public string ChoiceDescription { get; set; }
        [JsonProperty(PropertyName = "valid")]
        public List<ConfigurationOption> CoiceValidOptions { get; set; }
        [JsonProperty(PropertyName = "invalid")]
        public List<ConfigurationOption> CoiceInvalidOptions { get; set; }
    }
}
