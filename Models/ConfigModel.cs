using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace VWConfiguration.Models
{
    public class ConfigModel
    {
        [Key]
        [JsonProperty(PropertyName = "id")]
        public string ConfigModelId { get; set; }
        [JsonProperty(PropertyName = "name")]
        public string ConfigModelName { get; set; }

    }
}
