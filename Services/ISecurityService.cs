namespace SecurityApi.Services
{
    /// <summary>
    /// Contrato del servicio de cifrado AES. La interfaz define el "qué" (los métodos
    /// disponibles) y la implementación <c>Imp/SecurityService</c> el "cómo". Los
    /// controladores dependen de esta interfaz, no de la clase concreta: así el cifrado
    /// es intercambiable y fácil de testear.
    /// </summary>
    public interface ISecurityService
    {
        /// <summary>Cifra <paramref name="source"/> con AES y devuelve el resultado en Base64Url.</summary>
        string Encrypt(string source, string key, string iv);

        /// <summary>Descifra un texto AES en Base64Url y devuelve el original.</summary>
        string Decrypt(string source, string key, string iv);

        /// <summary>
        /// Descifra un token con la clave maestra y extrae de su interior la clave/IV de
        /// sesión. Devuelve <c>true</c> si el token es válido; rellena <paramref name="key"/>
        /// e <paramref name="iv"/> "por referencia" (estilo PowerScript).
        /// </summary>
        bool GetToken(string token, string masterKey, string masterIv, ref string key, ref string iv);
    }
}
