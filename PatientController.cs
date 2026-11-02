using BLL.DTOs;
using BLL.Services;
using Microsoft.AspNetCore.Mvc;

namespace ClinicApp.Controllers
{
    public class PatientController : Controller
    {
        private readonly PatientService patientService;

        public PatientController(PatientService patientService)
        {
            this.patientService = patientService;
        }

        // INDEX
        public IActionResult Index()
        {
            var data = patientService.Get();
            return View(data);
        }

        // CREATE GET
        [HttpGet]
        public IActionResult Create()
        {
            return View(new PatientDTO());
        }

        // CREATE POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(PatientDTO patient)
        {
            if (string.IsNullOrWhiteSpace(patient.Name))
            {
                ModelState.AddModelError("Name", "Please enter patient name");
            }

            if (string.IsNullOrWhiteSpace(patient.Phone))
            {
                ModelState.AddModelError("Phone", "Please enter phone number");
            }

            if (patient.Age < 0)
            {
                ModelState.AddModelError("Age", "Age cannot be negative");
            }

            if (string.IsNullOrWhiteSpace(patient.Gender))
            {
                ModelState.AddModelError("Gender", "Please select gender");
            }

            if (string.IsNullOrWhiteSpace(patient.Address))
            {
                patient.Address = "";
            }

            if (ModelState.IsValid)
            {
                var result = patientService.Create(patient);

                if (result)
                {
                    TempData["SuccessMessage"] = "Patient created successfully.";
                    return RedirectToAction("Index");
                }
            }

            return View(patient);
        }

        // EDIT GET
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var data = patientService.Get(id);

            if (data == null)
            {
                return NotFound();
            }

            return View(data);
        }

        // EDIT POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(PatientDTO patient)
        {
            if (string.IsNullOrWhiteSpace(patient.Name))
            {
                ModelState.AddModelError("Name", "Please enter patient name");
            }

            if (string.IsNullOrWhiteSpace(patient.Phone))
            {
                ModelState.AddModelError("Phone", "Please enter phone number");
            }

            if (patient.Age < 0)
            {
                ModelState.AddModelError("Age", "Age cannot be negative");
            }

            if (string.IsNullOrWhiteSpace(patient.Gender))
            {
                ModelState.AddModelError("Gender", "Please select gender");
            }

            if (string.IsNullOrWhiteSpace(patient.Address))
            {
                patient.Address = "";
            }

            if (ModelState.IsValid)
            {
                var result = patientService.Update(patient);

                if (result)
                {
                    TempData["SuccessMessage"] = "Patient updated successfully.";
                    return RedirectToAction("Index");
                }
            }

            return View(patient);
        }

        // DELETE GET
        [HttpGet]
        public IActionResult Delete(int id)
        {
            var data = patientService.Get(id);

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
                if (patientService.HasAppointments(id))
                {
                    TempData["ErrorMessage"] = "This patient has appointments. Delete appointments first.";
                    return RedirectToAction("Index");
                }

                var result = patientService.Delete(id);

                if (result)
                {
                    TempData["SuccessMessage"] = "Patient deleted successfully.";
                }
                else
                {
                    TempData["ErrorMessage"] = "Patient delete failed.";
                }
            }

            return RedirectToAction("Index");
        }
    }
}