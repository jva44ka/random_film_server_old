using Core.Interfaces;
using Core.Models;
using Services.Managers.Interfaces;
using System.Linq;
using System.Threading.Tasks;

namespace Services.Managers
{
    public class SelectionManager : ISelectionManager
    {
        private readonly IRepository<SelectionList> _selectionRepository;

        public SelectionManager(IRepository<SelectionList> selectionRepository)
        {
            _selectionRepository = selectionRepository;
        }

        public async Task RemoveAllSelectionsByUser(string userId)
        {
            var selectionsToRemove = _selectionRepository
                .Get()
                .Where(s => s.UserId == userId);

            _selectionRepository.DeleteRange(selectionsToRemove);
            await _selectionRepository.SaveAsync();
        }
    }
}
