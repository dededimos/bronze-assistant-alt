using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace BronzeFactoryApplication.ApplicationServices.ExcelXlsService
{
    public static class ExcelGenerationExceptions
    {
        public class EmptyWorkBookException : Exception
        {
            public EmptyWorkBookException(string? message = "WorkBook does not Contain any Worksheets") : base(message)
            {
            }
        }
        public class SaveCancelledException : Exception
        {
            public SaveCancelledException(string? message ="Save has been canceled by user") : base(message)
            {

            }
        }
    }
}
