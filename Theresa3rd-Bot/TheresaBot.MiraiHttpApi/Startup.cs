using SqlSugar.IOC;
using System.Net;
using TheresaBot.Main.Common;
using TheresaBot.Main.Dao;
using TheresaBot.Main.Datas;
using TheresaBot.Main.Helper;
using TheresaBot.Main.Timers;
using TheresaBot.MiraiHttpApi.Common;
using TheresaBot.MiraiHttpApi.Helper;
using TheresaBot.MiraiHttpApi.Reporter;
using TheresaBot.MiraiHttpApi.Session;

namespace TheresaBot.MiraiHttpApi
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

                MiraiHelper.LoadAppSettings(Configuration);
                ConfigHelper.LoadBotConfig();
                LogHelper.Info($"�����ļ��������...");

                MiraiHelper.ConnectMirai().Wait();
                LogHelper.Info($"���Զ�ȡBot��Ƭ...");
                Task.Delay(1000).Wait();
                MiraiHelper.LoadBotProfileAsync().Wait();

                LogHelper.Info($"��ʼ��ʼ�����ݿ�...");
                services.AddSqlSugar(new IocConfig()//ע��Sqlsuger
                {
                    DbType = IocDbType.MySql,
                    ConnectionString = MiraiConfig.ConnectionString,
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
                MiraiSession session = new MiraiSession();
                MiraiReporter reporter = new MiraiReporter();
                TimerManager.initReminderJob(session, reporter);
                TimerManager.initTimingSetuJob(session, reporter);
                TimerManager.initSubscribeTimers(session, reporter);
                TimerManager.initTimingRankingJobAsync(session, reporter);
                TimerManager.initCookieJobAsync(session, reporter);
                TimerManager.initTempClearJobAsync(session, reporter);
                TimerManager.initDownloadClearJobAsync(session, reporter);
                LogHelper.Info($"Theresa3rd-Bot������ϣ��汾��v{BotConfig.BotVersion}");
                Task welcomeTask = MiraiHelper.SendStartUpMessageAsync();
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex);
                new MiraiReporter().SendErrorForce(ex, "�����쳣").Wait();
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
                new MiraiReporter().SendErrorForce(ex, "�����쳣").Wait();
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
