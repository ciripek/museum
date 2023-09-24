using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using museum.Data;
using museum.Models;
using X.PagedList;

namespace museum.Controllers;

public class LabelController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public LabelController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    // GET: Label
    public async Task<IActionResult> Index(int? page)
    {
        var labels = await _context
            .Label
            .Select(label =>
                new Label
                {
                    Color = label.Color,
                    Display = label.Display,
                    Id = label.Id,
                    Name = label.Name
                }
            ).ToPagedListAsync(page, 30);
        return View(labels);
    }

    // GET: Label/Details/5
    public async Task<IActionResult> Details(int? id, int? page)
    {
        if (id == null) return NotFound();


        var label = await _context.Label
            .Select(label1 => new Label
            {
                Id = label1.Id,
                Color = label1.Color,
                Display = label1.Display,
                Name = label1.Name
            })
            .FirstOrDefaultAsync(m => m.Id == id);

        if (label == null) return NotFound();

        var items = await _context
            .Item
            .Where(item => item.Labels.Contains(label))
            .Select(item1 => new Item
            {
                Description = item1.Description,
                Id = item1.Id,
                Image = item1.Image,
                Name = item1.Name,
                Obtained = item1.Obtained
            })
            .OrderByDescending(item => item.Obtained)
            .ToPagedListAsync(page, 12);

        var tuple = new Tuple<Label, IEnumerable<Item>>(label, items);
        return View(tuple);
    }

    // GET: Label/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: Label/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Id,Name,Display,Color")] Label label)
    {
        if (!await IsAdmin()) return View(nameof(Index));

        if (!ModelState.IsValid) return View(label);


        _context.Add(label);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    // GET: Label/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (!await IsAdmin()) return View(nameof(Index));

        if (id == null) return NotFound();

        var label = await _context.Label.FindAsync(id);
        if (label == null) return NotFound();
        return View(label);
    }

    // POST: Label/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Display,Color,CreatedAt")] Label label)
    {
        if (!await IsAdmin()) return View(nameof(Index));

        if (id != label.Id) return NotFound();

        if (!ModelState.IsValid) return View(label);

        try
        {
            _context.Update(label);
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!LabelExists(label.Id)) return NotFound();

            throw;
        }

        return RedirectToAction(nameof(Index));
    }

    // GET: Label/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (!await IsAdmin()) return View(nameof(Index));

        if (id == null) return NotFound();

        var label = await _context.Label
            .FirstOrDefaultAsync(m => m.Id == id);
        if (label == null) return NotFound();

        return View(label);
    }

    // POST: Label/Delete/5
    [HttpPost]
    [ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        if (!await IsAdmin()) return View(nameof(Index));

        var label = await _context.Label.FindAsync(id);

        if (label != null) _context.Label.Remove(label);

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool LabelExists(int id)
    {
        return _context.Label.Any(e => e.Id == id);
    }

    private async Task<bool> IsAdmin()
    {
        return await _userManager.GetUserAsync(User) is { IsAdmin: true };
    }
}