using Newtonsoft.Json;

namespace BlazorPong.Shared
{
    public class GameObject
    {
        public double Left { get; set; }

        public double Top { get; set; }

        public string Id { get; set; }

        public string LeftPx => $"{(int)Left}px";

        public string TopPx => $"{(int) Top}px";

         public string LastUpdatedBy { get; set; }

        public bool Moved { get; set; }

        public bool Draggable { get; set; }

        public int Width { get; set; }

        public int Height { get; set; }
    }
}