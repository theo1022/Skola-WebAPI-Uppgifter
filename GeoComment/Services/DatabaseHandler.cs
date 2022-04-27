using GeoComment.Data;

namespace GeoComment.Services
{
    public class DatabaseHandler
    {
        private readonly GeoCommentDbContext _ctx;

        public DatabaseHandler(GeoCommentDbContext ctx)
        {
            _ctx = ctx;
        }

        public async Task RecreateDb()
        {
            await _ctx.Database.EnsureDeletedAsync();
            await _ctx.Database.EnsureCreatedAsync();
        }
    }
}
