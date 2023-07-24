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
                LogHelper.Info($"�����ļ��������...");

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
                throw;
            }

            try
            {
                WebsiteDatas.LoadWebsite();
                SubscribeDatas.LoadSubscribeTask();
                BanTagDatas.LoadDatas();
                BanMemberDatas.LoadDatas();
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
            }

            try
            {
                //services.AddControllers();
                //services.AddSwaggerGen(c =>
                //{
                //    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Theresa3rd_Bot", Version = "v1" });
                //});
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex);
                new CQReporter().SendErrorForce(ex, "�����쳣").Wait();
            }
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IHostApplicationLifetime appLifetime)
        {
            ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12 | SecurityProtocolType.Tls13;

            //if (env.IsDevelopment())
            //{
            //    app.UseDeveloperExceptionPage();
            //    app.UseSwagger();
            //    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Theresa3rd_Bot v1"));
            //}

            //app.UseHttpsRedirection();

            //app.UseRouting();

            //app.UseAuthorization();

            //app.UseEndpoints(endpoints =>
            //{
            //    endpoints.MapControllers();
            //});

            appLifetime.ApplicationStarted.Register(OnStarted);
            appLifetime.ApplicationStopping.Register(OnStopping);
            appLifetime.ApplicationStopped.Register(OnStopped);
        }

        private void OnStarted()
        {
            //"On-started" logic
        }

        private void OnStopping()
        {
            //"On-stopping" logic
        }

        private void OnStopped()
        {
            //"On-stopped" logic
        }


    }
}
