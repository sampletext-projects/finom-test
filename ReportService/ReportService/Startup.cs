using ReportService.Db;
using ReportService.Repositories;
using ReportService.Services;

namespace ReportService;

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
        services.AddMvc(options => options.EnableEndpointRouting = false);

        services.AddScoped<IReportService, Services.ReportService>();

        services.AddScoped<IDepartmentRepository, DepartmentRepository>();
        services.AddScoped<IEmployeeRepository, EmployeeRepository>();
        services.AddSingleton<IMonthNameResolver, MonthNameResolverImpl>();

        services.AddSingleton<IDbConnectionFactory, DbConnectionFactory>(
            _ => new DbConnectionFactory(
                Configuration.GetConnectionString("DbConnection")
                ?? throw new InvalidOperationException("DbConnection connection string not found.")
            )
        );

        services.AddHttpClient<IEmployeeCodeProvider, EmployeeCodeProvider>(
            client =>
                client.BaseAddress = new Uri(
                    Configuration.GetConnectionString("BuhService")
                    ?? throw new InvalidOperationException("BuhService connection string not found.")
                )
        );

        services.AddHttpClient<IEmployeeSalaryProvider, EmployeeSalaryProvider>(
            client =>
                client.BaseAddress = new Uri(
                    Configuration.GetConnectionString("SalaryService")
                    ?? throw new InvalidOperationException("SalaryService connection string not found.")
                )
        );
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseMvc();
    }
}