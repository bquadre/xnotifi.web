using Softmax.XMessager.Extensions;
using Softmax.XMessager.Data.Entities;
using Softmax.XMessager.Models;

namespace Softmax.XMessager.Data.Contracts.Validations
{
    public interface IRequestValidation
    {
        ValidationResult ValidateCreate(RequestModel model);
        ValidationResult ValidateUpdate(RequestModel model);
    }
}
