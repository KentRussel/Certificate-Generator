using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Monday_Tradeskola.Models
{
    public class ItemColumnValue
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "title")]
        public string Title { get; set; }

        [JsonProperty(PropertyName = "value")]
        public dynamic Value { get; set; }


        [JsonProperty(PropertyName = "text")]
        public dynamic Text { get; set; }
    }
}