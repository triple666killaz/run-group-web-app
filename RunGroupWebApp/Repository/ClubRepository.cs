using Microsoft.EntityFrameworkCore;
using RunGroupWebApp.Data;
using RunGroupWebApp.Interfaces;
using RunGroupWebApp.Models;

namespace RunGroupWebApp.Repository;

public class ClubRepository : IClubRepository
{
    private readonly ApplicationDbContext _context;

    public ClubRepository(ApplicationDbContext context)
    {
        _context = context;
    }
    
    public async Task<IEnumerable<Club>> GetClubsAsync()
    {
        return await _context.Clubs.ToListAsync();
    }

    public async Task<Club> GetClubByIdAsync(int id)
    {
        return await _context.Clubs.Include(c => c.Address).FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<IEnumerable<Club>> GetClubsByCityAsync(string city)
    {
        return await _context.Clubs.Where(c => c.Address.City.Contains(city)).ToListAsync();
    }

    public bool Add(Club club)
    {
        _context.Add(club);
        return Save();
    }

    public bool Update(Club club)
    {
        _context.Update(club);
        return Save();
    }

    public bool Delete(Club club)
    {
        _context.Remove(club);
        return Save();
    }

    public bool Save()
    {
        return _context.SaveChanges() > 0;
    }
}