using GeoComment.Data;
using GeoComment.Services;
using Microsoft.AspNetCore.Mvc;

namespace GeoComment.Controllers
{
    [Route("test")]
    [ApiController]
    public class DatabaseController : ControllerBase
    {
        private readonly GeoCommentDbContext _ctx;
        private readonly DatabaseHandler _databaseHandler;

        public DatabaseController(GeoCommentDbContext ctx, DatabaseHandler databaseHandler)
        {
            _ctx = ctx;
            _databaseHandler = databaseHandler;
        }

        [HttpGet]
        [ApiVersion("0.1")]
        [ApiVersion("0.2")]
        [Route("reset-db")]
        public async Task<IActionResult> ResetDatabase()
        {
            await _databaseHandler.RecreateDb();

            return Ok();
        }
    }
}
