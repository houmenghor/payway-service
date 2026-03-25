using payway;
using payway.Services;

var builder = WebApplication.CreateBuilder(args);

// Bind BakongSettings from appsettings.json
builder.Services.Configure<BakongSettings>(
    builder.Configuration.GetSection("BakongSettings"));

// HttpClient + Service
builder.Services.AddHttpClient<PaywayServices>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();
app.MapGet("/", () => Results.Ok(new
{
    status = "success",
    message = "Payway Service is running",
    version = "1.0.0",
    docs = "/swagger"
})).ExcludeFromDescription();
app.Run();