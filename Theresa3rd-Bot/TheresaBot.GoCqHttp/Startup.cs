using Microsoft.Extensions.FileProviders;
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
using TheresaBot.Main.Reporter;
using TheresaBot.Main.Session;
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
                LogHelper.Info($"�����ļ��������...");

                services.AddScoped<BaseSession, CQSession>();
                services.AddScoped<BaseReporter, CQReporter>();
                services.AddControllers();
                services.ConfigureJWT();
                services.AddCors(options => options.AddPolicy("cors", p => p.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));
                LogHelper.Info($"��̨��ʼ�����...");

                CQHelper.ConnectGoCqHttp().Wait();
                BotHelper.LoadBotProfileAsync(new CQSession()).Wait();
                BotHelper.LoadGroupInfosAsync(new CQSession()).Wait();

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
                Environment.Exit(-1);
                throw;
            }

            try
            {
                DataManager.LoadInitDatas();
                CQSession session = new CQSession();
                CQReporter reporter = new CQReporter();
                TimerManager.InitTimers(session, reporter);
                SchedulerManager.InitSchedulers(session, reporter).Wait();
                LogHelper.Info($"Theresa3rd-Bot������ϣ��汾��v{BotConfig.BotVersion}");
                if (RunningDatas.IsSendStartupMessage())
                {
                    Task welcomeTask = CQHelper.SendStartUpMessageAsync();
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex);
                new CQReporter().SendErrorForce(ex, "�����쳣").Wait();
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
