using CodeBehind.Interfaces;
using CodeBehind.Services;
using Data.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using System.IO;
using frontend.ModelBinders;

var contentRoot = ResolveContentRootPath();
var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
    Args = args,
    ContentRootPath = contentRoot,
    WebRootPath = Path.Combine(contentRoot, "wwwroot")
});

// Add services to the container.
builder.Services
    .AddRazorPages()
    .AddMvcOptions(options =>
    {
        options.ModelBinderProviders.Insert(0, new FlexibleDecimalModelBinderProvider());
    });
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("A connection string 'DefaultConnection' nao foi configurada.");

builder.Services.AddScoped<ILancamentoRepository>(_ => new SqlLancamentoRepository(connectionString));
builder.Services.AddScoped<LancamentoService>();

var app = builder.Build();

Console.WriteLine("ContentRoot: " + app.Environment.ContentRootPath);
Console.WriteLine("WebRoot: " + app.Environment.WebRootPath);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthorization();

app.UseStaticFiles();
app.MapRazorPages();

app.Run();

static string ResolveContentRootPath()
{
    var current = new DirectoryInfo(AppContext.BaseDirectory);

    while (current is not null)
    {
        var hasWwwroot = Directory.Exists(Path.Combine(current.FullName, "wwwroot"));
        var hasProjectFile = File.Exists(Path.Combine(current.FullName, "frontend.csproj"));

        if (hasWwwroot && hasProjectFile)
        {
            return current.FullName;
        }

        current = current.Parent;
    }

    return Directory.GetCurrentDirectory();
}
