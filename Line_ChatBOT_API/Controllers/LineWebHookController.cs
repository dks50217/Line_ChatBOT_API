using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using isRock.LineBot;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace isRock.Template 
{
    public class LineWebHookController : LineWebHookControllerBase
    {
        private string _ChannelAccessToken = String.Empty;
        private string _UserId = String.Empty;
        private readonly IConfiguration _config;

        public LineWebHookController(IConfiguration config)
        {
            _config = config;
            _ChannelAccessToken = _config.GetValue<string>("LineBot:Token");
            _UserId = _config.GetValue<string>("LineBot:UserID");
        }

        [Route("api/LineBotWebHook")]
        [HttpPost]
        public IActionResult POST()
        {
            try
            {
                //設定ChannelAccessToken
                this.ChannelAccessToken = _ChannelAccessToken;
                //取得Line Event
                var LineEvent = this.ReceivedMessage.events.FirstOrDefault();
                //配合Line verify 
                if (LineEvent.replyToken == "00000000000000000000000000000000") return Ok();
                var responseMsg = "";
                //準備回覆訊息
                if (LineEvent.type.ToLower() == "message" && LineEvent.message.type == "text")
                    responseMsg = $"你說了: {LineEvent.message.text}";
                else if (LineEvent.type.ToLower() == "message")
                    responseMsg = $"收到 event : {LineEvent.type} type: {LineEvent.message.type} ";
                else
                    responseMsg = $"收到 event : {LineEvent.type} ";
                //回覆訊息
                this.ReplyMessage(LineEvent.replyToken, responseMsg);
                //response OK
                return Ok();
            }
            catch (Exception ex)
            {
                //回覆訊息
                this.PushMessage(_UserId, "發生錯誤:\n" + ex.Message);
                //response OK
                return Ok();
            }
        }
    }
}