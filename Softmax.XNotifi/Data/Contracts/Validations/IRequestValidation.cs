using Softmax.XNotifi.Extensions;
using Softmax.XNotifi.Data.Entities;
using Softmax.XNotifi.Models;

namespace Softmax.XNotifi.Data.Contracts.Validations
{
    public interface IRequestValidation
    {
        ValidationResult ValidateCreate(RequestModel model);
        ValidationResult ValidateUpdate(RequestModel model);
    }
}
