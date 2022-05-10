using System;

namespace Theresa3rd_Bot.Common
{
    public static class HttpUrl
    {
        /// <summary>
        /// 月光社主页地址
        /// </summary>
        public static string YGSHomeUrl = "https://www.3rdguide.com";

        /// <summary>
        /// pixiv主页
        /// </summary>
        public static readonly string PixivHomeUrl = "https://www.pixiv.net";


        /*---------------------------------------------------------------pixiv-----------------------------------------------------------------------*/

        /// <summary>
        /// 返回pixiv搜索路径
        /// </summary>
        /// <param name="searchWord"></param>
        /// <returns></returns>
        public static string getPixivSearchUrl(string searchWord, int pageNo, bool isMatchAll)
        {
            string keyword = System.Web.HttpUtility.UrlEncode(searchWord).Replace("+", " ");
            string mode = "safe";//r18,safe;
            string s_mode = isMatchAll ? "s_tag_full" : "s_tag";//s_tag_full,s_tag,s_tc
            return $"{PixivHomeUrl}/ajax/search/illustrations/{keyword}?order=date_d&mode={mode}&p={pageNo}&s_mode={s_mode}&type=illust_and_ugoira&lang=zh";
        }

        /// <summary>
        /// 根据作品id获取收藏阅读量等相关信息
        /// </summary>
        /// <param name="workId"></param>
        /// <returns></returns>
        public static string getPixivWorkInfoUrl(string workId)
        {
            return $"{PixivHomeUrl}/ajax/illust/{workId}?lang=zh";
        }

        /// <summary>
        /// 画师作品地址
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        public static string getPixivUserWorkInfoUrl(string userid)
        {
            return $"{PixivHomeUrl}/ajax/user/{userid}/profile/top?lang=zh";
        }

        /// <summary>
        /// 动图地址信息
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        public static string getPixivUgoiraMetaUrl(string workId)
        {
            return $"{PixivHomeUrl}/ajax/illust/{workId}/ugoira_meta?lang=zh";
        }

        /// <summary>
        /// 返回pixiv搜索referer
        /// </summary>
        /// <param name="searchWord"></param>
        /// <returns></returns>
        public static string getPixivSearchReferer(string searchWord)
        {
            string keyword = System.Web.HttpUtility.UrlEncode(searchWord).Replace("+", " ");
            string mode = "safe";
            string s_mode = "s_tag";//s_tag_full/s_tag
            return $"{PixivHomeUrl}/tags/{keyword}/illustrations?mode={mode}&s_mode={s_mode}";
        }

        /// <summary>
        /// 画师作品地址的Referer
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        public static string getPixivUserWorkInfoReferer(string userid)
        {
            return $"{PixivHomeUrl}/user/{userid}";
        }

        /// <summary>
        /// 下载作品图片的Referer
        /// </summary>
        /// <param name="workId"></param>
        /// <returns></returns>
        public static string getPixivArtworksReferer(string workId)
        {
            return $"{PixivHomeUrl}/artworks/{workId}";
        }

        /*---------------------------------------------------------------Lolicon-----------------------------------------------------------------------*/

        /// <summary>
        /// 画师作品地址
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        public static string getLoliconApiV2Url()
        {
            return "https://api.lolicon.app/setu/v2";
        }

        /*---------------------------------------------------------------米游社-----------------------------------------------------------------------*/

        /// <summary>
        /// 米游社用户帖子列表地址
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="gids"></param>
        /// <returns></returns>
        public static string getMysPostListUrl(string userId, int gids)
        {
            return string.Format("https://api-takumi.mihoyo.com/post/wapi/userPost?gids={0}&size=5&uid={1}", gids, userId);
        }

        /// <summary>
        /// 米游社用户帖子详情地址
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static string getMysArticleUrl(string articleId)
        {
            return string.Format("https://bbs.mihoyo.com/bh3/article/{0}", articleId);
        }


        /*---------------------------------------------------------------BiliBili-----------------------------------------------------------------------*/

        /// <summary>
        /// bilibili用户动态地址
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static string getBLBLUserPostList(string userId)
        {
            return string.Format("https://api.vc.bilibili.com/dynamic_svr/v1/dynamic_svr/space_history?visitor_uid=0&host_uid={0}&offset_dynamic_id=0&need_top=1", userId);
        }

        /// <summary>
        /// bilibili用户动态refer
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static string getBLBLUserPostReferer(string userId)
        {
            return string.Format("https://space.bilibili.com/{0}/dynamic", userId);
        }

        /// <summary>
        /// bilibili用户动态地址
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static string getBLBLUserInfo(string userId)
        {
            return string.Format("https://api.bilibili.com/x/space/acc/info?mid={0}&jsonp=jsonp", userId);
        }

        /// <summary>
        /// bilibili用户动态referer
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static string getBLBLUserReferer(string userId)
        {
            return string.Format("https://space.bilibili.com/{0}", userId);
        }

        /// <summary>
        /// up主投稿视频地址
        /// </summary>
        /// <param name="videoCode"></param>
        /// <returns></returns>
        public static string getBLBLVideoUrl(string videoCode)
        {
            return string.Format("https://www.bilibili.com/video/{0}", videoCode);
        }

        /// <summary>
        /// up主发布文章地址
        /// </summary>
        /// <param name="articleId"></param>
        /// <returns></returns>
        public static string getBLBLArticleUrl(string articleId)
        {
            return string.Format("https://t.bilibili.com/{0}", articleId);
        }

        /// <summary>
        /// up主发布动态地址
        /// </summary>
        /// <param name="articleId"></param>
        /// <returns></returns>
        public static string getBLBLDynamicUrl(string dynamicId)
        {
            return string.Format("https://t.bilibili.com/{0}", dynamicId);
        }

        /// <summary>
        /// up主动态主页地址
        /// </summary>
        /// <param name="articleId"></param>
        /// <returns></returns>
        public static string getBLBLSpaceUrl(string userId)
        {
            return string.Format("https://space.bilibili.com/{0}/dynamic", userId);
        }


        /*---------------------------------------------------------------月光社-----------------------------------------------------------------------*/

        /// <summary>
        /// 获取月光社查找武器信息地址
        /// </summary>
        /// <param name="articleId"></param>
        /// <returns></returns>
        public static string getYGSArmSearchUrl(string keyword)
        {
            return string.Format("https://www.3rdguide.com/web/arm/index?searchKey={0}", keyword);
        }

        /// <summary>
        /// 获取月光社查找圣痕信息地址
        /// </summary>
        /// <param name="articleId"></param>
        /// <returns></returns>
        public static string getYGSStigmaSearchUrl(string keyword)
        {
            return string.Format("https://www.3rdguide.com/web/stig/search?name={0}", keyword);
        }

        /// <summary>
        /// 获取月光社角色详情地址
        /// </summary>
        /// <param name="articleId"></param>
        /// <returns></returns>
        public static string getYGSRoleDetailUrl(string id)
        {
            return string.Format("https://www.3rdguide.com/web/valk/detail?id={0}", id);
        }

        /// <summary>
        /// 获取武器列表地址
        /// </summary>
        /// <param name="articleId"></param>
        /// <returns></returns>
        public static string getYGSArmListUrl(int page, string armType)
        {
            return string.Format("https://www.3rdguide.com/web/arm/index?page={0}&type={1}", page, armType);
        }

        /// <summary>
        /// 获取武器列表地址
        /// </summary>
        /// <param name="articleId"></param>
        /// <returns></returns>
        public static string getYGSStigmaHomeUrl()
        {
            return string.Format("https://www.3rdguide.com/web/stig/index");
        }


        /*---------------------------------------------------------------本地图片位置-----------------------------------------------------------------------*/
        /*
        /// <summary>
        /// tomcat上gif的访问地址
        /// </summary>
        /// <param name="workId"></param>
        /// <returns></returns>
        public static string getTomcatGifUrl(string workId)
        {
            return string.Format("http://{0}/gif/{1}.gif", localHost, workId);
        }

        /// <summary>
        /// tomcat上图片的访问地址
        /// </summary>
        /// <param name="workId"></param>
        /// <returns></returns>
        public static string getTomcatTheresaImgUrl(string fileName)
        {
            return string.Format("http://{0}/theresa/{1}", localHost, fileName);
        }

        /// <summary>
        /// tomcat上图片的访问地址
        /// </summary>
        /// <param name="workId"></param>
        /// <returns></returns>
        public static string getTomcatRandomImgUrl(string dirName, string fileName)
        {
            return string.Format("http://{0}/randomImg/{1}/{2}", localHost, dirName, fileName);
        }
        */

        /*---------------------------------------------------------------签到相关-----------------------------------------------------------------------*/

        /// <summary>
        /// 获取原神签到奖励列表
        /// </summary>
        /// <returns></returns>
        public static string getBH3SignRewardUrl(string act_id, string uid, string region)
        {
            return string.Format("https://api-takumi.mihoyo.com/common/eutheniav2/index?act_id={0}&uid={1}&region={2}", act_id, uid, region);
        }

        /// <summary>
        /// 崩三签到
        /// </summary>
        /// <returns></returns>
        public static string getBH3SignUrl()
        {
            return string.Format("https://api-takumi.mihoyo.com/common/eutheniav2/sign");
        }

        /// <summary>
        /// 崩三签到Referer
        /// </summary>
        /// <param name="act_id"></param>
        /// <returns></returns>
        public static string getBH3SignReferer(string act_id)
        {
            return string.Format("https://webstatic.mihoyo.com/bh3/event/signin-cn/index.html?bbs_presentation_style=fullscreen&bbs_game_role_required=bh3_cn&bbs_auth_required=true&act_id={0}&utm_source=bbs&utm_medium=mys&utm_campaign=icon", act_id);
        }

        /// <summary>
        /// 获取原神签到奖励列表
        /// </summary>
        /// <returns></returns>
        public static string getYSSignRewardUrl(string act_id)
        {
            return string.Format("https://api-takumi.mihoyo.com/event/bbs_sign_reward/home?act_id={0}", act_id);
        }

        /// <summary>
        /// 获取原神签到奖励列表
        /// </summary>
        /// <returns></returns>
        public static string getYSSignInfoUrl(string act_id, string region, string uid)
        {
            return string.Format("https://api-takumi.mihoyo.com/event/bbs_sign_reward/info?act_id={0}&region={1}&uid={2}", act_id, region, uid);
        }

        /// <summary>
        /// 原神签到
        /// </summary>
        /// <returns></returns>
        public static string getYSSignUrl()
        {
            return string.Format("https://api-takumi.mihoyo.com/event/bbs_sign_reward/sign");
        }

        /// <summary>
        /// 原神签到Referer
        /// </summary>
        /// <param name="act_id"></param>
        /// <returns></returns>
        public static string getYSSignReferer(string act_id)
        {
            return string.Format("https://webstatic.mihoyo.com/bbs/event/signin-ys/index.html?bbs_auth_required=true&act_id={0}&utm_source=bbs&utm_medium=mys&utm_campaign=icon", act_id);
        }

        /// <summary>
        /// 米游社签到地址
        /// </summary>
        /// <param name="act_id"></param>
        /// <returns></returns>
        public static string getMysSignUrl()
        {
            return string.Format("https://bbs-api.mihoyo.com/apihub/sapi/signIn");
        }

        /// <summary>
        /// 米游社帖子推送地址
        /// </summary>
        /// <param name="act_id"></param>
        /// <returns></returns>
        public static string getMysExportPostUrl(int gameId, int size)
        {
            return string.Format("https://api-takumi.mihoyo.com/post/wapi/export/post?game_id={0}&size={1}&scene_id=1", gameId, size);
        }

        /// <summary>
        /// 米游社帖子详情url(浏览帖子)
        /// </summary>
        /// <returns></returns>
        public static string getMysPostFullUrl(string postId)
        {
            return string.Format("https://bbs-api.mihoyo.com/post/api/getPostFull?post_id={0}", postId);
        }

        /// <summary>
        /// 米游社点赞url(点赞)
        /// </summary>
        /// <returns></returns>
        public static string getMysUpvotePostUrl()
        {
            return string.Format("https://bbs-api.mihoyo.com/apihub/sapi/upvotePost");
        }

        /// <summary>
        /// 获取米游社帖子分享链接(分享体诶中)
        /// </summary>
        /// <param name="postId"></param>
        /// <returns></returns>
        public static string getMysShareConfUrl(string postId)
        {
            return string.Format("https://bbs-api.mihoyo.com/apihub/api/getShareConf?entity_id={0}&entity_type=1", postId);
        }

        /// <summary>
        /// 通过游戏业务编码获取角色信息
        /// </summary>
        /// <param name="postId"></param>
        /// <returns></returns>
        public static string getMysUserGameRolesUrl(string gameBiz)
        {
            return string.Format("https://api-takumi.mihoyo.com/binding/api/getUserGameRolesByCookie?game_biz={0}", gameBiz);
        }

        /// <summary>
        /// 获取stoken
        /// </summary>
        /// <param name="postId"></param>
        /// <returns></returns>
        public static string getMysMultiTokenUrl(string loginTicket, string uid)
        {
            return string.Format("https://api-takumi.mihoyo.com/auth/api/getMultiTokenByLoginTicket?login_ticket={0}&token_types=3&uid={1}", loginTicket, uid);
        }

        /// <summary>
        /// 通过米游社用户信息
        /// </summary>
        /// <param name="postId"></param>
        /// <returns></returns>
        public static string getMysUserFullInfoUrl(string uid)
        {
            return string.Format("https://bbs-api.mihoyo.com/user/api/getUserFullInfo?uid={0}", uid);
        }


    }
}
