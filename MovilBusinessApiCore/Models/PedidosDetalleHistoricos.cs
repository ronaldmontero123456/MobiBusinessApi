namespace MovilBusinessApiCore.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    

    public partial class PedidosDetalleHistoricos
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(15)]
        public string RepCodigo { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int PedSecuencia { get; set; }

        [Key]
        [Column(Order = 2)]
        [StringLength(20)]
        public string PedNumeroERP { get; set; }

        [Key]
        [Column(Order = 3)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int PedPosicion { get; set; }

        public int ProID { get; set; }

        public int? CliID { get; set; }

        public int? PedCantidad { get; set; }

        public int? PedCantidadDetalle { get; set; }

        public decimal? PedPrecio { get; set; }

        public decimal? PedItbis { get; set; }

        public decimal? PedSelectivo { get; set; }

        public decimal? PedAdValorem { get; set; }

        public decimal? PedDescuento { get; set; }

        public bool? PedIndicadorCompleto { get; set; }

        public bool? PedIndicadorOferta { get; set; }

        [StringLength(5)]
        public string CedCodigo { get; set; }

        public int? OfeID { get; set; }

        public decimal? PedDesPorciento { get; set; }

        [StringLength(64)]
        public string UsuInicioSesion { get; set; }

        public DateTime? pedFechaActualizacion { get; set; }

        public Guid rowguid { get; set; }

        public short? PedTipoOferta { get; set; }

        [StringLength(15)]
        public string UnmCodigo { get; set; }

        public short? PedEstatus { get; set; }

        public decimal? PedCantidadInventario { get; set; }

        public int? AutSecuencia { get; set; }

        [StringLength(15)]
        public string RepSupervisor { get; set; }

        public decimal? PedFlete { get; set; }
    }
}
