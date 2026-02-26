using Microsoft.Data.SqlClient;
using FurnitureCompany.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;

namespace FurnitureCompany.Data
{
    public class DatabaseHelper
    {
        private readonly string _connectionString;

        public DatabaseHelper(IConfiguration configuration)
        {
            // вычитываем строку из конфига
            var connStr = configuration.GetConnectionString("DefaultConnection");
            if (string.IsNullOrEmpty(connStr))
            {
                throw new Exception("А где строка подключения? Добавь в appsettings.json!");
            }
            _connectionString = connStr;
        }

        public List<Product> GetProductsWithProductionTime()
        {
            // тут будем складывать результат
            var productList = new List<Product>();

            // using гарантирует, что закроется даже при ошибке
            using (var dbConnection = new SqlConnection(_connectionString))
            {
                try
                {
                    dbConnection.Open();

                    // запрос: группируем продукты и суммируем время по цехам
                    // если в цехах ничего нет — SUM вернёт null, тогда ISNULL подставит 0
                    string sql = @"
                        SELECT 
                            p.id,
                            p.name,
                            p.article,
                            p.min_partner_cost,
                            CEILING(ISNULL(SUM(pw.production_time_hours), 0)) AS total_hours
                        FROM products p
                        LEFT JOIN product_workshops pw ON p.id = pw.product_id
                        GROUP BY p.id, p.name, p.article, p.min_partner_cost
                        ORDER BY p.name";

                    using (var sqlCmd = new SqlCommand(sql, dbConnection))
                    using (var rdr = sqlCmd.ExecuteReader())
                    {
                        // пока читаем, добавляем объекты в список
                        while (rdr.Read())
                        {
                            var product = new Product
                            {
                                Id = rdr.GetInt32(0),
                                Name = rdr.GetString(1),
                                Article = rdr.GetString(2),
                                MinPartnerCost = rdr.GetDecimal(3),
                                // Округляем вверх, чтобы не было 0 часов (иначе менеджеры занервничают)
                                ProductionTimeHours = Convert.ToInt32(rdr.GetDecimal(4))
                            };
                            productList.Add(product);
                        }
                    }
                }
                catch (SqlException ex)
                {
                    // пишем в консоль, чтоб видеть в отладке
                    Console.WriteLine($"SQL ERROR: {ex.Message}");
                    // возвращаем пустой список, но не падаем
                }
            }

            return productList;
        }
    }
}