using System.Threading.Tasks;

namespace Services.Managers.Interfaces
{
    public interface ISelectionManager
    {
        Task RemoveAllSelectionsByUser(string userId);
    }
}
