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
    public class ClientService : IClientService, IDisposable
    {
        private readonly IRepository<Client> _clientRepository;
        private readonly IClientValidation _clientValidation;
        private readonly IGenerator _generator;
        private readonly IMapper _mapper;
        

        public ClientService(IRepository<Client> clientRepository,
              IClientValidation clientValidation,
              IGenerator generator,
              IMapper mapper
              )
        {
            _clientRepository = clientRepository;
            _clientValidation = clientValidation;
            _generator = generator;
            _mapper = mapper;
        }

        public Response<ClientModel> Create(ClientModel model)
        {
            try
            {
                var validationResult = this._clientValidation.ValidateCreate(model);
                if (!validationResult.IsValid)
                    return new Response<ClientModel>
                    {
                        Message = validationResult.ErrorMessage,
                        ResultType = ResultType.ValidationError
                    };

                     model.DateCreated = DateTime.UtcNow;
                     model.IsActive = true;
                     model.AccessKey = _generator.GenerateGuid().Result;
                     model.Code = _generator.RandomNumber(1000, 9999).Result;
                     model.CodeExpired = DateTime.UtcNow.AddDays(1);

                        var toEntity = _mapper.Map<Client>(model);
                        _clientRepository.Insert(toEntity);
                        _clientRepository.Save();

                model = _mapper.Map<ClientModel>(toEntity);

                return new Response<ClientModel>
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

            return new Response<ClientModel>
            {
                ResultType = ResultType.Error
            };
        }

        public Response<ClientModel> Update(ClientModel model)
        {
            try
            {
                var validationResult = this._clientValidation.ValidateUpdate(model);
                if (!validationResult.IsValid)
                    return new Response<ClientModel>
                    {
                        Message = validationResult.ErrorMessage,
                        ResultType = ResultType.ValidationError
                    };

                var updatingObj = _clientRepository.GetById(model.ClientId);
                updatingObj.Company = model.Company;
                updatingObj.Address = model.Address;
                updatingObj.Role = model.Role;
                updatingObj.FirstName = model.FirstName;
                updatingObj.LastName = model.LastName;
                updatingObj.EmailAddress = model.EmailAddress;
                updatingObj.PhoneNumber = model.PhoneNumber;
                updatingObj.Balance = model.Balance;
                updatingObj.AccessKey = model.AccessKey;
                updatingObj.EmailConfirmed = model.EmailConfirmed;
                updatingObj.PhoneConfirmed = model.PhoneConfirmed;
                updatingObj.Code = model.Code;
                updatingObj.CodeExpired = model.CodeExpired;
                updatingObj.IsActive = model.IsActive;
                _clientRepository.Save();
               
                return new Response<ClientModel>
                {
                    Result = _mapper.Map<ClientModel>(updatingObj),
                    ResultType = ResultType.Success
                };
            }
            catch (Exception ex)
            {
                //online error log
                var err = ex.Message;
            }

            return new Response<ClientModel>
            {
                ResultType = ResultType.Error
            };
        }

        public Response<ClientModel> Delete(string id)
        {
            try
            {

                if (string.IsNullOrEmpty(id))
                {
                    return new Response<ClientModel>
                    {
                        ResultType = ResultType.ValidationError
                    };

                }
                var designation = this._clientRepository.GetById(id);
                designation.IsDeleted = true;
                _clientRepository.Update(designation);
                _clientRepository.Save();

                return new Response<ClientModel>
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

            return new Response<ClientModel>
            {
                ResultType = ResultType.Error
            };
        }

        public IQueryable<ClientModel> List()
        {
            var result = _clientRepository.GetAll();
            return result.ProjectTo<ClientModel>();
        }

        public ClientModel Get(string id)
        { 
            var result = _clientRepository.GetById(id);
            return _mapper.Map<ClientModel>(result);
        }

        public void Dispose()
        {
            _clientRepository?.Dispose();
        }    
    }
}