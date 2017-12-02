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
        private readonly IGenerator _generator;
        private readonly IMapper _mapper;

        public RequestService(IRepository<Request> requestRepository,
              IRequestValidation requestValidation,
              IGenerator generator,
              IMapper mapper
              )
        {
            _requestRepository = requestRepository;
            _requestValidation = requestValidation;
            _generator = generator;
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
                
                    this._requestRepository.Insert(_mapper.Map<Request>(model));
                    this._requestRepository.Save();

                return new Response<RequestModel>
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

        public Response<List<RequestModel>> List(string select="", string search="")
        {
            IQueryable<Request> result;
            try
            {

                if (string.IsNullOrEmpty(select) && string.IsNullOrEmpty(search))
                {
                    result = this._requestRepository.GetAll();

                }

                if (!string.IsNullOrEmpty(select) && string.IsNullOrEmpty(search))
                {
                    result = this._requestRepository.GetAll().Where(x=>x.ApplicationId.Equals(select));

                }
                if (!string.IsNullOrEmpty(select) && !string.IsNullOrEmpty(search))
                {
                    result = this._requestRepository.GetAll().Where(x => x.ApplicationId.Equals(select) && 
                    x.Application.Name.Contains(search, StringComparison.InvariantCultureIgnoreCase));

                }
                else
                {
                    result = this._requestRepository.GetAll()
                        .Where(x => x.Application.Name.Contains(search, StringComparison.InvariantCultureIgnoreCase));
                }

            }
            catch (Exception e)
            {
                //Console.WriteLine(e);
                //throw;
                return new Response<List<RequestModel>>()
                {
                    ResultType = ResultType.Error
                };
            }


            var model = result.ProjectTo<RequestModel>()
                .OrderBy(x => x.DateCreated)
                .ToList();
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