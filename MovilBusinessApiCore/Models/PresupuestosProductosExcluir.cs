namespace MovilBusinessApiCore.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    

    [Table("PresupuestosProductosExcluir")]
    public partial class PresupuestosProductosExcluir
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public short PreAnio { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public short PreMes { get; set; }

        [Key]
        [Column(Order = 2)]
        [StringLength(15)]
        public string PreTipo { get; set; }

        [Key]
        [Column(Order = 3)]
        [StringLength(15)]
        public string Proid { get; set; }

        [StringLength(64)]
        public string UsuInicioSesion { get; set; }

        public DateTime? PreFechaActualizacion { get; set; }

        public Guid rowguid { get; set; }
    }
}
