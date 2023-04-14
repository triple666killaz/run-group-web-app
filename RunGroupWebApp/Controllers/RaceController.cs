using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RunGroupWebApp.Data;
using RunGroupWebApp.Interfaces;

namespace RunGroupWebApp.Controllers;

public class RaceController : Controller
{
    private readonly IRaceRepository _raceRepository;

    public RaceController(IRaceRepository raceRepository)
    {
        _raceRepository = raceRepository;
    }
    
    public async Task<IActionResult> Index()
    {
        var races = await _raceRepository.GetRacesAsync();
        
        return View(races);
    }

    public async Task<IActionResult> Detail(int id)
    {
        var race = await _raceRepository.GetRaceByIdAsync(id);

        return View(race);
    }
}