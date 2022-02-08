using FinancesApi.Controllers;
using FinancesApi.DataFiles;
using FinancesApi.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using System.IO;

namespace FinancesApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IDatasetService, DatasetService>();
            services.AddSingleton<IDocumentDatasetService, DocumentDatasetService>();
            services.AddSingleton<IAccountingDatasetService, AccountingDatasetService>();
            services.AddSingleton<ICompressionService, CompressionService>();
            services.AddSingleton<ITransactionsService, TransactionsService>();
            services.AddSingleton<IBalanceService, BalanceService>();
            services.AddSingleton<IDocumentService, DocumentService>();
            services.AddSingleton<IStatisticsService, StatisticsService>();
            services.AddSingleton<ICurrenciesService, CurrenciesService>();
            services.AddSingleton<ICurrencyExchangeService, CurrencyExchangeService>();
            services.AddSingleton<ISkodaService, SkodaService>();
            services.AddSingleton<IMazdaService, MazdaService>();

            services.AddSingleton(typeof(TransactionsDataFile));
            services.AddSingleton(typeof(TransactionAccountsDataFile));
            services.AddSingleton(typeof(TransactionCategoriesDataFile));
            services.AddSingleton(typeof(TransactionAutoCategoriesDataFile));
            services.AddSingleton(typeof(BalancesDataFile));
            services.AddSingleton(typeof(DocumentsDataFile));
            services.AddSingleton(typeof(CurrenciesDataFile));
            services.AddSingleton(typeof(CurrencyExchangeDataFile));
            services.AddSingleton(typeof(SkodaDataFile));
            services.AddSingleton(typeof(MazdaDataFile));

            services.AddTransient(typeof(DatasetInfoDataFile));
            services.AddTransient(typeof(DocumentDatasetInfoDataFile));
            services.AddTransient(typeof(AccountingDatasetInfoDataFile));

            services.AddCors(); 
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "FinancesApi", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "FinancesApi v1"));
            }
            app.UseRouting();
            app.UseCors(x => x
               .AllowAnyMethod()
               .AllowAnyHeader()
               .AllowAnyOrigin()); // allow credentials

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
