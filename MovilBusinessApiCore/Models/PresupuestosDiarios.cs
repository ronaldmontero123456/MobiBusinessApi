namespace MovilBusinessApiCore.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    

    public partial class PresupuestosDiarios
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(15)]
        public string RepCodigo { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(15)]
        public string PreTipo { get; set; }

        [Key]
        [Column(Order = 2)]
        public DateTime PreFecha { get; set; }

        [Key]
        [Column(Order = 3)]
        [StringLength(20)]
        public string PreReferencia { get; set; }

        public decimal? PrePresupuesto { get; set; }

        public decimal? PreEjecutado { get; set; }

        [StringLength(64)]
        public string UsuInicioSesion { get; set; }

        public DateTime? PreFechaActualizacion { get; set; }

        public Guid rowguid { get; set; }
    }
}
