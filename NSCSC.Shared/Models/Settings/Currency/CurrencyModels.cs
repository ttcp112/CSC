using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSCSC.Shared.Models.Settings.Currency
{
    public class CurrencyModels
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public string Symbol { get; set; }
        public bool IsActive { get; set; }
    }

    public class CurrencyViewModels
    {
        public List<CurrencyModels> ListItem { get; set; }
        public CurrencyViewModels()
        {
            ListItem = new List<CurrencyModels>();
        }
    }
}
