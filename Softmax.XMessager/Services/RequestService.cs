using AutoMapper;
using Softmax.XMessager.Data.Contracts;
using Softmax.XMessager.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper.QueryableExtensions;
using Softmax.XMessager.Models;
using Softmax.XMessager.Extensions;
using Softmax.XMessager.Data.Enums;
using Softmax.XMessager.Data.Contracts.Services;
using Softmax.XMessager.Data.Contracts.Validations;

namespace Softmax.XMessager.Services
{
    public class RequestService : IRequestService, IDisposable
    {
        private readonly IRepository<Request> _requestRepository;
        private readonly IRequestValidation _requestValidation;
        private readonly IMapper _mapper;

        public RequestService(IRepository<Request> requestRepository,
              IRequestValidation requestValidation,
              IMapper mapper
              )
        {
            _requestRepository = requestRepository;
            _requestValidation = requestValidation;
            _mapper = mapper;
        }

        public Response<RequestModel> Create(RequestModel model)
        {
            try
            {
                var validationResult = this._requestValidation.ValidateCreate(model);
                if (!validationResult.IsValid)
                    return new Response<RequestModel>
                    {
                        Message = validationResult.ErrorMessage,
                        ResultType = ResultType.ValidationError
                    };

                   var entity = _mapper.Map<Request>(model);
                    _requestRepository.Insert(entity);
                    _requestRepository.Save();

                model = _mapper.Map<RequestModel>(entity);
                return new Response<RequestModel>()
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

            return new Response<RequestModel>
            {
                ResultType = ResultType.Error
            };
        }

        public Response<List<RequestModel>> List()
        {
            var  result = _requestRepository.GetAll();
            var model = result.ProjectTo<RequestModel>().ToList();
            return new Response<List<RequestModel>>()
            {
                ResultType = ResultType.Success,
                Result = model
            };
        }

        public void Dispose()
        {
            _requestRepository?.Dispose();
        }    
    }
}