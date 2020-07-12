using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

using Microsoft.AspNetCore.Cors;
using WebApi.ViewModels;
using System;
using Infrastructure.Managers.Interfaces;
using AutoMapper;
using Core.Models;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FilmsController : ControllerBase
    {
        private readonly IFilmManager _filmManager;
        private readonly IMapper _mapper;

        public FilmsController(IFilmManager filmManager, IMapper mapper)
        {
            _filmManager = filmManager;
            _mapper = mapper;
        }

        // GET: api/Films
        [HttpGet]
        public async Task<IList<FilmViewModel>> GetFilms()
        {
            var films = this._filmManager.GetAllFilms();
            var result = _mapper.Map<IList<Film>, IList<FilmViewModel>>(films);
            return result;
        }

        // GET: api/Films/5
        [HttpGet("{id}")]
        public async Task<ActionResult<FilmViewModel>> GetFilm(Guid id)
        {
            var film = this._filmManager.GetFilmById(id);

            if (film == null)
            {
                return NotFound();
            }

            var result = _mapper.Map<Film, FilmViewModel>(film);
            return result;
        }

        // GET: api/Films/Random
        [HttpGet("Random")]
        public async Task<IList<FilmViewModel>> GetRandomFilms()
        {
            var films = await this._filmManager.GetRandomShakedFilms();
            var result = _mapper.Map<IList<Film>, IList<FilmViewModel>>(films);
            return result;
        }

        [HttpGet("Specificity")]
        [Authorize]
        public async Task<IList<FilmViewModel>> GetSpecificityFilms()
        {
            var films = await this._filmManager.GetSpicifityFilms(HttpContext.User.Identity.Name);
            var result = _mapper.Map<IList<Film>, IList<FilmViewModel>>(films);
            return result;
        }

        // POST: api/Films
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<Film>> PostFilm([FromBody] FilmViewModel filmViewModel)
        {
            var film = _mapper.Map<FilmViewModel, Film>(filmViewModel);
            var newFilm = await _filmManager.CreateAsync(film);

            var result = _mapper.Map<Film, FilmViewModel>(newFilm);
            return Ok(result);
        }

        // PUT: api/Films/5
        [HttpPut("{id}")]
        [Authorize]
        public async Task<ActionResult<FilmViewModel>> PutFilm(Guid id, FilmViewModel filmViewModel)
        {
            if (id != filmViewModel.Id)
                return BadRequest();

            var film = _mapper.Map<FilmViewModel, Film>(filmViewModel);
            var newFilm = await _filmManager.UpdateAsync(id, film);
            if (newFilm == null)
                return NotFound();

            var result = _mapper.Map<Film, FilmViewModel>(newFilm);
            return Ok(result);
        }

        // DELETE: api/Films/5
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<ActionResult<bool>> DeleteFilm(Guid id)
        {
            var result = await _filmManager.DeleteAsync(id);
            if (result)
                return Ok();
            else
                return BadRequest();
        }
    }
}
