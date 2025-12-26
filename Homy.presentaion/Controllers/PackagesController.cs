using Homy.Application.Dtos;
using Homy.Application.Contract_Service;
using Microsoft.AspNetCore.Mvc;

namespace Homy.presentaion.Controllers
{
    public class PackagesController : Controller
    {
        private readonly IPackage_Service _packageService;

        public PackagesController(IPackage_Service packageService)
        {
            _packageService = packageService;
        }

        public async Task<IActionResult> Index()
        {
            var packages = await _packageService.GetAllPackagesAsync();
            return View(packages);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PackageDto packageDto)
        {
            if (ModelState.IsValid)
            {
                await _packageService.CreatePackageAsync(packageDto);
                return RedirectToAction(nameof(Index));
            }
            return View(packageDto);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var package = await _packageService.GetPackageByIdAsync(id);
            if (package == null)
            {
                return NotFound();
            }
            return View(package);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, PackageDto packageDto)
        {
            if (id != packageDto.Id)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                await _packageService.UpdatePackageAsync(packageDto);
                return RedirectToAction(nameof(Index));
            }
            return View(packageDto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            await _packageService.DeletePackageAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
