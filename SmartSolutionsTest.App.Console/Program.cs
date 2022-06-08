using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SmartSolutionsTest.Core.Helpers;
using SmartSolutionsTest.Data.Context;
using SmartSolutionsTest.Data.Seeds;
using SmartSolutionsTest.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartSolutionsTest.App.Console
{
    class Program
    {
        private static ApplicationDbContext _context;
        private static List<Order> Orders = new List<Order>();

        static async Task Main(string[] args)
        {
            System.Console.WriteLine("=== CONEXIÓN A SQL SERVER ===");
            var services = new ServiceCollection();

            services.AddDbContext<ApplicationDbContext>(options => 
                options.UseSqlServer("Server=localhost;Database=SmartSolutionsTestDB;Trusted_Connection=True;MultipleActiveResultSets=true", 
                sqlOpts => sqlOpts.CommandTimeout((int)TimeSpan.FromMinutes(10).TotalSeconds)),
                ServiceLifetime.Transient);

            System.Console.WriteLine("=== CONEXIÓN A SQL SERVER TERMINADA ===");

            var serviceProvider = services.BuildServiceProvider();
            _context = serviceProvider.GetService<ApplicationDbContext>();

            if(Constants.SEEDER.CLEAR)
            {
                System.Console.WriteLine("=== LIMPIANDO BASE DE DATOS ===");
                await DbSeeder.Clear(_context);
            }

            if(Constants.SEEDER.ENABLED)
            {
                System.Console.WriteLine("=== LLENANDO LA BASE DE DATOSE ===");
                await DbSeeder.Seed(_context);
            }

            System.Console.WriteLine("=== OBTENIENDO DATOS ===");

            Orders = _context.Orders.Include(x => x.Details).ToListAsync().Result;

            // MENU
            ShowMenu();

            System.Console.WriteLine("PRESIONE UNA TECLA PARA SALIR...");
            System.Console.ReadLine();
        }

        static void ShowMenu()
        {
            while(true)
            {
                System.Console.WriteLine("=== MENU ===");
                System.Console.WriteLine("1. LISTAR ORDENES");
                System.Console.WriteLine("2. LISTAR DETALLE DE ORDEN");
                System.Console.WriteLine("3. LISTAR ORDENES Y DETALLES");
                System.Console.WriteLine("4. FILTRAR ORDEN Y DETALLE POR TOTAL");
                System.Console.WriteLine("5. SALIR");
                System.Console.Write("Escoge una opción: ");
                
                var option = System.Console.ReadLine();

                System.Console.WriteLine();

                if (int.TryParse(option, out int optionId))
                {
                    if(optionId > 0 && optionId <= 5)
                    {
                        if (optionId == 5)
                            break;

                        switch(optionId)
                        {
                            case 1:
                                ShowOrders(Orders);
                                break;
                            case 2:
                                System.Console.Write("Ingresa el ID de la orden: ");
                                var orderStr = System.Console.ReadLine();
                                System.Console.WriteLine();

                                if (int.TryParse(orderStr, out int orderId))
                                {
                                    if (orderId > 0 && orderId <= Orders.Count)
                                    {
                                        ShowDetails(Orders.First(x => x.Id == orderId).Details);
                                    }
                                    else
                                    {
                                        System.Console.WriteLine("Opción Inválida...");
                                    }
                                }
                                else
                                {
                                    System.Console.WriteLine("Opción Inválida...");
                                }
                                break;
                            case 3:
                                ShowOrders(Orders, withDetails: true);
                                break;
                            case 4:
                                System.Console.Write("Ingresa un Precio mínimo: ");
                                var priceStr = System.Console.ReadLine();
                                if (int.TryParse(priceStr, out int price))
                                {
                                    if (price > 0)
                                    {
                                        System.Console.WriteLine($"\n*ÓRDENES CON UN TOTAL QUE NO PASA DE {price:00.00}*");
                                        ShowOrders(Orders.Where(x => x.Total <= price).ToList());
                                        System.Console.WriteLine($"\n*ORDENES CON ALGUN DETALLE CON UN SUBTOTAL QUE NO PASA DE {price:00.00}*");
                                        var ordersf = Orders.Where(x => x.Details.Any(d => d.SubTotal <= price)).ToList();
                                        ordersf.ForEach((order) => order.Details = order.Details.Where(d => d.SubTotal <= price).ToList());
                                        ShowOrders(ordersf, withDetails: true);
                                    }
                                    else
                                    {
                                        System.Console.WriteLine("Precio Inválido...");
                                    }
                                }
                                else
                                {
                                    System.Console.WriteLine("Precio Inválido...");
                                }
                                break;
                        }
                    }
                    else
                    {
                        System.Console.WriteLine("Opción Inválida...");
                    }
                }
                else
                {
                    System.Console.WriteLine("Opción Inválida...");
                }

                System.Console.WriteLine("\n");
            }
        }
        
        static void ShowOrders(List<Order> orders, bool withDetails = false)
        {
            System.Console.WriteLine($"=== LISTADO DE ORDENES {(withDetails ? "CON DETALLE ": "")}===");
            foreach (var order in orders)
            {
                System.Console.WriteLine($"#{order.Id} | {order.ClientName} | {order.Date.ToShortDateString()} | {order.Currency} | S/ {order.Total:00.00}");
                if (withDetails)
                {
                    order.Details.ForEach(item => ShowDetail(item, tabbed: true));
                    System.Console.WriteLine($"\tTotal: {order.Details.Count} elementos");
                }
            }
            System.Console.WriteLine($"Total: {orders.Count} órdenes");
        }

        static void ShowDetails(List<OrderDetail> details)
        {
            System.Console.WriteLine($"=== DETALLE DE ORDEN ===");
            foreach(var item in details)
            {
                ShowDetail(item);
            }
            System.Console.WriteLine($"Total: {details.Count} elementos");
        }

        static void ShowDetail(OrderDetail item, bool tabbed = false)
        {
            System.Console.WriteLine($"{(tabbed ? "\t" : string.Empty)}#{item.Id} | S/ {item.ProductDetail} | {item.Quantity} | {item.ProductPresentation} | S/ {item.ProductUnitPrice:00.00} | S/ {item.SubTotal:00.00}");
        }
    }
}
