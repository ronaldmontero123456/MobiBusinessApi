using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MovilBusinessApiCore.Models.Internal.Structs
{
    public class PresupuestosArgs
    {
        public string RepCodigo { get; set; }
        public string RepClave { get; set; }
        public string RepVisitador { get; set; }
        public string FechaUltimaActualizacion { get; set; }
    }
}