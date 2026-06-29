using Microsoft.AspNetCore.Mvc;
using SecurityApi.Services;
using Newtonsoft.Json.Linq;

namespace SecurityApi.Controllers
{
    /// <summary>
    /// Endpoints de cifrado para asegurar las comunicaciones de una app PowerBuilder.
    /// Expone tres operaciones bajo la ruta <c>api/Security</c>:
    /// <list type="bullet">
    ///   <item><description><c>POST api/Security/encrypt</c> — cifra un texto con AES.</description></item>
    ///   <item><description><c>POST api/Security/decrypt</c> — descifra un texto AES.</description></item>
    ///   <item><description><c>POST api/Security/token</c> — abre un "token" y devuelve la clave/IV de sesión.</description></item>
    /// </list>
    /// Todo viaja en <b>Base64Url</b> (Base64 apto para URLs: sin <c>+ / =</c>), por eso
    /// cada valor se decodifica con <see cref="ICoderService"/> antes de usarlo.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    // Constructor primario (C# 12): los parámetros 'securityService' y 'coderService'
    // los inyecta el framework (ver registros en Program.cs) y quedan disponibles como
    // campos en toda la clase. Nos ahorra el típico constructor + campos privados.
    public class SecurityController(ISecurityService securityService, ICoderService coderService) : ControllerBase
    {
        /// <summary>
        /// Cifra <c>source</c> con AES usando <c>key</c> e <c>iv</c>. Recibe un JSON con
        /// esos tres valores en Base64Url y devuelve <c>{ encrypted }</c> (también Base64Url).
        /// </summary>
        [HttpPost("encrypt")]
        public ActionResult<string> Encrypt([FromBody] string json)
        {
            JObject jsonObject = JObject.Parse(json);

            // Leemos los tres valores del JSON. El '!' le dice al compilador "fíate,
            // aquí no es null" (silencia el aviso de nullabilidad).
            string source = jsonObject["source"]!.ToString();
            string key = jsonObject["key"]!.ToString();
            string iv = jsonObject["iv"]!.ToString();

            // Llegan en Base64Url; los devolvemos a texto plano antes de cifrar.
            source = coderService.FromBase64Url(source);
            key = coderService.FromBase64Url(key);
            iv = coderService.FromBase64Url(iv);

            string encrypted = securityService.Encrypt(source, key, iv);

            return Ok(new { encrypted });
        }

        /// <summary>
        /// Operación inversa de <see cref="Encrypt"/>: descifra <c>source</c> con AES y
        /// devuelve <c>{ decrypted }</c> en texto plano.
        /// </summary>
        [HttpPost("decrypt")]
        public ActionResult<string> Decrypt([FromBody] string json)
        {
            JObject jsonObject = JObject.Parse(json);

            string source = jsonObject["source"]!.ToString();
            string key = jsonObject["key"]!.ToString();
            string iv = jsonObject["iv"]!.ToString();

            // De Base64Url a texto plano antes de descifrar.
            source = coderService.FromBase64Url(source);
            key = coderService.FromBase64Url(key);
            iv = coderService.FromBase64Url(iv);

            string decrypted = securityService.Decrypt(source, key, iv);

            return Ok(new { decrypted });
        }

        /// <summary>
        /// Abre un "token" cifrado con la clave maestra (<c>masterKey</c>/<c>masterIv</c>) y
        /// devuelve la pareja <c>{ key, iv }</c> de sesión que contenía. Es el patrón típico:
        /// la app sólo conoce la clave maestra y el servidor le entrega la clave de sesión.
        /// Responde <c>200 OK</c> si el token es válido, o <c>400 BadRequest</c> si no.
        /// </summary>
        [HttpPost("token")]
        public IActionResult GetToken([FromBody] string json)
        {
            JObject jsonObject = JObject.Parse(json);

            string token = jsonObject["token"]!.ToString();
            string masterKey = jsonObject["masterKey"]!.ToString();
            string masterIv = jsonObject["masterIv"]!.ToString();

            // De Base64Url a texto plano.
            token = coderService.FromBase64Url(token);
            masterKey = coderService.FromBase64Url(masterKey);
            masterIv = coderService.FromBase64Url(masterIv);

            // 'key' e 'iv' se rellenan dentro del servicio (parámetros 'ref', estilo
            // "por referencia" como en PowerScript): el método nos devuelve el resultado
            // booleano y, de paso, escribe en estas dos variables.
            string key = string.Empty;
            string iv = string.Empty;

            bool result = securityService.GetToken(token, masterKey, masterIv, ref key, ref iv);

            // Re-codificamos la respuesta a Base64Url para que viaje segura por la URL/JSON.
            key = coderService.ToBase64Url(key);
            iv = coderService.ToBase64Url(iv);

            if (result)
            {
                return Ok(new { key, iv });
            }
            else
            {
                return BadRequest("Invalid token or keys");
            }
        }
    }
}
