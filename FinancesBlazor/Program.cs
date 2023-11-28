using Finances.DependencyInjection;
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
            var app = builder.Build();
           
            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
            }


            app.UseStaticFiles();

            app.UseRouting();

            app.MapBlazorHub();
            app.MapFallbackToPage("/_Host");

            app.Run();
        }
    }
}