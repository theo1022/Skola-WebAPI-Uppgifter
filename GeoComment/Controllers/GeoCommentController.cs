using GeoComment.Services;
using Microsoft.AspNetCore.Mvc;

namespace GeoComment.Controllers
{
    [Route("test")]
    [ApiController]
    public class GeoCommentController : ControllerBase
    {
        private readonly DatabaseHandler _ctx;

        public GeoCommentController(DatabaseHandler ctx)
        {
            _ctx = ctx;
        }

        [HttpGet]
        [Route("reset-db")]
        public async Task<IActionResult> ResetDatabase()
        {
            await _ctx.RecreateDb();

            return Ok();
        }
    }
}
