using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json.Linq;
using Microsoft.AspNetCore.DataProtection.KeyManagement;

namespace SecurityApi.Services.Imp
{
    public class SecurityService : ISecurityService
    {
        public string Encrypt(string source, string key, string iv)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(source)) return "";

                //Corregimos tamaños Key y IV.
                key = key.PadRight(16, '*');
                iv = iv.PadRight(16, '0');
                
                byte[] InitialVectorBytes = Encoding.UTF8.GetBytes(iv);
                byte[] keyBytes = Encoding.UTF8.GetBytes(key);
                byte[] plainTextBytes = Encoding.UTF8.GetBytes(source);
                byte[] encrypted;
                
                var aes = Aes.Create();
                aes.KeySize = 128;
                aes.Key = keyBytes;
                aes.IV = InitialVectorBytes;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;
                
                
                // Create an encryptor to perform the stream transform.
                using (var encryptor = aes.CreateEncryptor())
                {
                    // Create the streams used for encryption.
                    using (var msEncrypt = new MemoryStream())
                    {
                        using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                        {
                            using (var swEncrypt = new StreamWriter(csEncrypt))
                            {
                                //Write all data to the stream.
                                swEncrypt.Write(source);
                            }
                            encrypted = msEncrypt.ToArray();
                        }
                    }
                }

                string sourceFinal = Convert.ToBase64String(encrypted);
                
                //Pasar a Base64URL:
                sourceFinal = sourceFinal.Replace("+", "-").Replace("/", "_").Replace("=", "");
                
                return sourceFinal;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            
        }
        public string Decrypt(string source, string key, string iv)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(source)) return "";

                //Corregimos tamaños Key y IV.
                key = key.PadRight(16, '*');
                iv = iv.PadRight(16, '0');
                
                byte[] InitialVectorBytes = Encoding.UTF8.GetBytes(iv);
                byte[] keyBytes = Encoding.UTF8.GetBytes(key);
                
                //Pasamos el De Base64URL a Base64
                source = source.Replace("-", "+").Replace("_", "/");
                while (source.Length % 4 != 0)
                {
                    source += "=";
                }

                // Convertir el mensaje cifrado de base64 a bytes
                byte[] cipherBytes = Convert.FromBase64String(source);
                
                var aes = Aes.Create();
                aes.KeySize = 128;
                aes.Key = keyBytes;
                aes.IV = InitialVectorBytes;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;
                
                
                string decrypted;
                
                //Create a decryptor to perform the stream transform.
                using (var decryptor = aes.CreateDecryptor())
                {
                    // Create the streams used for decryption.
                    using (var msDecrypt = new MemoryStream(cipherBytes))
                    {
                        using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                        {
                            using (var srDecrypt = new StreamReader(csDecrypt))
                            {
                                // Read the decrypted bytes from the decrypting stream
                                // and place them in a string.
                                decrypted = srDecrypt.ReadToEnd();
                            }
                        }
                    }
                }


                //decrypted = _coderservice.ToBase64Url(decrypted);
                return decrypted;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        public bool GetToken(string token, string masterKey, string masterIv, ref string key, ref string iv)
        {
            //Corregimos tamaños MasterKey y MasterIv.
            masterKey = masterKey.PadRight(16, '*');
            masterIv = masterIv.PadRight(16, '0');
            
            if (string.IsNullOrWhiteSpace(token) ||
                string.IsNullOrWhiteSpace(masterKey) ||
                string.IsNullOrWhiteSpace(masterIv)) return false;

            try
            {
                string json = Decrypt(token, masterKey, masterIv);
                
                // Parsear la cadena JSON utilizando JObject
                JObject jsonObject = JObject.Parse(json);
                
                // Obtener los valores de key y IV como cadenas
                key = jsonObject["key"].ToString();
                iv = jsonObject["IV"].ToString();
                
            }
            catch (Exception ex)
            {
                return false;
            }
            
            return true;
            
        }
    }
}
