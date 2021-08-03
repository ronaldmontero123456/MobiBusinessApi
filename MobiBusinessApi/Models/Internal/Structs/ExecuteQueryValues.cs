using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MovilBusinessApiCore.Models.Internal.Structs
{
    public class ExecuteQueryValues
    {
        public string Query { get; set; }
        public string TableName { get; set; }
        public string rowguid { get; set; }
        public string TipoScript { get; set; }
    }
}