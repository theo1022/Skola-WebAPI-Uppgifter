namespace GeoComment.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string Message { get; set; }
        public string Author { get; set; }
        public int Longitude { get; set; }
        public int Latitude { get; set; }
    }
}
