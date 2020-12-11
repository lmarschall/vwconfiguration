using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VWConfiguration.Models
{
    public class Image
    {
        [JsonProperty(PropertyName = "name")]
        public string ImageName { get; set; }
        [JsonProperty(PropertyName = "url")]
        public string ImageUrl { get; set; }
    }
}
