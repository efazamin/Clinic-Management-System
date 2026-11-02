using BLL.DTOs;
using BLL.Services;
using Microsoft.AspNetCore.Mvc;

namespace ClinicApp.Controllers
{
    public class DoctorController : Controller
    {
        private readonly DoctorService doctorService;
        private readonly SpecializationService specializationService;

        private readonly List<string> DefaultSpecializations = new List<string>
        {
            "General",
            "Medicine",
            "Cardiology",
            "Dental",
            "Surgery",
            "Orthopedics",
            "Neurology",
            "Pediatrics",
            "Dermatology",
            "ENT"
        };

        public DoctorController(
            DoctorService doctorService,
            SpecializationService specializationService)
        {
            this.doctorService = doctorService;
            this.specializationService = specializationService;
        }

        private void LoadDropdownData()
        {
            var specializations = specializationService.Get();

            foreach (var name in DefaultSpecializations)
            {
                var exists = specializations.Any(s => s.Name.ToLower() == name.ToLower());

                if (!exists)
                {
                    specializationService.Create(new SpecializationDTO
                    {
                        Name = name
                    });
                }
            }

            specializations = specializationService.Get()
                .Where(s => DefaultSpecializations.Contains(s.Name))
                .OrderBy(s => DefaultSpecializations.IndexOf(s.Name))
                .ToList();

            ViewBag.Specializations = specializations;
        }

        // INDEX
        public IActionResult Index()
        {
            var data = doctorService.Get();
            return View(data);
        }

        // CREATE GET
        [HttpGet]
        public IActionResult Create()
        {
            LoadDropdownData();
            return View(new DoctorDTO());
        }

        // CREATE POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(DoctorDTO doctor)
        {
            if (string.IsNullOrWhiteSpace(doctor.Name))
            {
                ModelState.AddModelError("Name", "Please enter doctor name");
            }

            if (string.IsNullOrWhiteSpace(doctor.Phone))
            {
                ModelState.AddModelError("Phone", "Please enter phone number");
            }

            if (doctor.Fee < 0)
            {
                ModelState.AddModelError("Fee", "Fee cannot be negative");
            }

            if (doctor.SpecializationId <= 0)
            {
                ModelState.AddModelError("SpecializationId", "Please select specialization");
            }

            if (string.IsNullOrWhiteSpace(doctor.Email))
            {
                doctor.Email = "";
            }

            if (ModelState.IsValid)
            {
                var result = doctorService.Create(doctor);

                if (result)
                {
                    TempData["SuccessMessage"] = "Doctor created successfully.";
                    return RedirectToAction("Index");
                }

                TempData["ErrorMessage"] = "Doctor create failed.";
            }

            LoadDropdownData();
            return View(doctor);
        }

        // EDIT GET
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var data = doctorService.Get(id);

            if (data == null)
            {
                return NotFound();
            }

            LoadDropdownData();
            return View(data);
        }

        // EDIT POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(DoctorDTO doctor)
        {
            if (string.IsNullOrWhiteSpace(doctor.Name))
            {
                ModelState.AddModelError("Name", "Please enter doctor name");
            }

            if (string.IsNullOrWhiteSpace(doctor.Phone))
            {
                ModelState.AddModelError("Phone", "Please enter phone number");
            }

            if (doctor.Fee < 0)
            {
                ModelState.AddModelError("Fee", "Fee cannot be negative");
            }

            if (doctor.SpecializationId <= 0)
            {
                ModelState.AddModelError("SpecializationId", "Please select specialization");
            }

            if (string.IsNullOrWhiteSpace(doctor.Email))
            {
                doctor.Email = "";
            }

            if (ModelState.IsValid)
            {
                var result = doctorService.Update(doctor);

                if (result)
                {
                    TempData["SuccessMessage"] = "Doctor updated successfully.";
                    return RedirectToAction("Index");
                }

                TempData["ErrorMessage"] = "Doctor update failed.";
            }

            LoadDropdownData();
            return View(doctor);
        }

        // DELETE GET
        [HttpGet]
        public IActionResult Delete(int id)
        {
            var data = doctorService.Get(id);

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
                if (doctorService.HasAppointments(id))
                {
                    TempData["ErrorMessage"] = "This doctor has appointments. Delete appointments first.";
                    return RedirectToAction("Index");
                }

                var result = doctorService.Delete(id);

                if (result)
                {
                    TempData["SuccessMessage"] = "Doctor deleted successfully.";
                }
                else
                {
                    TempData["ErrorMessage"] = "Doctor delete failed.";
                }
            }

            return RedirectToAction("Index");
        }
    }
}