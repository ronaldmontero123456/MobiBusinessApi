namespace MovilBusinessApiCore.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    

    public partial class PedidosConfirmados
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

        public int CliID { get; set; }

        public DateTime? PedFecha { get; set; }

        public short? PedEstatus { get; set; }

        public int? PedTotal { get; set; }

        public int? PedIndicadorCompleto { get; set; }

        public DateTime? PedFechaEntrega { get; set; }

        public short? PedIndicadorRevision { get; set; }

        public short? PedTipoPedido { get; set; }

        [StringLength(15)]
        public string PedOrdenCompra { get; set; }

        public int? VisSecuencia { get; set; }

        [StringLength(5)]
        public string MonCodigo { get; set; }

        [StringLength(10)]
        public string SecCodigo { get; set; }

        [StringLength(10)]
        public string orvCodigo { get; set; }

        [StringLength(10)]
        public string ofvCodigo { get; set; }

        public bool? PedIndicadorContado { get; set; }

        public decimal? PedMontoConfirmado { get; set; }

        public bool? PedIndicadorPushMoney { get; set; }

        [StringLength(15)]
        public string RepVendedor { get; set; }

        public int? VenSecuencia { get; set; }

        public int? ConID { get; set; }

        public int? CuaSecuencia { get; set; }

        public int? Motid { get; set; }

        [StringLength(15)]
        public string CldDirTipo { get; set; }

        public short? PedPrioridad { get; set; }

        [StringLength(10)]
        public string PedEstadoERP { get; set; }

        [StringLength(64)]
        public string UsuInicioSesion { get; set; }

        public DateTime? PedFechaActualizacion { get; set; }

        public Guid rowguid { get; set; }

        [StringLength(15)]
        public string NumeroPedido { get; set; }

        public DateTime? PedFechaSincronizacion { get; set; }

        [StringLength(10)]
        public string LipCodigo { get; set; }

        [StringLength(15)]
        public string mbVersion { get; set; }
    }
}
