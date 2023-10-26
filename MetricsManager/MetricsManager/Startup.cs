using AutoMapper;
using FluentMigrator.Runner;
using MetricsCommon;
using MetricsCommon.SQL_Settings;
using MetricsManager.Client;
using MetricsManager.DAL;
using MetricsManager.Jobs;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Polly;
using Quartz;
using Quartz.Impl;
using Quartz.Spi;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Threading.Tasks;

namespace MetricsManager
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
            services.AddHttpClient();
            services.AddHttpClient<IMetricsAgentClient, MetricsAgentClient>()
                    .AddTransientHttpErrorPolicy(p => p.WaitAndRetryAsync(3, _ => TimeSpan.FromMilliseconds(1000)));
            services.AddSingleton<ISQLSettings, SQLSettings>();
            services.AddFluentMigratorCore().ConfigureRunner(rb => rb.AddSQLite()
                                           .WithGlobalConnectionString(SQLSettings.ConnectionString)
                                           .ScanIn(typeof(Startup).Assembly).For.Migrations()).AddLogging(lb => lb
                                           .AddFluentMigratorConsole());
            services.AddSingleton(new SQLiteConnection(SQLSettings.ConnectionString));

            // Инициализация репозиториев
            services.AddSingleton<IAgentsRepository, AgentsRepository>();
            services.AddSingleton<ICpuMetricsRepository, CpuMetricsRepository>();
            services.AddSingleton<IDotNetMetricsRepository, DotNetMetricsRepository>();
            services.AddSingleton<IHddMetricsRepository, HddMetricsRepository>();
            services.AddSingleton<INetworkMetricsRepository, NetworkMetricsRepository>();
            services.AddSingleton<IRamMetricsRepository, RamMetricsRepository>();

            // Создание фоновых задач по снятию метрик
            services.AddHostedService<QuartzHostedService>();
            services.AddSingleton<IJobFactory, SingletonJobFactory>();
            services.AddSingleton<ISchedulerFactory, StdSchedulerFactory>();
            services.AddSingleton<CpuMetricJob>();
            services.AddSingleton(new JobSchedule(
                                  jobType: typeof(CpuMetricJob),
                                  cronExpression: "0/5 * * * * ?")); // запускать каждые 5 секунд
            services.AddSingleton<DotNetMetricJob>(); 
            services.AddSingleton(new JobSchedule(
                                  jobType: typeof(DotNetMetricJob),
                                  cronExpression: "0/5 * * * * ?")); // запускать каждые 5 секунд
            services.AddSingleton<HddMetricJob>();
            services.AddSingleton(new JobSchedule(
                                  jobType: typeof(HddMetricJob),
                                  cronExpression: "0/5 * * * * ?")); // запускать каждые 5 секунд
            services.AddSingleton<NetworkMetricJob>();
            services.AddSingleton(new JobSchedule(
                                  jobType: typeof(NetworkMetricJob),
                                  cronExpression: "0/5 * * * * ?")); // запускать каждые 5 секунд
            services.AddSingleton<RamMetricJob>();
            services.AddSingleton(new JobSchedule(
                                  jobType: typeof(RamMetricJob),
                                  cronExpression: "0/5 * * * * ?")); // запускать каждые 5 секунд
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "MetricsManager", Version = "v1" });
            });
            var mapperConfiguration = new MapperConfiguration(mp => mp.AddProfile(new MapperProfile()));
            var mapper = mapperConfiguration.CreateMapper();
            services.AddSingleton(mapper);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IMigrationRunner migrationRunner)
        {
            migrationRunner.MigrateUp();
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "MetricsManager v1"));
            }

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
