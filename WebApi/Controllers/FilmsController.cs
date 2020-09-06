using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

using WebApi.ViewModels;
using System;
using Services.Managers.Interfaces;
using AutoMapper;
using Core.Models;
using Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FilmsController : ControllerBase
    {
        private readonly IFilmManager _filmManager;
        private readonly IMapper _mapper;
        private readonly IRepository<Account> _accountsRepo;

        public FilmsController(IFilmManager filmManager, IMapper mapper, IRepository<Account> accountsRepo)
        {
            _filmManager = filmManager;
            _mapper = mapper;
            _accountsRepo = accountsRepo;
        }

        // GET: api/Films
        [HttpGet]
        public async Task<IList<FilmViewModel>> GetFilms(string forUserId = "")
        {
            var films = this._filmManager.GetAllFilms();
            var filmsVm = _mapper.Map<IList<Film>, IList<FilmViewModel>>(films);
            await MapFilms(filmsVm, forUserId);
            return filmsVm;
        }

        // GET: api/Films/5
        [HttpGet("{id}")]
        public async Task<ActionResult<FilmViewModel>> GetFilmById(Guid id)
        {
            var film = this._filmManager.GetFilmById(id);

            if (film == null)
            {
                return NotFound();
            }

            var filmVM = _mapper.Map<Film, FilmViewModel>(film);
            return filmVM;
        }

        // GET: api/Films/5
        [HttpGet("{id}/forUser")]
        [Authorize]
        public async Task<ActionResult<FilmViewModel>> GetFilmByIdForUser(Guid id)
        {
            var film = this._filmManager.GetFilmById(id);
            var user = _accountsRepo.Get()
                .AsNoTracking()
                .Single(x => x.UserName == User.Identity.Name);

            if (film == null)
            {
                return NotFound();
            }

            var filmVM = _mapper.Map<Film, FilmViewModel>(film);
            await MapFilm(filmVM, user.Id);
            return filmVM;
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
        public async Task<ActionResult<Film>> PostFilm([FromBody] FilmViewModel filmViewModel)
        {
            var film = _mapper.Map<FilmViewModel, Film>(filmViewModel);
            var newFilm = await _filmManager.CreateAsync(film);

            var result = _mapper.Map<Film, FilmViewModel>(newFilm);
            return Ok(result);
        }

        // PUT: api/Films/5
        [HttpPut("{id}")]
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
        public async Task<ActionResult<bool>> DeleteFilm(Guid id)
        {
            var result = await _filmManager.DeleteAsync(id);
            if (result)
                return Ok();
            else
                return BadRequest();
        }

        private async Task<FilmViewModel> MapFilm(FilmViewModel film, string userId)
        {
            film.IsLiked = await _filmManager.IsLiked(userId, film.Id);
            return film;
        }

        private async Task<IList<FilmViewModel>> MapFilms(IList<FilmViewModel> films, string userId)
        {
            foreach (var film in films)
            {
                film.IsLiked = await _filmManager.IsLiked(userId, film.Id);
            }
            return films;
        }
    }
}
