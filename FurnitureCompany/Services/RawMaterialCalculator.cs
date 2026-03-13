using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace FurnitureCompany.Services
{
    /// <summary>
    /// Метод расчёта количества сырья для производства продукции с учётом потерь.
    /// Вынесен в отдельный файл согласно требованиям.
    /// </summary>
    public class RawMaterialCalculator
    {
        private readonly string _connectionString;

        public RawMaterialCalculator(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection") ?? "";
        }

        /// <summary>
        /// Рассчитывает целое количество сырья для производства.
        /// Формула: (param1 * param2 * coefficient) * quantity * (1 + loss_percent/100), округление вверх.
        /// </summary>
        public int Calculate(int productTypeId, int materialTypeId, int quantity, double param1, double param2)
        {
            if (quantity <= 0 || param1 <= 0 || param2 <= 0)
                return -1;

            double coefficient;
            double lossPercent;

            using (var conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                var cmdPt = new SqlCommand("SELECT coefficient FROM product_types WHERE id = @id", conn);
                cmdPt.Parameters.AddWithValue("@id", productTypeId);
                var coefObj = cmdPt.ExecuteScalar();
                if (coefObj == null) return -1;
                coefficient = Convert.ToDouble(coefObj);

                var cmdMt = new SqlCommand("SELECT loss_percent FROM material_types WHERE id = @id", conn);
                cmdMt.Parameters.AddWithValue("@id", materialTypeId);
                var lossObj = cmdMt.ExecuteScalar();
                if (lossObj == null) return -1;
                lossPercent = Convert.ToDouble(lossObj);
            }

            if (coefficient < 0 || lossPercent < 0) return -1;

            double rawPerUnit = param1 * param2 * coefficient;
            double totalRaw = rawPerUnit * quantity;
            double withLosses = totalRaw * (1 + lossPercent / 100.0);

            return (int)Math.Ceiling(withLosses);
        }
    }
}
