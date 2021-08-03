namespace MovilBusinessApiCore.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    

    [Table("PedidosBackOrder")]
    public partial class PedidosBackOrder
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(15)]
        public string RepCodigo { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int CliID { get; set; }

        [Key]
        [Column(Order = 2)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int PedPosicion { get; set; }

        public DateTime? PedFecha { get; set; }

        public int ProID { get; set; }

        public int? PedCantidad { get; set; }

        public int? PedCantidadDetalle { get; set; }

        [StringLength(15)]
        public string UnmCodigo { get; set; }

        public DateTime? PedFechaActualizacion { get; set; }

        [StringLength(64)]
        public string UsuInicioSesion { get; set; }

        public Guid rowguid { get; set; }
    }
}
