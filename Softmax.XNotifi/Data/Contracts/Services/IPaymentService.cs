using System.Collections.Generic;
using Softmax.XNotifi.Extensions;
using System.Linq;
using Softmax.XNotifi.Models;

namespace Softmax.XNotifi.Data.Contracts.Services
{
    public interface IPaymentService 
    {
        Response<PaymentModel> Create(PaymentModel model);

        Response<PaymentModel> Update(PaymentModel model);

        Response<PaymentModel> Delete(string id);

        IQueryable<PaymentModel> List();

        PaymentModel Get(string id);
    }
}
