using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RoomCast.Data;
using RoomCast.Models;
using RoomCast.Models.Casting;
using RoomCast.ViewModels;

namespace RoomCast.Controllers
{
    public class ScreensController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ScreensController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ---------------------------------------------------------
        // LIST SCREENS
        // ---------------------------------------------------------
        public async Task<IActionResult> Index()
        {
            var screens = await _context.Screens
                .Include(s => s.ScreenMediaAssignments).ThenInclude(m => m.MediaFile)
                .Include(s => s.AlbumScreenAssignments).ThenInclude(a => a.Album)
                .ToListAsync();

            return View(screens);
        }

        // ---------------------------------------------------------
        // CREATE
        // ---------------------------------------------------------
        public IActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Screen screen)
        {
            if (!ModelState.IsValid)
                return View(screen);

            screen.ScreenId = Guid.NewGuid();
            _context.Screens.Add(screen);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // ---------------------------------------------------------
        // EDIT
        // ---------------------------------------------------------
        public async Task<IActionResult> Edit(Guid id)
        {
            var screen = await _context.Screens.FindAsync(id);
            if (screen == null)
                return NotFound();

            return View(screen);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, Screen screen)
        {
            if (id != screen.ScreenId)
                return NotFound();

            if (!ModelState.IsValid)
                return View(screen);

            _context.Update(screen);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // ---------------------------------------------------------
        // DELETE
        // ---------------------------------------------------------
        public async Task<IActionResult> Delete(Guid id)
        {
            var screen = await _context.Screens.FindAsync(id);
            if (screen == null)
                return NotFound();

            return View(screen);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var screen = await _context.Screens.FindAsync(id);
            if (screen != null)
            {
                _context.Screens.Remove(screen);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        // ---------------------------------------------------------
        // ASSIGN MEDIA (GET)
        // ---------------------------------------------------------
        public async Task<IActionResult> AssignMedia(Guid id)
        {
            var screen = await _context.Screens.FindAsync(id);
            if (screen == null)
                return NotFound();

            var assignedMedia = await _context.ScreenMediaAssignments
                .Where(x => x.ScreenId == id)
                .Select(x => x.MediaFileId)
                .ToListAsync();

            var vm = new ScreenAssignmentViewModel
            {
                ScreenId = id,
                ScreenName = screen.Name,
                AllMediaFiles = await _context.MediaFiles.ToListAsync(),
                SelectedMediaIds = assignedMedia
            };

            return View(vm);
        }

        // ---------------------------------------------------------
        // ASSIGN MEDIA (POST)
        // ---------------------------------------------------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignMedia(ScreenAssignmentViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.AllMediaFiles = await _context.MediaFiles.ToListAsync();
                return View(model);
            }

            var removeOld = _context.ScreenMediaAssignments
                .Where(x => x.ScreenId == model.ScreenId);

            _context.RemoveRange(removeOld);

            if (model.SelectedMediaIds != null)
            {
                foreach (var id in model.SelectedMediaIds)
                {
                    _context.ScreenMediaAssignments.Add(new ScreenMediaAssignment
                    {
                        ScreenId = model.ScreenId,
                        MediaFileId = id
                    });
                }
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // ---------------------------------------------------------
        // KIOSK DISPLAY + PREVIEW ROUTE (CRITICAL)
        // ---------------------------------------------------------
        [AllowAnonymous]
        [HttpGet("Screens/ScreenDisplay/{screenId:guid}")]
        public async Task<IActionResult> ScreenDisplay(Guid screenId)
        {
            var screen = await _context.Screens
                .Include(s => s.ScreenMediaAssignments).ThenInclude(f => f.MediaFile)
                .Include(s => s.AlbumScreenAssignments).ThenInclude(a => a.Album)
                .FirstOrDefaultAsync(s => s.ScreenId == screenId);

            if (screen == null)
                return Content("Screen not found");

            var vm = new ScreenDisplayViewModel
            {
                ScreenName = screen.Name,
                AssignedMediaFiles = screen.ScreenMediaAssignments.Select(x => x.MediaFile).ToList(),
                AssignedAlbums = screen.AlbumScreenAssignments.Select(x => x.Album).ToList()
            };

            return View(vm);
        }

        // ---------------------------------------------------------
        // START CASTING (POST BUTTON)
        // ---------------------------------------------------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> StartCasting(Guid screenId)
        {
            var cast = await _context.CastingAssignments.FirstAsync(c => c.Id == 1);
            cast.CurrentScreenId = screenId;

            await _context.SaveChangesAsync();
            TempData["Message"] = "Casting started!";
            return RedirectToAction(nameof(Index));
        }

        // ---------------------------------------------------------
        // START CASTING (GET URL ACCESS)
        // ---------------------------------------------------------
        [HttpGet("Screens/StartCasting")]
        [AllowAnonymous]
        public async Task<IActionResult> StartCastingGet(Guid? screenId)
        {
            if (screenId == null)
                return Content("ERROR: Missing screenId. Use /Screens/StartCasting?screenId=GUID");

            var cast = await _context.CastingAssignments.FirstAsync(c => c.Id == 1);
            cast.CurrentScreenId = screenId.Value;
            await _context.SaveChangesAsync();

            return Content($"Casting started for screen {screenId.Value}");
        }

        // ---------------------------------------------------------
        // STOP CASTING (POST BUTTON)
        // ---------------------------------------------------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> StopCasting()
        {
            var cast = await _context.CastingAssignments.FirstAsync(c => c.Id == 1);
            cast.CurrentScreenId = null;

            await _context.SaveChangesAsync();
            TempData["Message"] = "Casting stopped!";
            return RedirectToAction(nameof(Index));
        }

        // ---------------------------------------------------------
        // STOP CASTING (GET URL ACCESS)
        // ---------------------------------------------------------
        [HttpGet("Screens/StopCasting")]
        [AllowAnonymous]
        public async Task<IActionResult> StopCastingGet()
        {
            var cast = await _context.CastingAssignments.FirstAsync(c => c.Id == 1);
            cast.CurrentScreenId = null;
            await _context.SaveChangesAsync();

            return Content("Casting stopped");
        }
    }
}
