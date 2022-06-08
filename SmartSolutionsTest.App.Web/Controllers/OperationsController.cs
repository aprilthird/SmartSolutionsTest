using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartSolutionsTest.App.Web.ViewModels;
using SmartSolutionsTest.Data.Context;
using SmartSolutionsTest.Entities.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SmartSolutionsTest.App.Web.Controllers
{
    [Route("operaciones")]
    public class OperationsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public OperationsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(bool errors = false)
        {
            var operations = await _context.Operations
                .OrderBy(x => x.Id)
                .Select(x => new OperationViewModel
                {
                    Id = x.Id,
                    Date = x.Date.ToShortDateString(),
                    Description = x.Description,
                    ProductDetail = x.ProductDetail,
                    ProductPresentation = x.ProductPresentation,
                    Income = x.Income,
                    Outcome = x.Outcome,
                    Balance = x.Balance
                }).ToListAsync();

            if(errors)
            {
                var prevBalance = 0.00;
                foreach (var op in operations)
                {
                    op.HasError = prevBalance + op.Income - op.Outcome != op.Balance;
                    prevBalance = op.Balance;
                }
            }

            var model = new IndexViewModel
            {
                ShowErrors = errors,
                Operations = operations
            };

            return View(model);
        }

        [HttpGet("corregir")]
        public async Task<IActionResult> FixErrors()
        {
            var operations = await _context.Operations.ToListAsync();
            var prevBalance = 0.00;
            foreach (var op in operations)
            {
                op.Balance = prevBalance + op.Income - op.Outcome;
                prevBalance = op.Balance;
            }
            await _context.SaveChangesAsync();
            
            return RedirectToAction(nameof(Index), new { errors = true });
        }

        [HttpGet("regenerar")]
        public async Task<IActionResult> Random()
        {
            if (await _context.Operations.AnyAsync())
            {
                _context.Operations.RemoveRange(
                    await _context.Operations.ToListAsync());
                await _context.SaveChangesAsync();
            }

            var descs = new string[] { "COMPRA PRODUCTOS IMPORTADOS", "SALIDA POR VENTAS", "SALIDA POR TRASLADO ENTRE ALMACEN", "SALIDA POR AJUSTE" };
            Operation prevOp = null;
            for (var i = 0; i < 50; i++)
            {
                var descIndex = i > 5 ? new Random().Next(0, descs.Length) : 0;
                Operation op = new Operation { Date = DateTime.UtcNow, Description = descs[descIndex], ProductDetail = "HARINA DE MAIZ", ProductPresentation = "PAQUETE 10KG" };

                if (descIndex > 0)
                    op.Outcome = i == 2 ? new Random().Next(800, 1000)
                        : new Random().Next(1, 100);
                else
                    op.Income = new Random().Next(1000, 2000);

                op.Balance = prevOp?.Balance ?? 0;
                op.Balance += op.Income;
                op.Balance -= op.Outcome;

                // Grado de Error
                if (new Random().Next(0, 5) > 3)
                    op.Balance += new Random().Next(-10, 10);

                prevOp = op;

                await _context.Operations.AddAsync(op);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
