using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MovilBusinessApiCore.Models.Internal.Structs
{
    public struct QueryArgs
    {
        public UsuarioArgs User { get; set; }
        public string Query { get; set; }

    }
}