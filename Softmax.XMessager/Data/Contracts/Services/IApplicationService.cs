using System.Collections.Generic;
using Softmax.XMessager.Extensions;
using System.Linq;
using Softmax.XMessager.Data.Entities;
using Softmax.XMessager.Models;

namespace Softmax.XMessager.Data.Contracts.Services
{
    public interface IApplicationService 
    {
        Response<ApplicationModel> Create(ApplicationModel model);

        Response<ApplicationModel> Update(ApplicationModel model);

        Response<List<ApplicationModel>> List(string search = "");

        Response<ApplicationModel> Get(string id);

    }
}
