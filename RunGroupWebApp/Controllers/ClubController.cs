using Microsoft.AspNetCore.Mvc;
using RunGroupWebApp.Interfaces;
using RunGroupWebApp.Models;
using RunGroupWebApp.ViewModels;

namespace RunGroupWebApp.Controllers;

public class ClubController : Controller
{
    private readonly IClubRepository _clubRepository;
    private readonly IPhotoService _photoService;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ClubController(IClubRepository clubRepository, IPhotoService photoService, IHttpContextAccessor httpContextAccessor)
    {
        _clubRepository = clubRepository;
        _photoService = photoService;
        _httpContextAccessor = httpContextAccessor;
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
        //var currentUser = _httpContextAccessor.HttpContext.User.
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateClubViewModel clubVM)
    {
        if (ModelState.IsValid)
        {
            var photoResult = await _photoService.AddPhotoAsync(clubVM.Image);

            var club = new Club()
            {
                Title = clubVM.Title,
                Description = clubVM.Description,
                Image = photoResult.Url.ToString(),
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

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var club = await _clubRepository.GetClubByIdAsync(id);

        if (club == null)
            return View("Error");

        var clubVM = new EditClubViewModel
        {
            Title = club.Title,
            Description = club.Description,
            AddressId = club.AddressId,
            Address = club.Address,
            URL = club.Image,
            ClubCategory = club.ClubCategory
        };

        return View(clubVM);

    }

    [HttpPost]
    public async Task<IActionResult> Edit(int id, EditClubViewModel clubVM)
    {
        if (!ModelState.IsValid)
        {
            ModelState.AddModelError("", "Failed to edit club");
            return View("Edit", clubVM);
        }

        var userClub = await _clubRepository.GetClubByIdAsyncNoTracking(id);

        if (userClub != null)
        {
            try
            {
                await _photoService.DeletePhotoAsync(userClub.Image);
            }
            catch (Exception e)
            {
                ModelState.AddModelError("", "Could not delete photo");
                return View(clubVM);
            }

            var photoResult = await _photoService.AddPhotoAsync(clubVM.Image);

            var club = new Club
            {
                Id = id,
                Title = clubVM.Title,
                Description = clubVM.Description,
                Image = photoResult.Url.ToString(),
                AddressId = clubVM.AddressId,
                Address = clubVM.Address,
                ClubCategory = clubVM.ClubCategory
            };

            _clubRepository.Update(club);

            return RedirectToAction("Index");
        }
        
        return View(clubVM);
    }

    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var clubDetails = await _clubRepository.GetClubByIdAsync(id);
        
        if (clubDetails == null)
            return View("Error");
        
        return View(clubDetails);
    }

    [HttpPost, ActionName("Delete")]
    public async Task<IActionResult> DeleteClub(int id)
    {
        var clubDetails = await _clubRepository.GetClubByIdAsync(id);

        if (clubDetails == null)
            return View("Error");

        if (!string.IsNullOrEmpty(clubDetails.Image))
           _ = _photoService.DeletePhotoAsync(clubDetails.Image);

        _clubRepository.Delete(clubDetails);
        
        return RedirectToAction("Index");
    }

}