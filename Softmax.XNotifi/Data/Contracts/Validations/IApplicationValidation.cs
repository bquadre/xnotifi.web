using Softmax.XNotifi.Data.Entities;
using Softmax.XNotifi.Extensions;
using Softmax.XNotifi.Models;

namespace Softmax.XNotifi.Data.Contracts.Validations
{
    public interface IApplicationValidation
    {
        ValidationResult ValidateCreate(ApplicationModel model);
        ValidationResult ValidateUpdate(ApplicationModel model);
    }
}
