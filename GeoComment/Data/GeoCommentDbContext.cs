using Microsoft.EntityFrameworkCore;

namespace GeoComment.Data
{
    public class GeoCommentDbContext : DbContext
    {
        public GeoCommentDbContext(DbContextOptions options) : base(options) { }
    }
}
