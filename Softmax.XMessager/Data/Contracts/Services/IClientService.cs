using System.Collections.Generic;
using Softmax.XMessager.Extensions;
using System.Linq;
using Softmax.XMessager.Data.Entities;
using Softmax.XMessager.Models;

namespace Softmax.XMessager.Data.Contracts.Services
{
    public interface IClientService 
    {
        Response<ClientModel> Create(ClientModel model);

        Response<ClientModel> Update(ClientModel model);

        Response<ClientModel> Delete(string id);

        Response<List<ClientModel>> List(string search = "");

        Response<ClientModel> Get(string id);

    }
}
