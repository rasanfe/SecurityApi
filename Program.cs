using SecurityApi.Services;
using SecurityApi.Services.Imp;

// ─────────────────────────────────────────────────────────────────────────────
// Program.cs — punto de arranque de la Web API (modelo "minimal hosting" de
// ASP.NET Core). Fijaos que aquí NO hay clase Startup ni método Main: desde
// .NET 6 el propio fichero ES el Main. Se lee de arriba abajo:
//   1) Se crea un "builder" donde registramos servicios (la caja de dependencias).
//   2) Se construye la app.
//   3) Se configura el pipeline HTTP (el orden de los middleware IMPORTA).
//   4) app.Run() arranca el servidor web (Kestrel) y se queda escuchando.
// Para los que venís de PowerBuilder: esto equivale al arranque de la aplicación,
// pero montando de paso un servidor web embebido.
// ─────────────────────────────────────────────────────────────────────────────

var builder = WebApplication.CreateBuilder(args);

// Habilita los controladores (las clases *Controller donde viven los endpoints REST).
builder.Services.AddControllers();

// Swagger/OpenAPI: la "consola de pruebas" web para llamar a los endpoints sin
// escribir cliente. Muy cómoda para enseñar la API. Más info: https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ── Inyección de dependencias (DI) ───────────────────────────────────────────
// Registramos cada interfaz con su implementación. Cuando un controlador pida
// en su constructor un ISecurityService, el framework le entrega un SecurityService.
// Lo clave es el TIEMPO DE VIDA de cada servicio:
//
// Scoped (AddScoped): una instancia NUEVA por cada petición HTTP. Es la opción
// segura por defecto; aquí el cifrado no guarda estado, así que nos vale de sobra.
builder.Services.AddScoped<ISecurityService, SecurityService>();
builder.Services.AddScoped<ICoderService, CoderService>();

// Singleton (AddSingleton): una ÚNICA instancia compartida por toda la aplicación
// y por todas las peticiones. Lo usamos para el generador de claves porque no
// guarda estado por usuario y así nos ahorramos recrearlo en cada llamada.
builder.Services.AddSingleton<IKeyGeneratorService, KeyGeneratorService>();

var app = builder.Build();

// ── Pipeline HTTP ────────────────────────────────────────────────────────────
// Cada "Use..." es un eslabón por el que pasa la petición. El orden manda.

// Swagger sólo en desarrollo: no queremos exponer la consola de pruebas en producción.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Redirige automáticamente las peticiones http:// a https:// (tráfico cifrado).
app.UseHttpsRedirection();

app.UseAuthorization();

// Conecta las rutas declaradas con [Route]/[HttpPost] en los controladores.
app.MapControllers();

app.Run();
