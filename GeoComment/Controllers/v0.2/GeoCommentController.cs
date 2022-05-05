using GeoComment.Data;
using GeoComment.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GeoComment.Controllers.v0._2
{
    #region Input comment format - DTO's
    public class NewComment
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
    #endregion

    #region Return comment format - DTO's
    public class ReturnComment
    {
        public int id { get; set; }
        public ReturnCommentBody Body { get; set; }
        public int longitude { get; set; }
        public int latitude { get; set; }
    }
    public class ReturnCommentBody
    {
        public string author { get; set; }
        public string title { get; set; }
        public string message { get; set; }

    }
    #endregion

    [Route("api/geo-comments")]
    [ApiController]
    public class GeoCommentController : ControllerBase
    {
        private readonly GeoCommentDbContext _ctx;
        private readonly UserManager<User> _userManager;

        public GeoCommentController(GeoCommentDbContext ctx, UserManager<User> userManager)
        {
            _ctx = ctx;
            _userManager = userManager;
        }

        #region Controller Utility methods
        /// <summary>
        /// Converts a <see cref="Comment"/> received from the database to a format meant as output to user
        /// </summary>
        /// <param name="rawComment"></param>
        /// <returns><see cref="ReturnComment"/></returns>
        private ReturnComment BuildReturnComment(Comment rawComment)
        {
            var comment = new ReturnComment()
            {
                id = rawComment.Id,
                longitude = rawComment.Longitude,
                latitude = rawComment.Latitude,
                Body = new ReturnCommentBody()
                {
                    author = rawComment.Author,
                    title = rawComment.Title,
                    message = rawComment.Message
                }
            };

            return comment;
        }
        /// <summary>
        /// Converts <see cref="IQueryable"/>&lt;<see cref="Comment"/>&gt; received from the database to a format meant as output to user
        /// </summary>
        /// <param name="rawComments"></param>
        /// <returns><see cref="Array"/>&lt;<see cref="ReturnComment"/>&gt;</returns>
        private Array GetArrayOfReturnComments(IQueryable<Comment> rawComments)
        {
            var returnComments = new List<ReturnComment>();

            foreach (var rawComment in rawComments)
            {
                var comment = BuildReturnComment(rawComment);

                returnComments.Add(comment);
            }

            return returnComments.ToArray();
        }
        #endregion

        /// <summary>
        /// Converts a <see cref="NewComment"/> to be stored in database returns a format meant as output to user
        /// </summary>
        /// <param name="inputBody"></param>
        /// <returns><see cref="CreatedResult"/>(<see cref="ReturnComment"/>)</returns>
        [HttpPost]
        [Authorize]
        [ApiVersion("0.2")]
        public async Task<ActionResult<ReturnComment>> PostNewComment(NewComment inputBody)
        {
            var user = await _userManager.GetUserAsync(User);

            var newComment = new Comment()
            {
                Title = inputBody.Body.Title,
                Message = inputBody.Body.Message,
                Author = user.UserName,
                Longitude = inputBody.Longitude,
                Latitude = inputBody.Latitude
            };

            await _ctx.Comments.AddAsync(newComment);
            await _ctx.SaveChangesAsync();

            var createdComment = await _ctx.Comments.FirstAsync(c =>
                    c.Author == newComment.Author &&
                    c.Title == newComment.Title &&
                    c.Message == newComment.Message &&
                    c.Longitude == newComment.Longitude &&
                    c.Latitude == newComment.Latitude);

            var returnComment = BuildReturnComment(createdComment);

            return Created("", returnComment);
        }

        /// <summary>
        /// Finds the comment with the same id as the given path of the uri and returns it as a format meant as output to user
        /// </summary>
        /// <param name="id"></param>
        /// <returns><see cref="ReturnComment"/> if comments exist, or <see cref="NotFoundResult"/> if no comment is found</returns>
        [HttpGet]
        [ApiVersion("0.2")]
        [Route("{id:int}")]
        public ActionResult<ReturnComment> GetCommentFromId(int id)
        {
            if (id < 1 || id > _ctx.Comments.Count()) return NotFound();

            var rawComment = _ctx.Comments.First(c => c.Id == id);

            var comment = BuildReturnComment(rawComment);

            return Ok(comment);
        }

        /// <summary>
        /// Finds all comments made by the username in the given path and returns them as a format meant as output to user
        /// </summary>
        /// <param name="username"></param>
        /// <returns><see cref="Array"/>&lt;<see cref="ReturnComment"/>&gt; if comments exist, or <see cref="NotFoundResult"/> if no comment is found</returns>
        [HttpGet]
        [ApiVersion("0.2")]
        [Route("{username}")]
        public ActionResult<Array> GetCommentFromUsername(string username)
        {
            var rawComments = _ctx.Comments.Where(c => c.Author == username);

            if (rawComments.Count() == 0) return NotFound();

            var returnComments = GetArrayOfReturnComments(rawComments);

            return Ok(returnComments);
        }

        /// <summary>
        /// Finds all comments that fall within the range of the parameters given as queries and returns them as a format meant as output to user
        /// </summary>
        /// <param name="minLon"></param>
        /// <param name="maxLon"></param>
        /// <param name="minLat"></param>
        /// <param name="maxLat"></param>
        /// <returns><see cref="Array"/>&lt;<see cref="ReturnComment"/>&gt; if succesfull, or <see cref="BadRequestResult"/> if any of the queries is not specified</returns>
        [HttpGet]
        [ApiVersion("0.2")]
        public ActionResult<Array> GetCommentsWithinRange(
            int? minLon, int? maxLon, int? minLat, int? maxLat)
        {
            if (minLon == null || maxLon == null ||
                minLat == null || maxLat == null) return BadRequest();

            var rawComments = _ctx.Comments.Where(c =>
                c.Longitude >= minLon && c.Longitude <= maxLon &&
                c.Latitude >= minLat && c.Latitude <= maxLat);

            var returnComments = GetArrayOfReturnComments(rawComments);

            return Ok(returnComments);
        }
    }
}
