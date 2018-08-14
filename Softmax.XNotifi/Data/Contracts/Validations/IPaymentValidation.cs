using Softmax.XNotifi.Extensions;
using Softmax.XNotifi.Models;

namespace Softmax.XNotifi.Data.Contracts.Validations
{
    public interface IPaymentValidation
    {
        ValidationResult ValidateCreate(PaymentModel model);
        ValidationResult ValidateUpdate(PaymentModel model);
    }
}
