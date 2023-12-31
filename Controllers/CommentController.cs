using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using museum.Data;
using museum.Models;

namespace museum.Controllers;

public class CommentController : Controller
{
    private readonly ApplicationDbContext _context;

    public CommentController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: Comment
    public async Task<IActionResult> Index()
    {
        return View(await _context.Comment.ToListAsync());
    }

    // GET: Comment/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null) return NotFound();

        var comment = await _context.Comment
            .FirstOrDefaultAsync(m => m.Id == id);
        if (comment == null) return NotFound();

        return View(comment);
    }

    // GET: Comment/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: Comment/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Id,Text,CreatedAt,UpdatedAt")] Comment comment)
    {
        if (ModelState.IsValid)
        {
            _context.Add(comment);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        return View(comment);
    }

    // GET: Comment/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();

        var comment = await _context.Comment.FindAsync(id);
        if (comment == null) return NotFound();
        return View(comment);
    }

    // POST: Comment/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,Text,CreatedAt,UpdatedAt")] Comment comment)
    {
        if (id != comment.Id) return NotFound();

        if (!ModelState.IsValid) return View(comment);

        try
        {
            _context.Update(comment);
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!CommentExists(comment.Id))
                return NotFound();
            throw;
        }

        return RedirectToAction(nameof(Index));
    }

    // GET: Comment/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();

        var comment = await _context.Comment
            .FirstOrDefaultAsync(m => m.Id == id);
        if (comment == null) return NotFound();

        return View(comment);
    }

    // POST: Comment/Delete/5
    [HttpPost]
    [ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var comment = await _context.Comment.FindAsync(id);
        if (comment != null) _context.Comment.Remove(comment);

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool CommentExists(int id)
    {
        return _context.Comment.Any(e => e.Id == id);
    }
}