using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MovilBusinessApiCore.Models.Internal.Structs
{
    public class PresupuestosCombosResult
    {
        public string CodigoUso { get; set; }
        public string CodigoGrupo { get; set; }
        public string Descripcion { get; set; }
        public string Value { get; set; }//Retorna el año
        public int PreAnio { get; set; }
        public int PreMes { get; set; }

    }
}