using Softmax.XNotifi.Data.Contracts;
using Softmax.XNotifi.Data.Entities;
using System;
using System.Linq;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Softmax.XNotifi.Extensions;
using Softmax.XNotifi.Data.Enums;
using Softmax.XNotifi.Data.Contracts.Services;
using Softmax.XNotifi.Data.Contracts.Validations;
using Softmax.XNotifi.Models;

namespace Softmax.XNotifi.Services
{
    public class PaymentService : IPaymentService, IDisposable
    {
        private readonly IRepository<Payment> _paymentRepository;
        private readonly IPaymentValidation _paymentValidation;
        private readonly IGenerator _generator;
        private readonly IMapper _mapper;
      
        public PaymentService(IRepository<Payment> paymentRepository,
              IPaymentValidation paymentValidation,
              IGenerator generator,
              IMapper mapper)
        {
            _paymentRepository = paymentRepository;
            _paymentValidation =  paymentValidation;
            _generator = generator;
            _mapper = mapper;        
        }

        public Response<PaymentModel> Create(PaymentModel model)
        {
            try
            {
                var validationResult = _paymentValidation.ValidateCreate(model);
                if (!validationResult.IsValid)
                    return new Response<PaymentModel>
                    {
                        Message = validationResult.ErrorMessage,
                        ResultType = ResultType.ValidationError
                    };

                   model.DateCreated = DateTime.UtcNow;

                _paymentRepository.Insert(_mapper.Map<Payment>(model));
                _paymentRepository.Save();

                return new Response<PaymentModel>
                {
                    Result = model,
                    ResultType = ResultType.Success
                };
            }
            catch (Exception ex)
            {
                //online error log
                var err = ex.Message;
            }

            return new Response<PaymentModel>
            {
                ResultType = ResultType.Error
            };
        }

        public Response<PaymentModel> Update(PaymentModel model)
        {
            try
            {
                var validationResult = this._paymentValidation.ValidateUpdate(model);
                if (!validationResult.IsValid)
                    return new Response<PaymentModel>
                    {
                        Message = validationResult.ErrorMessage,
                        ResultType = ResultType.ValidationError
                    };

                _paymentRepository.Update(_mapper.Map<Payment>(model));
                _paymentRepository.Save();

                return new Response<PaymentModel>
                {
                    Result = model,
                    ResultType = ResultType.Success
                };
            }
            catch (Exception ex)
            {
                //online error log
                var err = ex.Message;
            }

            return new Response<PaymentModel>
            {
                ResultType = ResultType.Error
            };
        }

        public Response<PaymentModel> Delete(string id)
        {
            try
            {

                if (string.IsNullOrEmpty(id))
                {
                    return new Response<PaymentModel>
                    {
                        ResultType = ResultType.ValidationError
                    };

                }
                var branch = this._paymentRepository.GetById(id);
                this._paymentRepository.Update(branch);
                this._paymentRepository.Save();

                return new Response<PaymentModel>
                {
                    //Result = model,
                    ResultType = ResultType.Success
                };
            }
            catch (Exception ex)
            {
                //online error log
                var err = ex.Message;
            }

            return new Response<PaymentModel>
            {
                ResultType = ResultType.Error
            };
        }

        public IQueryable<PaymentModel> List()
        {
            var result = _paymentRepository.GetAll();
            return result?.ProjectTo<PaymentModel>();
        }

        public PaymentModel Get(string id)
        {
            var result = _paymentRepository.GetById(id);
            return result == null ? null : _mapper.Map<PaymentModel>(result);
        }

        public void Dispose()
        {
            _paymentRepository?.Dispose();
        }
    }
}