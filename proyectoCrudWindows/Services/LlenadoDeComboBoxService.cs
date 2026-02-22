using System;
using System.Data;
using System.Data.SqlClient;

namespace proyectoCrudWindows.Services
{
    public class LlenadoDeComboBoxService
    {
        private string connectionString;

        public LlenadoDeComboBoxService(string cn)
        {
            connectionString = cn;
        }

        public DataTable ObtenerClientes()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("SELECT idCliente, nombre, RFC FROM Clientes ORDER BY nombre", conn);
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    return dt;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al cargar clientes: {ex.Message}", ex);
            }
        }

        public DataTable ObtenerVehiculos(int idCliente)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(
                        "SELECT numeroDeSerie, CONCAT(marca, ' ', modelo, ' (', placas, ')') as descripcion, placas FROM Vehiculos WHERE idCliente = @idCliente ORDER BY marca",
                        conn);
                    cmd.Parameters.AddWithValue("@idCliente", idCliente);
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    return dt;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al cargar vehículos: {ex.Message}", ex);
            }
        }

        public DataTable ObtenerServicios()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("SELECT idServicio, CONCAT(nombreServicio, ' - $', CAST(costoBase AS VARCHAR)) as descripcion FROM Servicios ORDER BY nombreServicio", conn);
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    return dt;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al cargar servicios: {ex.Message}", ex);
            }
        }
    }
}
