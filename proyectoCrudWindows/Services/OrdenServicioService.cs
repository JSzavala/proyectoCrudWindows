using System;
using System.Data;
using System.Data.SqlClient;
using proyectoCrudWindows.Models;

namespace proyectoCrudWindows.Services
{
    public class OrdenServicioService
    {
        private const string connectionString = "Server=MWindowsIIS.;Database=Taller;Integrated Security=True;";

        public bool GuardarOrden(OrdenServicio orden)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // Insertar la orden de servicio
                    SqlCommand cmdOrden = new SqlCommand(
                        "INSERT INTO OrdenesDeServicio (fechaDeIngreso, fechaEstimadaDeEntrega, fechaRealDeEntrega, estado, costoTotal, numeroDeSerie) " +
                        "VALUES (@fechaIngreso, @fechaEstimada, @fechaReal, @estado, @costoTotal, @numeroDeSerie); " +
                        "SELECT SCOPE_IDENTITY();",
                        conn);

                    cmdOrden.Parameters.AddWithValue("@fechaIngreso", orden.FechaDeIngreso);
                    cmdOrden.Parameters.AddWithValue("@fechaEstimada", orden.FechaEstimadaDeEntrega);
                    cmdOrden.Parameters.AddWithValue("@fechaReal", DateTime.Now);
                    cmdOrden.Parameters.AddWithValue("@estado", "abierta");
                    cmdOrden.Parameters.AddWithValue("@costoTotal", orden.CostoTotal);
                    cmdOrden.Parameters.AddWithValue("@numeroDeSerie", orden.NumeroDeSerie);

                    int folioOrden = (int)(decimal)cmdOrden.ExecuteScalar();

                    // Insertar los detalles de servicios
                    foreach (var detalle in orden.Detalles)
                    {
                        SqlCommand cmdDetalle = new SqlCommand(
                            "INSERT INTO ordenesSServicios (folioDeOrden, idServicio) VALUES (@folioDeOrden, @idServicio)",
                            conn);

                        cmdDetalle.Parameters.AddWithValue("@folioDeOrden", folioOrden);
                        cmdDetalle.Parameters.AddWithValue("@idServicio", detalle.IdServicio);

                        cmdDetalle.ExecuteNonQuery();
                    }

                    return true;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al guardar la orden de servicio: {ex.Message}", ex);
            }
        }
    }
}
