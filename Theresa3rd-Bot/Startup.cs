using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SqlSugar.IOC;
using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Theresa3rd_Bot.Common;
using Theresa3rd_Bot.Dao;
using Theresa3rd_Bot.Model.Config;
using Theresa3rd_Bot.Timer;
using Theresa3rd_Bot.Util;
using YamlDotNet.Serialization;

namespace Theresa3rd_Bot
{
    public class Startup
    {
        private static Task MiraiTask;

        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            LogHelper.ConfigureLog();//log4net
            LogHelper.Info($"��־�������...");

            LoadConfig();
            LogHelper.Info($"�����ļ���ȡ���...");

            LogHelper.Info($"��ʼ��ʼ�����ݿ�...");
            services.AddSqlSugar(new IocConfig()//ע��Sqlsuger
            {
                DbType = IocDbType.MySql,
                ConnectionString = BotConfig.DBConfig.ConnectionString,
                IsAutoCloseConnection = true//�Զ��ͷ�
            });
            new DBClient().CreateDB();
            LogHelper.Info($"���ݿ��ʼ�����...");

            ConfigHelper.loadWebsite();
            ConfigHelper.loadSubscribeTask();
            ConfigHelper.loadBanTag();
            ConfigHelper.loadBanMember();

            //services.AddControllers();
            //services.AddSwaggerGen(c =>
            //{
            //    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Theresa3rd_Bot", Version = "v1" });
            //});
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
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

            MiraiTask = MiraiHelper.ConnectMirai();
            TimerManager.init();
            LogHelper.Info($"Theresa3rd-Bot������ϣ��汾��{BotConfig.BotVersion}");
        }

        /// <summary>
        /// �������ļ��е���Ϣ���ص��ڴ�
        /// </summary>
        private void LoadConfig()
        {
            BotConfig.DBConfig.ConnectionString = Configuration["Database:ConnectionString"];

            BotConfig.MiraiConfig.Host = Configuration["Mirai:host"];
            BotConfig.MiraiConfig.Port = Convert.ToInt32(Configuration["Mirai:port"]);
            BotConfig.MiraiConfig.AuthKey = Configuration["Mirai:authKey"];
            BotConfig.MiraiConfig.BotQQ = Convert.ToInt64(Configuration["Mirai:botQQ"]);

            Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            using FileStream fileStream = new FileStream("botsettings.yml", FileMode.Open, FileAccess.Read);
            using TextReader reader = new StreamReader(fileStream, Encoding.GetEncoding("gb2312"));
            Deserializer deserializer = new Deserializer();
            BotConfigDto botConfig = deserializer.Deserialize<BotConfigDto>(reader);
            BotConfig.GeneralConfig = botConfig.General;
            BotConfig.PermissionsConfig = botConfig.Permissions;
            BotConfig.ManageConfig = botConfig.Manage;
            BotConfig.MenuConfig = botConfig.Menu;
            BotConfig.RepeaterConfig = botConfig.Repeater;
            BotConfig.WelcomeConfig = botConfig.Welcome;
            BotConfig.ReminderConfig = botConfig.Reminder;
            BotConfig.SetuConfig = botConfig.Setu;
            BotConfig.SaucenaoConfig = botConfig.Saucenao;
            BotConfig.SubscribeConfig = botConfig.Subscribe;
        }


    }
}
