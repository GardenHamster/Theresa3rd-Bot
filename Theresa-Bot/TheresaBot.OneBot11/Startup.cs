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
                LogHelper.Info($"��־�������...");

                OBHelper.LoadAppSettings(Configuration);
                ConfigHelper.LoadBotConfig();
                LogHelper.Info($"�����ļ��������...");

                services.AddScoped<BaseSession, OBSession>();
                services.AddScoped<BaseReporter, OBReporter>();
                services.AddControllers();
                services.ConfigureJWT();
                services.AddCors(options => options.AddPolicy("cors", p => p.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));
                LogHelper.Info($"��̨��ʼ�����...");

                OBHelper.ConnectOneBot().Wait();
                BotHelper.LoadBotProfileAsync(new OBSession()).Wait();
                BotHelper.LoadGroupInfosAsync(new OBSession()).Wait();

                LogHelper.Info($"��ʼ��ʼ�����ݿ�...");
                services.AddSqlSugar(new IocConfig()//ע��Sqlsuger
                {
                    DbType = IocDbType.MySql,
                    ConnectionString = OBConfig.ConnectionString,
                    IsAutoCloseConnection = true//�Զ��ͷ�
                });
                new DBClient().CreateDB();
                LogHelper.Info($"���ݿ��ʼ�����...");
            }
            catch (Exception ex)
            {
                LogHelper.FATAL(ex, "�����쳣");
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
                LogHelper.Info($"Theresa-Bot������ϣ��汾��v{BotConfig.BotVersion}");
                if (RunningDatas.IsSendStartupMessage())
                {
                    Task welcomeTask = OBHelper.SendStartUpMessageAsync();
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex);
                new OBReporter().SendErrorForce(ex, "�����쳣").Wait();
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
                app.UseCors("cors");//�������
                app.UseAuthentication();//������֤
                app.UseAuthorization();//������Ȩ
                app.UseDefaultFiles();
                app.UseStaticFiles();
                app.UseStaticFiles(new StaticFileOptions()//��̬�ļ���������
                {
                    RequestPath = new PathString(FilePath.ImgHttpPath),//����ķ���·��
                    FileProvider = new PhysicalFileProvider(FilePath.GetBotImgDirectory())//ָ��ʵ������·��
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
                LogHelper.FATAL(ex, "�����쳣");
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
