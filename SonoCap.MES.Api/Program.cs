
using Microsoft.EntityFrameworkCore;

namespace SonoCap.MES.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // -------------------------------------------------------------
            // **[1] 기존 DB Context 및 Repository 등록 코드 복사/변환**
            // -------------------------------------------------------------

            // 1. AppSettings 구조가 없으므로, 설정 파일에서 직접 Connection String을 가져옵니다.
            // 기존 코드: appSettings.ConnectionStrings.MariaDBConnection
            var mariaDbConnectionString = builder.Configuration.GetConnectionString("MariaDBConnection");

            // 2. MESDbContext 등록 (WPF App의 ConfigureDbContext와 동일하게 설정)
            builder.Services.AddDbContext<Repositories.Context.MESDbContext>(options =>
            {
                options.UseLazyLoadingProxies(false); // Lazy Loading 유지
                options.EnableSensitiveDataLogging(false); // 민감한 데이터 로깅 비활성화 유지

                // 3. MariaDB 연결 설정 유지
                options.UseMySql(mariaDbConnectionString,
                                 Microsoft.EntityFrameworkCore.ServerVersion.AutoDetect(mariaDbConnectionString),
                                 mysqlOptions => mysqlOptions.CommandTimeout(30));
            }, ServiceLifetime.Transient); // WPF 앱과 동일하게 Transient로 설정

            // 3. Repository 등록 (WPF 앱의 RegisterRepositories 함수 내용 복사)
            // using 구문으로 네임스페이스를 정리했다면, 아래 네임스페이스는 제거 가능합니다.
            builder.Services.AddTransient<Repositories.Interfaces.IAppSettingsRepository, Repositories.AppSettingsRepository>();
            builder.Services.AddTransient<Repositories.Interfaces.IMotorModuleRepository, Repositories.MotorModuleRepository>();
            builder.Services.AddTransient<Repositories.Interfaces.IPcRepository, Repositories.PcRepository>();
            builder.Services.AddTransient<Repositories.Interfaces.IProbeRepository, Repositories.ProbeRepository>();
            builder.Services.AddTransient<Repositories.Interfaces.IPTRViewRepository, Repositories.PTRViewRepository>();
            builder.Services.AddTransient<Repositories.Interfaces.ISharedSeqNoRepository, Repositories.SharedSeqNoRepository>();
            builder.Services.AddTransient<Repositories.Interfaces.ITestCategoryRepository, Repositories.TestCategoryRepository>();
            builder.Services.AddTransient<Repositories.Interfaces.ITesterRepository, Repositories.TesterRepository>();
            builder.Services.AddTransient<Repositories.Interfaces.ITestRepository, Repositories.TestRepository>();
            builder.Services.AddTransient<Repositories.Interfaces.ITestTypeRepository, Repositories.TestTypeRepository>();
            builder.Services.AddTransient<Repositories.Interfaces.ITransducerRepository, Repositories.TransducerRepository>();
            builder.Services.AddTransient<Repositories.Interfaces.ITransducerModuleRepository, Repositories.TransducerModuleRepository>();
            builder.Services.AddTransient<Repositories.Interfaces.ITransducerTypeRepository, Repositories.TransducerTypeRepository>();

            // -------------------------------------------------------------

            // Add services to the container.
            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
