using Microsoft.Extensions.FileProviders;
using SqlSugar.IOC;
using System.Net;
using TheresaBot.OneBot11.Common;
using TheresaBot.OneBot11.Helper;
using TheresaBot.OneBot11.Reporter;
using TheresaBot.OneBot11.Session;
using TheresaBot.Core.Common;
using TheresaBot.Core.Dao;
using TheresaBot.Core.Datas;
using TheresaBot.Core.Helper;
using TheresaBot.Core.Reporter;
using TheresaBot.Core.Session;
using TheresaBot.Core.Timers;

namespace TheresaBot.OneBot11
{
    public class Startup
    {
        private IConfiguration Configuration;

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            try
            {
                LogHelper.ConfigureLog();
                LogHelper.Info($"日志配置完毕...");

                OBHelper.LoadAppSettings(Configuration);
                ConfigHelper.LoadBotConfig();
                LogHelper.Info($"配置文件加载完毕...");

                services.AddScoped<BaseSession, OBSession>();
                services.AddScoped<BaseReporter, OBReporter>();
                services.AddControllers();
                services.ConfigureJWT();
                services.AddCors(options => options.AddPolicy("cors", p => p.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));
                LogHelper.Info($"后台初始化完毕...");

                OBHelper.ConnectOneBot().Wait();
                BotHelper.LoadBotProfileAsync(new OBSession()).Wait();
                BotHelper.LoadGroupInfosAsync(new OBSession()).Wait();

                LogHelper.Info($"开始初始化数据库...");
                services.AddSqlSugar(new IocConfig()//注入Sqlsuger
                {
                    DbType = IocDbType.MySql,
                    ConnectionString = OBConfig.ConnectionString,
                    IsAutoCloseConnection = true//自动释放
                });
                new DBClient().CreateDB();
                LogHelper.Info($"数据库初始化完毕...");
            }
            catch (Exception ex)
            {
                LogHelper.FATAL(ex, "启动异常");
                Environment.Exit(-1);
                throw;
            }

            try
            {
                DataManager.LoadInitDatas();
                OBSession session = new OBSession();
                OBReporter reporter = new OBReporter();
                TimerManager.InitTimers(session, reporter);
                SchedulerManager.InitSchedulers(session, reporter).Wait();
                LogHelper.Info($"Theresa-Bot启动完毕，版本：v{BotConfig.BotVersion}");
                if (RunningDatas.IsSendStartupMessage())
                {
                    Task welcomeTask = OBHelper.SendStartUpMessageAsync();
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex);
                new OBReporter().SendErrorForce(ex, "启动异常").Wait();
                Environment.Exit(-1);
                throw;
            }

        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IHostApplicationLifetime appLifetime)
        {
            try
            {
                ConfigHelper.SetAppConfig(app);
                ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12 | SecurityProtocolType.Tls13;
                app.UseRouting();
                app.UseCors("cors");//允许跨域
                app.UseAuthentication();//开启认证
                app.UseAuthorization();//开启授权
                app.UseDefaultFiles();
                app.UseStaticFiles();
                app.UseStaticFiles(new StaticFileOptions()//静态文件访问配置
                {
                    RequestPath = new PathString(FilePath.ImgHttpPath),//对外的访问路径
                    FileProvider = new PhysicalFileProvider(FilePath.GetBotImgDirectory())//指定实际物理路径
                });
                app.UseEndpoints(endpoints =>
                {
                    endpoints.MapControllers();
                });
                appLifetime.ApplicationStarted.Register(OnStarted);
                appLifetime.ApplicationStopping.Register(OnStopping);
                appLifetime.ApplicationStopped.Register(OnStopped);
                BusinessHelper.ShowBackstageInfos();
            }
            catch (Exception ex)
            {
                LogHelper.FATAL(ex, "启动异常");
                Environment.Exit(-1);
                throw;
            }
        }

        private void OnStarted()
        {
        }

        private void OnStopping()
        {
            Environment.Exit(0);
        }

        private void OnStopped()
        {
            Environment.Exit(0);
        }

    }
}
