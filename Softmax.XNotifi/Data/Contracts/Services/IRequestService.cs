using System.Collections.Generic;
using Softmax.XNotifi.Extensions;
using System.Linq;
using Softmax.XNotifi.Models;

namespace Softmax.XNotifi.Data.Contracts.Services
{
    public interface IRequestService 
    {
        Response<RequestModel> Create(RequestModel model);

       IQueryable<RequestModel> List();
    }
}
