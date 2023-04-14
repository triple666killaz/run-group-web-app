using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RunGroupWebApp.Data;
using RunGroupWebApp.Interfaces;
using RunGroupWebApp.Models;

namespace RunGroupWebApp.Controllers;

public class ClubController : Controller
{
    private readonly IClubRepository _clubRepository;

    public ClubController(IClubRepository clubRepository)
    {
        _clubRepository = clubRepository;
    }
    
    public async Task<IActionResult> Index()
    {
        var clubs = await _clubRepository.GetClubsAsync();
        
        return View(clubs);
    }

    public async Task<IActionResult> Detail(int id)
    {
        var club = await _clubRepository.GetClubByIdAsync(id);

        return View(club);
    }
}