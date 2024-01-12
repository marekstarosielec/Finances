using DataSource;
using DataSource.Document;
using Finances.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Radzen;

namespace FinancesBlazor
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddRazorPages();
            builder.Services.AddServerSideBlazor();
            builder.Services.InjectServices();
            builder.Services.AddScoped<DataViewManager>();
            builder.Services.InjectViews(builder.Configuration);
            builder.Services.AddRadzenComponents();
            builder.Services.AddScoped<DialogService>();
            builder.Services.AddSingleton(new DocumentManagerOptions { 
                DecompressionPath = Path.Combine(builder.Environment.WebRootPath, "documents")
            });
            builder.Services.AddSingleton<IDocumentManager, DocumentManager>();
            builder.Services.AddSingleton<ICompressionService, CompressionService>();
            builder.Services.AddDirectoryBrowser();
            var app = builder.Build();
           
            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
            }


            app.UseStaticFiles();
            var fileProvider = new PhysicalFileProvider(Path.Combine(builder.Environment.WebRootPath, "documents"));
            var requestPath = "/documents";

            // Enable displaying browser links.
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = fileProvider,
                RequestPath = requestPath
            });

            app.UseDirectoryBrowser(new DirectoryBrowserOptions
            {
                FileProvider = fileProvider,
                RequestPath = requestPath
            });

            app.UseRouting();
            

            app.MapBlazorHub();
            app.MapFallbackToPage("/_Host");
            
            app.Run();
        }
    }
}