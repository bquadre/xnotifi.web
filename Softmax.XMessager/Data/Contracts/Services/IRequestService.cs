using System.Collections.Generic;
using Softmax.XMessager.Extensions;
using System.Linq;
using Softmax.XMessager.Models;

namespace Softmax.XMessager.Data.Contracts.Services
{
    public interface IRequestService 
    {
        Response<RequestModel> Create(RequestModel model);

        Response<List<RequestModel>> List(string select="", string search = "");
    }
}
