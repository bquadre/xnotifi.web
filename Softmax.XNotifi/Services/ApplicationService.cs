using AutoMapper;
using Softmax.XNotifi.Data.Contracts;
using Softmax.XNotifi.Data.Entities;
using System;
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
        private readonly IMapper _mapper;
        

        public ApplicationService(IRepository<Application> applicationRepository,
              IApplicationValidation applicationValidation,
              IMapper mapper)
                {
                    _applicationRepository = applicationRepository;
                    _applicationValidation = applicationValidation;
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

                   model.IsActive = true;
                   model.DateCreated = DateTime.UtcNow;

                var toEnity = _mapper.Map<Application>(model);
                    _applicationRepository.Insert(toEnity);
                    _applicationRepository.Save();

                var toModel = _mapper.Map<ApplicationModel>(toEnity);
                return new Response<ApplicationModel>
                {
                    Result = toModel,
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

        public  ApplicationModel Get(string id)
        {
            var result = this._applicationRepository.GetById(id);
            return _mapper.Map<ApplicationModel>(result);
        }

        public void Dispose()
        {
            _applicationRepository?.Dispose();
        }    
    }
}