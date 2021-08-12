using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MovilBusinessApiCore.Models.Internal.Structs
{
    public class PresupuestosCombosArgs
    {
        public UsuarioArgs User { get; set; }
        public int Tipo { get; set; }
        public int Campo { get; set; }
        public string Repcodigo { get; set; } = null;
        public string PreTipo { get; set; } = null;
        public int?  PreAnio { get; set; }
    }
}