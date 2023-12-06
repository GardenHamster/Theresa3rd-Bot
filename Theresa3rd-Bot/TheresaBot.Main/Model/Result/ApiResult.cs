using TheresaBot.Main.Common;

namespace TheresaBot.Main.Model.Result
{
    public class ApiResult
    {
        public bool Error { get; init; }
        public int Code { get; init; }
        public string Message { get; init; }
        public object Data { get; init; }

        public ApiResult(bool success, int code, string message = "", object data = null)
        {
            Error = success == false;
            Code = code;
            Message = message;
            Data = data;
        }

        public static ApiResult Success()
        {
            return new ApiResult(true, ResultCode.Success, "ok", null);
        }

        public static ApiResult Success(string message)
        {
            return new ApiResult(true, ResultCode.Success, message, null);
        }

        public static ApiResult Success(object data)
        {
            return new ApiResult(true, ResultCode.Success, "ok", data);
        }

        public static ApiResult Success(string message, object data)
        {
            return new ApiResult(true, ResultCode.Success, message, data);
        }

        public static ApiResult Fail(string message)
        {
            return new ApiResult(false, ResultCode.Error, message);
        }

        public static ApiResult Fail(int code, string message)
        {
            return new ApiResult(false, code, message);
        }

        /// <summary>
        /// 未登录
        /// </summary>
        public static ApiResult NoLogin => Fail(ResultCode.NoLogin, "身份验证过期，请重新登录");

        /// <summary>
        /// 参数错误
        /// </summary>
        public static ApiResult ParamError => Fail(ResultCode.ParamError, "参数错误");

    }
}
