using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using museum.Data;
using museum.Models;
using X.PagedList;

namespace museum.Controllers;

public class ItemController : Controller
{
    private readonly ApplicationDbContext _context;

    public ItemController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: Item
    public async Task<IActionResult> Index(int? page)
    {
        page ??= 1;
        var items = await _context.Item
            .Select(item => new Item
            {
                Id = item.Id,
                Name = item.Name,
                Description = item.Description,
                Obtained = item.Obtained,
                Image = item.Image
            })
            .OrderByDescending(item => item.Obtained)
            .ToPagedListAsync(page, 25);

        return View(items);
    }

    // GET: Item/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null) return NotFound("The id is null");

        var itemObject = await _context
            .Item
            .Select(item => new Item
            {
                Id = item.Id,
                Name = item.Name,
                Description = item.Description,
                Obtained = item.Obtained,
                Image = item.Image,
                Labels = item.Labels.Select(label => new Label
                {
                    Name = label.Name,
                    Color = label.Color,
                    Display = label.Display
                }).ToList(),

                Comments = item.Comments.Select(comment => new Comment
                {
                    Text = comment.Text,
                    Item = null!,
                    ApplicationUser = new ApplicationUser
                    {
                        Email = comment.ApplicationUser.Email,
                        Id = comment.ApplicationUser.Id
                    }
                }).ToList()
            })
            .AsSplitQuery()
            .FirstOrDefaultAsync(m => m.Id == id);
        if (itemObject == null) return NotFound($"Can't find id with {id} value");

        return View(itemObject);
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
        if (!ModelState.IsValid) return View(item);

        _context.Add(item);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    // GET: Item/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();

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
        if (id == null) return NotFound();

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
        var item = await _context.Item.FindAsync(id);
        if (item != null) _context.Item.Remove(item);

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool ItemExists(int id)
    {
        return _context.Item.Any(e => e.Id == id);
    }
}