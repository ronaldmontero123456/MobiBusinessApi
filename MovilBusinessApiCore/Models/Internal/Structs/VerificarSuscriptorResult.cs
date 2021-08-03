using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MovilBusinessApiCore.Models.Internal.Structs
{
    public struct VerificarSuscriptorResult
    {
        public string RepSuscriptor { get; set; }
        public long CantidadCambios { get; set; }
    }
}