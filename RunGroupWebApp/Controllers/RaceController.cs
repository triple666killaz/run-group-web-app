using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
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

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var race = await _raceRepository.GetRaceByIdAsync(id);

        if (race == null)
            return View("Error");

        var raceVM = new EditRaceViewModel
        {
            Title = race.Title,
            Description = race.Description,
            URL = race.Image,
            RaceCategory = race.RaceCategory
        };

        return View(raceVM);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(int id, EditRaceViewModel raceVM)
    {
        if (!ModelState.IsValid)
        {
            ModelState.AddModelError("", "Failed to edit race");
            return View("Edit", raceVM);
        }

        var userRace = await _raceRepository.GetRaceByIdAsyncNoTracking(id);

        if (userRace != null)
        {
            try
            {
                await _photoService.DeletePhotoAsync(userRace.Image);
            }
            catch (Exception e)
            {
                ModelState.AddModelError("", "Could not delete photo");
                return View(raceVM);
            }

            var photoResult = await _photoService.AddPhotoAsync(raceVM.Image);

            var race = new Race()
            {
                Id = id,
                Title = raceVM.Title,
                Description = raceVM.Description,
                Image = photoResult.Url.ToString(),
                AddressId = raceVM.AddressId,
                Address = raceVM.Address,
                RaceCategory = raceVM.RaceCategory
            };

            _raceRepository.Update(race);

            return RedirectToAction("Index");
        }

        return View(raceVM);
    }


    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var raceDetails = await _raceRepository.GetRaceByIdAsync(id);

        if (raceDetails == null)
            return View("Error");

        return View(raceDetails);
    }

    [HttpPost, ActionName("Delete")]
    public async Task<IActionResult> DeleteRace(int id)
    {
        var raceDetails = await _raceRepository.GetRaceByIdAsync(id);

        if (raceDetails == null)
            return View("Error");

        if (!string.IsNullOrEmpty(raceDetails.Image))
           _ = _photoService.DeletePhotoAsync(raceDetails.Image);

        _raceRepository.Delete(raceDetails);

        return RedirectToAction("Index");
    }

}