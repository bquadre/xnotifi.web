using Softmax.XNotifi.Data.Contracts;
using Softmax.XNotifi.Data.Entities;
using System;
using System.Collections.Generic;
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
    public class GatewayService : IGatewayService, IDisposable
    {
        private readonly IRepository<Gateway> _gatewayRepository;
        private readonly IGatewayValidation _gatewayValidation;
        private readonly IGenerator _generator;
        private readonly IMapper _mapper;
      
        public GatewayService(IRepository<Gateway> gatewayRepository,
              IGatewayValidation gatewayValidation,
              IGenerator generator,
              IMapper mapper)
        {
            _gatewayRepository = gatewayRepository;
            _gatewayValidation =  gatewayValidation;
            _generator = generator;
            _mapper = mapper;        
        }

        public Response<GatewayModel> Create(GatewayModel model)
        {
            try
            {
                var validationResult = _gatewayValidation.ValidateCreate(model);
                if (!validationResult.IsValid)
                    return new Response<GatewayModel>
                    {
                        Message = validationResult.ErrorMessage,
                        ResultType = ResultType.ValidationError
                    };

                   model.GatewayId = _generator.GenerateGuid().Result;
                   model.Password = _generator.Encrypt(model.Password).Result;
                   model.IsActive =true;
                   model.DateCreated = DateTime.UtcNow;

                _gatewayRepository.Insert(_mapper.Map<Gateway>(model));
                _gatewayRepository.Save();

                return new Response<GatewayModel>
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

            return new Response<GatewayModel>
            {
                ResultType = ResultType.Error
            };
        }

        public Response<GatewayModel> Update(GatewayModel model)
        {
            try
            {
                var validationResult = this._gatewayValidation.ValidateUpdate(model);
                if (!validationResult.IsValid)
                    return new Response<GatewayModel>
                    {
                        Message = validationResult.ErrorMessage,
                        ResultType = ResultType.ValidationError
                    };

                model.Password = _generator.Encrypt(model.Password).Result;
                _gatewayRepository.Update(_mapper.Map<Gateway>(model));
                _gatewayRepository.Save();

                return new Response<GatewayModel>
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

            return new Response<GatewayModel>
            {
                ResultType = ResultType.Error
            };
        }

        public Response<GatewayModel> Delete(string id)
        {
            try
            {

                if (string.IsNullOrEmpty(id))
                {
                    return new Response<GatewayModel>
                    {
                        ResultType = ResultType.ValidationError
                    };

                }
                var branch = this._gatewayRepository.GetById(id);
                branch.IsDeleted = true;
                this._gatewayRepository.Update(branch);
                this._gatewayRepository.Save();

                return new Response<GatewayModel>
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

            return new Response<GatewayModel>
            {
                ResultType = ResultType.Error
            };
        }

        public IQueryable<GatewayModel> List()
        {
            var result = _gatewayRepository.GetAll();
            return result?.ProjectTo<GatewayModel>();
        }

        public GatewayModel Get(string id)
        {
            var result = _gatewayRepository.GetById(id);
            return result == null ? null : _mapper.Map<GatewayModel>(result);
        }

        public void Dispose()
        {
            _gatewayRepository?.Dispose();
        }
    }
}