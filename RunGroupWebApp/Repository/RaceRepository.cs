using Microsoft.EntityFrameworkCore;
using RunGroupWebApp.Data;
using RunGroupWebApp.Interfaces;
using RunGroupWebApp.Models;

namespace RunGroupWebApp.Repository;

public class RaceRepository : IRaceRepository
{
    private readonly ApplicationDbContext _context;

    public RaceRepository(ApplicationDbContext context)
    {
        _context = context;
    }
    public async Task<IEnumerable<Race>> GetRacesAsync()
    {
        return await _context.Races.ToListAsync();
    }

    public async Task<Race> GetRaceByIdAsync(int id)
    {
        return await _context.Races.Include(r => r.Address).FirstOrDefaultAsync(r => r.Id == id);
    }

    public async Task<IEnumerable<Race>> GetRacesByCityAsync(string city)
    {
        return await _context.Races.Where(r => r.Address.City.Contains(city)).ToListAsync();
    }

    public bool Add(Race race)
    {
        _context.Add(race);
        return Save();
    }

    public bool Update(Race race)
    {
        _context.Update(race);
        return Save();
    }

    public bool Delete(Race race)
    {
        _context.Remove(race);
        return Save();
    }

    public bool Save()
    {
        return _context.SaveChanges() > 0;
    }
}