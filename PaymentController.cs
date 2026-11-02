using BLL.DTOs;
using BLL.Services;
using Microsoft.AspNetCore.Mvc;

namespace ClinicApp.Controllers
{
    public class PaymentController : Controller
    {
        private readonly PaymentService paymentService;
        private readonly AppointmentService appointmentService;

        public PaymentController(
            PaymentService paymentService,
            AppointmentService appointmentService)
        {
            this.paymentService = paymentService;
            this.appointmentService = appointmentService;
        }

        private void LoadDropdownData()
        {
            ViewBag.Appointments = appointmentService.Get();
        }

        // INDEX
        public IActionResult Index()
        {
            var data = paymentService.Get();
            return View(data);
        }

        // CREATE GET
        [HttpGet]
        public IActionResult Create()
        {
            LoadDropdownData();

            var payment = new PaymentDTO
            {
                PaymentDate = DateTime.Today,
                PaymentStatus = "Unpaid"
            };

            return View(payment);
        }

        // CREATE POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(PaymentDTO payment)
        {
            if (payment.Amount <= 0)
            {
                ModelState.AddModelError("Amount", "Amount must be greater than 0");
            }

            if (string.IsNullOrWhiteSpace(payment.PaymentStatus))
            {
                payment.PaymentStatus = "Unpaid";
            }

            if (payment.AppointmentId <= 0)
            {
                ModelState.AddModelError("AppointmentId", "Please select appointment");
            }

            if (ModelState.IsValid)
            {
                var result = paymentService.Create(payment);

                if (result)
                {
                    TempData["SuccessMessage"] = "Payment created successfully.";
                    return RedirectToAction("Index");
                }
            }

            LoadDropdownData();
            return View(payment);
        }

        // EDIT GET
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var data = paymentService.Get(id);

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
        public IActionResult Edit(PaymentDTO payment)
        {
            if (payment.Amount <= 0)
            {
                ModelState.AddModelError("Amount", "Amount must be greater than 0");
            }

            if (string.IsNullOrWhiteSpace(payment.PaymentStatus))
            {
                payment.PaymentStatus = "Unpaid";
            }

            if (payment.AppointmentId <= 0)
            {
                ModelState.AddModelError("AppointmentId", "Please select appointment");
            }

            if (ModelState.IsValid)
            {
                var result = paymentService.Update(payment);

                if (result)
                {
                    TempData["SuccessMessage"] = "Payment updated successfully.";
                    return RedirectToAction("Index");
                }
            }

            LoadDropdownData();
            return View(payment);
        }
        // RECEIPT GET
        [HttpGet]
        public IActionResult Receipt(int id)
        {
            var data = paymentService.Get(id);

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
            var data = paymentService.Get(id);

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
                var result = paymentService.Delete(id);

                if (result)
                {
                    TempData["SuccessMessage"] = "Payment deleted successfully.";
                }
                else
                {
                    TempData["ErrorMessage"] = "Payment delete failed.";
                }
            }

            return RedirectToAction("Index");
        }
    }
}