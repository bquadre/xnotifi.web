using System.Collections.Generic;
using Softmax.XNotifi.Extensions;
using System.Linq;
using Softmax.XNotifi.Models;

namespace Softmax.XNotifi.Data.Contracts.Services
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
