using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSCSC.Shared.Models.Settings.Currency
{
    public class CurrencyRequest : RequestBaseModels
    {
    }

    public class CreateOrEditCurrencyRequest : RequestBaseModels
    {
        public CurrencyDTO Currency { get; set; }
    }

    public class DeleteCurrencyRequest : RequestBaseModels { }

    public class GetCurrencyRequest : RequestBaseModels {
        public bool IsActive { get; set; }
    }

    public class CurrencyDTO
    {
        public string ID { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Symbol { get; set; }
        public bool IsActive { get; set; }
    }
}
