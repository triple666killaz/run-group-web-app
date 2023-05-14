using RunGroupWebApp.Models;

namespace RunGroupWebApp.Interfaces;

public interface IRaceRepository
{
    Task<IEnumerable<Race>> GetRacesAsync();
    Task<Race> GetRaceByIdAsync(int id);
    Task<Race> GetRaceByIdAsyncNoTracking(int id);
    Task<IEnumerable<Race>> GetRacesByCityAsync(string city);
    bool Add(Race race);
    bool Update(Race race);
    bool Delete(Race race);
    bool Save();
}