using Microsoft.AspNetCore.Mvc;
using SecurityApi.Services;
using Newtonsoft.Json.Linq;

namespace KeyGeneratorApi.Controllers
{
    /// <summary>
    /// Endpoints para generar y validar contraseñas/claves robustas, bajo la ruta
    /// <c>api/KeyGenerator</c>:
    /// <list type="bullet">
    ///   <item><description><c>GET  api/KeyGenerator/generate</c> — genera una clave nueva (Base64Url).</description></item>
    ///   <item><description><c>POST api/KeyGenerator/validate</c> — comprueba si una clave cumple las reglas.</description></item>
    ///   <item><description><c>POST api/KeyGenerator/settotalchars</c> — ajusta la longitud mínima exigida.</description></item>
    /// </list>
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    // Constructor primario: el framework inyecta el generador y el codificador Base64Url.
    public class KeyGeneratorController(IKeyGeneratorService keyGeneratorService, ICoderService coderService) : ControllerBase
    {
        /// <summary>
        /// Genera una clave robusta y la devuelve en <c>{ generatedKey }</c> (en Base64Url,
        /// para que viaje sin problemas por URL/JSON).
        /// </summary>
        [HttpGet("generate")]
        public ActionResult<string> Generate()
        {
            string generatedKey = keyGeneratorService.Generate();
            generatedKey = coderService.ToBase64Url(generatedKey);

            return Ok(new { generatedKey });
        }

        /// <summary>
        /// Valida una clave recibida en Base64Url contra las reglas de complejidad
        /// (mayúsculas, minúsculas, números, símbolos y longitud mínima). Devuelve un booleano.
        /// </summary>
        [HttpPost("validate")]
        public ActionResult<bool> Validate([FromBody] string json)
        {
            JObject jsonObject = JObject.Parse(json);

            string key = jsonObject["key"]!.ToString();

            // La clave llega en Base64Url; la decodificamos antes de validarla.
            key = coderService.FromBase64Url(key);

            bool isValid = keyGeneratorService.Validate(key);
            return Ok(isValid);
        }

        /// <summary>
        /// Ajusta la longitud mínima (en caracteres) que exigirá la validación y la generación.
        /// Responde <c>204 NoContent</c> porque no hay nada que devolver.
        /// </summary>
        [HttpPost("settotalchars")]
        public IActionResult SetTotalChars([FromBody] int totalCharacters)
        {
            keyGeneratorService.SetTotalChars(totalCharacters);
            return NoContent();
        }
    }
}
