using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using museum.Data;
using museum.Models;

namespace museum.Controllers;

public class ItemController : Controller
{
    private readonly ApplicationDbContext _context;

    public ItemController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: Item
    public async Task<IActionResult> Index()
    {
        return _context.Item != null
            ? View(await _context.Item
                .OrderByDescending(item => item.Obtained)
                .ToListAsync())
            : Problem("Entity set 'ApplicationDbContext.Item'  is null.");
    }

    // GET: Item/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null || _context.Item == null) return NotFound();

        var item = await _context
            .Item
            .Include(item1 => item1.Labels)
            .Include(item1 => item1.Comments)!
            .ThenInclude(comment => comment.ApplicationUser)
            .Where(item1 => item1.Id == id)
            .FirstOrDefaultAsync(m => m.Id == id);
        if (item == null) return NotFound();

        return View(item);
    }

    // GET: Item/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: Item/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Id,Name,Description,Obtained,Image,CreatedAt,UpdatedAt")] Item item)
    {
        if (ModelState.IsValid)
        {
            _context.Add(item);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        return View(item);
    }

    // GET: Item/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null || _context.Item == null) return NotFound();

        var item = await _context.Item.FindAsync(id);
        if (item == null) return NotFound();
        return View(item);
    }

    // POST: Item/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id,
        [Bind("Id,Name,Description,Obtained,Image,CreatedAt,UpdatedAt")]
        Item item)
    {
        if (id != item.Id) return NotFound();

        if (!ModelState.IsValid) return View(item);

        try
        {
            _context.Update(item);
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!ItemExists(item.Id))
                return NotFound();
            throw;
        }

        return RedirectToAction(nameof(Index));
    }

    // GET: Item/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null || _context.Item == null) return NotFound();

        var item = await _context.Item
            .FirstOrDefaultAsync(m => m.Id == id);
        if (item == null) return NotFound();

        return View(item);
    }

    // POST: Item/Delete/5
    [HttpPost]
    [ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        if (_context.Item == null) return Problem("Entity set 'ApplicationDbContext.Item'  is null.");
        var item = await _context.Item.FindAsync(id);
        if (item != null) _context.Item.Remove(item);

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool ItemExists(int id)
    {
        return (_context.Item?.Any(e => e.Id == id)).GetValueOrDefault();
    }
}