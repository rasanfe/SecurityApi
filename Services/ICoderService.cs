namespace SecurityApi.Services
{
    /// <summary>
    /// Contrato del codificador <b>Base64Url</b>. Base64Url es Base64 "apto para URLs":
    /// cambia los caracteres conflictivos (<c>+</c>, <c>/</c>) por (<c>-</c>, <c>_</c>) y
    /// quita el relleno <c>=</c>. Así las claves y textos cifrados viajan sin romperse
    /// por la URL ni por el JSON.
    /// </summary>
    public interface ICoderService
    {
        /// <summary>Texto plano → Base64Url.</summary>
        string ToBase64Url(string input);

        /// <summary>Base64Url → texto plano.</summary>
        string FromBase64Url(string input);
    }
}
