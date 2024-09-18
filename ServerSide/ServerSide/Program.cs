using System.Data.SqlClient;

var builder = WebApplication.CreateBuilder(args);



builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();  


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin",
        builder => builder.WithOrigins("http://localhost:5173")  
                          .AllowAnyHeader()
                          .AllowAnyMethod());
});


var app = builder.Build();

// Enable CORS with the defined policy
app.UseCors("AllowSpecificOrigin");

// Enable Swagger only in development mode
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "API V1");
        c.RoutePrefix = string.Empty;  
    });
}
else
{
    
    app.UseHsts();
}


app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();  


app.Run();



