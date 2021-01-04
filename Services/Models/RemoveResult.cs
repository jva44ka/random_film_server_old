using Core.Interfaces;

namespace Services.Models
{
    public class RemoveResult<T> where T : class, IDataModel
    {
        public bool IsSuccess { get; set; }
        public T Data { get; set; }
    }
}
