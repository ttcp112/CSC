using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSCSC.Shared.Models.CRM
{
    public class RatingAndCommentModels
    {
        public string ID { get; set; }
        public string ProductName { get; set; }
        public string AccountName { get; set; }
        public double Rating { get; set; }
        public string Comment { get; set; }
        public bool IsHidden { get; set; }
        public DateTime CreatedTime { get; set; }
    }
    public class RatingAndCommentViewModels
    {
        public List<RatingAndCommentModels> ListRatingAndComment { get; set; }
        public RatingAndCommentViewModels()
        {
            ListRatingAndComment = new List<RatingAndCommentModels>();
        }
    }
}
