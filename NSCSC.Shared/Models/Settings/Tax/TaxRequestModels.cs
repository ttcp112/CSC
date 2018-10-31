using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSCSC.Shared.Models.Settings.Tax
{
    public class TaxRequestModels:RequestBaseModels
    {
        public List<TaxModels> listTaxModels { get; set; }
        public TaxModels TaxData { get; set; }
    }
}
