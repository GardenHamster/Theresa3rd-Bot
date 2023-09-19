using SqlSugar.IOC;
using System.Net;
using TheresaBot.GoCqHttp.Common;
using TheresaBot.GoCqHttp.Helper;
using TheresaBot.GoCqHttp.Reporter;
using TheresaBot.GoCqHttp.Session;
using TheresaBot.Main.Common;
using TheresaBot.Main.Dao;
using TheresaBot.Main.Datas;
using TheresaBot.Main.Helper;
using TheresaBot.Main.Timers;

namespace TheresaBot.GoCqHttp
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
                LogHelper.Info($"��־�������...");

                CQHelper.LoadAppSettings(Configuration);
                ConfigHelper.LoadBotConfig();
                ConfigHelper.InitBotConfig();
                LogHelper.Info($"�����ļ��������...");

                services.AddControllers();
                services.ConfigureJWT();
                LogHelper.Info($"JWT��ʼ�����...");

                CQHelper.ConnectGoCqHttp().Wait();
                LogHelper.Info($"���Զ�ȡBot��Ƭ...");
                Task.Delay(1000).Wait();
                CQHelper.LoadBotProfileAsync().Wait();

                LogHelper.Info($"��ʼ��ʼ�����ݿ�...");
                services.AddSqlSugar(new IocConfig()//ע��Sqlsuger
                {
                    DbType = IocDbType.MySql,
                    ConnectionString = CQConfig.ConnectionString,
                    IsAutoCloseConnection = true//�Զ��ͷ�
                });
                new DBClient().CreateDB();
                LogHelper.Info($"���ݿ��ʼ�����...");
            }
            catch (Exception ex)
            {
                LogHelper.FATAL(ex, "�����쳣");
                Environment.Exit(0);
                throw;
            }

            try
            {
                DataManager.LoadInitDatas();
                CQSession session = new CQSession();
                CQReporter reporter = new CQReporter();
                TimerManager.initReminderJob(session, reporter);
                TimerManager.initTimingSetuJob(session, reporter);
                TimerManager.initSubscribeTimers(session, reporter);
                TimerManager.initTimingRankingJobAsync(session, reporter);
                TimerManager.initWordCloudTimers(session, reporter);
                TimerManager.initCookieJobAsync(session, reporter);
                TimerManager.initTempClearJobAsync(session, reporter);
                TimerManager.initDownloadClearJobAsync(session, reporter);
                LogHelper.Info($"Theresa3rd-Bot������ϣ��汾��v{BotConfig.BotVersion}");
                Task welcomeTask = CQHelper.SendStartUpMessageAsync();
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex);
                new CQReporter().SendErrorForce(ex, "�����쳣").Wait();
                Environment.Exit(0);
                throw;
            }

        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IHostApplicationLifetime appLifetime)
        {
            try
            {
                ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12 | SecurityProtocolType.Tls13;
                app.UseHttpsRedirection();
                app.UseRouting();
                app.UseAuthentication();//������֤
                app.UseAuthorization();//������Ȩ
                app.UseEndpoints(endpoints =>
                {
                    endpoints.MapControllers();
                });
                appLifetime.ApplicationStarted.Register(OnStarted);
                appLifetime.ApplicationStopping.Register(OnStopping);
                appLifetime.ApplicationStopped.Register(OnStopped);
            }
            catch (Exception ex)
            {
                LogHelper.FATAL(ex, "�����쳣");
                Environment.Exit(0);
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
