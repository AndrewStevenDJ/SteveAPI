var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers(); // Mantén esta línea si usas controladores
// builder.Services.AddEndpointsApiExplorer(); // Si usas Minimal APIs, también necesitarás esto
// Si no usas Minimal APIs, esta línea ya la tienes por defecto con AddControllers

// >>>>>>>>> SECCIÓN DE SERVICIOS PARA SWAGGER <<<<<<<<<
// Estas dos líneas deben ir aquí, ANTES de var app = builder.Build();
builder.Services.AddEndpointsApiExplorer(); // Permite a Swagger descubrir tus endpoints
builder.Services.AddSwaggerGen();           // Registra el generador de especificaciones OpenAPI
// >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment()) // Swagger UI se habilita comúnmente solo en desarrollo
{
    // >>>>>>>>> SECCIÓN DE MIDDLEWARE PARA SWAGGER <<<<<<<<<
    // Estas dos líneas deben ir aquí, DESPUÉS de var app = builder.Build();
    // y DENTRO del bloque if (app.Environment.IsDevelopment())
    app.UseSwagger();   // Habilita el middleware que sirve el documento JSON de Swagger
    app.UseSwaggerUI(); // Habilita el middleware que sirve la interfaz de usuario de Swagger
    // Puedes personalizar la URL si lo deseas, por ejemplo:
    // app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "SteveAPI v1"));
    // Si quieres que se cargue en la raíz (ej. http://localhost:5179/), puedes añadir:
    // c.RoutePrefix = string.Empty;
    // >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
}

app.UseHttpsRedirection(); // Redirección a HTTPS (si aplica)
app.UseAuthorization();    // Middleware de autorización (si lo usas)

app.MapControllers(); // Mapea tus controladores

// Si usas Minimal APIs, tendrías líneas como:
// app.MapGet("/api/hello", () => "Hello World!");

app.Run(); // Esta línea inicia tu aplicación. NADA de código ejecutable después de aquí.