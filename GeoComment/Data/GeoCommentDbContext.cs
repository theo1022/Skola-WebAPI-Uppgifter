using GeoComment.Models;
using Microsoft.EntityFrameworkCore;

namespace GeoComment.Data
{
    public class GeoCommentDbContext : DbContext
    {
        public GeoCommentDbContext(DbContextOptions options) : base(options) { }

        public DbSet<CommentResult> Comments { get; set; }
    }
}
