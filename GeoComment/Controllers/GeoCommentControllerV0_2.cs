using GeoComment.Data;
using GeoComment.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GeoComment.Controllers
{
    [Route("api/geo-comments")]
    [ApiController]
    public class GeoCommentControllerV0_2 : ControllerBase
    {
        private readonly GeoCommentDbContext _ctx;

        public GeoCommentControllerV0_2(GeoCommentDbContext ctx)
        {
            _ctx = ctx;
        }

        [HttpPost]
        [Authorize]
        [ApiVersion("0.2")]
        public ActionResult<CommentResult> PostNewComment(CommentInput input)
        {
            var newComment = new CommentResult()
            {
                Message = input.Message,
                Author = input.Author,
                Longitude = input.Longitude,
                Latitude = input.Latitude
            };

            _ctx.Comments.Add(newComment);
            _ctx.SaveChanges();

            return Created("", newComment);
        }
    }
}
