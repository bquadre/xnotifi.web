using System.Collections.Generic;
using Softmax.XNotifi.Extensions;
using System.Linq;
using Softmax.XNotifi.Data.Entities;
using Softmax.XNotifi.Models;

namespace Softmax.XNotifi.Data.Contracts.Services
{
    public interface IClientService 
    {
        Response<ClientModel> Create(ClientModel model);

        Response<ClientModel> Update(ClientModel model);

        Response<ClientModel> Delete(string id);

        IQueryable<ClientModel> List();

        Response<ClientModel> Get(string id);

    }
}
