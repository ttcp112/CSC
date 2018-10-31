using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSCSC.Shared.Models.ManagementTool.Feedbacks
{
    public class CreateOrEditFeedbackRequest : RequestBaseModels
    {
        public FeedbackDTO Feedback { get; set; }
    }
    public class DeleteFeedbackRequest : RequestBaseModels { }
    public class GetFeedbackRequest : RequestBaseModels    { }
    public class ExportFeedbackRequest : RequestBaseModels
    {
        public DateTime DateForm { get; set; }
        public DateTime DateTo { get; set; }
    }

    public class FeedbackDTO
    {
        public string ID { get; set; }
        public string CustomerID { get; set; }
        public string CustomerName { get; set; }
        public string Subject { get; set; }
        public string Message { get; set; }
        public string CustomerEmail { get; set; }
        public DateTime Time { get; set; }
        public int FeedbackType { get; set; }

        public string CreatedUser { get; set; }
    }
}
