using Microsoft.AspNetCore.Mvc;
using SecurityApi.Services;
using Newtonsoft.Json.Linq;

namespace SecurityApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SecurityController : ControllerBase
    {
        private readonly ISecurityService _securityService;
        private readonly ICoderService _coderService;
        
        public SecurityController(ISecurityService securityService, ICoderService coderService)
        {
            _securityService = securityService;
            _coderService = coderService;
        }
        
        // Endpoint para Encrypt
        [HttpPost("encrypt")]
        public ActionResult<string> Encrypt([FromBody] string json)
        {
            JObject jsonObject = JObject.Parse(json);

            // Obtener los valores de key y IV como cadenas
            string source = jsonObject["source"].ToString();
            string key = jsonObject["key"].ToString();
            string iv = jsonObject["iv"].ToString();
            source = _coderService.FromBase64Url(source);
            key = _coderService.FromBase64Url(key);
            iv = _coderService.FromBase64Url(iv);

            string encrypted = _securityService.Encrypt(source, key, iv);
    
            return Ok(new { encrypted });
        }

        // Endpoint para Decrypt
        [HttpPost("decrypt")]
        public ActionResult<string> Decrypt([FromBody] string json)
        {
            JObject jsonObject = JObject.Parse(json);

            // Obtener los valores de key y IV como cadenas
            string source = jsonObject["source"].ToString();
            string key = jsonObject["key"].ToString();
            string iv = jsonObject["iv"].ToString();
            source = _coderService.FromBase64Url(source);
            key = _coderService.FromBase64Url(key);
            iv = _coderService.FromBase64Url(iv);

            string decrypted = _securityService.Decrypt(source, key, iv);
      
            return Ok(new { decrypted });

        }

        // Endpoint para GetToken
        [HttpPost("token")]
        public IActionResult GetToken([FromBody] string json)
        {
            JObject jsonObject = JObject.Parse(json);

            // Obtener los valores de key y IV como cadenas
            string token = jsonObject["token"].ToString();
            string masterKey = jsonObject["masterKey"].ToString();
            string masterIv = jsonObject["masterIv"].ToString();
            token = _coderService.FromBase64Url(token);
            masterKey = _coderService.FromBase64Url(masterKey);
            masterIv = _coderService.FromBase64Url(masterIv);
            string key = string.Empty;
            string iv = string.Empty;
            
            bool result = _securityService.GetToken(token, masterKey, masterIv, ref key, ref iv);

            //Encode Base 64
            key = _coderService.ToBase64Url(key);
            iv = _coderService.ToBase64Url(iv);
            
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
