using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RoomCast.Models;
using RoomCast.Data;
using RoomCast.Models.Casting;
using RoomCast.Models.ViewModels;

namespace RoomCast.Controllers
{
    [Authorize]
    public class AlbumsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public AlbumsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // ✅ Index with optional search
        [HttpGet]
        public async Task<IActionResult> Index(string? searchString)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var query = _context.Albums
                .Where(a => a.UserId == user.Id)
                .Include(a => a.AlbumFiles)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchString))
                query = query.Where(a => a.AlbumName.Contains(searchString));

            var albums = await query.ToListAsync();
            ViewData["SearchString"] = searchString;
            return View(albums);
        }

        // ✅ Create Album (GET)
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // ✅ Create Album (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("AlbumName")] Album album)
        {
            if (!ModelState.IsValid)
                return View(album);

            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            album.UserId = user.Id;
            album.Timestamp = DateTime.UtcNow;

            _context.Albums.Add(album);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // ✅ Add Media (GET)
        [HttpGet]
        public async Task<IActionResult> AddMedia(int id)
        {
            var album = await _context.Albums.FindAsync(id);
            if (album == null) return NotFound();

            var userId = _userManager.GetUserId(User);
            var mediaFiles = await _context.MediaFiles
                .Where(m => m.UserId == userId)
                .ToListAsync();

            ViewBag.AlbumId = album.AlbumId;
            ViewBag.Files = mediaFiles;
            return View();
        }

        // ✅ Add Media (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddMedia(int albumId, int fileId)
        {
            var exists = await _context.AlbumFiles
                .AnyAsync(x => x.AlbumId == albumId && x.FileId == fileId);

            if (!exists)
            {
                _context.AlbumFiles.Add(new AlbumFile
                {
                    AlbumId = albumId,
                    FileId = fileId
                });
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Details), new { id = albumId });
        }

        // ✅ Album Details
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var album = await _context.Albums
                .Include(a => a.AlbumFiles)
                .ThenInclude(af => af.MediaFile)
                .FirstOrDefaultAsync(a => a.AlbumId == id);

            if (album == null) return NotFound();

            var viewModel = new AlbumDetailsViewModel
            {
                Album = album,
                MediaFiles = album.AlbumFiles.Select(af => af.MediaFile).ToList()
            };

            return View(viewModel);
        }

        // ✅ Edit Album (GET)
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var album = await _context.Albums.FindAsync(id);
            if (album == null) return NotFound();
            return View(album);
        }

        // ✅ Edit Album (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("AlbumId,AlbumName")] Album album)
        {
            if (id != album.AlbumId) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    var existingAlbum = await _context.Albums
                        .AsNoTracking()
                        .FirstOrDefaultAsync(a => a.AlbumId == id);

                    if (existingAlbum == null) return NotFound();

                    album.UserId = existingAlbum.UserId;
                    album.Timestamp = existingAlbum.Timestamp;

                    _context.Entry(album).State = EntityState.Modified;
                    await _context.SaveChangesAsync();

                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException ex)
                {
                    Console.WriteLine($"Error updating album: {ex.InnerException?.Message}");
                    ModelState.AddModelError("", "Error saving changes. Please ensure the album is still valid.");
                }
            }

            return View(album);
        }

        // ✅ Delete Album (GET)
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var album = await _context.Albums.FirstOrDefaultAsync(a => a.AlbumId == id);
            if (album == null) return NotFound();
            return View(album);
        }

        // ✅ Delete Album (POST)
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var album = await _context.Albums
                .Include(a => a.AlbumFiles)
                .FirstOrDefaultAsync(a => a.AlbumId == id);

            if (album != null)
            {
                // Remove related AlbumFiles before deleting Album (avoid FK error)
                if (album.AlbumFiles != null)
                    _context.AlbumFiles.RemoveRange(album.AlbumFiles);

                _context.Albums.Remove(album);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        // ✅ Assign to Screen (GET)
        [HttpGet]
        public async Task<IActionResult> AssignToScreen(int id)
        {
            var album = await _context.Albums.FindAsync(id);
            if (album == null) return NotFound();

            var screens = await _context.Screens.ToListAsync();

            var model = new AlbumAssignmentViewModel
            {
                AlbumId = id,
                Screens = screens
            };

            return View(model);
        }

        // ✅ Assign to Screen (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignToScreen(AlbumAssignmentViewModel model)
        {
            var exists = await _context.AlbumScreenAssignments
                .AnyAsync(x => x.AlbumId == model.AlbumId && x.ScreenId == model.SelectedScreenId);

            if (!exists)
            {
                var assignment = new AlbumScreenAssignment
                {
                    AlbumId = model.AlbumId,
                    ScreenId = model.SelectedScreenId
                };

                _context.AlbumScreenAssignments.Add(assignment);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
