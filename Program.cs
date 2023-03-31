using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Azure.Storage.Blobs;
using BlobApiDemo.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
//add keyvault secret
#region keyvault config 
string AzureBlobStorageUrl = builder.Configuration["AzureKeyVaultUrl"]!;
var secretClient = new SecretClient(new Uri(AzureBlobStorageUrl), new DefaultAzureCredential());
var AzureBlobStorageConn = secretClient.GetSecret("ConnectionStrings--AzureBlobStorage");

#endregion
//add azure blob service
builder.Services.AddScoped(_=>{
    return new BlobServiceClient(AzureBlobStorageConn.Value.Value);
});

builder.Services.AddScoped<IFileService, FileService>();

var app = builder.Build();
app.UseCors(builder => builder
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials().SetIsOriginAllowed(origin => true));
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
