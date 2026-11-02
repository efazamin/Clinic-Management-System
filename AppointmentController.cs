using BLL.DTOs;
using BLL.Services;
using Microsoft.AspNetCore.Mvc;

namespace ClinicApp.Controllers
{
    public class AppointmentController : Controller
    {
        private readonly AppointmentService appointmentService;
        private readonly DoctorService doctorService;
        private readonly PatientService patientService;
        private readonly RoomService roomService;
        private readonly SpecializationService specializationService;

        public AppointmentController(
            AppointmentService appointmentService,
            DoctorService doctorService,
            PatientService patientService,
            RoomService roomService,
            SpecializationService specializationService)
        {
            this.appointmentService = appointmentService;
            this.doctorService = doctorService;
            this.patientService = patientService;
            this.roomService = roomService;
            this.specializationService = specializationService;
        }

        private void LoadDropdownData()
        {
            ViewBag.Rooms = roomService.Get();
        }

        private string GenerateAutoPhone(string prefix)
        {
            return prefix + DateTime.Now.ToString("MMddHHmmssfff");
        }

        private int GetDefaultSpecializationId()
        {
            var specializations = specializationService.Get();

            var existing = specializations
                .FirstOrDefault(s => s.Name.ToLower() == "general");

            if (existing != null)
            {
                return existing.Id;
            }

            specializationService.Create(new SpecializationDTO
            {
                Name = "General"
            });

            var created = specializationService.Get()
                .FirstOrDefault(s => s.Name.ToLower() == "general");

            return created.Id;
        }

        private int GetOrCreateDoctorId(string doctorName)
        {
            doctorName = doctorName.Trim();

            var existingDoctor = doctorService.Get()
                .FirstOrDefault(d => d.Name.ToLower() == doctorName.ToLower());

            if (existingDoctor != null)
            {
                return existingDoctor.Id;
            }

            var doctor = new DoctorDTO
            {
                Name = doctorName,
                Phone = GenerateAutoPhone("D"),
                Email = "",
                Fee = 0,
                SpecializationId = GetDefaultSpecializationId()
            };

            doctorService.Create(doctor);

            var createdDoctor = doctorService.Get()
                .FirstOrDefault(d => d.Name.ToLower() == doctorName.ToLower());

            return createdDoctor.Id;
        }

        private int GetOrCreatePatientId(string patientName)
        {
            patientName = patientName.Trim();

            var existingPatient = patientService.Get()
                .FirstOrDefault(p => p.Name.ToLower() == patientName.ToLower());

            if (existingPatient != null)
            {
                return existingPatient.Id;
            }

            var patient = new PatientDTO
            {
                Name = patientName,
                Phone = GenerateAutoPhone("P"),
                Age = 0,
                Gender = "Unknown",
                Address = ""
            };

            patientService.Create(patient);

            var createdPatient = patientService.Get()
                .FirstOrDefault(p => p.Name.ToLower() == patientName.ToLower());

            return createdPatient.Id;
        }

        // INDEX
        public IActionResult Index()
        {
            var data = appointmentService.Get();
            return View(data);
        }

        // CREATE GET
        [HttpGet]
        public IActionResult Create()
        {
            LoadDropdownData();

            var appointment = new AppointmentDTO
            {
                AppointmentDate = DateTime.Today,
                AppointmentTime = new TimeSpan(9, 0, 0),
                Status = "Pending"
            };

            return View(appointment);
        }

        // CREATE POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(AppointmentDTO appointment)
        {
            if (string.IsNullOrWhiteSpace(appointment.DoctorName))
            {
                ModelState.AddModelError("DoctorName", "Please enter doctor name");
            }

            if (string.IsNullOrWhiteSpace(appointment.PatientName))
            {
                ModelState.AddModelError("PatientName", "Please enter patient name");
            }

            if (string.IsNullOrWhiteSpace(appointment.Status))
            {
                appointment.Status = "Pending";
            }

            if (appointment.RoomId <= 0)
            {
                ModelState.AddModelError("RoomId", "Please select room");
            }

            if (ModelState.IsValid)
            {
                appointment.DoctorId = GetOrCreateDoctorId(appointment.DoctorName);
                appointment.PatientId = GetOrCreatePatientId(appointment.PatientName);

                var result = appointmentService.Create(appointment);

                if (result)
                {
                    TempData["SuccessMessage"] = "Appointment created successfully.";
                    return RedirectToAction("Index");
                }

                TempData["ErrorMessage"] = "Appointment create failed.";
            }

            LoadDropdownData();
            return View(appointment);
        }

        // EDIT GET
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var data = appointmentService.Get(id);

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
        public IActionResult Edit(AppointmentDTO appointment)
        {
            if (string.IsNullOrWhiteSpace(appointment.DoctorName))
            {
                ModelState.AddModelError("DoctorName", "Please enter doctor name");
            }

            if (string.IsNullOrWhiteSpace(appointment.PatientName))
            {
                ModelState.AddModelError("PatientName", "Please enter patient name");
            }

            if (string.IsNullOrWhiteSpace(appointment.Status))
            {
                appointment.Status = "Pending";
            }

            if (appointment.RoomId <= 0)
            {
                ModelState.AddModelError("RoomId", "Please select room");
            }

            if (ModelState.IsValid)
            {
                appointment.DoctorId = GetOrCreateDoctorId(appointment.DoctorName);
                appointment.PatientId = GetOrCreatePatientId(appointment.PatientName);

                var result = appointmentService.Update(appointment);

                if (result)
                {
                    TempData["SuccessMessage"] = "Appointment updated successfully.";
                    return RedirectToAction("Index");
                }

                TempData["ErrorMessage"] = "Appointment update failed.";
            }

            LoadDropdownData();
            return View(appointment);
        }

        // DELETE GET
        [HttpGet]
        public IActionResult Delete(int id)
        {
            var data = appointmentService.Get(id);

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
                if (appointmentService.HasPayments(id))
                {
                    TempData["ErrorMessage"] = "This appointment has payment. Delete payment first.";
                    return RedirectToAction("Index");
                }

                if (appointmentService.HasPrescriptions(id))
                {
                    TempData["ErrorMessage"] = "This appointment has prescription. Delete prescription first.";
                    return RedirectToAction("Index");
                }

                var result = appointmentService.Delete(id);

                if (result)
                {
                    TempData["SuccessMessage"] = "Appointment deleted successfully.";
                }
                else
                {
                    TempData["ErrorMessage"] = "Appointment delete failed.";
                }
            }

            return RedirectToAction("Index");
        }
    }
}