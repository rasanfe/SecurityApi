using Microsoft.AspNetCore.Mvc;
using SecurityApi.Services;
using Newtonsoft.Json.Linq;

namespace KeyGeneratorApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class KeyGeneratorController : ControllerBase
    {
        private readonly IKeyGeneratorService _keyGeneratorService;
        private readonly ICoderService _coderService;
        
        public KeyGeneratorController(IKeyGeneratorService keyGeneratorService, ICoderService coderService)
        {
            _keyGeneratorService = keyGeneratorService;
            _coderService = coderService;
        }

        // Endpoint para Generate
        [HttpGet("generate")]
        public ActionResult<string> Generate()
        {
            string generatedKey = _keyGeneratorService.Generate();
            generatedKey = _coderService.ToBase64Url(generatedKey);

            return Ok(new { generatedKey });
        }

        // Endpoint para Validate
        [HttpPost("validate")]
        public ActionResult<bool> Validate([FromBody] string json)
        {
            JObject jsonObject = JObject.Parse(json);

            // Obtener los valores de key y IV como cadenas
            string key = jsonObject["key"].ToString();

            //Decode
            key = _coderService.FromBase64Url(key);
        
            bool isValid = _keyGeneratorService.Validate(key);
            return Ok(isValid);
        }

        // Endpoint para SetTotalCharacters
        [HttpPost("settotalchars")]
        public IActionResult SetTotalChars([FromBody] int totalCharacters)
        {
            _keyGeneratorService.SetTotalChars(totalCharacters);
            return NoContent();
        }
    }
}
