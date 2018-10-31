using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSCSC.Shared.Models.Settings.Currency
{
    public class CurrencyResponse
    {
    }
    public class ResponseGetCurrency : NSApiResponseBase
    {
        public List<CurrencyDTO> ListCurrency { get; set; }
    }
}
