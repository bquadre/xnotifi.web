using System.Collections.Generic;
using Softmax.XMessager.Extensions;
using System.Linq;
using Softmax.XMessager.Models;

namespace Softmax.XMessager.Data.Contracts.Services
{
    public interface IGatewayService 
    {
        Response<GatewayModel> Create(GatewayModel model);

        Response<GatewayModel> Update(GatewayModel model);

        Response<GatewayModel> Delete(string id);

        Response<List<GatewayModel>> List();

        Response<GatewayModel> Get(string id);
    }
}
