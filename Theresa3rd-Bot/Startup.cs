using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Theresa3rd_Bot.Model.Config;

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
            LoadConfig();
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Theresa3rd_Bot", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Theresa3rd_Bot v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            MiraiTask = MiraiHelper.ConnectMirai();
        }

        /// <summary>
        /// 将配置文件中的信息加载到内存
        /// </summary>
        private void LoadConfig()
        {
            GlobalConfig.MiraiConfig.Host = Configuration["Mirai:host"];
            GlobalConfig.MiraiConfig.Port = Convert.ToInt32(Configuration["Mirai:port"]);
            GlobalConfig.MiraiConfig.AuthKey = Configuration["Mirai:authKey"];
            GlobalConfig.MiraiConfig.BotQQ = Convert.ToInt64(Configuration["Mirai:botQQ"]);

            var botJson = File.ReadAllText("botsettings.json", Encoding.UTF8);
            BotConfig botConfig = JsonConvert.DeserializeObject<BotConfig>(botJson);
            GlobalConfig.GeneralConfig = botConfig.General;
            GlobalConfig.RepeaterConfig = botConfig.Repeater;
            GlobalConfig.WelcomeConfig = botConfig.Welcome;
            GlobalConfig.ReminderConfig = botConfig.Reminder;
        }


    }
}
