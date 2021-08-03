using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MovilBusinessApiCore.Models.Internal.Structs
{
    public struct VerificarSuscriptorArgs
    {
        public string RepCodigo { get; set; }
        public string RepClave { get; set; } //must be in md5
        public string Key { get; set; }
    }
}