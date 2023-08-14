using Microsoft.AspNet.WebHooks;
using Microsoft.AspNetCore.Mvc;
using Monday_Tradeskola.Controllers;
using Monday_Tradeskola.Helper;
using Monday_Tradeskola.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Monday_Tradeskola.Handlers
{
    public class GenericJsonWebHookHandler : WebHookHandler
    {
        public GenericJsonWebHookHandler()
        {
            this.Receiver = "genericjson";
        }

        [IgnoreAntiforgeryToken]
        public override Task ExecuteAsync(string generator, WebHookHandlerContext context)
        {

            var challenge = context.Data.ToString();

            if (challenge.Contains("challenge"))
            {
                context.Response = context.Request.CreateResponse();
                context.Response.Content = new StringContent(challenge);
                context.Response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
                context.Response.StatusCode = HttpStatusCode.OK;
                context.Response.ReasonPhrase = "OK";
                return Task.FromResult(true);
            }
            else
            {
                JObject data = context.GetDataOrDefault<JObject>();
                var json = data == null ? "" : data.ToString();

                PulseJson pulse = JsonConvert.DeserializeObject<PulseJson>(json);

                var pulseID = pulse.PulseDetails.PulseID.ToString();
                var pulseName = pulse.PulseDetails.PulseName.ToString();
                var boardID = pulse.PulseDetails.PulseBoard.ToString();

                if (context.Id == "z")
                {
                    string apiToken = "";
                    string apiRoot = "https://api.monday.com/v2";

                    using (var client = new MondayClient(apiToken, apiRoot))
                    {
                        var service = new MondayService(client);

                        List<Item> itemDetails = service.GetItemDetails(pulseID);

                        if (itemDetails.Count > 0)
                        {
                            service.GeneratePDF(itemDetails);
                        }
                    }
                }
            }
            return Task.FromResult(true);
        }
    }
}