using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MovilBusinessApiCore.Models.Internal.Structs
{
    public class CambiarContrasenaArgs
    {
        public string OldPass { get; set; }
        public string NewPass { get; set; }
        public string RepCodigo { get; set; }
        public string Suscriptor { get; set; }
    }
}