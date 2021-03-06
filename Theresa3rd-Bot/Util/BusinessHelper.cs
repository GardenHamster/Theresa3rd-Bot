using Mirai.CSharp.HttpApi.Models.ChatMessages;
using Mirai.CSharp.HttpApi.Models.EventArgs;
using Mirai.CSharp.HttpApi.Session;
using Mirai.CSharp.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Theresa3rd_Bot.Business;
using Theresa3rd_Bot.Cache;
using Theresa3rd_Bot.Common;
using Theresa3rd_Bot.Model.Config;
using Theresa3rd_Bot.Model.PO;
using Theresa3rd_Bot.Type;

namespace Theresa3rd_Bot.Util
{
    public static class BusinessHelper
    {
        /// <summary>
        /// image码
        /// </summary>
        private static string ImageCodeRegex = @"\[image:[^\[\]]+?\]";

        /// <summary>
        /// image头
        /// </summary>
        private static string ImageCodeHeader = @"[image:";

        /// <summary>
        /// 判断是否以某一个指令开头
        /// </summary>
        /// <param name="instructions"></param>
        /// <param name="command"></param>
        /// <returns></returns>
        public static bool StartWithCommand(this string instructions, string command)
        {
            return string.IsNullOrWhiteSpace(command) == false && instructions.StartsWith(command.Trim());
        }

        /// <summary>
        /// 判断是否以某一个指令开头
        /// </summary>
        /// <param name="instructions"></param>
        /// <param name="command"></param>
        /// <returns></returns>
        public static bool StartWithCommand(this string instructions, string[] commands)
        {
            if (commands == null || commands.Length == 0) return false;
            return commands.Where(o => string.IsNullOrWhiteSpace(o) == false && instructions.StartsWith(o.Trim())).Any();
        }

        /// <summary>
        /// 判断是否可以处理一个群的消息
        /// </summary>
        /// <param name="groupId"></param>
        /// <returns></returns>
        public static bool IsHandleMessage(long groupId)
        {
            PermissionsConfig permissionsConfig = BotConfig.PermissionsConfig;
            if (permissionsConfig == null) return false;
            List<long> acceptGroups = permissionsConfig.AcceptGroups;
            if (acceptGroups == null) return false;
            return acceptGroups.Contains(groupId);
        }

        /// <summary>
        /// 判断标签中是否包含R18标签
        /// </summary>
        /// <param name="tags"></param>
        /// <returns></returns>
        public static bool IsR18(this List<string> tags)
        {
            if (tags == null || tags.Count == 0) return false;
            return tags.Where(o => o.ToUpper().StartsWith("R-18") || o.ToUpper().StartsWith("R18") || o.ToUpper().StartsWith("R18G")).Any();
        }

        /// <summary>
        /// 判断标签中是否包含不合法标签
        /// </summary>
        /// <param name="tags"></param>
        /// <returns></returns>
        public static bool IsImproper(this List<string> tags)
        {
            if (tags == null || tags.Count == 0) return false;
            if (tags.Where(o => o.ToUpper().Replace("-", "").Replace(" ", "").Contains("R18G")).Any()) return true;
            return false;
        }

        /// <summary>
        /// 判断标签中是否包含动图标签
        /// </summary>
        /// <param name="tags"></param>
        /// <returns></returns>
        public static bool IsGif(this List<string> tags)
        {
            if (tags == null || tags.Count == 0) return false;
            return tags.Where(o => o == "うごイラ" || o == "动图" || o.ToLower() == "ugoira").Any();
        }

        /// <summary>
        /// 检查pixiv cookie是否已经过期
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public static bool IsPixivCookieAvailable()
        {
            if (string.IsNullOrWhiteSpace(BotConfig.WebsiteConfig.Pixiv.Cookie)) return false;
            if (DateTime.Now > BotConfig.WebsiteConfig.Pixiv.CookieExpireDate) return false;
            if (BotConfig.WebsiteConfig.Pixiv.UserId <= 0) return false;
            return true;
        }

        /// <summary>
        /// 检查pixiv cookie是否已经过期
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public static async Task<bool> CheckPixivCookieAvailableAsync(this IMiraiHttpSession session, IGroupMessageEventArgs args)
        {
            if (string.IsNullOrWhiteSpace(BotConfig.WebsiteConfig.Pixiv.Cookie))
            {
                await session.SendMessageWithAtAsync(args, new PlainMessage("缺少pixiv cookie，请设置cookie"));
                return false;
            }
            if (DateTime.Now > BotConfig.WebsiteConfig.Pixiv.CookieExpireDate)
            {
                await session.SendTemplateWithAtAsync(args, BotConfig.SetuConfig.Pixiv.CookieExpireMsg, "cookie过期了，让管理员更新cookie吧");
                return false;
            }
            if (BotConfig.WebsiteConfig.Pixiv.UserId <= 0)
            {
                await session.SendMessageWithAtAsync(args, new PlainMessage("缺少userId，请更新cookie"));
                return false;
            }
            return true;
        }

        /// <summary>
        /// 检查订阅功能是否开启
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public static async Task<bool> CheckSubscribeEnableAsync(this IMiraiHttpSession session, IGroupMessageEventArgs args, BaseSubscribeConfig subscribeConfig)
        {
            if (BotConfig.PermissionsConfig.SubscribeGroups.Contains(args.Sender.Group.Id) == false)
            {
                await session.SendTemplateWithAtAsync(args, BotConfig.GeneralConfig.NoPermissionsMsg, "该功能未授权");
                return false;
            }
            if (subscribeConfig == null || subscribeConfig.Enable == false)
            {
                await session.SendTemplateWithAtAsync(args, BotConfig.GeneralConfig.DisableMsg, "该功能已关闭");
                return false;
            }
            return true;
        }

        /// <summary>
        /// 检查涩图功能是否可用
        /// </summary>
        /// <param name="session"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static async Task<bool> CheckSetuEnableAsync(this IMiraiHttpSession session, IGroupMessageEventArgs args)
        {
            if (BotConfig.PermissionsConfig.SetuGroups.Contains(args.Sender.Group.Id) == false)
            {
                await session.SendTemplateWithAtAsync(args, BotConfig.GeneralConfig.NoPermissionsMsg, "该功能未授权");
                return false;
            }
            if (BotConfig.SetuConfig?.Pixiv == null || BotConfig.SetuConfig.Pixiv.Enable == false)
            {
                await session.SendTemplateWithAtAsync(args, BotConfig.SetuConfig.Pixiv.DisableMsg, "该功能已关闭");
                return false;
            }
            return true;
        }

        /// <summary>
        /// 检查涩图标签是否被禁止
        /// </summary>
        /// <param name="session"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static async Task<bool> CheckSetuTagEnableAsync(this IMiraiHttpSession session, IGroupMessageEventArgs args, string message)
        {
            message = message.ToLower().Trim();
            long groupId = args.Sender.Group.Id;
            if (BotConfig.SetuConfig.DisableTags.Where(o => message == o.ToLower()).Any())
            {
                await session.SendTemplateWithAtAsync(args, BotConfig.SetuConfig.DisableTagsMsg, "禁止查找这个类型的涩图");
                return false;
            }

            List<BanWordPO> banSetuList = BotConfig.BanSetuMap.ContainsKey(groupId) ? BotConfig.BanSetuMap[groupId] : new List<BanWordPO>();
            if (banSetuList.Where(o => message.IndexOf(o.KeyWord.ToLower()) > -1).Any())
            {
                await session.SendTemplateWithAtAsync(args, BotConfig.SetuConfig.DisableTagsMsg, "禁止查找这个类型的涩图");
                return false;
            }
            return true;
        }

        /// <summary>
        /// 检查原图功能是否可用
        /// </summary>
        /// <param name="session"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static async Task<bool> CheckSaucenaoEnableAsync(this IMiraiHttpSession session, IGroupMessageEventArgs args)
        {
            if (BotConfig.PermissionsConfig.SaucenaoGroups.Contains(args.Sender.Group.Id) == false)
            {
                await session.SendTemplateWithAtAsync(args, BotConfig.GeneralConfig.NoPermissionsMsg, "该功能未授权");
                return false;
            }
            if (BotConfig.SaucenaoConfig == null || BotConfig.SaucenaoConfig.Enable == false)
            {
                await session.SendTemplateWithAtAsync(args, BotConfig.SaucenaoConfig.DisableMsg, "该功能已关闭");
                return false;
            }
            return true;
        }

        /// <summary>
        /// 检查是否超级管理员
        /// </summary>
        /// <param name="session"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static async Task<bool> CheckSuperManagersAsync(this IMiraiHttpSession session, IGroupMessageEventArgs args)
        {
            if (BotConfig.PermissionsConfig.SuperManagers.Contains(args.Sender.Id) == false)
            {
                await session.SendTemplateWithAtAsync(args, BotConfig.GeneralConfig.ManagersRequiredMsg, "该功能需要管理员执行");
                return false;
            }
            return true;
        }

        /// <summary>
        /// 检查是否超级管理员
        /// </summary>
        /// <param name="session"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static async Task<bool> CheckSuperManagersAsync(this IMiraiHttpSession session, IFriendMessageEventArgs args)
        {
            if (BotConfig.PermissionsConfig.SuperManagers.Contains(args.Sender.Id) == false)
            {
                await session.SendTemplateAsync(args, BotConfig.GeneralConfig.ManagersRequiredMsg, "该功能需要管理员执行");
                return false;
            }
            return true;
        }


        /// <summary>
        /// 检查涩图功能是否在冷却中
        /// </summary>
        /// <param name="session"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static async Task<bool> CheckMemberSetuCoolingAsync(this IMiraiHttpSession session, IGroupMessageEventArgs args)
        {
            if (BotConfig.PermissionsConfig.SetuNoneCDGroups.Contains(args.Sender.Group.Id)) return false;
            int cdSecond = CoolingCache.GetMemberSetuCooling(args.Sender.Group.Id, args.Sender.Id);
            if (cdSecond <= 0) return false;
            await session.SendMessageWithAtAsync(args, new PlainMessage($" 功能冷却中，{cdSecond}秒后再来哦~"));
            return true;
        }

        /// <summary>
        /// 检查原图功能是否在冷却中
        /// </summary>
        /// <param name="session"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static async Task<bool> CheckMemberSaucenaoCoolingAsync(this IMiraiHttpSession session, IGroupMessageEventArgs args)
        {
            if (BotConfig.PermissionsConfig.SetuNoneCDGroups.Contains(args.Sender.Group.Id)) return false;
            int cdSecond = CoolingCache.GetMemberSaucenaoCooling(args.Sender.Group.Id, args.Sender.Id);
            if (cdSecond <= 0) return false;
            await session.SendMessageWithAtAsync(args, new PlainMessage($" 功能冷却中，{cdSecond}秒后再来哦~"));
            return true;
        }

        /// <summary>
        /// 检查涩图功能是否开启
        /// </summary>
        /// <param name="session"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static async Task<bool> ChecekGroupSetuCoolingAsync(this IMiraiHttpSession session, IGroupMessageEventArgs args)
        {
            if (BotConfig.PermissionsConfig.SetuNoneCDGroups.Contains(args.Sender.Group.Id)) return false;
            int cdSecond = CoolingCache.GetGroupSetuCooling(args.Sender.Group.Id, args.Sender.Id);
            if (cdSecond <= 0) return false;
            await session.SendMessageWithAtAsync(args, new PlainMessage($" 群功能冷却中，{cdSecond}秒后再来哦~"));
            return true;
        }

        /// <summary>
        /// 检查涩图功能可用次数
        /// </summary>
        /// <param name="session"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static async Task<bool> CheckSetuUseUpAsync(this IMiraiHttpSession session, IGroupMessageEventArgs args)
        {
            if (BotConfig.PermissionsConfig.SetuLimitlessGroups.Contains(args.Sender.Group.Id)) return false;
            if (BotConfig.SetuConfig.MaxDaily == 0) return false;
            int useCount = new RequestRecordBusiness().getUsedCountToday(args.Sender.Group.Id, args.Sender.Id, CommandType.Setu);
            if (useCount < BotConfig.SetuConfig.MaxDaily) return false;
            await session.SendMessageWithAtAsync(args, new PlainMessage(" 你今天的使用次数已经达到上限了，明天再来吧"));
            return true;
        }

        /// <summary>
        /// 检查原图功能可用次数
        /// </summary>
        /// <param name="session"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static async Task<bool> CheckSaucenaoUseUpAsync(this IMiraiHttpSession session, IGroupMessageEventArgs args)
        {
            if (BotConfig.SaucenaoConfig.MaxDaily == 0) return false;
            int useCount = new RequestRecordBusiness().getUsedCountToday(args.Sender.Group.Id, args.Sender.Id, CommandType.Saucenao);
            if (useCount < BotConfig.SaucenaoConfig.MaxDaily) return false;
            await session.SendMessageWithAtAsync(args, new PlainMessage(" 你今天的使用次数已经达到上限了，明天再来吧"));
            return true;
        }

        /// <summary>
        /// 检查是否有涩图请求在处理中
        /// </summary>
        /// <param name="session"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static async Task<bool> CheckHandingAsync(this IMiraiHttpSession session, IGroupMessageEventArgs args)
        {
            if (CoolingCache.IsHanding(args.Sender.Group.Id, args.Sender.Id) == false) return false;
            await session.SendMessageWithAtAsync(args, new PlainMessage(" 你的一个请求正在处理中，稍后再来吧"));
            return true;
        }

        /// <summary>
        /// 检查是否拥有自定义涩图权限
        /// </summary>
        /// <param name="session"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static async Task<bool> CheckSetuCustomEnableAsync(this IMiraiHttpSession session, IGroupMessageEventArgs args)
        {
            if (BotConfig.PermissionsConfig.SetuCustomGroups.Contains(args.Sender.Group.Id)) return true;
            await session.SendTemplateWithAtAsync(args, BotConfig.GeneralConfig.SetuCustomDisableMsg, " 自定义功能已关闭");
            return false;
        }

        /// <summary>
        /// 获取今日涩图可用次数
        /// </summary>
        /// <param name="session"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static long GetSetuLeftToday(this IMiraiHttpSession session, IGroupMessageEventArgs args)
        {
            if (BotConfig.SetuConfig.MaxDaily == 0) return 0;
            if (BotConfig.PermissionsConfig.SetuLimitlessGroups.Contains(args.Sender.Group.Id)) return BotConfig.SetuConfig.MaxDaily;
            RequestRecordBusiness requestRecordBusiness = new RequestRecordBusiness();
            int todayUseCount = requestRecordBusiness.getUsedCountToday(args.Sender.Group.Id, args.Sender.Id, CommandType.Setu);
            long leftToday = BotConfig.SetuConfig.MaxDaily - todayUseCount - 1;
            return leftToday < 0 ? 0 : leftToday;
        }

        /// <summary>
        /// 获取今日原图可用次数
        /// </summary>
        /// <param name="session"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static int GetSaucenaoLeftToday(this IMiraiHttpSession session, IGroupMessageEventArgs args)
        {
            if (BotConfig.SaucenaoConfig.MaxDaily == 0) return 0;
            RequestRecordBusiness requestRecordBusiness = new RequestRecordBusiness();
            int todayUseCount = requestRecordBusiness.getUsedCountToday(args.Sender.Group.Id, args.Sender.Id, CommandType.Saucenao);
            int leftToday = BotConfig.SaucenaoConfig.MaxDaily - todayUseCount - 1;
            return leftToday < 0 ? 0 : leftToday;
        }

        /// <summary>
        /// 将模版转换为消息链
        /// </summary>
        /// <param name="session"></param>
        /// <param name="template"></param>
        /// <returns></returns>
        public static async Task<List<IChatMessage>> SplitToChainAsync(this IMiraiHttpSession session, string template, UploadTarget uploadTarget = UploadTarget.Group)
        {
            List<IChatMessage> chatMessages = new List<IChatMessage>();
            List<string> splitList = SplitImageCode(template);
            foreach (var item in splitList)
            {
                string code = item.Trim();
                if (string.IsNullOrEmpty(item)) continue;
                if (Regex.Match(code, ImageCodeRegex).Success)
                {
                    string path = code.Substring(ImageCodeHeader.Length, code.Length - ImageCodeHeader.Length - 1);
                    if (File.Exists(path) == false) continue;
                    chatMessages.Add((IChatMessage)await session.UploadPictureAsync(uploadTarget, path));
                }
                else
                {
                    chatMessages.Add(new PlainMessage(item));
                }
            }
            return chatMessages;
        }

        /// <summary>
        /// 根据image码拆分模版
        /// </summary>
        /// <param name="template"></param>
        /// <returns></returns>
        public static List<string> SplitImageCode(this string template)
        {
            if (string.IsNullOrEmpty(template)) return new List<string>();
            string[] textArr = Regex.Split(template, ImageCodeRegex);
            if (textArr.Length == 1) return new List<string>() { template };
            List<string> SplitList = new List<string>();
            for (int i = 0; i < textArr.Length; i++)
            {
                string text = textArr[i];
                if (text.Length == 0) continue;
                int textIndex = template.IndexOf(text);
                if (textIndex > 0)
                {
                    SplitList.Add(template.Substring(0, textIndex));
                    template = template.Substring(textIndex, template.Length - textIndex);
                }
                SplitList.Add(text);
                template = template.Substring(text.Length, template.Length - text.Length);
            }
            if (template.Length > 0) SplitList.Add(template);
            return SplitList;
        }


    }
}
