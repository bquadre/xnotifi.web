using Softmax.XNotifi.Extensions;
using Softmax.XNotifi.Models;

namespace Softmax.XNotifi.Data.Contracts.Validations
{
    public interface IGatewayValidation
    {
        ValidationResult ValidateCreate(GatewayModel model);
        ValidationResult ValidateUpdate(GatewayModel model);
    }
}
