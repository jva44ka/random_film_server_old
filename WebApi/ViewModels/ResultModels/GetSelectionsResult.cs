using System.Collections.Generic;

namespace WebApi.ViewModels.ResultModels
{
    public class GetSelectionsResult
    {
        public IList<FilmViewModel> RandomFilms { get; set; }
        public IList<FilmViewModel> SameUserFilms { get; set; }
        public IList<FilmViewModel> KnnFilms { get; set; }
        public IList<FilmViewModel> PopularFilms { get; set; }
    }
}
