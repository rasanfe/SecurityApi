using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SecurityApi.Services
{
    public interface IKeyGeneratorService
    {
        public bool Validate(string password);
        public string Generate();
        public void SetTotalChars(int totalCharacters);
        //private string Shuffle(string str, Random random);

        //private string ToBase64Url(string input);
    }
}
