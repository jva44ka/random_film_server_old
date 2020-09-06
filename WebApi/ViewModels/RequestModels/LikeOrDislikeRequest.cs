using System;

namespace WebApi.ViewModels.RequestModels
{
    public class LikeOrDislikeRequest
    {
        public string UserId { get; set; }
        public Guid FilmId { get; set; }
        public bool? LikeOrDislike { get; set; }
    }
}
