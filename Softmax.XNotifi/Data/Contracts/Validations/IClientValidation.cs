using Softmax.XNotifi.Data.Entities;
using Softmax.XNotifi.Extensions;
using Softmax.XNotifi.Models;

namespace Softmax.XNotifi.Data.Contracts.Validations
{
    public interface IClientValidation
    {
        ValidationResult ValidateCreate(ClientModel model);
        ValidationResult ValidateUpdate(ClientModel model);
    }
}
