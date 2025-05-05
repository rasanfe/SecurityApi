using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SecurityApi.Services
{
    public interface ISecurityService
    {
        public string Encrypt(string source, string key, string iv);
        public string Decrypt(string source, string key, string iv);
        public bool GetToken(string token, string masterKey, string masterIv, ref string key, ref string iv);
    }
}
