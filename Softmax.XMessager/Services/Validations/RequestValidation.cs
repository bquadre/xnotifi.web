﻿using Softmax.XMessager.Data.Contracts.Validations;
using Softmax.XMessager.Data.Entities;
using Softmax.XMessager.Extensions;
using Softmax.XMessager.Models;

namespace Softmax.XMessager.Services.Validations
{
    public class RequestValidation : IRequestValidation {

        private ValidationResult validationResult;
        
        public ValidationResult ValidateCreate(RequestModel model)
        {
            this.validationResult = new ValidationResult();

            if (model == null)
            {
                this.validationResult.AddErrorMessage(ResourceDesignation.Invalid_Designation);
                return this.validationResult;
            }

            this.ValidateName(model.ApplicationId);

            return this.validationResult;
        }

        public ValidationResult ValidateUpdate(RequestModel model)
        {
            this.validationResult = new ValidationResult();

            if (model == null)
            {
                this.validationResult.AddErrorMessage(ResourceDesignation.Invalid_Designation);
                return this.validationResult;
            }

            this.ValidateName(model.ApplicationId);

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
