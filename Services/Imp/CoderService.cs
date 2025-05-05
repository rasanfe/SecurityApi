using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json.Linq;

namespace SecurityApi.Services.Imp
{
    public class CoderService : ICoderService
    {
        public string ToBase64Url(string input)
        {
            var bytes = Encoding.UTF8.GetBytes(input);
            var base64 = Convert.ToBase64String(bytes);

            // Convert to Base64URL
            base64 = base64.Replace('+', '-').Replace('/', '_').TrimEnd('=');

            return base64;
        }
        public string FromBase64Url(string input)
        {
            // Convert from Base64URL to Base64
            string base64 = input.Replace('-', '+').Replace('_', '/');

            // Add padding if necessary
            switch (base64.Length % 4)
            {
                case 2: base64 += "=="; break;
                case 3: base64 += "="; break;
            }

            var bytes = Convert.FromBase64String(base64);
            return Encoding.UTF8.GetString(bytes);
        }
        
    }
}
