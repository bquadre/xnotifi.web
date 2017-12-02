using Softmax.XMessager.Data.Contracts;
using Softmax.XMessager.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Softmax.XMessager.Extensions;
using Softmax.XMessager.Data.Enums;
using Softmax.XMessager.Data.Contracts.Services;
using Softmax.XMessager.Data.Contracts.Validations;
using Softmax.XMessager.Models;

namespace Softmax.XMessager.Services
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
                   model.IsSecure = true;
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

        public Response<List<GatewayModel>> List(string search)
        {
            IQueryable<Gateway> result;
            try
            {

                if (string.IsNullOrEmpty(search))
                {
                    result = this._gatewayRepository.GetAll();
                }
                else
                {
                    result = this._gatewayRepository.GetAll();
                                  //.Where(x => x.Location.Contains(search, StringComparison.InvariantCultureIgnoreCase)
                                  //  || x.BranchCode.Contains(search, StringComparison.InvariantCultureIgnoreCase));
                }

            }
            catch (Exception e)
            {
                //Console.WriteLine(e);
                //throw;
                return new Response<List<GatewayModel>>()
                {
                    ResultType = ResultType.Error
                };
            }


             var model = result.ProjectTo<GatewayModel>()
                .Where(x=>x.IsDeleted==false)
                .OrderBy(x=>x.DateCreated)
                .ToList();
            return new Response<List<GatewayModel>>()
            {
                ResultType = ResultType.Success,
                Result = model
        };


        }
  
        public Response<GatewayModel> Get(string id)
        {
            try
            {
                var result = this._gatewayRepository.GetById(id);
                return new Response<GatewayModel>()
                {
                    ResultType = ResultType.Success,
                    Result = _mapper.Map<GatewayModel>(result)
                };
            }
            catch (Exception ex)
            {
                //online error log
                var err = ex.Message;
            }

            return new Response<GatewayModel>()
            {
                ResultType = ResultType.Error
            };
        }
  
        public void Dispose()
        {
            _gatewayRepository?.Dispose();
        }
    }
}