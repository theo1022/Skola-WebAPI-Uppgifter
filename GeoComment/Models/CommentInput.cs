namespace GeoComment.Models
{
    public class CommentInput
    {
        public string Message { get; set; }
        public string Author { get; set; }
        public int Longitude { get; set; }
        public int Latitude { get; set; }
    }
}
