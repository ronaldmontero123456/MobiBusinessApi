using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MovilBusinessApiCore.Models.Internal.Structs
{
    public class ImagenesCargarArgs
    {
        public string ImagePath { get; set; }
        public UsuarioArgs User { get; set; }
    }
}