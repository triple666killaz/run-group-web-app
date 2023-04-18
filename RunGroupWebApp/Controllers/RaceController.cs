﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using RunGroupWebApp.Data;
using RunGroupWebApp.Interfaces;
using RunGroupWebApp.Models;
using RunGroupWebApp.ViewModels;

namespace RunGroupWebApp.Controllers;

public class RaceController : Controller
{
    private readonly IRaceRepository _raceRepository;
    private readonly IPhotoService _photoService;

    public RaceController(IRaceRepository raceRepository, IPhotoService photoService)
    {
        _raceRepository = raceRepository;
        _photoService = photoService;
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

    public IActionResult Create()
    {
        return View();
    }
    
    [HttpPost]
    public async Task<IActionResult> Create(CreateRaceViewModel raceVM)
    {
        if (ModelState.IsValid)
        {
            var result = await _photoService.AddPhotoAsync(raceVM.Image);

            var race = new Race()
            {
                Title = raceVM.Title,
                Description = raceVM.Description,
                Image = result.Url.ToString(),
                Address = new Address()
                {
                    City = raceVM.Address.City,
                    State = raceVM.Address.State,
                    Street = raceVM.Address.Street
                }
            };
            
            _raceRepository.Add(race);
            return RedirectToAction("Index");
        }
        
        ModelState.AddModelError("", "Photo upload failed");
        return View(raceVM);

    }

}