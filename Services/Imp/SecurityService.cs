using System.IO;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json.Linq;

namespace SecurityApi.Services.Imp
{
    /// <summary>
    /// Implementación del cifrado simétrico <b>AES</b> (<see cref="ISecurityService"/>).
    /// Pequeño glosario para los que venís de PowerBuilder:
    /// <list type="bullet">
    ///   <item><description><b>AES</b>: algoritmo simétrico (la MISMA clave cifra y descifra).</description></item>
    ///   <item><description><b>Key</b>: la clave secreta. Aquí AES-128, o sea 16 bytes.</description></item>
    ///   <item><description><b>IV</b> (vector de inicialización): 16 bytes que "aleatorizan" el cifrado
    ///     para que el mismo texto no produzca siempre el mismo resultado.</description></item>
    ///   <item><description><b>CBC</b>: modo de encadenamiento de bloques; <b>PKCS7</b>: relleno
    ///     del último bloque hasta completar los 16 bytes.</description></item>
    /// </list>
    /// Clave e IV deben coincidir EXACTAMENTE al cifrar y al descifrar, o el resultado es basura.
    /// </summary>
    public class SecurityService : ISecurityService
    {
        /// <inheritdoc/>
        public string Encrypt(string source, string key, string iv)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(source)) return "";

                // AES-128 necesita clave e IV de 16 bytes EXACTOS. Si llegan más cortos,
                // los rellenamos a 16 (truco didáctico para no obligar al cliente a medir;
                // en producción usaríais claves/IV de longitud correcta de origen).
                key = key.PadRight(16, '*');
                iv = iv.PadRight(16, '0');

                byte[] InitialVectorBytes = Encoding.UTF8.GetBytes(iv);
                byte[] keyBytes = Encoding.UTF8.GetBytes(key);
                byte[] encrypted;

                // Aes.Create() nos da la implementación de AES del sistema. Lo configuramos:
                var aes = Aes.Create();
                aes.KeySize = 128;
                aes.Key = keyBytes;
                aes.IV = InitialVectorBytes;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;

                // El cifrado se hace "en streaming": texto → CryptoStream → MemoryStream.
                // Los 'using' garantizan que cada stream se cierra/vacía al salir (libera
                // recursos), parecido al DESTROY de PowerBuilder pero automático.
                using (var encryptor = aes.CreateEncryptor())
                {
                    using (var msEncrypt = new MemoryStream())
                    {
                        using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                        {
                            using (var swEncrypt = new StreamWriter(csEncrypt))
                            {
                                // Escribimos el texto; al cerrar el StreamWriter se vuelca
                                // todo cifrado al MemoryStream.
                                swEncrypt.Write(source);
                            }
                            encrypted = msEncrypt.ToArray();
                        }
                    }
                }

                string sourceFinal = Convert.ToBase64String(encrypted);

                // Pasamos a Base64URL para que viaje sin problemas por URL/JSON.
                sourceFinal = sourceFinal.Replace("+", "-").Replace("/", "_").Replace("=", "");

                return sourceFinal;
            }
            catch (Exception ex)
            {
                // Patrón didáctico: devolvemos el mensaje en vez de propagar la excepción.
                return ex.Message;
            }
        }

        /// <inheritdoc/>
        public string Decrypt(string source, string key, string iv)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(source)) return "";

                // Mismo ajuste de tamaños que en Encrypt: clave e IV de 16 bytes.
                key = key.PadRight(16, '*');
                iv = iv.PadRight(16, '0');

                byte[] InitialVectorBytes = Encoding.UTF8.GetBytes(iv);
                byte[] keyBytes = Encoding.UTF8.GetBytes(key);

                // El texto cifrado llega en Base64URL; lo devolvemos a Base64 estándar
                // y reponemos el relleno '=' (la longitud debe ser múltiplo de 4).
                source = source.Replace("-", "+").Replace("_", "/");
                while (source.Length % 4 != 0)
                {
                    source += "=";
                }

                // Y de Base64 a los bytes cifrados de verdad.
                byte[] cipherBytes = Convert.FromBase64String(source);

                var aes = Aes.Create();
                aes.KeySize = 128;
                aes.Key = keyBytes;
                aes.IV = InitialVectorBytes;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;

                string decrypted;

                // Descifrado en streaming, el camino inverso al de Encrypt.
                using (var decryptor = aes.CreateDecryptor())
                {
                    using (var msDecrypt = new MemoryStream(cipherBytes))
                    {
                        using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                        {
                            using (var srDecrypt = new StreamReader(csDecrypt))
                            {
                                // Leemos del stream ya descifrado el texto original.
                                decrypted = srDecrypt.ReadToEnd();
                            }
                        }
                    }
                }

                return decrypted;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        /// <inheritdoc/>
        public bool GetToken(string token, string masterKey, string masterIv, ref string key, ref string iv)
        {
            // La clave maestra también se ajusta a 16 bytes.
            masterKey = masterKey.PadRight(16, '*');
            masterIv = masterIv.PadRight(16, '0');

            if (string.IsNullOrWhiteSpace(token) ||
                string.IsNullOrWhiteSpace(masterKey) ||
                string.IsNullOrWhiteSpace(masterIv)) return false;

            try
            {
                // El token NO es más que un JSON { key, IV } cifrado con la clave maestra.
                // Lo desciframos reutilizando el propio Decrypt...
                string json = Decrypt(token, masterKey, masterIv);

                // ...y extraemos de su interior la clave/IV de sesión.
                JObject jsonObject = JObject.Parse(json);

                key = jsonObject["key"]!.ToString();
                iv = jsonObject["IV"]!.ToString();
            }
            catch (Exception)
            {
                // Si el token no era válido (no descifra o no es JSON), avisamos con false.
                return false;
            }

            return true;
        }
    }
}
