using Newtonsoft.Json;

namespace BlazorPong.Shared
{
    public class GameObject
    {
        [JsonProperty("left")] public double Left { get; set; }

        [JsonProperty("top")] public double Top { get; set; }

        [JsonProperty("id")] public string Id { get; set; }

        [JsonIgnore] public string LeftPx => $"{(int)Left}px";

        [JsonIgnore] public string TopPx => $"{(int) Top}px";

        [JsonIgnore] public string LastUpdatedBy { get; set; }

        [JsonIgnore] public bool Moved { get; set; }

        [JsonIgnore] public bool Draggable { get; set; }

        [JsonIgnore] public int Width { get; set; }

        [JsonIgnore] public int Height { get; set; }
    }
}