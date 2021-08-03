namespace MovilBusinessApiCore.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    

    [Table("PedidosPresentacion")]
    public partial class PedidosPresentacion
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(15)]
        public string RepCodigo { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int PedSecuencia { get; set; }

        [Required]
        [StringLength(50)]
        public string PedSolicitante { get; set; }

        [Required]
        [StringLength(50)]
        public string PedCalle { get; set; }

        [Required]
        [StringLength(50)]
        public string PedCiudad { get; set; }

        [Required]
        [StringLength(50)]
        public string PedTelefono { get; set; }

        [Required]
        [StringLength(20)]
        public string PedRNC { get; set; }

        [StringLength(64)]
        public string UsuInicioSesion { get; set; }

        public DateTime? PedFechaActualizacion { get; set; }

        public Guid rowguid { get; set; }

        public virtual UsuarioSistema UsuarioSistema { get; set; }
    }
}
