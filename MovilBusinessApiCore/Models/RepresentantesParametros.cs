namespace MovilBusinessApiCore.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    

    public partial class RepresentantesParametros
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(15)]
        public string RepCodigo { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(15)]
        public string ParReferencia { get; set; }

        [StringLength(50)]
        public string ParDescripcion { get; set; }

        [StringLength(100)]
        public string ParValor { get; set; }

        public Guid rowguid { get; set; }

        [StringLength(64)]
        public string UsuInicioSesion { get; set; }

        public DateTime? RepFechaActualizacion { get; set; }

        public virtual UsuarioSistema UsuarioSistema { get; set; }
    }
}
