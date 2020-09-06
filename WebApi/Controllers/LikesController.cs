using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Core.Models;
using Services.Managers.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApi.ViewModels;
using WebApi.ViewModels.RequestModels;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LikesController : ControllerBase
    {
        private readonly IUserFilmManager _likeManager;
        private readonly IMapper _mapper;

        public LikesController(IUserFilmManager likeManager, IMapper mapper)
        {
            this._likeManager = likeManager;
            this._mapper = mapper;
        }

        // GET: api/Likes
        [HttpGet]
        public IList<LikeViewModel> GetLikes()
        {
            var likes = _likeManager.GetLikes();
            var result = _mapper.Map<IList<UserFilm>, IList<LikeViewModel>>(likes);
            return result;
        }

        // GET: api/Likes/5
        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<LikeViewModel>> GetLike(Guid id)
        {
            var like = _likeManager.GetLikeById(id);
            var result = _mapper.Map<UserFilm, LikeViewModel>(like);
            return result;
        }

        // GET: api/Likes/ByFilm/5
        [HttpGet("ByFilm/{filmId}")]
        [Authorize]
        public LikeViewModel GetLikeByFilm(Guid filmId)
        {
            var like = _likeManager.GetLikeByFilm(HttpContext.User.Identity.Name, filmId);
            var result = _mapper.Map<UserFilm, LikeViewModel>(like);
            return result;
        }

        // PUT: api/Films/5
        [HttpPut("{id}")]
        public async Task<ActionResult<bool>> LikeOrDislike(Guid id, [FromBody] LikeOrDislikeRequest request)
        {
            if (id != request.FilmId)
                return BadRequest();

            var film = await _likeManager.LikeOrDislike(request.FilmId, request.UserId, request.LikeOrDislike);
            return Ok(film);
        }

        // DELETE: api/Likes/ByFilm/5
        [HttpDelete("ByFilm/{filmId}")]
        [Authorize]
        public async Task<ActionResult> DeleteLikeByFilm(Guid filmId)
        {
            var deletingResult = await _likeManager.DeleteByFilmAsync(HttpContext.User.Identity.Name, filmId);
            if (deletingResult)
                return Ok();
            else
                return BadRequest();
        }

        // DELETE: api/Likes/5
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<ActionResult> DeleteLike(Guid id)
        {
            var deletingResult = await _likeManager.DeleteAsync(id);
            if (deletingResult)
                return Ok();
            else
                return BadRequest();
        }
    }
}
