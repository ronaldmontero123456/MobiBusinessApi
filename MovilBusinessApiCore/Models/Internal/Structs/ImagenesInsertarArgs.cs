using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MovilBusinessApiCore.Models.Internal.Structs
{
    public class ImagenesInsertarArgs
    {
        public List<TransaccionesImagenesTablasTemp> Imagenes { get; set; }
        public UsuarioArgs User { get; set; }
    }
}