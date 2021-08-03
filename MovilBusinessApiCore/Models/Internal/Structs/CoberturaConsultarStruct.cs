using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MovilBusinessApiCore.Models.Internal.Structs
{
    public class CoberturaConsultarStruct
    {
        public int Count { get; set; }
        public List<string> Columnas { get; set; }
        public List<String> Col_Alig { get; set; }
        public List<Dictionary<String, String>> Registros { get; set; }
    }
}