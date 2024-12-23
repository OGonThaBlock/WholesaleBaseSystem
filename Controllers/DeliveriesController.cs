using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WholesaleBase.Models;
using WholesaleBase.ViewModels;

namespace WholesaleBase.Controllers
{
    public class DeliveriesController : Controller
    {
        private readonly AppDbContext _context;

        public DeliveriesController(AppDbContext context)
        {
            _context = context;
        }

        //Управление статусами
        private List<SelectListItem> GetStatusList()
        {
            return new List<SelectListItem>
            {
                new SelectListItem { Value = "Ожидает", Text = "Ожидает" },
                new SelectListItem { Value = "Доставлено", Text = "Доставлено" }
            };
        }


        // GET: Deliveries
        public async Task<IActionResult> Index()
        {
            return View(await _context.Deliveries.ToListAsync());
        }

        // GET: Deliveries/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var delivery = await _context.Deliveries
                .FirstOrDefaultAsync(m => m.Number == id);
            if (delivery == null)
            {
                return NotFound();
            }

            return View(delivery);
        }

        // GET: Deliveries/Create
        [Authorize(Roles = "Admin,User")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Deliveries/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Number,Supplier_id,Date,CurrentStatus")] Delivery delivery)
        {
            if (ModelState.IsValid)
            {
                delivery.Date = DateTime.Now;
                delivery.CurrentStatus = "Ожидает"; 

                try
                {
                    _context.Add(delivery);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    // Логируем ошибку для диагностики
                    Console.WriteLine($"Ошибка при создании Delivery: {ex.Message}");
                }
            }
            else
            {
                // Логирование ошибок валидации
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    Console.WriteLine($"Ошибка валидации: {error.ErrorMessage}");
                }
            }
            return View(delivery);
        }

        // GET: Deliveries/Edit/5
        [Authorize(Roles = "Admin,User")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var delivery = await _context.Deliveries.FindAsync(id);
            if (delivery == null)
            {
                return NotFound();
            }

            ViewBag.StatusList = GetStatusList();
            return View(delivery);
        }

        // POST: Deliveries/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Number,Supplier_id,Date,CurrentStatus")] Delivery delivery)
        {
            if (id != delivery.Number)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(delivery);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DeliveryExists(delivery.Number))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(ViewDeliveriesWithContents));
            }
            ViewBag.StatusList = GetStatusList();
            return View(delivery);
        }

        // GET: Deliveries/Delete/5
        [Authorize(Roles = "Admin,User")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var delivery = await _context.Deliveries
                .FirstOrDefaultAsync(m => m.Number == id);
            if (delivery == null)
            {
                return NotFound();
            }

            return View(delivery);
        }

        // POST: Deliveries/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            // Найти запись Delivery
            var delivery = await _context.Deliveries.FindAsync(id);
            if (delivery == null)
            {
                return NotFound();
            }

            try
            {
                // Получить все связанные строки DeliveryContent
                var deliveryContents = await _context.DeliveryContents
                                                     .Where(dc => dc.DeliveryNumber == id)
                                                     .ToListAsync();

                foreach (var content in deliveryContents)
                {
                    _context.DeliveryContents.Remove(content);
                }
                
                _context.Deliveries.Remove(delivery);
                
                // Сохраняем изменения
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // Логирование ошибки или вывод пользователю сообщения
                ModelState.AddModelError(string.Empty, $"Ошибка при удалении записи: {ex.Message}");
                return RedirectToAction(nameof(ViewDeliveriesWithContents));
            }

            return RedirectToAction(nameof(ViewDeliveriesWithContents));
        }


        // GET: Delivery/ChangeStatus/5
        [Authorize(Roles = "Admin,User")]
        public async Task<IActionResult> ChangeStatus(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var delivery = await _context.Deliveries.FindAsync(id);
            if (delivery == null)
            {
                return NotFound();
            }

            ViewBag.StatusList = GetStatusList();

            return View(delivery);
        }

        // POST: Delivery/ChangeStatus/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeStatus(int id, [Bind("Number,CurrentStatus")] Delivery delivery)
        {
            if (id != delivery.Number)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existingDelivery = await _context.Deliveries.FindAsync(id);
                    if (existingDelivery == null)
                    {
                        return NotFound();
                    }

                    // Проверяем, изменился ли статус на "Доставлено"
                    if (delivery.CurrentStatus == "Доставлено" && existingDelivery.CurrentStatus != "Доставлено")
                    {
                        // Получаем все строки DeliveryContent для данного Delivery
                        var deliveryContents = await _context.DeliveryContents
                                                             .Where(dc => dc.DeliveryNumber == id)
                                                             .ToListAsync();

                        foreach (var content in deliveryContents)
                        {
                            // Находим соответствующий продукт
                            var product = await _context.Products.FirstOrDefaultAsync(p => p.Product_id == content.Product_id);
                            if (product != null)
                            {
                                // Обновляем количество
                                product.Amount += content.Amount;
                            }
                        }
                    }

                    // Обновляем статус и дату доставки
                    existingDelivery.CurrentStatus = delivery.CurrentStatus;
                    existingDelivery.Date = DateTime.Now;

                    _context.Update(existingDelivery);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DeliveryExists(delivery.Number))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(ViewDeliveriesWithContents));
            }

            ViewBag.StatusList = GetStatusList();

            return View(delivery);
        }



        //СОВМЕСТНОЕ ПРЕДСТАВЛЕНИЕ ДЛЯ СОЗДАНИЯ
        // GET: Deliveries/CreateWithContents
        [Authorize(Roles = "Admin,User")]
        public IActionResult CreateWithContents()
        {
            ViewBag.Products = _context.Products
                .Select(p => new { p.Product_id, p.Name })
                .ToList();
            ViewBag.Suppliers = _context.Suppliers
                .Select(c => new { c.Supplier_id, c.Name })
                .ToList();
            return View(new DeliveryWithContentsViewModel());
        }

        // POST: Deliveries/CreateWithContents
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateWithContents(DeliveryWithContentsViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Products = _context.Products
                    .Select(p => new { p.Product_id, p.Name })
                    .ToList();
                ViewBag.Suppliers = _context.Suppliers
                    .Select(c => new { c.Supplier_id, c.Name })
                    .ToList();

                return View(viewModel);
            }

            // Создание записи в Delivery
            var delivery = new Delivery
            {
                Supplier_id = viewModel.Supplier_id,
                Date = DateTime.Now,
                CurrentStatus = viewModel.CurrentStatus
            };
            _context.Deliveries.Add(delivery);
            await _context.SaveChangesAsync();

            // Создание записей в DeliveryContent
            foreach (var content in viewModel.DeliveryContents)
            {
                var deliveryContent = new DeliveryContent
                {
                    Product_id = content.Product_id,
                    Amount = content.Amount,
                    DeliveryNumber = delivery.Number
                };

                _context.DeliveryContents.Add(deliveryContent);

            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(ViewDeliveriesWithContents));
        }


        //представление общее для просмотра
        public async Task<IActionResult> ViewDeliveriesWithContents()
        {
            // Получаем все доставки
            var deliveries = await _context.Deliveries.ToListAsync();

            // Создаем список моделей
            var deliveryViewModels = new List<DeliveryViewModel>();

            foreach (var delivery in deliveries)
            {
                // Получаем записи DeliveryContent для текущей доставки
                var deliveryContents = await _context.DeliveryContents
                    .Where(dc => dc.DeliveryNumber == delivery.Number)
                    .ToListAsync();

                // Создаем список DeliveryContentViewModel
                var deliveryContentViewModelPrice = new List<DeliveryContentViewModelPrice>();

                foreach (var content in deliveryContents)
                {
                    var product = await _context.Products.FirstOrDefaultAsync(p => p.Product_id == content.Product_id);

                    if (product != null)
                    {
                        deliveryContentViewModelPrice.Add(new DeliveryContentViewModelPrice
                        {
                            Id = content.Id,
                            Product_id = content.Product_id,
                            ProductName = product.Name,
                            Amount = content.Amount
                        });
                    }
                }

                // Добавляем модель доставки в список
                deliveryViewModels.Add(new DeliveryViewModel
                {
                    Delivery = delivery,
                    DeliveryContents2 = deliveryContentViewModelPrice
                });
            }

            return View(deliveryViewModels);
        }

        public async Task<IActionResult> ViewDeliveriesWithContentsByDate(DateTime? startDate, DateTime? endDate)
        {
            // Передача значений в ViewBag для использования в представлении
            ViewBag.StartDate = startDate?.ToString("yyyy-MM-dd") ?? string.Empty;
            ViewBag.EndDate = endDate?.ToString("yyyy-MM-dd") ?? string.Empty;

            // Логика фильтрации по датам
            var deliveriesQuery = _context.Deliveries.AsQueryable();

            if (startDate.HasValue)
            {
                deliveriesQuery = deliveriesQuery.Where(d => d.Date >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                deliveriesQuery = deliveriesQuery.Where(d => d.Date <= endDate.Value);
            }

            var deliveries = await deliveriesQuery.ToListAsync();

            // Создаем список моделей
            var deliveryViewModels = new List<DeliveryViewModel>();

            foreach (var delivery in deliveries)
            {
                // Получаем записи DeliveryContent для текущей доставки
                var deliveryContents = await _context.DeliveryContents
                    .Where(dc => dc.DeliveryNumber == delivery.Number)
                    .ToListAsync();

                var deliveryContentViewModelPrice = new List<DeliveryContentViewModelPrice>();

                foreach (var content in deliveryContents)
                {
                    var product = await _context.Products.FirstOrDefaultAsync(p => p.Product_id == content.Product_id);

                    if (product != null)
                    {
                        deliveryContentViewModelPrice.Add(new DeliveryContentViewModelPrice
                        {
                            Id = content.Id,
                            Product_id = content.Product_id,
                            ProductName = product.Name,
                            Amount = content.Amount
                        });
                    }
                }

                // Добавляем модель доставки в список
                deliveryViewModels.Add(new DeliveryViewModel
                {
                    Delivery = delivery,
                    DeliveryContents2 = deliveryContentViewModelPrice
                });
            }

            return View("ViewDeliveriesWithContents", deliveryViewModels);
        }

        private bool DeliveryExists(int id)
        {
            return _context.Deliveries.Any(e => e.Number == id);
        }
    }
}
