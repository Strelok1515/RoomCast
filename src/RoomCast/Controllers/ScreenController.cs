using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RoomCast.Data;
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

        // GET: Screens
        public async Task<IActionResult> Index()
        {
            var screens = await _context.Screens
                .Include(s => s.ScreenMediaAssignments)
                    .ThenInclude(sma => sma.MediaFile)
                .Include(s => s.AlbumScreenAssignments)
                    .ThenInclude(asa => asa.Album)
                .ToListAsync();

            return View(screens);
        }

        // GET: Screens/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Screens/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Screen screen)
        {
            if (ModelState.IsValid)
            {
                screen.ScreenId = Guid.NewGuid();
                _context.Screens.Add(screen);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(screen);
        }

        // GET: Screens/Edit/{id}
        public async Task<IActionResult> Edit(Guid id)
        {
            var screen = await _context.Screens.FindAsync(id);
            if (screen == null) return NotFound();
            return View(screen);
        }

        // POST: Screens/Edit/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, Screen screen)
        {
            if (id != screen.ScreenId) return NotFound();

            if (ModelState.IsValid)
            {
                _context.Update(screen);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(screen);
        }

        // GET: Screens/Delete/{id}
        public async Task<IActionResult> Delete(Guid id)
        {
            var screen = await _context.Screens.FindAsync(id);
            if (screen == null) return NotFound();
            return View(screen); // Looks for Views/Screens/Delete.cshtml
        }

        // POST: Screens/Delete/{id}
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

        

        // GET: Screens/AssignMedia/{id}
        public async Task<IActionResult> AssignMedia(Guid id)
        {
            var screen = await _context.Screens.FindAsync(id);
            if (screen == null) return NotFound();

            var assignedMediaIds = await _context.ScreenMediaAssignments
                .Where(x => x.ScreenId == id)
                .Select(x => x.MediaFileId)
                .ToListAsync();

            var viewModel = new ScreenAssignmentViewModel
            {
                ScreenId = screen.ScreenId,
                ScreenName = screen.Name,
                AllMediaFiles = await _context.MediaFiles.ToListAsync(),
                SelectedMediaIds = assignedMediaIds
            };

            return View(viewModel);
        }

        // POST: Screens/AssignMedia
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignMedia(ScreenAssignmentViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.AllMediaFiles = await _context.MediaFiles.ToListAsync();
                return View(model);
            }

            var existingAssignments = _context.ScreenMediaAssignments
                .Where(x => x.ScreenId == model.ScreenId);
            _context.ScreenMediaAssignments.RemoveRange(existingAssignments);

            if (model.SelectedMediaIds != null && model.SelectedMediaIds.Any())
            {
                foreach (var fileId in model.SelectedMediaIds)
                {
                    _context.ScreenMediaAssignments.Add(new ScreenMediaAssignment
                    {
                        ScreenId = model.ScreenId,
                        MediaFileId = fileId
                    });
                }
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Screens/ScreenDisplay/{screenId}
        [Route("Screens/ScreenDisplay/{screenId}")]
        public async Task<IActionResult> ScreenDisplay(Guid screenId)
        {
            var screen = await _context.Screens
                .Include(s => s.ScreenMediaAssignments)
                    .ThenInclude(sma => sma.MediaFile)
                .Include(s => s.AlbumScreenAssignments)
                    .ThenInclude(asa => asa.Album)
                .FirstOrDefaultAsync(s => s.ScreenId == screenId);

            if (screen == null)
                return NotFound();

            var viewModel = new ScreenDisplayViewModel
            {
                ScreenName = screen.Name,
                AssignedMediaFiles = screen.ScreenMediaAssignments
                    .Select(sma => sma.MediaFile)
                    .Where(m => m != null)
                    .ToList(),
                AssignedAlbums = screen.AlbumScreenAssignments
                    .Select(asa => asa.Album)
                    .Where(a => a != null)
                    .ToList()
            };

            return View(viewModel);
        }
    }
}
