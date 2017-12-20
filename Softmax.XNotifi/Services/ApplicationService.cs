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
    public class ApplicationService : IApplicationService, IDisposable
    {
        private readonly IRepository<Application> _applicationRepository;
        private readonly IApplicationValidation _applicationValidation;
        private readonly IGenerator _generator;
        private readonly IMapper _mapper;
        

        public ApplicationService(IRepository<Application> applicationRepository,
              IApplicationValidation applicationValidation,
              IGenerator generator,
              IMapper mapper
              )
        {
            _applicationRepository = applicationRepository;
            _applicationValidation = applicationValidation;
            _generator = generator;
            _mapper = mapper;
        }

        public Response<ApplicationModel> Create(ApplicationModel model)
        {
            try
            {
                var validationResult = _applicationValidation.ValidateCreate(model);
                if (!validationResult.IsValid)
                    return new Response<ApplicationModel>
                    {
                        Message = validationResult.ErrorMessage,
                        ResultType = ResultType.ValidationError
                    };

                   model.Key = _generator.GenerateGuid().Result;
                   model.IsActive = true;
                   model.DateCreated = DateTime.UtcNow;

                
                    _applicationRepository.Insert(_mapper.Map<Application>(model));
                    _applicationRepository.Save();

                return new Response<ApplicationModel>
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

            return new Response<ApplicationModel>
            {
                ResultType = ResultType.Error
            };
        }

        public Response<ApplicationModel> Update(ApplicationModel model)
        {
            try
            {
                var validationResult = this._applicationValidation.ValidateUpdate(model);
                if (!validationResult.IsValid)
                    return new Response<ApplicationModel>
                    {
                        Message = validationResult.ErrorMessage,
                        ResultType = ResultType.ValidationError
                    };

                var updatingObj = _applicationRepository.GetById(model.ApplicationId);
                updatingObj.IsActive = model.IsActive;
                updatingObj.Name = model.Name;
                updatingObj.Url = model.Url;
                updatingObj.Key = model.Key;

                _applicationRepository.Save();
               
                return new Response<ApplicationModel>
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

            return new Response<ApplicationModel>
            {
                ResultType = ResultType.Error
            };
        }

        public IQueryable<ApplicationModel> List()
        {
           var result = _applicationRepository.GetAll();
           return result.ProjectTo<ApplicationModel>();
        }

        public Response<ApplicationModel> Get(string id)
        {
            try
            {
                var result = this._applicationRepository.GetById(id);
           
                return new Response<ApplicationModel>()
                {
                    ResultType = ResultType.Success,
                    Result = _mapper.Map<ApplicationModel>(result)
                };
            }
            catch (Exception ex)
            {
                //online error log
                var err = ex.Message;
            }

            return new Response<ApplicationModel>() { ResultType = ResultType.Error };
        }

        public void Dispose()
        {
            _applicationRepository?.Dispose();
        }    
    }
}