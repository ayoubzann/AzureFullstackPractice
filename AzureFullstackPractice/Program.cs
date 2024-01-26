using Azure.Storage.Blobs;
using AzureFullstackPractice.Data;
using AzureFullstackPractice.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<PersonDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("AzureConn") ??
     throw new InvalidOperationException("Connection string 'AzureConn' not found.")));

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddCors(); 

//Step 1
string connectionString = builder.Configuration.GetConnectionString("zannachiblob");
builder.Services.AddScoped<BlobServiceClient>(x => new BlobServiceClient(connectionString));
builder.Services.AddScoped<BlobStorageService>();


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });
                c.OperationFilter<FormFileOperationFilter>();
            });

var app = builder.Build();

app.UseCors(policy =>
        {
            policy.AllowAnyOrigin()
                  .AllowAnyMethod()
                  .AllowAnyHeader();
        });

app.Use(async (context, next) => 
{
    await next.Invoke();
});

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI(c =>
       {
           c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
       });



app.UseHttpsRedirection();

app.MapControllers();

app.Run();