using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSCSC.Shared.Models.Settings.PrefixSettings
{
    public class PrefixSettingsModels
    {
        public string ID { get; set; }
        public string DisplayName { get; set; }
        public string Value { get; set; }
        public string ValueType { get; set; }
        public int Code { get; set; }
    }
    public class PrefixSettingsTextModels
    {
        public string ID { get; set; }
        //================================       
        public string IDReceiptPrefix { get; set; }
       
        public string ReceiptPrefix { get; set; }
        
        public int CodeReceiptPrefix { get; set; }
       
        //========================       
        public string IDOrderPrefix { get; set; }
       
        public string OrderPrefix { get; set; }   
        public int CodeOrderPrefix { get; set; }
        //==================================

        public string IDStartNumber { get; set; }
       
        public string StartNumber { get; set; }
        public int CodeStartNumber { get; set; }

        //==================================
        public string Name { get; set; }
    }
}
