using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WholesaleBase.Models;

namespace WholesaleBase.Controllers
{
    public class OrderContentsController : Controller
    {
        private readonly AppDbContext _context;

        public OrderContentsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: OrderContents
        public async Task<IActionResult> Index()
        {
            return View(await _context.OrderContents.ToListAsync());
        }

        // GET: OrderContents/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var orderContent = await _context.OrderContents
                .FirstOrDefaultAsync(m => m.Id == id);
            if (orderContent == null)
            {
                return NotFound();
            }

            return View(orderContent);
        }

        // GET: OrderContents/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: OrderContents/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Product_id,OrderNumber,Amount")] OrderContent orderContent)
        {
            if (ModelState.IsValid)
            {
                _context.Add(orderContent);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(orderContent);
        }

        // GET: OrderContents/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var orderContent = await _context.OrderContents.FindAsync(id);
            if (orderContent == null)
            {
                return NotFound();
            }
            return View(orderContent);
        }

        // POST: OrderContents/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Product_id,OrderNumber,Amount")] OrderContent orderContent)
        {
            if (id != orderContent.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(orderContent);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderContentExists(orderContent.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(orderContent);
        }

        // GET: OrderContents/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var orderContent = await _context.OrderContents
                .FirstOrDefaultAsync(m => m.Id == id);
            if (orderContent == null)
            {
                return NotFound();
            }

            return View(orderContent);
        }

        // POST: OrderContents/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var orderContent = await _context.OrderContents.FindAsync(id);
            if (orderContent != null)
            {
                _context.OrderContents.Remove(orderContent);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OrderContentExists(int id)
        {
            return _context.OrderContents.Any(e => e.Id == id);
        }
    }
}
