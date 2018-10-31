using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace NSCSC.Shared.Models.ManagementTool.Feedbacks
{
    public class FeedbacksModels
    {
        public string ID { get; set; }
        public string CustomerID { get; set; }
        public string CustomerName { get; set; }
        public string CustomerEmail { get; set; }

        public string Subject { get; set; }
        public string Message { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime Time { get; set; }
        public int FeedbackType { get; set; }
    }

    public class FeedbacksViewModels
    {
        public List<FeedbacksModels> ListItem { get; set; }
    }
}
