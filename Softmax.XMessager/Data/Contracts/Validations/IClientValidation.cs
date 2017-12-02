using Softmax.XMessager.Data.Entities;
using Softmax.XMessager.Extensions;
using Softmax.XMessager.Models;

namespace Softmax.XMessager.Data.Contracts.Validations
{
    public interface IClientValidation
    {
        ValidationResult ValidateCreate(ClientModel model);
        ValidationResult ValidateUpdate(ClientModel model);
    }
}
