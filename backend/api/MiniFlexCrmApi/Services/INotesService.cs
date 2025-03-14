using MiniFlexCrmApi.Models;

namespace MiniFlexCrmApi.Services;

public interface INotesService
{
    Task<IEnumerable<NoteModel>> Get5LatestNotes(int userId, int tenantId, string route);
    Task<int> CreateAsync(NoteModel note);
    Task<int> UpdateAsync(NoteModel note);
    Task DeleteAsync(int id, int userId);
    Task TogglePinAsync(int id, int userId);
}