using Edgias.MurimiOS.API.Extensions;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using System.Net;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<MurimiOSDbContext>();

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.WebHost.ConfigureKestrel((context, serverOptions) =>
{
    //serverOptions.ListenAnyIP(IPAddress.Any, 5000);
    serverOptions.Listen(IPAddress.Any, 5000, listenOptions =>
    {
        listenOptions.Protocols = HttpProtocols.Http1AndHttp2;
        //listenOptions.UseHttps("testCert.pfx", "testPassword");
    });
});

WebApplication app = builder.Build();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI();
//}
app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();

app.RegisterEndpoints();

app.Run();
