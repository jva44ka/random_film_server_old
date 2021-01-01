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
using Infrastructure.Auth;
using WebApi.ViewModels.ResultModels;

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
        public async Task<ActionResult<FilmViewModel>> GetFilmById(Guid id, string forUserId = "")
        {
            var film = this._filmManager.GetFilmById(id);

            if (film == null)
            {
                return NotFound();
            }

            var filmVM = _mapper.Map<Film, FilmViewModel>(film);

            if (!string.IsNullOrEmpty(forUserId))
            {
                var user = _accountsRepo.Get()
                    .AsNoTracking()
                    .Single(x => x.Id == forUserId);

                if(user != null)
                    await MapFilm(filmVM, forUserId);
            }

            return filmVM;
        }

        // GET: api/Films/Random
        [HttpGet("Random")]
        public async Task<IList<FilmViewModel>> GetRandomFilms([FromQuery]string forUserId = "")
        {
            var userId = HttpContext.User.Claims.FirstOrDefault(x => x.Type == AuthExtensions.UserId)?.Value;
            var films = await this._filmManager.GetRandomShakedFilms(userId);
            var filmsVM = _mapper.Map<IList<Film>, IList<FilmViewModel>>(films);

            if (!string.IsNullOrEmpty(forUserId))
                await MapFilms(filmsVM, forUserId);

            return filmsVM;
        }

        [HttpGet("Specificity")]
        [Authorize]
        public async Task<IList<FilmViewModel>> GetSpecificityFilms()
        {
            var userId = HttpContext.User.Claims.Single(c => c.Type == AuthExtensions.UserId).Value;
            var films = await _filmManager.GetSameUsersFilms(userId);
            var filmsVM = _mapper.Map<IList<Film>, IList<FilmViewModel>>(films);
            var result = await MapFilms(filmsVM, userId);
            return result;
        }

        // GET: api/films/selections
        [HttpGet("selections")]
        public async Task<GetSelectionsResult> GetSelections()
        {
            var userId = HttpContext.User.Claims.FirstOrDefault(x => x.Type == AuthExtensions.UserId)?.Value;
            var resultModel = new GetSelectionsResult();

            if (string.IsNullOrEmpty(userId))
            {
                resultModel.RandomFilms = _mapper.Map<IList<Film>, IList<FilmViewModel>>
                    (await _filmManager.GetRandomShakedFilms());
                resultModel.PopularFilms = _mapper.Map<IList<Film>, IList<FilmViewModel>>
                    (await _filmManager.GetPopularFilms());
            }
            else
            {
                // Потом можно вынести в сервис и распараллелить
                resultModel.RandomFilms = _mapper.Map<IList<Film>, IList<FilmViewModel>>
                    (await _filmManager.GetRandomShakedFilms(userId));
                resultModel.SameUserFilms = _mapper.Map<IList<Film>, IList<FilmViewModel>>
                    (await _filmManager.GetSameUsersFilms(userId));
                resultModel.PopularFilms = _mapper.Map<IList<Film>, IList<FilmViewModel>>
                    (await _filmManager.GetPopularFilms(userId));

                await MapFilms(resultModel.RandomFilms, userId);
                await MapFilms(resultModel.SameUserFilms, userId);
            }

            return resultModel;
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
