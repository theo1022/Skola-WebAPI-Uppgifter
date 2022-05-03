﻿using GeoComment.Data;
using GeoComment.Models;
using GeoComment.Services;
using Microsoft.AspNetCore.Mvc;

namespace GeoComment.Controllers
{
    [Route("api/geo-comments")]
    [ApiController]
    public class GeoCommentController : ControllerBase
    {
        private readonly GeoCommentDbContext _ctx;
        private readonly DatabaseHandler _databaseHandler;

        public GeoCommentController(GeoCommentDbContext ctx, DatabaseHandler databaseHandler)
        {
            _ctx = ctx;
            _databaseHandler = databaseHandler;
        }

        [HttpGet]
        [ApiVersion("0.1")]
        [ApiVersion("0.2")]
        [Route("/test/reset-db")]
        public async Task<IActionResult> ResetDatabase()
        {
            await _databaseHandler.RecreateDb();

            return Ok();
        }

        [HttpPost]
        [ApiVersion("0.1")]
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

        [HttpGet]
        [ApiVersion("0.1")]
        [Route("{id:int}")]
        public ActionResult<CommentResult> GetCommentFromId(int id)
        {
            if (id < 1 || id > _ctx.Comments.Count()) return NotFound();

            var comment = _ctx.Comments.First(c => c.Id == id);

            return Ok(comment);
        }

        [HttpGet]
        [ApiVersion("0.1")]
        public ActionResult<Array> GetCommentsWithingRange(
            int? minLon, int? maxLon, int? minLat, int? maxLat)
        {
            if (minLon == null || maxLon == null ||
                minLat == null || maxLat == null) return BadRequest();

            var comments = _ctx.Comments.Where(c =>
                c.Longitude >= minLon && c.Longitude <= maxLon &&
                c.Latitude >= minLat && c.Latitude <= maxLat).ToArray();

            return Ok(comments);
        }
    }
}
