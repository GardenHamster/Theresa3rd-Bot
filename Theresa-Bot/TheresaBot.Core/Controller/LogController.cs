using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TheresaBot.Core.Helper;
using TheresaBot.Core.Model.Log;
using TheresaBot.Core.Model.Result;
using TheresaBot.Core.Model.VO;

namespace TheresaBot.Core.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class LogController : BaseController
    {

        [HttpGet]
        [Authorize]
        [Route("list")]
        public ApiResult ListLogRecords()
        {
            var records = LogHelper.GetRecords();
            var recordVos = ToRecordVos(records);
            return ApiResult.Success(recordVos);
        }

        [HttpGet]
        [Authorize]
        [Route("pull")]
        public ApiResult PullLogRecords(long lastAt)
        {
            var records = LogHelper.GetRecords(lastAt);
            var recordVos = ToRecordVos(records);
            return ApiResult.Success(recordVos);
        }

        private LogRecordVo ToRecordVo(LogRecord record)
        {
            var remind = record.Remind ?? string.Empty;
            var message = record.Exception?.Message ?? string.Empty;
            var innerMessage = record.Exception?.InnerException?.Message ?? string.Empty;
            var stackTrace = record.Exception?.StackTrace ?? string.Empty;
            return new LogRecordVo(remind, message, innerMessage, stackTrace, record.CreateAt, record.Level);
        }

        private List<LogRecordVo> ToRecordVos(List<LogRecord> records)
        {
            return records.Select(o => ToRecordVo(o)).ToList();
        }

    }
}
