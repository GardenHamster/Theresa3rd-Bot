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
            LogHelper.Info($"日志配置完毕...");

            LoadConfig();
            LogHelper.Info($"配置文件读取完毕...");

            LogHelper.Info($"开始初始化数据库...");
            services.AddSqlSugar(new IocConfig()//注入Sqlsuger
            {
                DbType = IocDbType.MySql,
                ConnectionString = BotConfig.DBConfig.ConnectionString,
                IsAutoCloseConnection = true//自动释放
            });
            new DBClient().CreateDB();
            LogHelper.Info($"数据库初始化完毕...");

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
            LogHelper.Info($"Theresa3rd-Bot启动完毕，版本：{BotConfig.BotVersion}");
        }

        /// <summary>
        /// 将配置文件中的信息加载到内存
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
