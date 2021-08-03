namespace MovilBusinessApiCore.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    

    public partial class ReplicacionesSuscriptoresSincronizaciones
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int RepID { get; set; }

        [Key]
        [Column(Order = 1)]
        public Guid RepSuscriptor { get; set; }

        [Key]
        [Column(Order = 2)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int RssSecuencia { get; set; }

        [StringLength(64)]
        public string UsuInicioSesion { get; set; }

        public double? RssLatitud { get; set; }

        public double? RssLongitud { get; set; }

        public short? RssEstatus { get; set; }

        public DateTime? RssFechaInicio { get; set; }

        public DateTime? RssFechaFin { get; set; }

        public int? RssCantidadRegistros { get; set; }

        public Guid? rowguid { get; set; }

        public DateTime? RepFechaActualizacion { get; set; }
    }
}
