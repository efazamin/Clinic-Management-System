using BLL.DTOs;
using BLL.Services;
using Microsoft.AspNetCore.Mvc;

namespace ClinicApp.Controllers
{
    public class PrescriptionController : Controller
    {
        private readonly PrescriptionService prescriptionService;
        private readonly AppointmentService appointmentService;

        public PrescriptionController(
            PrescriptionService prescriptionService,
            AppointmentService appointmentService)
        {
            this.prescriptionService = prescriptionService;
            this.appointmentService = appointmentService;
        }

        private void LoadDropdownData()
        {
            ViewBag.Appointments = appointmentService.Get();
        }

        // INDEX
        public IActionResult Index()
        {
            var data = prescriptionService.Get();
            return View(data);
        }

        // CREATE GET
        [HttpGet]
        public IActionResult Create()
        {
            LoadDropdownData();
            return View(new PrescriptionDTO());
        }

        // CREATE POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(PrescriptionDTO prescription)
        {
            if (string.IsNullOrWhiteSpace(prescription.Diagnosis))
            {
                ModelState.AddModelError("Diagnosis", "Please enter diagnosis");
            }

            if (string.IsNullOrWhiteSpace(prescription.Medicines))
            {
                ModelState.AddModelError("Medicines", "Please enter medicines");
            }

            if (string.IsNullOrWhiteSpace(prescription.Advice))
            {
                prescription.Advice = "";
            }

            if (prescription.AppointmentId <= 0)
            {
                ModelState.AddModelError("AppointmentId", "Please select appointment");
            }

            if (ModelState.IsValid)
            {
                var result = prescriptionService.Create(prescription);

                if (result)
                {
                    TempData["SuccessMessage"] = "Prescription created successfully.";
                    return RedirectToAction("Index");
                }
            }

            LoadDropdownData();
            return View(prescription);
        }

        // EDIT GET
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var data = prescriptionService.Get(id);

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
        public IActionResult Edit(PrescriptionDTO prescription)
        {
            if (string.IsNullOrWhiteSpace(prescription.Diagnosis))
            {
                ModelState.AddModelError("Diagnosis", "Please enter diagnosis");
            }

            if (string.IsNullOrWhiteSpace(prescription.Medicines))
            {
                ModelState.AddModelError("Medicines", "Please enter medicines");
            }

            if (string.IsNullOrWhiteSpace(prescription.Advice))
            {
                prescription.Advice = "";
            }

            if (prescription.AppointmentId <= 0)
            {
                ModelState.AddModelError("AppointmentId", "Please select appointment");
            }

            if (ModelState.IsValid)
            {
                var result = prescriptionService.Update(prescription);

                if (result)
                {
                    TempData["SuccessMessage"] = "Prescription updated successfully.";
                    return RedirectToAction("Index");
                }
            }

            LoadDropdownData();
            return View(prescription);
        }
        // PRINT GET
        [HttpGet]
        public IActionResult Print(int id)
        {
            var data = prescriptionService.Get(id);

            if (data == null)
            {
                return NotFound();
            }

            return View(data);
        }

        // DELETE GET
        [HttpGet]
        public IActionResult Delete(int id)
        {
            var data = prescriptionService.Get(id);

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
                var result = prescriptionService.Delete(id);

                if (result)
                {
                    TempData["SuccessMessage"] = "Prescription deleted successfully.";
                }
                else
                {
                    TempData["ErrorMessage"] = "Prescription delete failed.";
                }
            }

            return RedirectToAction("Index");
        }
    }
}