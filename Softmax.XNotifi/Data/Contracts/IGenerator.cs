using Softmax.XNotifi.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Softmax.XNotifi.Data.Contracts
{
    public interface IGenerator
    {
        /// <summary>
        /// Generates guid.
        /// </summary>
        /// <returns> Returns generated unique guid </returns>
        Response<string> GenerateGuid();

        /// <summary>
        /// Generates random numbers.
        /// </summary>
        /// <returns> Returns generated unique guid </returns>
        Response<string> RandomNumber(int min, int max);


        Response<string> DateCodeString();

        Response<string> TempPassword(int num);

        Response<string> Encrypt(string text);
        Response<string> Decrypt(string text);

    }

}
