using RunGroupWebApp.Models;

namespace RunGroupWebApp.Interfaces;

public interface IClubRepository
{
    Task<IEnumerable<Club>> GetClubsAsync();
    Task<Club> GetClubByIdAsync(int id);
    Task<IEnumerable<Club>> GetClubsByCityAsync(string city);
    bool Add(Club club);
    bool Update(Club club);
    bool Delete(Club club);
    bool Save();
}