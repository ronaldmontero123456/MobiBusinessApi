namespace MovilBusinessApiCore.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    

    public partial class UsosMultiples
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(15)]
        public string CodigoGrupo { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(50)]
        public string CodigoUso { get; set; }

        [StringLength(100)]
        public string Descripcion { get; set; }

        [StringLength(64)]
        public string UsuInicioSesion { get; set; }

        public DateTime? UsoFechaActualizacion { get; set; }

        public Guid rowguid { get; set; }

    }
}
