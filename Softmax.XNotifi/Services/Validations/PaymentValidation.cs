using Softmax.XNotifi.Data.Contracts.Validations;
using Softmax.XNotifi.Extensions;
using Softmax.XNotifi.Models;

namespace Softmax.XNotifi.Services.Validations
{
    public class PaymentValidation : IPaymentValidation {

        private ValidationResult validationResult;
        
        public ValidationResult ValidateCreate(PaymentModel model)
        {
            this.validationResult = new ValidationResult();

            if (model == null)
            {
                this.validationResult.AddErrorMessage(ResourceDesignation.Invalid_Designation);
                return this.validationResult;
            }

            this.ValidateName(model.PaymentMethod);

            return this.validationResult;
        }

        public ValidationResult ValidateUpdate(PaymentModel model)
        {
            this.validationResult = new ValidationResult();

            if (model == null)
            {
                this.validationResult.AddErrorMessage(ResourceDesignation.Invalid_Designation);
                return this.validationResult;
            }

            this.ValidateName(model.PaymentMethod);

            return this.validationResult;
        }

        private void ValidateName(string title)
        {
            if (string.IsNullOrEmpty(title))
            {
                this.validationResult.AddErrorMessage(ResourceDesignation.Invalid_Designation);
            }
        }
    }
}
