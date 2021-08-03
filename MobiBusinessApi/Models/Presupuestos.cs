namespace MovilBusinessApiCore.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    

    public partial class Presupuestos
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(15)]
        public string RepCodigo { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(15)]
        public string PreTipo { get; set; }

        [Key]
        [Column(Order = 2)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public short PreAnio { get; set; }

        [Key]
        [Column(Order = 3)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public short PreMes { get; set; }

        [Key]
        [Column(Order = 4)]
        [StringLength(20)]
        public string PreReferencia { get; set; }

        public decimal? PrePresupuesto { get; set; }

        public decimal? PreEjecutado { get; set; }

        [StringLength(64)]
        public string UsuInicioSesion { get; set; }

        public DateTime? PreFechaActualizacion { get; set; }

        public Guid rowguid { get; set; }

        [StringLength(15)]
        public string RepSupervisor { get; set; }

        public virtual UsuarioSistema UsuarioSistema { get; set; }

        public virtual UsuarioSistema UsuarioSistema1 { get; set; }
    }
}
