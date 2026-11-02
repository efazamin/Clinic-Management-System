using BLL.DTOs;
using BLL.Services;
using Microsoft.AspNetCore.Mvc;

namespace ClinicApp.Controllers
{
    public class SpecializationController : Controller
    {
        private readonly SpecializationService specializationService;

        public SpecializationController(SpecializationService specializationService)
        {
            this.specializationService = specializationService;
        }

        // INDEX
        public IActionResult Index()
        {
            var data = specializationService.Get();
            return View(data);
        }

        // CREATE GET
        [HttpGet]
        public IActionResult Create()
        {
            return View(new SpecializationDTO());
        }

        // CREATE POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(SpecializationDTO specialization)
        {
            if (string.IsNullOrWhiteSpace(specialization.Name))
            {
                ModelState.AddModelError("Name", "Please select specialization name");
            }

            var existingSpecialization = specializationService.Get()
                .FirstOrDefault(s => s.Name.ToLower() == specialization.Name.ToLower());

            if (existingSpecialization != null)
            {
                ModelState.AddModelError("Name", "This specialization already exists.");
            }

            if (ModelState.IsValid)
            {
                var result = specializationService.Create(specialization);

                if (result)
                {
                    TempData["SuccessMessage"] = "Specialization created successfully.";
                    return RedirectToAction("Index");
                }
            }

            return View(specialization);
        }

        // EDIT GET
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var data = specializationService.Get(id);

            if (data == null)
            {
                return NotFound();
            }

            return View(data);
        }

        // EDIT POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(SpecializationDTO specialization)
        {
            if (string.IsNullOrWhiteSpace(specialization.Name))
            {
                ModelState.AddModelError("Name", "Please select specialization name");
            }

            var existingSpecialization = specializationService.Get()
                .FirstOrDefault(s =>
                    s.Name.ToLower() == specialization.Name.ToLower()
                    && s.Id != specialization.Id);

            if (existingSpecialization != null)
            {
                ModelState.AddModelError("Name", "This specialization already exists.");
            }

            if (ModelState.IsValid)
            {
                var result = specializationService.Update(specialization);

                if (result)
                {
                    TempData["SuccessMessage"] = "Specialization updated successfully.";
                    return RedirectToAction("Index");
                }
            }

            return View(specialization);
        }

        // DELETE GET
        [HttpGet]
        public IActionResult Delete(int id)
        {
            var data = specializationService.Get(id);

            if (data == null)
            {
                return NotFound();
            }

            return View(data);
        }

        // DELETE POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id, string Decision)
        {
            if (Decision == "Yes")
            {
                if (specializationService.HasDoctors(id))
                {
                    TempData["ErrorMessage"] = "This specialization has doctors. Delete doctors first.";
                    return RedirectToAction("Index");
                }

                var result = specializationService.Delete(id);

                if (result)
                {
                    TempData["SuccessMessage"] = "Specialization deleted successfully.";
                }
                else
                {
                    TempData["ErrorMessage"] = "Specialization delete failed.";
                }
            }

            return RedirectToAction("Index");
        }
    }
}