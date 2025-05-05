using SecurityApi.Services;
using SecurityApi.Services.Imp;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


//Scoped (AddScoped): Nueva instancia por cada solicitud HTTP.
builder.Services.AddScoped<ISecurityService, SecurityService>();
builder.Services.AddScoped<ICoderService, CoderService>();
//Singleton(AddSingleton): Única instancia compartida para toda la aplicación.
builder.Services.AddSingleton<IKeyGeneratorService, KeyGeneratorService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Redireccionar automáticamente a HTTPS.
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();


app.Run();
