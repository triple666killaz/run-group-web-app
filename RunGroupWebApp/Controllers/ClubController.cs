using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RunGroupWebApp.Data;
using RunGroupWebApp.Interfaces;
using RunGroupWebApp.Models;
using RunGroupWebApp.ViewModels;

namespace RunGroupWebApp.Controllers;

public class ClubController : Controller
{
    private readonly IClubRepository _clubRepository;
    private readonly IPhotoService _photoService;

    public ClubController(IClubRepository clubRepository, IPhotoService photoService)
    {
        _clubRepository = clubRepository;
        _photoService = photoService;
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
    
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateClubViewModel clubVM)
    {
        if (ModelState.IsValid)
        {
            var result = await _photoService.AddPhotoAsync(clubVM.Image);

            var club = new Club()
            {
                Title = clubVM.Title,
                Description = clubVM.Description,
                Image = result.Url.ToString(),
                Address = new Address()
                {
                    City = clubVM.Address.City,
                    State = clubVM.Address.State,
                    Street = clubVM.Address.Street
                }
            };
            
            _clubRepository.Add(club);
            return RedirectToAction("Index");
        }
        
        ModelState.AddModelError("", "Photo upload failed");
        return View(clubVM);
    }
}