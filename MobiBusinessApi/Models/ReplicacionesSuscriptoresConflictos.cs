namespace MovilBusinessApiCore.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    

    public partial class ReplicacionesSuscriptoresConflictos
    {
        [Key]
        public int RscKey { get; set; }

        public int RepID { get; set; }

        public Guid RepSuscriptor { get; set; }

        [StringLength(64)]
        public string RscTabla { get; set; }

        public string RscScript { get; set; }

        [StringLength(64)]
        public string UsuInicioSesion { get; set; }

        public DateTime? RscFechaActualizacion { get; set; }
    }
}
