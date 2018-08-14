using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Softmax.XNotifi.Utitities
{
    
        public class EmailSettings
        {
            public static int Port
            {
                get
                {
                    return 587;
                }
            }

            public static bool EnableSSL
            {
                get
                {
                    return true;
                }
            }

            public static string HostName
            {
                get
                {
                    return "smtp.gmail.com";
                }
            }

            public static string UserName
            {
                get
                {

                    return "no_reply@lseb.com";
                }
            }

            public static string ServerToken
            {
                get
                {
                    return "5404d5bb-9ff5-4d78-85c1-b149b7e47913";
                    //."5d148c39-9de9-4db2-92e3-6f1d7675d02a""2fc1023d-0e96-434f-b017-f5f83b630410";//"7fc9c137-bd41-43d9-99d0-211ac8fab3f3";
                }
            }


            public static string EmailName
            {
                get
                {
                    return "Softmax Portal";
                }
            }
        }
    
}
