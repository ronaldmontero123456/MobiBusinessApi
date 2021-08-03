using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MovilBusinessApiCore.Models.Internal.Structs
{
    public class PresupuestosResult
    {
        public string RepCodigo { get; set; }
        public string RepNombre { get; set; }
        public string ZonDescripcion { get; set; }
        public string PreTipo { get; set; }
        public short PreAnio { get; set; }
        public short PreMes { get; set; }
        public string ProNombre { get; set; }
        public string ProDescripcion { get; set; }
        public int LinID { get; set; }
        public string LinDescripcion { get; set; }
        public string PreReferencia { get; set; }
        public string Descripcion { get; set; }
        public string PreDescripcion { get; set; }
        public decimal PrePresupuesto { get; set; }
        public decimal PreEjecutado { get; set; }
        public decimal PreCumplimiento { get; set; }
        public int RowNumber { get; set; }

    }
}