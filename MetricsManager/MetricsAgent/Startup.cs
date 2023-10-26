using AutoMapper;
using FluentMigrator.Runner;
using MetricsAgent.DTO;
using MetricsAgent.Jobs;
using MetricsCommon;
using MetricsCommon.SQL_Settings;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Quartz;
using Quartz.Impl;
using Quartz.Spi;
using System.Data.SQLite;

namespace MetricsAgent
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

            services.AddControllers();
            services.AddSingleton<ISQLSettings, SQLSettings>();
            services.AddFluentMigratorCore().ConfigureRunner(rb => rb
                                           // ��������� ��������� SQLite
                                           .AddSQLite()
                                           // ������������� ������ �����������
                                           .WithGlobalConnectionString(SQLSettings.ConnectionString)
                                           // ������������ ��� ������ ������ � ����������
                                           .ScanIn(typeof(Startup).Assembly).For.Migrations()).AddLogging(lb => lb
                                           .AddFluentMigratorConsole());
            services.AddSingleton(new SQLiteConnection(SQLSettings.ConnectionString));

            // ������������� ������������
            services.AddSingleton<ICpuMetricsRepository, CpuMetricsRepository>();
            services.AddSingleton<IDotNetMetricsRepository, DotNetMetricsRepository>();
            services.AddSingleton<IHddMetricsRepository, HddMetricsRepository>();
            services.AddSingleton<INetworkMetricsRepository, NetworkMetricsRepository>();
            services.AddSingleton<IRamMetricsRepository, RamMetricsRepository>();

            // �������� ������� ����� �� ������ ������
            services.AddHostedService<QuartzHostedService>();
            services.AddSingleton<IJobFactory, SingletonJobFactory>();
            services.AddSingleton<ISchedulerFactory, StdSchedulerFactory>();
            services.AddSingleton<CpuMetricJob>();
            services.AddSingleton(new JobSchedule(
                                  jobType: typeof(CpuMetricJob),
                                  cronExpression: "0/5 * * * * ?")); // ��������� ������ 5 ������
            services.AddSingleton<DotNetMetricJob>();
            services.AddSingleton(new JobSchedule(
                                  jobType: typeof(DotNetMetricJob),
                                  cronExpression: "0/5 * * * * ?"));
            services.AddSingleton<HddMetricJob>();
            services.AddSingleton(new JobSchedule(
                                  jobType: typeof(HddMetricJob),
                                  cronExpression: "0/5 * * * * ?"));
            services.AddSingleton<NetworkMetricJob>();
            services.AddSingleton(new JobSchedule(
                                  jobType: typeof(NetworkMetricJob),
                                  cronExpression: "0/5 * * * * ?"));
            services.AddSingleton<RamMetricJob>();
            services.AddSingleton(new JobSchedule(
                                  jobType: typeof(RamMetricJob),
                                  cronExpression: "0/5 * * * * ?")); 
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "MetricsAgent", Version = "v1" });
            });
            var mapperConfiguration = new MapperConfiguration(mp => mp.AddProfile(new MapperProfile()));
            var mapper = mapperConfiguration.CreateMapper();
            services.AddSingleton(mapper);           
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IMigrationRunner migrationRunner)
        {
            migrationRunner.MigrateUp();
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "MetricsAgent v1"));
            }
            
            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

    }
}
