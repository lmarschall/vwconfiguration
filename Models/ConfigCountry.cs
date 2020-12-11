using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace VWConfiguration.Models
{
    public class ConfigCountry
    {
        [Key]
        [JsonProperty(PropertyName = "countryCode")]
        public string ConfigCountryCode { get; set; }
        [JsonProperty(PropertyName = "name")]
        public string ConfigCountryName { get; set; }
    }
}
