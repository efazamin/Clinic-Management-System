using BLL.DTOs;
using BLL.Services;
using Microsoft.AspNetCore.Mvc;

namespace ClinicApp.Controllers
{
    public class RoomController : Controller
    {
        private readonly RoomService roomService;

        public RoomController(RoomService roomService)
        {
            this.roomService = roomService;
        }

        // INDEX
        public IActionResult Index()
        {
            var data = roomService.Get();
            return View(data);
        }

        // CREATE GET
        [HttpGet]
        public IActionResult Create()
        {
            return View(new RoomDTO());
        }

        // CREATE POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(RoomDTO room)
        {
            if (string.IsNullOrWhiteSpace(room.RoomNo))
            {
                ModelState.AddModelError("RoomNo", "Please enter room number");
            }

            if (room.FloorNo < 0)
            {
                ModelState.AddModelError("FloorNo", "Floor number cannot be negative");
            }

            if (string.IsNullOrWhiteSpace(room.RoomType))
            {
                ModelState.AddModelError("RoomType", "Please enter room type");
            }

            if (ModelState.IsValid)
            {
                var result = roomService.Create(room);

                if (result)
                {
                    TempData["SuccessMessage"] = "Room created successfully.";
                    return RedirectToAction("Index");
                }
            }

            return View(room);
        }

        // EDIT GET
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var data = roomService.Get(id);

            if (data == null)
            {
                return NotFound();
            }

            return View(data);
        }

        // EDIT POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(RoomDTO room)
        {
            if (string.IsNullOrWhiteSpace(room.RoomNo))
            {
                ModelState.AddModelError("RoomNo", "Please enter room number");
            }

            if (room.FloorNo < 0)
            {
                ModelState.AddModelError("FloorNo", "Floor number cannot be negative");
            }

            if (string.IsNullOrWhiteSpace(room.RoomType))
            {
                ModelState.AddModelError("RoomType", "Please enter room type");
            }

            if (ModelState.IsValid)
            {
                var result = roomService.Update(room);

                if (result)
                {
                    TempData["SuccessMessage"] = "Room updated successfully.";
                    return RedirectToAction("Index");
                }
            }

            return View(room);
        }

        // DELETE GET
        [HttpGet]
        public IActionResult Delete(int id)
        {
            var data = roomService.Get(id);

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
                if (roomService.HasAppointments(id))
                {
                    TempData["ErrorMessage"] = "This room has appointments. Delete appointments first.";
                    return RedirectToAction("Index");
                }

                var result = roomService.Delete(id);

                if (result)
                {
                    TempData["SuccessMessage"] = "Room deleted successfully.";
                }
                else
                {
                    TempData["ErrorMessage"] = "Room delete failed.";
                }
            }

            return RedirectToAction("Index");
        }
    }
}