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
                    _clientRepository.Insert(_mapper.Map<Client>(model));
                    _clientRepository.Save();

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
                updatingObj.Role = model.Role;
                updatingObj.FirstName = model.FirstName;
                updatingObj.LastName = model.LastName;
                updatingObj.EmailAddress = model.EmailAddress;
                updatingObj.PhoneNumber = model.PhoneNumber;
                updatingObj.Balance = model.Balance;
                updatingObj.IsActive = model.IsActive;
                _clientRepository.Save();
               
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

        public Response<ClientModel> Get(string id)
        {
            try
            {
                var result = this._clientRepository.GetById(id);
           
                return new Response<ClientModel>()
                {
                    ResultType = ResultType.Success,
                    Result = _mapper.Map<ClientModel>(result)
                };
            }
            catch (Exception ex)
            {
                //online error log
                var err = ex.Message;
            }

            return new Response<ClientModel>() { ResultType = ResultType.Error };
        }

        public void Dispose()
        {
            _clientRepository?.Dispose();
        }    
    }
}