using Microsoft.EntityFrameworkCore;
using SmartSolutionsTest.Data.Context;
using SmartSolutionsTest.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartSolutionsTest.Data.Seeds
{
    public static class DbSeeder
    {
        public static async Task Clear(ApplicationDbContext context)
        {
            if (!await context.OrderDetails.AnyAsync())
            {
                context.RemoveRange(await context.OrderDetails.ToListAsync());
                await context.SaveChangesAsync();
            }

            if(!await context.Orders.AnyAsync())
            {
                context.RemoveRange(await context.Orders.ToListAsync());
                await context.SaveChangesAsync();
            }

            if(!await context.Operations.AnyAsync())
            {
                context.RemoveRange(await context.Operations.ToListAsync());
                await context.SaveChangesAsync();
            }
        }

        public static async Task Seed(ApplicationDbContext context)
        {
            if(!await context.Orders.AnyAsync())
            {
                var orders = new List<Order>
                {
                    new Order { Date = DateTime.UtcNow, ClientName = "BERFRANZ SAC", Currency = "SOLES", Total = 19650 },
                    new Order { Date = DateTime.UtcNow, ClientName = "INDUSTRIAS INOX JFJ ROJAS SAC", Currency = "SOLES", Total = 5480 },
                    new Order { Date = DateTime.UtcNow, ClientName = "TIENDAS DON PEPITO EIRL", Currency = "DOLARES", Total = 1250 },
                };
                await context.Orders.AddRangeAsync(orders);
                await context.SaveChangesAsync();
            }

            if(!await context.OrderDetails.AnyAsync())
            {
                var orders = await context.Orders.ToListAsync();
                var details = new List<OrderDetail>
                {
                    new OrderDetail { OrderId = orders[0].Id, ProductDetail = "ARVEJA VERDE PARTIDA NRO1 USA", Quantity = 100, ProductPresentation = "SACO 25KG", ProductUnitPrice = 45, SubTotal = 4500 },
                    new OrderDetail { OrderId = orders[0].Id, ProductDetail = "ALPISTE NRO 1 CANADIENSE", Quantity = 150, ProductPresentation = "SACO 15KG", ProductUnitPrice = 55, SubTotal = 8250 },
                    new OrderDetail { OrderId = orders[0].Id, ProductDetail = "AVENA EN GRANO CHILENA DIONYS", Quantity = 50, ProductPresentation = "SACO 45KG", ProductUnitPrice = 90, SubTotal = 4500 },
                    new OrderDetail { OrderId = orders[0].Id, ProductDetail = "FRIJOL NAVY NRO 1 USA", Quantity = 40, ProductPresentation = "SACO 80KG", ProductUnitPrice = 60, SubTotal = 2400 },

                    new OrderDetail { OrderId = orders[1].Id, ProductDetail = "GASEOSA INKA KOLA", Quantity = 120, ProductPresentation = "BOTELLA 3L", ProductUnitPrice = 8, SubTotal = 960 },
                    new OrderDetail { OrderId = orders[1].Id, ProductDetail = "PAPEL HIGIÉNICO SUAVE GOLD ESENCIAS ELEGANTE", Quantity = 80, ProductPresentation = "PAQUETE 12 ROLLOS", ProductUnitPrice = 18, SubTotal = 1440 },
                    new OrderDetail { OrderId = orders[1].Id, ProductDetail = "DETERGENTE LÍQUIDO ARIEL REVITACOLOR", Quantity = 56, ProductPresentation = "BOTELLA 2.8L", ProductUnitPrice = 55, SubTotal = 3080 },

                    new OrderDetail { OrderId = orders[2].Id, ProductDetail = "MEMORIA USB 3.2 KINGSTON", Quantity = 40, ProductPresentation = "CAPACIDAD 64GB COLOR NEGRO", ProductUnitPrice = 8, SubTotal = 320 },
                    new OrderDetail { OrderId = orders[2].Id, ProductDetail = "DUALSHOCK 4 MANDO DE PS4", Quantity = 12, ProductPresentation = "V2.0 ORIGINAL DISEÑO DARK", ProductUnitPrice = 45, SubTotal = 540 },
                    new OrderDetail { OrderId = orders[2].Id, ProductDetail = "MOUSE LOGITECH G502 HERO KDA", Quantity = 5, ProductPresentation = "BLANCO 25K", ProductUnitPrice = 78, SubTotal = 390 },
                };
                await context.OrderDetails.AddRangeAsync(details);
                await context.SaveChangesAsync();
            }

            if(!await context.Operations.AnyAsync())
            {
                var descs = new string[] { "COMPRA PRODUCTOS IMPORTADOS", "SALIDA POR VENTAS", "SALIDA POR TRASLADO ENTRE ALMACEN", "SALIDA POR AJUSTE" };
                Operation prevOp = null;
                for (var i = 0; i < 50; i++)
                {
                    var descIndex = i > 5 ? new Random().Next(0, descs.Length) : 0;
                    Operation op = new Operation { Date = DateTime.UtcNow, Description = descs[descIndex], ProductDetail = "HARINA DE MAIZ", ProductPresentation = "PAQUETE 10KG" };
                    
                    if(descIndex > 0)
                        op.Outcome = i == 2 ? new Random().Next(800, 1000) 
                            : new Random().Next(1, 100);
                    else
                        op.Income = new Random().Next(1000, 2000);

                    op.Balance = prevOp?.Balance ?? 0;
                    op.Balance += op.Income;
                    op.Balance -= op.Outcome;

                    // Grado de Error
                    if(new Random().Next(0, 5) > 3)
                        op.Balance += new Random().Next(-10, 10);
                    
                    prevOp = op;

                    await context.Operations.AddAsync(op);
                    await context.SaveChangesAsync();
                }
            }
        }
    }
}
