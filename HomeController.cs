using ClinicApp.Models;
using DAL.DAL.EF;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace ClinicApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly CmsContext db;

        public HomeController(CmsContext db)
        {
            this.db = db;
        }

        public IActionResult Index()
        {
            ViewBag.UserName = HttpContext.Session.GetString("UserName");

            ViewBag.DoctorCount = db.Doctors.Count();
            ViewBag.PatientCount = db.Patients.Count();
            ViewBag.RoomCount = db.Rooms.Count();
            ViewBag.AppointmentCount = db.Appointments.Count();
            ViewBag.SpecializationCount = db.Specializations.Count();

            ViewBag.PendingAppointmentCount = db.Appointments.Count(a => a.Status == "Pending");
            ViewBag.CompletedAppointmentCount = db.Appointments.Count(a => a.Status == "Completed");
            ViewBag.ConfirmedAppointmentCount = db.Appointments.Count(a => a.Status == "Confirmed");

            ViewBag.TotalRevenue = db.Payments.Sum(p => (decimal?)p.Amount) ?? 0;

            ViewBag.RecentAppointments = db.Appointments
                .Include(a => a.Doctor)
                .Include(a => a.Patient)
                .Include(a => a.Room)
                .OrderByDescending(a => a.AppointmentDate)
                .ThenByDescending(a => a.AppointmentTime)
                .Take(5)
                .ToList();

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            });
        }
    }
}