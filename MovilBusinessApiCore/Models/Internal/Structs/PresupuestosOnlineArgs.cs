using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MovilBusinessApiCore.Models.Internal.Structs
{
    public class PresupuestosOnlineArgs
    {

        public UsuarioArgs User { get; set; }
        public int Cliid { get; set; }
        public string PreTipo { get; set; }
        public string PreAnio { get; set; }
        public string PreMes { get; set; }
    }
}