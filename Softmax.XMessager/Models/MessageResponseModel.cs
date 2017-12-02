using System.ComponentModel.DataAnnotations;
using Softmax.XMessager.Data.Enums;

namespace Softmax.XMessager.Models
{
    public class MessageResponseModel
    {
        
        public ErrorCode Error { get; set; }
        
        public string Message { get; set; }
        
        public string Reference { get; set; }
       
    }
}
