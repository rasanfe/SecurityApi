using System.Text;

namespace SecurityApi.Services.Imp
{
    /// <summary>
    /// Generador y validador de claves robustas. Garantiza que toda clave generada lleve
    /// al menos una mayúscula, una minúscula, un número y un símbolo, y la longitud mínima
    /// configurada. Se registra como Singleton (ver Program.cs).
    /// </summary>
    public class KeyGeneratorService : IKeyGeneratorService
    {
        // Los "alfabetos" de los que tiramos para construir y validar las claves.
        private const string LowercaseLetters = "abcdefghijklmnopqrstuvwxyz";
        private const string UppercaseLetters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        private const string Numbers = "1234567890";
        private const string Symbols = "!@#$%^&*()-_+={}[]|;:.<>?`~,";

        // Longitud mínima exigida. Ajustable con SetTotalChars.
        private int totalCharacters = 12;

        /// <inheritdoc/>
        public bool Validate(string password)
        {
            bool hasUppercase = false;
            bool hasLowercase = false;
            bool hasNumber = false;
            bool hasSymbol = false;
            bool isLengthValid = true;

            // Primero la longitud; si no llega, ya es inválida.
            if (password.Length < totalCharacters)
            {
                isLengthValid = false;
            }
            else
            {
                // Recorremos la clave marcando qué tipos de carácter aparecen.
                foreach (char character in password)
                {
                    if (UppercaseLetters.Contains(character)) hasUppercase = true;
                    if (LowercaseLetters.Contains(character)) hasLowercase = true;
                    if (Numbers.Contains(character)) hasNumber = true;
                    if (Symbols.Contains(character)) hasSymbol = true;
                }
            }

            // Válida sólo si cumple longitud Y los cuatro tipos de carácter.
            return isLengthValid && hasUppercase && hasLowercase && hasNumber && hasSymbol;
        }

        /// <inheritdoc/>
        public string Generate()
        {
            Random random = new Random();
            StringBuilder password = new StringBuilder();

            // Sembramos un carácter de cada tipo para garantizar la complejidad mínima.
            password.Append(UppercaseLetters[random.Next(UppercaseLetters.Length)]);
            password.Append(LowercaseLetters[random.Next(LowercaseLetters.Length)]);
            password.Append(Numbers[random.Next(Numbers.Length)]);
            password.Append(Symbols[random.Next(Symbols.Length)]);

            // El resto, hasta la longitud pedida, lo rellenamos de cualquier tipo.
            string allCharacters = UppercaseLetters + LowercaseLetters + Numbers + Symbols;
            for (int i = 4; i < totalCharacters; i++)
            {
                password.Append(allCharacters[random.Next(allCharacters.Length)]);
            }

            // Barajamos para que los 4 caracteres sembrados no queden siempre al principio.
            string shuffledPassword = Shuffle(password.ToString(), random);

            // Red de seguridad: si por lo que sea no valida (p. ej. longitud < 4), reintenta.
            if (!Validate(shuffledPassword))
            {
                return Generate();
            }

            return shuffledPassword;
        }

        /// <inheritdoc/>
        public void SetTotalChars(int totalCharacters)
        {
            this.totalCharacters = totalCharacters;
        }

        // Baraja de Fisher-Yates: mezcla los caracteres de forma uniforme.
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
