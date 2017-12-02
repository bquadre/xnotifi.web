using Softmax.XMessager.Data.Contracts.Validations;
using Softmax.XMessager.Data.Entities;
using Softmax.XMessager.Extensions;
using Softmax.XMessager.Models;

namespace Softmax.XMessager.Services.Validations
{
    public class GatewayValidation : IGatewayValidation {

        private ValidationResult validationResult;
        
        public ValidationResult ValidateCreate(GatewayModel model)
        {
            this.validationResult = new ValidationResult();

            if (model == null)
            {
                this.validationResult.AddErrorMessage(ResourceDesignation.Invalid_Designation);
                return this.validationResult;
            }

            this.ValidateName(model.Provider);

            return this.validationResult;
        }

        public ValidationResult ValidateUpdate(GatewayModel model)
        {
            this.validationResult = new ValidationResult();

            if (model == null)
            {
                this.validationResult.AddErrorMessage(ResourceDesignation.Invalid_Designation);
                return this.validationResult;
            }

            this.ValidateName(model.Provider);

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
