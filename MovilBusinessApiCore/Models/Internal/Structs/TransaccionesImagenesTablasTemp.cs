using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MovilBusinessApiCore.Models.Internal.Structs
{
    public class TransaccionesImagenesTablasTemp
    {
        public string RepCodigo { get; set; }
        public string RepTabla { get; set; }
        public string RepTablaKey { get; set; }
        public int TraPosicion { get; set; }
        public byte[] TraImagen { get; set; }
        public string TraFormato { get; set; }
        public string TraTamano { get; set; }
        public int TitId { get; set; }
        public bool ForFirma { get; set; }
    }
}