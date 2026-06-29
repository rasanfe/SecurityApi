using System.Text;

namespace SecurityApi.Services.Imp
{
    /// <summary>
    /// Implementación del codificador <see cref="ICoderService"/>. Convierte entre texto
    /// plano y <b>Base64Url</b> (la variante de Base64 segura para URLs).
    /// </summary>
    public class CoderService : ICoderService
    {
        /// <inheritdoc/>
        public string ToBase64Url(string input)
        {
            var bytes = Encoding.UTF8.GetBytes(input);
            var base64 = Convert.ToBase64String(bytes);

            // Pasamos de Base64 a Base64URL: + → -, / → _, y fuera el relleno '='.
            base64 = base64.Replace('+', '-').Replace('/', '_').TrimEnd('=');

            return base64;
        }

        /// <inheritdoc/>
        public string FromBase64Url(string input)
        {
            // Deshacemos el cambio: de Base64URL volvemos a Base64 estándar.
            string base64 = input.Replace('-', '+').Replace('_', '/');

            // Base64 exige que la longitud sea múltiplo de 4; reponemos el relleno '='
            // que habíamos quitado al codificar.
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
