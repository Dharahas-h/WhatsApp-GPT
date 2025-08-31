using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using WhatsApp_GPT.Models;
using WhatsApp_GPT.Services;

namespace WhatsApp_GPT.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WebHookController : ControllerBase
    {
        private readonly string Apptoken;
        private readonly OpenAIService OpenAIService;
        private readonly WhatsAppService WhatsAppService;

        public WebHookController(IConfiguration config, OpenAIService openAIService, WhatsAppService whatsAppService)
        {
            string? _apptoken = config["Apptoken"];

            if (whatsAppService == null) throw new ArgumentNullException(nameof(whatsAppService));
            if (openAIService == null) throw new ArgumentNullException(nameof(openAIService));
            if (_apptoken == null) throw new ArgumentNullException("Apptoken needs to be set in config");

            Apptoken = _apptoken;
            OpenAIService = openAIService;
            WhatsAppService = whatsAppService;
        }

        [HttpGet("/")]
        public IActionResult Home()
        {
            return Ok();
        }

        [HttpGet]
        public IActionResult VerifyWebhook([FromQuery(Name = "hub.mode")] string mode, [FromQuery(Name = "hub.challenge")] string challenge, [FromQuery(Name = "hub.verify_token")] string token)
        {
            if (mode == "subscribe" && token == Apptoken)
            {
                Ok(challenge);
            }
            return Forbid();
        }

        [HttpPost]
        public async Task<IActionResult> RecieveWebhook([FromBody] MessagePayLoad body)
        {
            var entry = body.Entry.FirstOrDefault()!;
            var change = entry.Changes.FirstOrDefault()!;

            var changeValue = change.Value;
            var phoneNoId = changeValue.Metadata.PhoneNumberId;

            var message = change.Value.Messages.FirstOrDefault()!;
            var messaging_product = change.Value.MessagingProduct;

            if (message == null) return Ok();

            var from = message.From;
            var msg = message.Text.Body;

            if (msg == string.Empty)
            {
                return Ok();
            }

            var promptResponse = await OpenAIService.GetCompletionAsync(msg);
            if (string.IsNullOrWhiteSpace(promptResponse))
                return StatusCode(500, "OpenAI failed to return a response");

            var success = await WhatsAppService.SendMessageAsync(phoneNoId, from, promptResponse);
            if (!success)
                return StatusCode(500, "Failed to send WhatsApp message");

            return Ok();
        }
    }
}
