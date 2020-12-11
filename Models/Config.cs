using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace VWConfiguration.Models
{ 
    public class Config
    {
        [Key]
        public long ConfigId { get; set; }
        [Display(Name = "Country")]
        public ConfigCountry ConfigCountry { get; set; }
        [Display(Name = "Brand")]
        public ConfigBrand ConfigBrand { get; set; }
        [Display(Name = "Model")]
        public ConfigModel ConfigModel { get; set; }
        [Display(Name = "Type")]
        public ConfigType ConfigType { get; set; }

        public string ConfigString { get; set; }
    }
}
