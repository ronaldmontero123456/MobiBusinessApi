namespace MovilBusinessApiCore.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    

    [Table("RepresentantesDetalle")]
    public partial class RepresentantesDetalle
    {
        [Key]
        [StringLength(15)]
        public string RepCodigo { get; set; }

        public int? PedSecuencia { get; set; }

        public int? VenSecuencia { get; set; }

        public int? RecSecuencia { get; set; }

        public int? DevSecuencia { get; set; }

        public int? InvSecuencia { get; set; }

        public int? CuaSecuencia { get; set; }

        public int? EntSecuencia { get; set; }

        public int? VisSecuencia { get; set; }

        public int? DepSecuencia { get; set; }

        public int? SolSecuencia { get; set; }

        public int? ConSecuencia { get; set; }

        [StringLength(64)]
        public string UsuInicioSesion { get; set; }

        public DateTime? RepFechaActualizacion { get; set; }

        public Guid rowguid { get; set; }

    }
}
