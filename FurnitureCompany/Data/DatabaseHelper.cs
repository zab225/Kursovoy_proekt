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
            _connectionString = configuration.GetConnectionString("DefaultConnection");
            if (string.IsNullOrEmpty(_connectionString))
                throw new Exception("Строка подключения не найдена");
        }

        public List<ProductType> GetProductTypes()
        {
            var list = new List<ProductType>();
            using (var conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                var cmd = new SqlCommand("SELECT id, name, coefficient FROM product_types ORDER BY name", conn);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new ProductType
                        {
                            Id = reader.GetInt32(0),
                            Name = reader.GetString(1),
                            Coefficient = reader.GetDecimal(2)
                        });
                    }
                }
            }
            return list;
        }

        /// <summary>
        /// Список продукции. Время изготовления — сумма времени в каждом цехе (целое неотрицательное).
        /// </summary>
        public List<Product> GetProductsWithProductionTime()
        {
            var list = new List<Product>();
            using (var conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                var sql = @"
                    SELECT p.id, p.product_type_id, p.name, p.article, p.min_partner_cost, p.main_material_id,
                           ISNULL(CAST(ROUND(SUM(pw.production_time_hours), 0) AS INT), 0)
                    FROM products p
                    LEFT JOIN product_workshop pw ON pw.product_id = p.id
                    GROUP BY p.id, p.product_type_id, p.name, p.article, p.min_partner_cost, p.main_material_id
                    ORDER BY p.name";
                try
                {
                    var cmd = new SqlCommand(sql, conn);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new Product
                            {
                                Id = reader.GetInt32(0),
                                ProductTypeId = reader.GetInt32(1),
                                Name = reader.GetString(2),
                                Article = reader.GetString(3),
                                MinPartnerCost = reader.GetDecimal(4),
                                MainMaterialId = reader.GetInt32(5),
                                ProductionTimeHours = Math.Max(0, reader.GetInt32(6))
                            });
                        }
                    }
                }
                catch (SqlException)
                {
                    // если product_workshop отсутствует — время = 0
                    var cmd = new SqlCommand("SELECT id, product_type_id, name, article, min_partner_cost, main_material_id FROM products ORDER BY name", conn);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new Product
                            {
                                Id = reader.GetInt32(0),
                                ProductTypeId = reader.GetInt32(1),
                                Name = reader.GetString(2),
                                Article = reader.GetString(3),
                                MinPartnerCost = reader.GetDecimal(4),
                                MainMaterialId = reader.GetInt32(5),
                                ProductionTimeHours = 0
                            });
                        }
                    }
                }
            }
            return list;
        }

        public List<MaterialType> GetMaterialTypes()
        {
            var list = new List<MaterialType>();
            using (var conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                var cmd = new SqlCommand("SELECT id, name, loss_percent FROM material_types ORDER BY name", conn);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new MaterialType
                        {
                            Id = reader.GetInt32(0),
                            Name = reader.GetString(1),
                            LossPercent = reader.GetDecimal(2)
                        });
                    }
                }
            }
            return list;
        }

        public Product GetProductById(int id)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                var cmd = new SqlCommand("SELECT id, product_type_id, name, article, min_partner_cost, main_material_id FROM products WHERE id = @id", conn);
                cmd.Parameters.AddWithValue("@id", id);
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new Product
                        {
                            Id = reader.GetInt32(0),
                            ProductTypeId = reader.GetInt32(1),
                            Name = reader.GetString(2),
                            Article = reader.GetString(3),
                            MinPartnerCost = reader.GetDecimal(4),
                            MainMaterialId = reader.GetInt32(5)
                        };
                    }
                }
            }
            return null;
        }

        public void InsertProduct(Product product)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                var cmd = new SqlCommand(@"
                    INSERT INTO products (product_type_id, name, article, min_partner_cost, main_material_id)
                    VALUES (@ptid, @name, @article, @cost, @mmid)", conn);
                cmd.Parameters.AddWithValue("@ptid", product.ProductTypeId);
                cmd.Parameters.AddWithValue("@name", product.Name);
                cmd.Parameters.AddWithValue("@article", product.Article);
                cmd.Parameters.AddWithValue("@cost", product.MinPartnerCost);
                cmd.Parameters.AddWithValue("@mmid", product.MainMaterialId);
                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Собирает данные для отчёта: сводка по продукции, количество по типам, топ по стоимости.
        /// </summary>
        public ReportViewModel GetReportData()
        {
            var report = new ReportViewModel();
            using (var conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                // 1) Общая сводка
                var cmdAgg = new SqlCommand(@"
                    SELECT COUNT(*), ISNULL(SUM(min_partner_cost), 0), ISNULL(AVG(min_partner_cost), 0)
                    FROM products", conn);
                using (var r = cmdAgg.ExecuteReader())
                {
                    if (r.Read())
                    {
                        report.TotalProductsCount = r.GetInt32(0);
                        report.TotalMinCost = r.GetDecimal(1);
                        report.AverageMinCost = r.GetDecimal(2);
                    }
                }

                // 2) Количество по типам продукции
                var cmdByType = new SqlCommand(@"
                    SELECT pt.name, COUNT(p.id) as cnt
                    FROM product_types pt
                    LEFT JOIN products p ON p.product_type_id = pt.id
                    GROUP BY pt.id, pt.name
                    ORDER BY cnt DESC", conn);
                using (var r = cmdByType.ExecuteReader())
                {
                    while (r.Read())
                        report.ProductsByType.Add(new ProductTypeCount { TypeName = r.GetString(0), Count = r.GetInt32(1) });
                }

                // 3) Топ-5 по стоимости
                var cmdTop = new SqlCommand(@"
                    SELECT TOP 5 id, product_type_id, name, article, min_partner_cost, main_material_id
                    FROM products ORDER BY min_partner_cost DESC", conn);
                using (var r = cmdTop.ExecuteReader())
                {
                    while (r.Read())
                    {
                        report.TopProductsByCost.Add(new Product
                        {
                            Id = r.GetInt32(0),
                            ProductTypeId = r.GetInt32(1),
                            Name = r.GetString(2),
                            Article = r.GetString(3),
                            MinPartnerCost = r.GetDecimal(4),
                            MainMaterialId = r.GetInt32(5),
                            ProductionTimeHours = 0
                        });
                    }
                }
            }
            return report;
        }

        /// <summary>
        /// Список цехов для производства конкретного продукта.
        /// Ожидаются таблицы: workshops(id, name), product_workshop(product_id, workshop_id, workers_count, production_time_hours)
        /// </summary>
        public List<Workshop> GetWorkshopsByProductId(int productId)
        {
            var list = new List<Workshop>();
            using (var conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                var cmd = new SqlCommand(@"
                    SELECT w.id, w.name, pw.workers_count, pw.production_time_hours
                    FROM workshops w
                    INNER JOIN product_workshop pw ON pw.workshop_id = w.id
                    WHERE pw.product_id = @pid
                    ORDER BY w.name", conn);
                cmd.Parameters.AddWithValue("@pid", productId);
                try
                {
                    using (var r = cmd.ExecuteReader())
                    {
                        while (r.Read())
                            list.Add(new Workshop
                            {
                                Id = r.GetInt32(0),
                                Name = r.GetString(1),
                                WorkersCount = r.GetInt32(2),
                                ProductionTimeHours = r.GetDecimal(3)
                            });
                    }
                }
                catch (SqlException) { /* таблицы могут отсутствовать */ }
            }
            return list;
        }

        public void UpdateProduct(Product product)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                var cmd = new SqlCommand(@"
                    UPDATE products 
                    SET product_type_id = @ptid, name = @name, article = @article, 
                        min_partner_cost = @cost, main_material_id = @mmid
                    WHERE id = @id", conn);
                cmd.Parameters.AddWithValue("@ptid", product.ProductTypeId);
                cmd.Parameters.AddWithValue("@name", product.Name);
                cmd.Parameters.AddWithValue("@article", product.Article);
                cmd.Parameters.AddWithValue("@cost", product.MinPartnerCost);
                cmd.Parameters.AddWithValue("@mmid", product.MainMaterialId);
                cmd.Parameters.AddWithValue("@id", product.Id);
                cmd.ExecuteNonQuery();
            }
        }
    }
}