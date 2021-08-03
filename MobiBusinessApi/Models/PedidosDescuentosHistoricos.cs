namespace MovilBusinessApiCore.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    

    public partial class PedidosDescuentosHistoricos
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
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int PedPosicion { get; set; }

        [Key]
        [Column(Order = 3)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int DesID { get; set; }

        public int? ProID { get; set; }

        public int? CliID { get; set; }

        public decimal? DesValorDescuento { get; set; }

        [StringLength(1)]
        public string DesMetodo { get; set; }

        [StringLength(1)]
        public string DesContrato { get; set; }

        [StringLength(1)]
        public string DesEscalon { get; set; }

        [StringLength(64)]
        public string UsuInicioSesion { get; set; }

        public DateTime? PedFechaActualizacion { get; set; }

        [StringLength(15)]
        public string PedNumeroERP { get; set; }
    }
}
