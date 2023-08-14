using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Monday_Tradeskola.Models
{
    public class Board
    {
        [JsonProperty(PropertyName = "id")]
        public double Id { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "items")]
        public IList<Item> Items { get; set; }

    }
}