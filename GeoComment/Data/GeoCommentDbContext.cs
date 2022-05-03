using GeoComment.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace GeoComment.Data
{
    public class GeoCommentDbContext : IdentityDbContext<User>
    {
        public GeoCommentDbContext(DbContextOptions options) : base(options) { }

        public DbSet<CommentResult> Comments { get; set; }
    }
}
