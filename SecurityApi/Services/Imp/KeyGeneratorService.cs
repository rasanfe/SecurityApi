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
    public class KeyGeneratorService : IKeyGeneratorService
    {
        private const string LowercaseLetters = "abcdefghijklmnopqrstuvwxyz";
        private const string UppercaseLetters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        private const string Numbers = "1234567890";
        private const string Symbols = "!@#$%^&*()-_+={}[]|;:.<>?`~,";
        private int totalCharacters = 12;
       
        public bool Validate(string password)
        {
            bool hasUppercase = false;
            bool hasLowercase = false;
            bool hasNumber = false;
            bool hasSymbol = false;
            bool isLengthValid = true;

            if (password.Length < totalCharacters)
            {
                isLengthValid = false;
            }
            else
            {
                foreach (char character in password)
                {
                    if (UppercaseLetters.Contains(character)) hasUppercase = true;
                    if (LowercaseLetters.Contains(character)) hasLowercase = true;
                    if (Numbers.Contains(character)) hasNumber = true;
                    if (Symbols.Contains(character)) hasSymbol = true;
                }
            }

            return isLengthValid && hasUppercase && hasLowercase && hasNumber && hasSymbol;
        }

        public string Generate()
        {
            Random random = new Random();
            StringBuilder password = new StringBuilder();

            password.Append(UppercaseLetters[random.Next(UppercaseLetters.Length)]);
            password.Append(LowercaseLetters[random.Next(LowercaseLetters.Length)]);
            password.Append(Numbers[random.Next(Numbers.Length)]);
            password.Append(Symbols[random.Next(Symbols.Length)]);

            string allCharacters = UppercaseLetters + LowercaseLetters + Numbers + Symbols;
            for (int i = 4; i < totalCharacters; i++)
            {
                password.Append(allCharacters[random.Next(allCharacters.Length)]);
            }

            string shuffledPassword = Shuffle(password.ToString(), random);

            if (!Validate(shuffledPassword))
            {
                return Generate();
            }
                        
            return shuffledPassword;
        }

        public void SetTotalChars(int totalCharacters)
        {
            this.totalCharacters = totalCharacters;
        }

        private string Shuffle(string str, Random random)
        {
            char[] array = str.ToCharArray();
            for (int i = array.Length - 1; i > 0; i--)
            {
                int j = random.Next(i + 1);
                var temp = array[i];
                array[i] = array[j];
                array[j] = temp;
            }
            return new string(array);
        }


    }
}
