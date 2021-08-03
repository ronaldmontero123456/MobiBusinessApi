namespace MovilBusinessApiCore.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    

    public partial class PedidosSugeridos
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

        public int ProID { get; set; }

        public int? PedCantidad { get; set; }

        public int? PedCantidadDetalle { get; set; }

        [StringLength(15)]
        public string UnmCodigo { get; set; }

        public DateTime? PedFechaActualizacion { get; set; }

        [StringLength(64)]
        public string UsuInicioSesion { get; set; }

        public Guid rowguid { get; set; }

        public int? InvCantInicial { get; set; }

        [Column(TypeName = "smalldatetime")]
        public DateTime? InvInicialFecha { get; set; }

        public int? InvCantFinal { get; set; }

        [Column(TypeName = "smalldatetime")]
        public DateTime? InvFinalFecha { get; set; }

        public int? InvCantPedido { get; set; }

        public DateTime? PedFecha { get; set; }
    }
}
