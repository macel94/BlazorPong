using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;

namespace BlazorPong.Shared
{
    public class GameObject
    {
        [JsonProperty("left")]
        public double Left { get; set; }
        public string LeftPx => $"{(int)this.Left}px";

        [JsonProperty("top")]
        public double Top { get; set; }
        public string TopPx => $"{(int)this.Top}px";

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonIgnore]
        public string LastUpdatedBy { get; set; }

        [JsonIgnore]
        public bool Moved { get; set; }
        public bool Draggable { get; set; }
    }
}