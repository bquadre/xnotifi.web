using AutoMapper;
using Softmax.XNotifi.Data.Contracts;
using Softmax.XNotifi.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper.QueryableExtensions;
using Softmax.XNotifi.Models;
using Softmax.XNotifi.Extensions;
using Softmax.XNotifi.Data.Enums;
using Softmax.XNotifi.Data.Contracts.Services;
using Softmax.XNotifi.Data.Contracts.Validations;

namespace Softmax.XNotifi.Services
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

        public IQueryable<RequestModel> List()
        {
            var  result = _requestRepository.GetAll();
            return result.ProjectTo<RequestModel>();
        }

        public void Dispose()
        {
            _requestRepository?.Dispose();
        }    
    }
}