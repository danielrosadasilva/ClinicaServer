
using ClinicaServer.Models;
using ClinicaServer.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Text;
using System.Text.Json.Serialization;



var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
var SignalOirigins = "signalr";

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
                      policy =>
                      {
                      policy.WithOrigins("http://localhost:3000");
                      //policy.AllowAnyOrigin();
                      policy.AllowAnyHeader();
                      policy.AllowAnyMethod();
                      //policy.SetIsOriginAllowed(hostName => true);
                      policy.AllowCredentials();
                      });
    /*options.AddPolicy(name: "signalr",
                      policy =>
                      {
                          policy.WithOrigins("*");
                          policy.AllowAnyHeader();
                          policy.AllowAnyMethod();
                          policy.AllowCredentials();
                          policy.SetIsOriginAllowed(hostName => true);
                      }); */
});

//builder.Services.AddScoped<IPaciente, pacienteRepositorio>();
//builder.Services.AddDbContext<DBClinicaContext>(option => option.UseNpgsql("Host=localhost;Port=5432;Pooling=true;Database=DBClinica;User Id=postgres;Password=123456;"));
builder.Services.AddDbContext<DBClinicaContext>(option => option.UseMySQL("Server=localhost;Database=DBClinica;Uid=root;Pwd=123456;"));
builder.Services.AddControllers();

/* 
builder.Services.AddControllers().AddJsonOptions(x =>
{

    x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;

});
*/

builder.Services.AddScoped<IRepositoryWrapper, RepositoryWrapper>();


//builder.Services.AddEntityFrameworkNpgsql().AddDbContext<DBClinicaContext>(option => option.UseNpgsql("Host=localhost;Port=5432;Pooling=true;Database=DBClinica;User Id=postgres;Password=123456;"));

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddSignalR();
//builder.Services.AddSingleton<IDictionary<string, UserConnection>>(opts=>new Dictionary<string, UserConnection>());



var app = builder.Build();





// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.MapGet("/exame/teste", (string nome) =>
{
    return Results.Ok(nome);
}).Produces<ActionResult>(StatusCodes.Status200OK);



//builder.Configuration.AddConfiguration(ContentResultConfiguration().Create());
app.UseHttpsRedirection();
app.UseRouting();

app.UseCors(MyAllowSpecificOrigins);

app.UseEndpoints(endpoints =>
{
   // endpoints.MapHub<AtendimentoHub>("/ws/atendimentos");


});

app.UseAuthorization();



/*
var webSocketOptions = new WebSocketOptions
{
    KeepAliveInterval = TimeSpan.FromMinutes(2)
};

app.UseWebSockets(webSocketOptions);
*/




app.MapControllers();

app.Run();
