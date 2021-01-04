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
using System.Linq;
using Infrastructure.Auth;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LikesController : ControllerBase
    {
        private readonly IUserFilmManager _userFilmManager;
        private readonly ISelectionManager _selectionManager;
        private readonly IMapper _mapper;

        public LikesController(IUserFilmManager userFilmManager, ISelectionManager selectionManager, IMapper mapper)
        {
            _userFilmManager = userFilmManager;
            _selectionManager = selectionManager;
            _mapper = mapper;
        }

        // GET: api/Likes
        [HttpGet]
        public IList<LikeViewModel> GetLikes()
        {
            var likes = _userFilmManager.GetLikes();
            var result = _mapper.Map<IList<UserFilm>, IList<LikeViewModel>>(likes);
            return result;
        }

        // GET: api/Likes/5
        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<LikeViewModel>> GetLike(Guid id)
        {
            var like = _userFilmManager.GetLikeById(id);
            var result = _mapper.Map<UserFilm, LikeViewModel>(like);
            return result;
        }

        // GET: api/Likes/ByFilm/5
        [HttpGet("ByFilm/{filmId}")]
        [Authorize]
        public LikeViewModel GetLikeByFilm(Guid filmId)
        {
            var like = _userFilmManager.GetLikeByFilm(HttpContext.User.Identity.Name, filmId);
            var result = _mapper.Map<UserFilm, LikeViewModel>(like);
            return result;
        }

        // PUT: api/Films/5
        [HttpPut("{id}")]
        public async Task<ActionResult<bool>> LikeOrDislike(Guid id, [FromBody] LikeOrDislikeRequest request)
        {
            if (id != request.FilmId)
                return BadRequest();

            await _selectionManager.RemoveAllSelectionsByUser(request.UserId);
            var film = await _userFilmManager.LikeOrDislike(request.FilmId, request.UserId, request.LikeOrDislike);
            return Ok(film);
        }

        // DELETE: api/Likes/ByFilm/5
        [HttpDelete("ByFilm/{filmId}")]
        [Authorize]
        public async Task<ActionResult> DeleteLikeByFilm(Guid filmId)
        {
            var userId = HttpContext.User.Claims.FirstOrDefault(c => c.Type == AuthExtensions.UserId).Value;
            await _selectionManager.RemoveAllSelectionsByUser(userId);
            var deletingResult = await _userFilmManager.DeleteByFilmAsync(HttpContext.User.Identity.Name, filmId);
            if (deletingResult.IsSuccess)
                return Ok();
            else
                return BadRequest();
        }

        // DELETE: api/Likes/5
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<ActionResult> DeleteLike(Guid id)
        {
            var deletingResult = await _userFilmManager.DeleteAsync(id);
            await _selectionManager.RemoveAllSelectionsByUser(deletingResult.Data.UserId);
            if (deletingResult.IsSuccess)
                return Ok();
            else
                return BadRequest();
        }
    }
}
