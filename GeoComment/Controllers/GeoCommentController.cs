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
        [Route("/test/reset-db")]
        public async Task<IActionResult> ResetDatabase()
        {
            await _databaseHandler.RecreateDb();

            return Ok();
        }

        [HttpPost]
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
        [Route("{id:int}")]
        public ActionResult<CommentResult> GetCommentFromId(int id)
        {
            if (id < 1 || id > _ctx.Comments.Count()) return NotFound();

            var comment = _ctx.Comments.First(c => c.Id == id);

            return Ok(comment);
        }

        [HttpGet]
        public ActionResult<Array> GetCommentsWithingRange(
            int minLon, int maxLon, int minLat, int maxLat)
        {
            var comments = _ctx.Comments.Where(c =>
                c.Longitude >= minLon && c.Longitude <= maxLon &&
                c.Latitude >= minLat && c.Latitude <= maxLat).ToArray();

            return Ok(comments);
        }
    }
}
