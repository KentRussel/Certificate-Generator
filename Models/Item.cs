using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Monday_Tradeskola.Models
{
    public class Item
    {
        [JsonProperty(PropertyName = "pulseId")]
        public double pulseId { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "group")]
        public ItemGroup Group { get; set; }

        [JsonProperty(PropertyName = "column_values")]
        public List<ItemColumnValue> ItemColumnValues { get; set; }

        public ItemDetails ItemDetails { get; set; }

        [JsonProperty(PropertyName = "parentItemId")]
        public string parentID { get; set; }

        [JsonProperty(PropertyName = "assets")]
        public List<AssetDetails> Assets { get; set; }




    }

    public class AssetDetails
    {
        [JsonProperty(PropertyName = "public_url")]
        public string AssetURl { get; set; }

        [JsonProperty(PropertyName = "id")]
        public string AssetId { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string AssetName { get; set; }
    }

    public class PulseJson
    {
        [JsonProperty("event")]
        public Pulse PulseDetails { get; set; }

        [JsonProperty("error_message")]
        public Pulse errmsg { get; set; }
    }

    public class Pulse
    {
        [JsonProperty("pulseId")]
        public string PulseID { get; set; }

        [JsonProperty("pulseName")]
        public string PulseName { get; set; }

        [JsonProperty("boardId")]
        public string PulseBoard { get; set; }


        [JsonProperty("columnId")]
        public string PulseCol { get; set; }

        [JsonProperty("parentItemId")]
        public string parentID { get; set; }

    }

    public class Duplicate
    {
        [JsonProperty("duplicate_item")]
        public DuplicatedItem dupItem { get; set; }
    }

    public class DuplicatedItem
    {
        [JsonProperty("id")]
        public string itemID { get; set; }
        [JsonProperty("board")]
        public DupBoard board { get; set; }
    }
    public class DupBoard
    {
        [JsonProperty("id")]
        public string boardID { get; set; }
    }

    public class ItemDetails
    {
        [JsonProperty("pulseId")]
        public string PulseID { get; set; }

        [JsonProperty("pulseName")]
        public string PulseName { get; set; }
    }
    //linkedPulseIds
    public class PeopleValue
    {
        [JsonProperty("personsAndTeams")]
        public Linked details { get; set; }
    }

    public class Linked
    {
        [JsonProperty("id")]
        public string peopleID { get; set; }
    }

    //linkedPulseIds
    public class LinkedPulse
    {
        [JsonProperty("linkedPulseIds")]
        public LinkedID LinkedDetails { get; set; }
    }

    public class LinkedID
    {
        [JsonProperty("linkedPulseId")]
        public string PulseID { get; set; }
    }

    public class SubItem
    {
        [JsonProperty("linkedPulseIds")]
        public List<Sub> data { get; set; }
    }

    public class Sub
    {
        [JsonProperty("linkedPulseId")]
        public string linkedPulseId { get; set; }
    }

}