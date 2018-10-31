
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSCSC.Shared.Models
{
    public class ExceptionResponse
    {
        public Exception InnerEx { get; set; }
        public ValidationException ValidatedEx { get; set; }
        //public ValidationError[] Errors { get; set; }
    }
    public class NSApiErrorResponse
    {
        public string Code { get; set; }

        public string Description { get; set; }

        public ExceptionResponse ExceptionData { get; set; }

        public NSApiErrorResponse()
        {
            Code = "";
            Description = "";
            ExceptionData = new ExceptionResponse();
        }

        public NSApiErrorResponse(string code, string description, ExceptionResponse ex)
        {
            Code = code;
            Description = description;
            ExceptionData = ex;
        }
    }
}
