using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace VWConfiguration.Models
{
    public class ConfigBrand
    {
        [Key]
        [JsonProperty(PropertyName = "brand_id")]
        public string ConfigBrandId { get; set; }
        [JsonProperty(PropertyName = "name")]
        public string ConfigBrandName { get; set; }
    }
}
