using System;
using System.Collections.Generic;
using proyectoCrudWindows.Models;

namespace proyectoCrudWindows.Models
{
    public class OrdenServicio
    {
        public int Folio { get; set; }
        public DateTime FechaDeIngreso { get; set; }
        public DateTime FechaEstimadaDeEntrega { get; set; }
        public int IdCliente { get; set; }
        public int NumeroDeSerie { get; set; }
        public List<DetalleServicio> Detalles { get; set; }
        public decimal CostoTotal { get; set; }

        public OrdenServicio()
        {
            Detalles = new List<DetalleServicio>();
            FechaDeIngreso = DateTime.Now;
            FechaEstimadaDeEntrega = DateTime.Now.AddDays(5);
        }
    }
}
