using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MovilBusinessApiCore.Models.Internal.Structs
{
    public class ReplicacionesSuscriptoresCambiosLeerArgs
    {
        public UsuarioArgs User { get; set; }
        public int Limit { get; set; }
        public string DeleteQuery { get; set; }
        public bool IsSincronizar { get; set; }
    }
}