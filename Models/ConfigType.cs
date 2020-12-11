using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace VWConfiguration.Models
{
    public class ConfigType
    {
        [Key]
        [JsonProperty(PropertyName = "id")]
        public string ConfigTypeId { get; set; }
        [JsonProperty(PropertyName = "name")]
        public string ConfigTypeName { get; set; }
        [JsonProperty(PropertyName = "extensions")]
        public List<ConfigurationOption> ConfigTypeOptions { get; set; }
    }
}
