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