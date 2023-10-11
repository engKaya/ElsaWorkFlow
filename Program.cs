using Elsa;
using Elsa.Persistence.EntityFramework.Core.Extensions;
using Elsa.Persistence.EntityFramework.SqlServer;
using ElsaWorkFlow.Data;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
var elsaSection = builder.Configuration.GetSection("Elsa");

builder.Services.AddElsa(elsa => elsa
                   .UseEntityFrameworkPersistence(ef => 
                   DbContextOptionsBuilderExtensions.UseSqlServer(ef, builder.Configuration.GetConnectionString("ElsaConnectionString")))

                    .AddConsoleActivities()
                    .AddHttpActivities(elsaSection.GetSection("Server").Bind)
                    .AddQuartzTemporalActivities()
                    .AddWorkflowsFrom<Program>()
                );
builder.Services.AddElsaApiEndpoints(); 

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles()
.UseHttpActivities()
.UseRouting()
.UseEndpoints(endpoints =>
{
    // Elsa API Endpoints are implemented as regular ASP.NET Core API controllers.
    endpoints.MapControllers();

    // For Dashboard.
    endpoints.MapFallbackToPage("/_Host");
});
app.MapControllers();
app.UseRouting();

app.MapBlazorHub(); 

app.Run();
