using Softmax.XMessager.Extensions;
using Softmax.XMessager.Models;

namespace Softmax.XMessager.Data.Contracts.Validations
{
    public interface IGatewayValidation
    {
        ValidationResult ValidateCreate(GatewayModel model);
        ValidationResult ValidateUpdate(GatewayModel model);
    }
}
