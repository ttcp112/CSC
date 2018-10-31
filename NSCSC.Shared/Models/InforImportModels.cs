using System.Collections.Generic;

namespace NSCSC.Shared.Models.CommonModels
{
    public class InforImportModels
    {
        public bool IsValidRow { get; set; }
        public List<string> StoresAffeted { get; set; }
        public List<string> StoresFailed { get; set; }
        public List<string> Errors { get; set; }

        public InforImportModels()
        {
            IsValidRow = true;
            StoresAffeted = new List<string>();
            StoresFailed = new List<string>();
            Errors = new List<string>();
        }
    }
}
