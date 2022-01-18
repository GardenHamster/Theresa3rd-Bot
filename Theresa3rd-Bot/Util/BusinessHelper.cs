﻿using Mirai.CSharp.HttpApi.Models.ChatMessages;
using Mirai.CSharp.HttpApi.Models.EventArgs;
using Mirai.CSharp.HttpApi.Session;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Theresa3rd_Bot.Common;
using Theresa3rd_Bot.Model.Config;

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
        /// 判断是否可以处理一个群的消息
        /// </summary>
        /// <param name="groupId"></param>
        /// <returns></returns>
        public static bool IsHandleMessage(long groupId)
        {
            GeneralConfig generalConfig = BotConfig.GeneralConfig;
            if (generalConfig == null) return false;
            List<long> acceptGroups = generalConfig.AcceptGroups;
            if (acceptGroups == null) return false;
            List<long> refuseGroups = generalConfig.RefuseGroups;
            if (refuseGroups == null) return false;
            if (refuseGroups.Contains(groupId)) return false;
            if (acceptGroups.Count > 0 && acceptGroups.Contains(groupId) == false) return false;
            return true;
        }

        /// <summary>
        /// 检查pixiv cookie是否已经过期
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public static async Task<bool> CheckPixivCookieExpireAsync(this IMiraiHttpSession session, IGroupMessageEventArgs args)
        {
            if (DateTime.Now <= BotConfig.WebsiteConfig.Pixiv.CookieExpireDate) return false;
            string cookieExpireMsg = BotConfig.SetuConfig?.Pixiv?.CookieExpireMsg ?? "";
            if (string.IsNullOrWhiteSpace(cookieExpireMsg)) cookieExpireMsg = "cookie过期了，让管理员更新cookie吧~";
            List<IChatMessage> chatList = session.SplitToChainAsync(cookieExpireMsg).Result;
            await session.SendMessageWithAtAsync(args, chatList);
            return true;
        }

        public static bool CheckSTBanWord(this IMiraiHttpSession session, IGroupMessageEventArgs args,string message)
        {
            //string message = e.Message.Text.Trim().ToLower();
            //if (message.Contains("r18") || message.Contains("r17")) return true;
            //string banWord = StringHelper.isContainsWord(e.Message.Text, Setting.Word.BanSTKeyWord);
            //return string.IsNullOrEmpty(banWord) == false;
            return false;
        }

        public static bool CheckSTAllowCustom(this IMiraiHttpSession session, IGroupMessageEventArgs args)
        {
            //if (Setting.Permissions.STCustomGroups.Contains(e.FromGroup.Id)) return true;
            //e.SendMessageWithAt(" 自定义涩图功能暂时关闭，请直接使用 #涩图 命令");
            //return false;
            return true;
        }

        public static int GetSTLeftToday(this IMiraiHttpSession session, IGroupMessageEventArgs args)
        {
            //if (Setting.Permissions.LimitlessGroups.Contains(e.FromGroup.Id)) return Setting.Robot.MaxSTUseOneDay;
            //FunctionRecordBusiness functionRecordBusiness = new FunctionRecordBusiness();
            //int[] functionTypeArr = new int[] { FunctionType.PixivicST.TypeId, FunctionType.PixivGeneralST.TypeId, FunctionType.PixivR18ST.TypeId };
            //int todayUseCount = functionRecordBusiness.getUsedCountToday(e.FromGroup.Id, e.FromQQ.Id, functionTypeArr);
            //int leftToday = Setting.Robot.MaxSTUseOneDay - todayUseCount - 1;
            //return leftToday < 0 ? 0 : leftToday;
            return BotConfig.SetuConfig.MaxDaily;
        }

        /// <summary>
        /// 将模版转换为消息链
        /// </summary>
        /// <param name="session"></param>
        /// <param name="template"></param>
        /// <returns></returns>
        public static async Task<List<IChatMessage>> SplitToChainAsync(this IMiraiHttpSession session,string template)
        {
            List<IChatMessage> chatMessages = new List<IChatMessage>();
            List<string> splitList = SplitImageCode(template);
            foreach (var item in splitList)
            {
                string code = item.Trim();
                if (Regex.Match(code, ImageCodeRegex).Success)
                {
                    string path = code.Substring(ImageCodeHeader.Length, code.Length - ImageCodeHeader.Length - 1);
                    if (File.Exists(path) == false) continue;
                    chatMessages.Add((IChatMessage)await session.UploadPictureAsync(Mirai.CSharp.Models.UploadTarget.Group, path));
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
