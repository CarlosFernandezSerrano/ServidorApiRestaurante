var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Llamo al m�todo de BDDController para crear la base de datos
ServidorApiRestaurante.Controllers.BDDController.CrearBDD();  // Creo la base de datos, si no est� creada, antes de iniciar la aplicaci�n
ServidorApiRestaurante.Controllers.BDDController.CrearTablas();
ServidorApiRestaurante.Controllers.BDDController.InsertarRegistrosRol();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
