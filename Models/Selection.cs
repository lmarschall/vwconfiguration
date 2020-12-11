using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VWConfiguration.Models
{
    public class Selection
    {
        public long SelectionId { get; set; }
        public string SelectionCountryCode { get; set; }
        public string SelectionBrandCode { get; set; }
        public string SelectionModelCode { get; set; }

        public string SelectionTypeCode { get; set; }

        public List<ConfigCountry> SelectionCountries { get; set; }
        public List<ConfigBrand> SelectionBrands { get; set; }
        public List<ConfigModel> SelectionModels { get; set; }

        public List<ConfigType> SelectionTypes { get; set; }
    }
}
