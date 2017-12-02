using Softmax.XMessager.Data.Entities;
using Softmax.XMessager.Extensions;
using Softmax.XMessager.Models;

namespace Softmax.XMessager.Data.Contracts.Validations
{
    public interface IApplicationValidation
    {
        ValidationResult ValidateCreate(ApplicationModel model);
        ValidationResult ValidateUpdate(ApplicationModel model);
    }
}
