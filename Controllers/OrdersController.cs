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
    public class OrdersController : Controller
    {
        private readonly AppDbContext _context;

        public OrdersController(AppDbContext context)
        {
            _context = context;
        }

        //Управление статусами
        private List<SelectListItem> GetStatusList()
        {
            return new List<SelectListItem>
            {
                new SelectListItem { Value = "Ожидает", Text = "Ожидает" },
                new SelectListItem { Value = "Отгружен", Text = "Отгружен" }
            };
        }

        // GET: Orders
        public async Task<IActionResult> Index()
        {
            return View(await _context.Orders.ToListAsync());
        }

        // GET: Orders/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .FirstOrDefaultAsync(m => m.Number == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // GET: Orders/Create
        [Authorize(Roles = "Admin,User")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Orders/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Number,Customer_id,Date,CurrentStatus")] Order order)
        {
            if (ModelState.IsValid)
            {
                _context.Add(order);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(order);
        }

        // GET: Orders/Edit/5
        [Authorize(Roles = "Admin,User")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }
            return View(order);
        }

        // POST: Orders/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Number,Customer_id,Date,CurrentStatus")] Order order)
        {
            if (id != order.Number)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(order);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderExists(order.Number))
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
            return View(order);
        }

        // GET: Orders/Delete/5
        [Authorize(Roles = "Admin,User")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .FirstOrDefaultAsync(m => m.Number == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // POST: Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }

            try
            {
                // Получить все связанные строки OrderContent
                var orderContents = await _context.OrderContents
                                                   .Where(oc => oc.OrderNumber == id)
                                                   .ToListAsync();

                foreach (var content in orderContents)
                {
                    _context.OrderContents.Remove(content); // Удаляем все связанные OrderContents
                }

                _context.Orders.Remove(order); // Удаляем сам заказ

                // Сохраняем изменения
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // Логирование ошибки или вывод пользователю сообщения
                ModelState.AddModelError(string.Empty, $"Ошибка при удалении записи: {ex.Message}");
                return RedirectToAction(nameof(ViewOrdersWithContents));
            }

            return RedirectToAction(nameof(ViewOrdersWithContents)); // Перенаправляем на страницу с заказами
        }

        //Представление ОБЩЕЕ
        public async Task<IActionResult> ViewOrdersWithContents()
        {
            // Получаем все заказы
            var orders = await _context.Orders.ToListAsync();

            // Создаем список моделей
            var orderViewModels = new List<OrderViewModel>();

            foreach (var order in orders)
            {
                // Получаем записи OrderContent для текущего заказа
                var orderContents = await _context.OrderContents
                    .Where(oc => oc.OrderNumber == order.Number)
                    .ToListAsync();

                // Создаем список OrderContentViewModel
                var orderContentViewModelPrice = new List<OrderContentViewModelPrice>();

                foreach (var content in orderContents)
                {
                    var product = await _context.Products.FirstOrDefaultAsync(p => p.Product_id == content.Product_id);

                    if (product != null)
                    {
                        orderContentViewModelPrice.Add(new OrderContentViewModelPrice
                        {
                            Id = content.Id,
                            Product_id = content.Product_id,
                            ProductName = product.Name,
                            Amount = content.Amount
                        });
                    }
                }
                
                

                // Добавляем модель заказа в список
                orderViewModels.Add(new OrderViewModel
                {
                    Order = order,
                    OrderContents = orderContentViewModelPrice
                });
            }

            return View(orderViewModels);
        }


        // GET: Orders/ChangeStatus/5
        [Authorize(Roles = "Admin,User")]
        public async Task<IActionResult> ChangeStatus(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }

            ViewBag.StatusList = GetStatusList();

            return View(order);
        }

        // POST: Orders/ChangeStatus/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeStatus(int id, [Bind("Number,CurrentStatus")] Order order)
        {
            if (id != order.Number)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existingOrder = await _context.Orders.FindAsync(id);
                    if (existingOrder == null)
                    {
                        return NotFound();
                    }

                    // Проверяем, изменился ли статус на "Выполнено"
                    if (order.CurrentStatus == "Отгружен" && existingOrder.CurrentStatus != "Отгружен")
                    {
                        // Получаем все строки OrderContent для данного заказа
                        var orderContents = await _context.OrderContents
                                                          .Where(oc => oc.OrderNumber == id)
                                                          .ToListAsync();

                        foreach (var content in orderContents)
                        {
                            // Находим соответствующий продукт
                            var product = await _context.Products.FirstOrDefaultAsync(p => p.Product_id == content.Product_id);
                            if (product != null)
                            {
                                // Проверяем, достаточно ли товара на складе
                                if (product.Amount < content.Amount)
                                {
                                    ModelState.AddModelError("", $"Недостаточно товара \"{product.Name}\" на складе. Требуется: {content.Amount}, доступно: {product.Amount}.");
                                    ViewBag.StatusList = GetStatusList();
                                    return View(order);
                                }
                            }
                        }

                        // Если проверка пройдена, уменьшаем количество товаров на складе
                        foreach (var content in orderContents)
                        {
                            var product = await _context.Products.FirstOrDefaultAsync(p => p.Product_id == content.Product_id);
                            if (product != null)
                            {
                                product.Amount -= content.Amount;
                            }
                        }
                    }

                    // Обновляем статус и дату заказа
                    existingOrder.CurrentStatus = order.CurrentStatus;
                    existingOrder.Date = DateTime.Now;

                    _context.Update(existingOrder);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderExists(order.Number))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(ViewOrdersWithContents));
            }

            ViewBag.StatusList = GetStatusList();

            return View(order);
        }


        //создаём новый заказ с содержанием
        // GET: Orders/CreateWithContents
        [Authorize(Roles = "Admin,User,Manager")]
        public IActionResult CreateWithContents()
        {
            ViewBag.Products = _context.Products
                .Select(p => new { p.Product_id, p.Name })
                .ToList();

            ViewBag.Customers = _context.Customers
                .Select(c => new { c.Customer_id, c.Name })
                .ToList();

            return View(new OrderWithContentsViewModel());
        }

        // POST: Orders/CreateWithContents
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateWithContents(OrderWithContentsViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Products = _context.Products
                    .Select(p => new { p.Product_id, p.Name })
                    .ToList();

                ViewBag.Customers = _context.Customers
                    .Select(c => new { c.Customer_id, c.Name })
                    .ToList();

                return View(viewModel);
            }


            // Создание записи в Order
            var order = new Order
            {
                Customer_id = viewModel.Customer_id,
                Date = DateTime.Now,
                CurrentStatus = viewModel.CurrentStatus
            };
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            // Создание записей в OrderContent
            foreach (var content in viewModel.OrderContents)
            {
                var orderContent = new OrderContent
                {
                    Product_id = content.Product_id,
                    Amount = content.Amount,
                    OrderNumber = order.Number
                };
                _context.OrderContents.Add(orderContent);

            }

            await _context.SaveChangesAsync();


            // Рассчитываем и обновляем TotalCost для заказа
            order.TotalCost = CalculateTotalCost(viewModel.OrderContents, order.Customer_id);
            // Сохраняем обновленную информацию о заказе
            await _context.SaveChangesAsync();


            // Обновляем PromoCodes для клиентов
            await UpdatePromoCodes();

            return RedirectToAction(nameof(ViewOrdersWithContents));
        }


        //Обновляем скидку для клиента
        private async Task UpdatePromoCodes()
        {
            // Получаем всех клиентов
            var customers = await _context.Customers.ToListAsync();

            foreach (var customer in customers)
            {
                // Считаем общую сумму покупок для клиента
                var totalSpent = await _context.Orders
                    .Where(o => o.Customer_id == customer.Customer_id)
                    .Join(
                        _context.OrderContents,
                        order => order.Number,
                        content => content.OrderNumber,
                        (order, content) => content.Amount * _context.Products
                            .FirstOrDefault(p => p.Product_id == content.Product_id).Price
                    )
                    .SumAsync();

                // Устанавливаем значение PromoCode на основе общей суммы
                if (totalSpent > 20000)
                {
                    customer.PromoCode = 0.90f;
                }
                else if (totalSpent > 10000)
                {
                    customer.PromoCode = 0.95f;
                }
                else
                {
                    customer.PromoCode = 1f;
                }

                // Обновляем запись в базе данных
                _context.Customers.Update(customer);
            }

            // Сохраняем изменения
            await _context.SaveChangesAsync();
        }


        public async Task<IActionResult> ViewOrdersWithContentsByDate(DateTime? startDate, DateTime? endDate)
        {
            // Передача значений в ViewBag для использования в представлении
            ViewBag.StartDate = startDate?.ToString("yyyy-MM-dd") ?? string.Empty;
            ViewBag.EndDate = endDate?.ToString("yyyy-MM-dd") ?? string.Empty;

            // Логика фильтрации по датам
            var ordersQuery = _context.Orders.AsQueryable();

            if (startDate.HasValue)
            {
                ordersQuery = ordersQuery.Where(o => o.Date >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                ordersQuery = ordersQuery.Where(o => o.Date <= endDate.Value);
            }

            var orders = await ordersQuery.ToListAsync();

            // Создаем список моделей
            var orderViewModels = new List<OrderViewModel>();

            foreach (var order in orders)
            {
                var orderContents = await _context.OrderContents
                    .Where(oc => oc.OrderNumber == order.Number)
                    .ToListAsync();

                var orderContentViewModelPrice = new List<OrderContentViewModelPrice>();

                foreach (var content in orderContents)
                {
                    var product = await _context.Products.FirstOrDefaultAsync(p => p.Product_id == content.Product_id);

                    if (product != null)
                    {
                        orderContentViewModelPrice.Add(new OrderContentViewModelPrice
                        {
                            Id = content.Id,
                            Product_id = content.Product_id,
                            ProductName = product.Name,
                            Amount = content.Amount
                        });
                    }
                }


                orderViewModels.Add(new OrderViewModel
                {
                    Order = order,
                    OrderContents = orderContentViewModelPrice
                });
            }

            return View("ViewOrdersWithContents", orderViewModels);
        }


        //метод для расчёта totalCost
        public decimal CalculateTotalCost(List<OrderContentViewModel> orderContents, int customerId)
        {
            // Получаем клиента по ID
            var customer = _context.Customers.FirstOrDefault(c => c.Customer_id == customerId);
            if (customer == null)
            {
                throw new Exception("Customer not found.");
            }

            // Получаем промокод клиента
            var promoCode = customer?.PromoCode ?? 1.0f; // Если промокод не задан, ставим 1 (без скидки)

            decimal totalCost = 0m;

            foreach (var content in orderContents)
            {
                var product = _context.Products.FirstOrDefault(p => p.Product_id == content.Product_id);
                if (product != null)
                {
                    // Добавляем стоимость этого продукта в общую стоимость
                    totalCost += product.Price * content.Amount;
                }
            }

            // Применяем скидку с промокодом
            decimal totalCostWithDiscount = (decimal)((float)totalCost * promoCode);

            // Возвращаем итоговую стоимость с учетом скидки
            return totalCostWithDiscount;
        }


        private bool OrderExists(int id)
        {
            return _context.Orders.Any(e => e.Number == id);
        }
    }
}
