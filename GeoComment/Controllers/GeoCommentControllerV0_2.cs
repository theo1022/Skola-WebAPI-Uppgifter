using GeoComment.Data;
using GeoComment.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GeoComment.Controllers
{
    public class NewCommentV0_2
    {
        public NewCommentBody Body { get; set; }
        public int Longitude { get; set; }
        public int Latitude { get; set; }
    }
    public class NewCommentBody
    {
        public string Title { get; set; }
        public string Message { get; set; }
    }

    public class ReturnComment
    {
        public int id { get; set; }
        public ReturnBody body { get; set; }
        public int longitude { get; set; }
        public int latitude { get; set; }
    }
    public class ReturnBody
    {
        public string author { get; set; }
        public string title { get; set; }
        public string message { get; set; }

    }

    [Route("api/geo-comments")]
    [ApiController]
    public class GeoCommentControllerV0_2 : ControllerBase
    {
        private readonly GeoCommentDbContext _ctx;
        private readonly UserManager<User> _userManager;

        public GeoCommentControllerV0_2(GeoCommentDbContext ctx, UserManager<User> userManager)
        {
            _ctx = ctx;
            _userManager = userManager;
        }

        [HttpPost]
        [Authorize]
        [ApiVersion("0.2")]
        public async Task<ActionResult<Comment>> PostNewComment(NewCommentV0_2 input)
        {
            var user = await _userManager.GetUserAsync(User);

            var newComment = new Comment()
            {
                Title = input.Body.Title,
                Message = input.Body.Message,
                Author = user.UserName,
                Longitude = input.Longitude,
                Latitude = input.Latitude
            };

            await _ctx.Comments.AddAsync(newComment);
            await _ctx.SaveChangesAsync();

            var createdComment = await _ctx.Comments.FirstAsync(c =>
                    c.Author == newComment.Author &&
                    c.Title == newComment.Title &&
                    c.Message == newComment.Message &&
                    c.Longitude == newComment.Longitude &&
                    c.Latitude == newComment.Latitude);

            var addedComment = new ReturnComment()
            {
                id = createdComment.Id,
                longitude = createdComment.Longitude,
                latitude = createdComment.Latitude,
                body = new ReturnBody()
                {
                    author = createdComment.Author,
                    title = createdComment.Title,
                    message = createdComment.Message,
                }
            };

            return Created("", addedComment);
        }

        [HttpGet]
        [ApiVersion("0.2")]
        [Route("{id:int}")]
        public ActionResult<Comment> GetCommentFromId(int id)
        {
            if (id < 1 || id > _ctx.Comments.Count()) return NotFound();

            var rawComment = _ctx.Comments.First(c => c.Id == id);

            var comment = new ReturnComment()
            {
                id = rawComment.Id,
                longitude = rawComment.Longitude,
                latitude = rawComment.Latitude,
                body = new ReturnBody()
                {
                    author = rawComment.Author,
                    title = rawComment.Title,
                    message = rawComment.Message
                }
            };

            return Ok(comment);
        }

        [HttpGet]
        [ApiVersion("0.2")]
        [Route("{username}")]
        public ActionResult<Array> GetCommentFromUsername(string username)
        {
            var rawComments = _ctx.Comments.Where(c => c.Author == username)
                .ToList();

            if (rawComments.Count == 0) return NotFound();

            var returnComments = new List<ReturnComment>();

            foreach (var rawComment in rawComments)
            {
                var comment = new ReturnComment()
                {
                    id = rawComment.Id,
                    longitude = rawComment.Longitude,
                    latitude = rawComment.Latitude,
                    body = new ReturnBody()
                    {
                        author = rawComment.Author,
                        title = rawComment.Title,
                        message = rawComment.Message
                    }
                };
                returnComments.Add(comment);
            }

            return Ok(returnComments.ToArray());
        }

        [HttpGet]
        [ApiVersion("0.2")]
        public ActionResult<Array> GetCommentsWithinRange(
            int? minLon, int? maxLon, int? minLat, int? maxLat)
        {
            if (minLon == null || maxLon == null ||
                minLat == null || maxLat == null) return BadRequest();

            var rawComments = _ctx.Comments.Where(c =>
                c.Longitude >= minLon && c.Longitude <= maxLon &&
                c.Latitude >= minLat && c.Latitude <= maxLat).ToList();

            var returnComments = new List<ReturnComment>();

            foreach (var rawComment in rawComments)
            {
                var comment = new ReturnComment()
                {
                    id = rawComment.Id,
                    longitude = rawComment.Longitude,
                    latitude = rawComment.Latitude,
                    body = new ReturnBody()
                    {
                        author = rawComment.Author,
                        title = rawComment.Title,
                        message = rawComment.Message
                    }
                };
                returnComments.Add(comment);
            }

            return Ok(returnComments.ToArray());
        }
    }
}
