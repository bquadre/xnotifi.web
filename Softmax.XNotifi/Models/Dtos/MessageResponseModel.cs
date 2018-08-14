using System.ComponentModel.DataAnnotations;
using Softmax.XNotifi.Data.Enums;

namespace Softmax.XNotifi.Models
{
    public class MessageResponseModel
    {
        
        public StatusCode Error { get; set; }
        
        public string Message { get; set; }
        
        public string Reference { get; set; }
       
    }
}
