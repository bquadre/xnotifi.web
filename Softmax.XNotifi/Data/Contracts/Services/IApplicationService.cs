using System.Collections.Generic;
using Softmax.XNotifi.Extensions;
using System.Linq;
using Softmax.XNotifi.Data.Entities;
using Softmax.XNotifi.Models;

namespace Softmax.XNotifi.Data.Contracts.Services
{
    public interface IApplicationService 
    {
        Response<ApplicationModel> Create(ApplicationModel model);

        Response<ApplicationModel> Update(ApplicationModel model);

        IQueryable<ApplicationModel> List();

        ApplicationModel Get(string id);

    }
}
