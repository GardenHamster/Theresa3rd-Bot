using System;
using Theresa3rd_Bot.Type;

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


        /// <summary>
        /// Saucenao搜索地址
        /// </summary>
        public static readonly string SaucenaoUrl = "https://saucenao.com/search.php";

        /// <summary>
        /// Ascii2d搜索地址
        /// </summary>
        public static readonly string Ascii2dUrl = "https://ascii2d.net/search/uri";

        /// <summary>
        /// 默认pixiv图片代理地址
        /// </summary>
        public static readonly string PixivImgProxyUrl = "https://pixiv.lolisuki.cn";

        /*---------------------------------------------------------------pixiv-----------------------------------------------------------------------*/

        /// <summary>
        /// 返回pixiv搜索路径
        /// </summary>
        /// <param name="searchWord"></param>
        /// <returns></returns>
        public static string getPixivSearchUrl(string searchWord, int pageNo, bool isMatchAll, bool includeR18)
        {
            string keyword = System.Web.HttpUtility.UrlEncode(searchWord).Replace("+", "%20");
            string mode = includeR18 ? "all" : "safe";//all,safe,r18;
            string s_mode = isMatchAll ? "s_tag_full" : "s_tag";//s_tag_full,s_tag,s_tc
            return $"{PixivHomeUrl}/ajax/search/illustrations/{keyword}?word={keyword}&order=date_d&mode={mode}&p={pageNo}&s_mode={s_mode}&type=illust_and_ugoira&lang=zh";
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
        /// 关注列表url
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        public static string getPixivFollowUrl(long loginId, int offset, int limit)
        {
            return $"{PixivHomeUrl}/ajax/user/{loginId}/following?offset={offset}&limit={limit}&rest=show&tag=&lang=zh";
        }

        /// <summary>
        /// 收藏列表url
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        public static string getPixivBookmarkUrl(long loginId, int offset, int limit)
        {
            return $"{PixivHomeUrl}/ajax/user/{loginId}/illusts/bookmarks?tag=&offset={offset}&limit={limit}&rest=show&lang=zh";
        }

        /// <summary>
        /// 关注用户最新作品列表url
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        public static string getPixivFollowLatestUrl(int page)
        {
            return $"{PixivHomeUrl}/ajax/follow_latest/illust?p={page}&mode=all&lang=zh";
        }

        /// <summary>
        /// 返回pixiv搜索referer
        /// </summary>
        /// <param name="searchWord"></param>
        /// <returns></returns>
        public static string getPixivSearchReferer()
        {
            return PixivHomeUrl;
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

        /// <summary>
        /// 关注列表Referer
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        public static string getPixivFollowReferer(long loginId)
        {
            return $"{PixivHomeUrl}/users/{loginId}/following";
        }

        /// <summary>
        /// 收藏列表Referer
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        public static string getPixivBookmarkReferer(long loginId)
        {
            return $"{PixivHomeUrl}/users/{loginId}/bookmarks/artworks";
        }

        /// <summary>
        /// 关注用户最新作品列表Referer
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        public static string getPixivFollowLatestReferer()
        {
            return $"{PixivHomeUrl}/bookmark_new_illust.php";
        }

        /*---------------------------------------------------------------Lolicon-----------------------------------------------------------------------*/

        /// <summary>
        /// Lolicon Api 地址
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        public static string getLoliconApiV2Url()
        {
            return "https://api.lolicon.app/setu/v2";
        }

        /*---------------------------------------------------------------Lolisuki-----------------------------------------------------------------------*/

        /// <summary>
        /// Lolisuki Api 地址
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        public static string getLolisukiApiUrl()
        {
            return "https://lolisuki.cc/api/setu/v1";
        }

        /*---------------------------------------------------------------米游社-----------------------------------------------------------------------*/

        /// <summary>
        /// 米游社用户帖子列表地址
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="gids"></param>
        /// <returns></returns>
        public static string getMysPostListUrl(string userId, int size)
        {
            return $"https://api-takumi.mihoyo.com/post/wapi/userPost?uid={userId}&size={size}";
        }

        /// <summary>
        /// 米游社用户帖子详情地址
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static string getMysArticleUrl(string articleId)
        {
            return $"https://bbs.mihoyo.com/bh3/article/{articleId}";
        }

        /// <summary>
        /// 米游社用户详细信息
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static string getMysUserInfoUrl(string userId)
        {
            return $"https://bbs-api.mihoyo.com/user/wapi/getUserFullInfo?uid={userId}";
        }

        /// <summary>
        /// 米游社用户帖子列表地址
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="gids"></param>
        /// <returns></returns>
        public static string getMysPostListRefer(string userId)
        {
            return $"https://bbs.mihoyo.com/bh3/accountCenter/postList?id={userId}";
        }

        /// <summary>
        /// 米游社用户详细信息
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static string getMysUserInfoRefer(string userId)
        {
            return $"https://bbs.mihoyo.com/bh3/accountCenter/postList?id={userId}";
        }


        /*---------------------------------------------------------------BiliBili-----------------------------------------------------------------------*/

        /// <summary>
        /// bilibili用户动态地址
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static string getBLBLUserPostList(string userId)
        {
            return $"https://api.vc.bilibili.com/dynamic_svr/v1/dynamic_svr/space_history?visitor_uid=0&host_uid={userId}&offset_dynamic_id=0&need_top=1";
        }

        /// <summary>
        /// bilibili用户动态refer
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static string getBLBLUserPostReferer(string userId)
        {
            return $"https://space.bilibili.com/{userId}/dynamic";
        }

        /// <summary>
        /// bilibili用户动态地址
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static string getBLBLUserInfo(string userId)
        {
            return $"https://api.bilibili.com/x/space/acc/info?mid={userId}&jsonp=jsonp";
        }

        /// <summary>
        /// bilibili用户动态referer
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static string getBLBLUserReferer(string userId)
        {
            return $"https://space.bilibili.com/{userId}";
        }

        /// <summary>
        /// up主投稿视频地址
        /// </summary>
        /// <param name="videoCode"></param>
        /// <returns></returns>
        public static string getBLBLVideoUrl(string videoCode)
        {
            return $"https://www.bilibili.com/video/{videoCode}";
        }

        /// <summary>
        /// up主发布文章地址
        /// </summary>
        /// <param name="articleId"></param>
        /// <returns></returns>
        public static string getBLBLArticleUrl(string articleId)
        {
            return $"https://t.bilibili.com/{articleId}";
        }

        /// <summary>
        /// up主发布动态地址
        /// </summary>
        /// <param name="articleId"></param>
        /// <returns></returns>
        public static string getBLBLDynamicUrl(string dynamicId)
        {
            return $"https://t.bilibili.com/{dynamicId}";
        }

        /// <summary>
        /// up主动态主页地址
        /// </summary>
        /// <param name="articleId"></param>
        /// <returns></returns>
        public static string getBLBLSpaceUrl(string userId)
        {
            return $"https://space.bilibili.com/{userId}/dynamic";
        }



    }
}
