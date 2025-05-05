using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SecurityApi.Services
{
    public interface ICoderService
    {
        public string ToBase64Url(string input);
        public string FromBase64Url(string input);
        
    }
}
