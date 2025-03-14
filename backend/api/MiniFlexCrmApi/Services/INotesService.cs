using MiniFlexCrmApi.Models;

namespace MiniFlexCrmApi.Services;

public interface INotesService
{
    Task<IEnumerable<NoteModel>> ListAsync(int userId, int tenantId, string route);
    Task<int> CreateAsync(NoteModel note);
    Task DeleteAsync(int id, int userId);
    Task TogglePinAsync(int id, int userId);
}