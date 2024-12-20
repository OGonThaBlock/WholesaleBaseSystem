using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WholesaleBase.Models;
using WholesaleBase.ViewModels;

namespace WholesaleBase.Controllers
{
    public class DeliveryContentsController : Controller
    {
        private readonly AppDbContext _context;

        public DeliveryContentsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: DeliveryContents
        public async Task<IActionResult> Index()
        {
            return View(await _context.DeliveryContents.ToListAsync());
        }

        // GET: DeliveryContents/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var deliveryContent = await _context.DeliveryContents
                .FirstOrDefaultAsync(m => m.Id == id);
            if (deliveryContent == null)
            {
                return NotFound();
            }

            return View(deliveryContent);
        }

        // GET: DeliveryContents/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: DeliveryContents/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Product_id,DeliveryNumber,Amount")] DeliveryContent deliveryContent)
        {
            if (ModelState.IsValid)
            {
                _context.Add(deliveryContent);
                

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(DeliveriesController.ViewDeliveriesWithContents));
            }
            return View(deliveryContent);
        }

        // GET: DeliveryContents/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var deliveryContent = await _context.DeliveryContents.FindAsync(id);
            if (deliveryContent == null)
            {
                return NotFound();
            }
            return View(deliveryContent);
        }

        // POST: DeliveryContents/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Product_id,DeliveryNumber,Amount")] DeliveryContent deliveryContent)
        {
            if (id != deliveryContent.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Получаем оригинальную запись из базы данных
                    var originalDeliveryContent = await _context.DeliveryContents.FirstOrDefaultAsync(d => d.Id == id);
                    if (originalDeliveryContent == null)
                    {
                        return NotFound();
                    }

                    // Обновляем количество продуктов на складе
                    /*
                    var product = await _context.Products.FirstOrDefaultAsync(p => p.Product_id == deliveryContent.Product_id);
                    if (product != null)
                    {
                        // Убираем старое количество
                        product.Amount -= originalDeliveryContent.Amount;
                        // Добавляем новое количество
                        product.Amount += deliveryContent.Amount;
                    }*/

                    // Обновляем данные оригинальной сущности
                    originalDeliveryContent.Product_id = deliveryContent.Product_id;
                    originalDeliveryContent.DeliveryNumber = deliveryContent.DeliveryNumber;
                    originalDeliveryContent.Amount = deliveryContent.Amount;

                    // Сохраняем изменения
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DeliveryContentExists(deliveryContent.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("ViewDeliveriesWithContents", "Deliveries");
            }
            return View(deliveryContent);
        }

        // GET: DeliveryContents/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var deliveryContent = await _context.DeliveryContents
                .FirstOrDefaultAsync(m => m.Id == id);
            if (deliveryContent == null)
            {
                return NotFound();
            }

            return View(deliveryContent);
        }

        // POST: DeliveryContents/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var deliveryContent = await _context.DeliveryContents.FindAsync(id);
            if (deliveryContent != null)
            {
                //удаление количества товара
                /*
                var product = await _context.Products.FirstOrDefaultAsync(p => p.Product_id == deliveryContent.Product_id);
                if (product != null)
                {
                    product.Amount -= deliveryContent.Amount;
                }*/
                _context.DeliveryContents.Remove(deliveryContent);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("ViewDeliveriesWithContents", "Deliveries");
        }

        private bool DeliveryContentExists(int id)
        {
            return _context.DeliveryContents.Any(e => e.Id == id);
        }
    }
}
