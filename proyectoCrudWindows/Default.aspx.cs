using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using proyectoCrudWindows.Models;
using proyectoCrudWindows.Services;

namespace proyectoCrudWindows
{
    public partial class _Default : Page
    {
        private const string connectionString = "Server=MWindowsIIS.;Database=Taller;Integrated Security=True;";
        private LlenadoDeComboBoxService comboBoxService;

        protected void Page_Load(object sender, EventArgs e)
        {
            comboBoxService = new LlenadoDeComboBoxService(connectionString);
            if (!IsPostBack)
            {
                InicializarTabla();
                CargarClientes();
                CargarServicios();
                CargarVehiculos();
                AsignarFolio();
                txtFecha.Text = DateTime.Now.ToString("yyyy-MM-dd");
            }
            else
            {
                if (Table1.Rows.Count == 0)
                {
                    AgregarEncabezadosTabla();
                }
                ReconstruirTabla();
            }
        }
        private void AsignarFolio()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("SELECT ISNULL(MAX(folioDeOrden), 0) + 1 FROM OrdenesDeServicio", conn);
                    int nuevoFolio = (int)cmd.ExecuteScalar();
                    txtFolio.Text = nuevoFolio.ToString("D6");
                }
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", $"alert('Error al asignar folio: {ex.Message}');", true);
            }
        }

        private void CargarClientes()
        {
            try
            {
                DataTable dt = comboBoxService.ObtenerClientes();
                ddlCliente.DataSource = dt;
                ddlCliente.DataTextField = "nombre";
                ddlCliente.DataValueField = "idCliente";
                ddlCliente.DataBind();
                ddlCliente.Items.Insert(0, new ListItem("Seleccionar Cliente", "0"));
                lblRFC.Text = "";
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", $"alert('Error al cargar clientes: {ex.Message}');", true);
            }
        }

        private void CargarVehiculos()
        {
            ddlCarro.Items.Clear();
            ddlCarro.Items.Add(new ListItem("Seleccionar Vehículo", "0"));

            if (ddlCliente.SelectedValue == "0")
                return;

            try
            {
                int idCliente = int.Parse(ddlCliente.SelectedValue);
                DataTable dt = comboBoxService.ObtenerVehiculos(idCliente);
                ddlCarro.DataSource = dt;
                ddlCarro.DataTextField = "descripcion";
                ddlCarro.DataValueField = "numeroDeSerie";
                ddlCarro.DataBind();
                ddlCarro.Items.Insert(0, new ListItem("Seleccionar Vehículo", "0"));
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", $"alert('Error al cargar vehículos: {ex.Message}');", true);
            }
        }

        private void CargarServicios()
        {
            try
            {
                DataTable dt = comboBoxService.ObtenerServicios();
                ddlServicio.DataSource = dt;
                ddlServicio.DataTextField = "descripcion";
                ddlServicio.DataValueField = "idServicio";
                ddlServicio.DataBind();
                ddlServicio.Items.Insert(0, new ListItem("Seleccionar Servicio", "0"));
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", $"alert('Error al cargar servicios: {ex.Message}');", true);
            }
        }

        protected void ddlCliente_SelectedIndexChanged(object sender, EventArgs e)
        {
            ViewState["Servicios"] = null;  // Limpiar servicios cuando cambia cliente
            lblRFC.Text = "";
            lblPlaca.Text = "";
            LimpiarTabla();
            CargarVehiculos();
            LimpiarCostos();

            // Obtener y mostrar RFC del cliente seleccionado
            if (ddlCliente.SelectedValue != "0")
            {
                try
                {
                    DataTable dt = comboBoxService.ObtenerClientes();
                    DataRow clienteRow = dt.AsEnumerable().FirstOrDefault(r => (int)r["idCliente"] == int.Parse(ddlCliente.SelectedValue));
                    if (clienteRow != null)
                    {
                        lblRFC.Text = clienteRow["RFC"].ToString();
                    }
                }
                catch (Exception ex)
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", $"alert('Error: {ex.Message}');", true);
                }
            }
        }

        protected void ddlCarro_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Obtener y mostrar placa del vehículo seleccionado
            lblPlaca.Text = "";
            if (ddlCarro.SelectedValue != "0")
            {
                try
                {
                    int idCliente = int.Parse(ddlCliente.SelectedValue);
                    DataTable dt = comboBoxService.ObtenerVehiculos(idCliente);
                    DataRow vehiculoRow = dt.AsEnumerable().FirstOrDefault(r => (int)r["numeroDeSerie"] == int.Parse(ddlCarro.SelectedValue));
                    if (vehiculoRow != null)
                    {
                        lblPlaca.Text = vehiculoRow["placas"].ToString();
                    }
                }
                catch (Exception ex)
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", $"alert('Error: {ex.Message}');", true);
                }
            }
        }

        protected void btnAgregar_Click(object sender, EventArgs e)
        {
            if (ddlServicio.SelectedValue == "0")
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Por favor selecciona un servicio');", true);
                return;
            }

            int cantidad;
            if (!int.TryParse(txtCantidad.Text, out cantidad) || cantidad <= 0)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Ingresa una cantidad válida');", true);
                return;
            }

            try
            {
                int idServicio = int.Parse(ddlServicio.SelectedValue);
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(
                        "SELECT idServicio, Descripcion, costoBase FROM Servicios WHERE idServicio = @idServicio", 
                        conn);
                    cmd.Parameters.AddWithValue("@idServicio", idServicio);
                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        int id = (int)reader["idServicio"];
                        string descripcion = reader["Descripcion"].ToString();
                        decimal precio = (decimal)Convert.ToDouble(reader["costoBase"]);
                        decimal importe = precio * cantidad;

                        // Agregar o actualizar el servicio
                        AgregarOActualizarServicio(id, descripcion, cantidad, precio, importe);
                        ReconstruirTabla();
                        ActualizarTotal();
                        txtCantidad.Text = "1";
                    }
                }
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", $"alert('Error al agregar servicio: {ex.Message}');", true);
            }
        }

        private void AgregarOActualizarServicio(int idServicio, string descripcion, int cantidad, decimal precio, decimal importe)
        {
            DataTable dt = ObtenerServiciosDeViewState();

            // Buscar si el servicio ya existe
            DataRow servicioExistente = dt.AsEnumerable().FirstOrDefault(r => (int)r["IdServicio"] == idServicio);

            if (servicioExistente != null)
            {
                // Actualizar cantidad e importe
                int cantidadActual = (int)servicioExistente["Cantidad"];
                decimal precioActual = (decimal)servicioExistente["Precio"];

                servicioExistente["Cantidad"] = cantidadActual + cantidad;
                servicioExistente["Importe"] = (cantidadActual + cantidad) * precioActual;
            }
            else
            {
                // Agregar nuevo servicio
                DataRow row = dt.NewRow();
                row["IdServicio"] = idServicio;
                row["Descripcion"] = descripcion;
                row["Cantidad"] = cantidad;
                row["Precio"] = precio;
                row["Importe"] = importe;
                dt.Rows.Add(row);
            }

            // IMPORTANTE: Reasignar el DataTable al ViewState para forzar serialización
            ViewState["Servicios"] = dt;
        }

        private DataTable ObtenerServiciosDeViewState()
        {
            if (ViewState["Servicios"] == null)
            {
                DataTable dt = new DataTable();
                dt.Columns.Add("IdServicio", typeof(int));
                dt.Columns.Add("Descripcion", typeof(string));
                dt.Columns.Add("Cantidad", typeof(int));
                dt.Columns.Add("Precio", typeof(decimal));
                dt.Columns.Add("Importe", typeof(decimal));
                ViewState["Servicios"] = dt;
            }
            return (DataTable)ViewState["Servicios"];
        }

        private void ActualizarTotal()
        {
            decimal total = 0;
            for (int i = 1; i < Table1.Rows.Count; i++) 
            {
                string importeText = Table1.Rows[i].Cells[4].Text.Replace("$", "").Replace(",", "");
                if (decimal.TryParse(importeText, out decimal importe))
                {
                    total += importe;
                }
            }

            lblImporte.Text = "$" + total.ToString("N2");
            lblIVA.Text = "$" + (total * (decimal)0.16).ToString("N2");
            lblTotal.Text = "$" + (total + total * (decimal)0.16).ToString("N2");
        }

        protected void btnRegistrar_Click(object sender, EventArgs e)
        {
            // Validar que haya datos
            if (ddlCliente.SelectedValue == "0")
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Por favor selecciona un cliente');", true);
                return;
            }

            if (ddlCarro.SelectedValue == "0")
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Por favor selecciona un vehículo');", true);
                return;
            }

            if (Table1.Rows.Count <= 1)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Por favor agrega al menos un servicio a la tabla');", true);
                return;
            }

            try
            {
                // Crear la orden de servicio
                OrdenServicio orden = new OrdenServicio();
                orden.IdCliente = int.Parse(ddlCliente.SelectedValue);
                orden.NumeroDeSerie = int.Parse(ddlCarro.SelectedValue);
                orden.FechaDeIngreso = DateTime.Now;
                orden.FechaEstimadaDeEntrega = DateTime.Now.AddDays(5);

                // Agregar los detalles de servicios desde la tabla
                DataTable dt = ObtenerServiciosDeViewState();
                foreach (DataRow dtRow in dt.Rows)
                {
                    DetalleServicio detalle = new DetalleServicio();
                    detalle.IdServicio = (int)dtRow["IdServicio"];
                    detalle.Descripcion = dtRow["Descripcion"].ToString();
                    detalle.Cantidad = (int)dtRow["Cantidad"];
                    detalle.Precio = (decimal)dtRow["Precio"];
                    detalle.Importe = (decimal)dtRow["Importe"];

                    orden.Detalles.Add(detalle);
                    orden.CostoTotal += detalle.Importe;
                }
                orden.CostoTotal += orden.CostoTotal * (decimal)0.16; 
                // Guardar en la base de datos
                OrdenServicioService servicioOrden = new OrdenServicioService();
                bool resultado = servicioOrden.GuardarOrden(orden);

                if (resultado)
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Orden de servicio registrada exitosamente');", true);
                    LimpiarFormulario();
                }
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", $"alert('Error al registrar la orden: {ex.Message}');", true);
            }
        }

        private void LimpiarFormulario()
        {
            // Limpiar todos los controles
            ddlCliente.SelectedIndex = 0;
            ddlCarro.Items.Clear();
            ddlCarro.Items.Add(new ListItem("Seleccionar Vehículo", "0"));
            ddlServicio.SelectedIndex = 0;
            txtCantidad.Text = "1";
            lblTotal.Text = "$0.00";
            lblRFC.Text = "";
            lblPlaca.Text = "";
            lblImporte.Text = "$0.00";
            lblIVA.Text = "$0.00";
            ViewState["Servicios"] = null;
            LimpiarTabla();

            // Reinicializar
            CargarClientes();
            CargarServicios();
            AsignarFolio();
            txtFecha.Text = DateTime.Now.ToString("yyyy-MM-dd");
        }
        private void LimpiarCostos()
        {
            lblTotal.Text = "$0.00";
            lblImporte.Text = "$0.00";
            lblIVA.Text = "$0.00";
        }

        private void ReconstruirTabla()
        {
            // Limpiar todas las filas excepto el encabezado si existe
            if (Table1.Rows.Count == 0)
            {
                AgregarEncabezadosTabla();
            }

            // Eliminar todas las filas excepto el encabezado (fila 0)
            while (Table1.Rows.Count > 1)
            {
                Table1.Rows.RemoveAt(Table1.Rows.Count - 1);
            }

            // Agregar filas de servicios desde ViewState
            DataTable dt = ObtenerServiciosDeViewState();
            foreach (DataRow dtRow in dt.Rows)
            {
                int idServicio = (int)dtRow["IdServicio"];
                string descripcion = dtRow["Descripcion"].ToString();
                int cantidad = (int)dtRow["Cantidad"];
                decimal precio = (decimal)dtRow["Precio"];
                decimal importe = (decimal)dtRow["Importe"];

                AgregarFilaTabla(idServicio, descripcion, cantidad, precio, importe);
            }
        }

        private void AgregarEncabezadosTabla()
        {
            // Solo agrega los encabezados visuales SIN tocar ViewState
            TableHeaderRow headerRow = new TableHeaderRow();
            headerRow.BackColor = System.Drawing.Color.FromArgb(68, 114, 196);
            headerRow.ForeColor = System.Drawing.Color.White;

            TableHeaderCell headerCellId = new TableHeaderCell();
            headerCellId.Text = "IDServicio";
            headerCellId.Width = Unit.Percentage(15);
            headerRow.Cells.Add(headerCellId);

            TableHeaderCell headerCellDesc = new TableHeaderCell();
            headerCellDesc.Text = "Descripción";
            headerCellDesc.Width = Unit.Percentage(40);
            headerRow.Cells.Add(headerCellDesc);

            TableHeaderCell headerCellCant = new TableHeaderCell();
            headerCellCant.Text = "Cantidad";
            headerCellCant.Width = Unit.Percentage(15);
            headerRow.Cells.Add(headerCellCant);

            TableHeaderCell headerCellPrecio = new TableHeaderCell();
            headerCellPrecio.Text = "Precio Unitario";
            headerCellPrecio.Width = Unit.Percentage(15);
            headerRow.Cells.Add(headerCellPrecio);

            TableHeaderCell headerCellImporte = new TableHeaderCell();
            headerCellImporte.Text = "Importe";
            headerCellImporte.Width = Unit.Percentage(15);
            headerRow.Cells.Add(headerCellImporte);

            Table1.Rows.Add(headerRow);
        }

        private void AgregarFilaTabla(int idServicio, string descripcion, int cantidad, decimal precio, decimal importe)
        {
            TableRow row = new TableRow();
            row.BackColor = System.Drawing.Color.White;

            TableCell cellId = new TableCell();
            cellId.Text = idServicio.ToString();
            cellId.Width = Unit.Percentage(15);
            row.Cells.Add(cellId);

            TableCell cellDesc = new TableCell();
            cellDesc.Text = descripcion;
            cellDesc.Width = Unit.Percentage(40);
            row.Cells.Add(cellDesc);

            TableCell cellCant = new TableCell();
            cellCant.Text = cantidad.ToString();
            cellCant.Width = Unit.Percentage(15);
            cellCant.HorizontalAlign = HorizontalAlign.Center;
            row.Cells.Add(cellCant);

            TableCell cellPrecio = new TableCell();
            cellPrecio.Text = "$" + precio.ToString("N2");
            cellPrecio.Width = Unit.Percentage(15);
            cellPrecio.HorizontalAlign = HorizontalAlign.Right;
            row.Cells.Add(cellPrecio);

            TableCell cellImporte = new TableCell();
            cellImporte.Text = "$" + importe.ToString("N2");
            cellImporte.Width = Unit.Percentage(15);
            cellImporte.HorizontalAlign = HorizontalAlign.Right;
            cellImporte.Font.Bold = true;
            row.Cells.Add(cellImporte);

            Table1.Rows.Add(row);
        }

        private void LimpiarTabla()
        {
            // Limpiar completamente la tabla y el ViewState
            Table1.Rows.Clear();
            ViewState["Servicios"] = null;
            InicializarTabla();
        }

        private void InicializarTabla()
        {
            // Crear la tabla vacía con encabezados
            TableHeaderRow headerRow = new TableHeaderRow();
            headerRow.BackColor = System.Drawing.Color.FromArgb(68, 114, 196);
            headerRow.ForeColor = System.Drawing.Color.White;

            TableHeaderCell headerCellId = new TableHeaderCell();
            headerCellId.Text = "IDServicio";
            headerCellId.Width = Unit.Percentage(15);
            headerRow.Cells.Add(headerCellId);

            TableHeaderCell headerCellDesc = new TableHeaderCell();
            headerCellDesc.Text = "Descripción";
            headerCellDesc.Width = Unit.Percentage(40);
            headerRow.Cells.Add(headerCellDesc);

            TableHeaderCell headerCellCant = new TableHeaderCell();
            headerCellCant.Text = "Cantidad";
            headerCellCant.Width = Unit.Percentage(15);
            headerRow.Cells.Add(headerCellCant);

            TableHeaderCell headerCellPrecio = new TableHeaderCell();
            headerCellPrecio.Text = "Precio Unitario";
            headerCellPrecio.Width = Unit.Percentage(15);
            headerRow.Cells.Add(headerCellPrecio);

            TableHeaderCell headerCellImporte = new TableHeaderCell();
            headerCellImporte.Text = "Importe";
            headerCellImporte.Width = Unit.Percentage(15);
            headerRow.Cells.Add(headerCellImporte);

            Table1.Rows.Add(headerRow);

            
            DataTable dt = new DataTable();
            dt.Columns.Add("IdServicio", typeof(int));
            dt.Columns.Add("Descripcion", typeof(string));
            dt.Columns.Add("Cantidad", typeof(int));
            dt.Columns.Add("Precio", typeof(decimal));
            dt.Columns.Add("Importe", typeof(decimal));
            ViewState["Servicios"] = dt;
        }
    }
}