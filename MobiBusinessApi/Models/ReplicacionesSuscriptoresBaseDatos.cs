namespace MovilBusinessApiCore.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    

    public partial class ReplicacionesSuscriptoresBaseDatos
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
        public int rsbSecuencia { get; set; }

        public DateTime? RepFecha { get; set; }

        [StringLength(64)]
        public string UsuIniciosesion { get; set; }

        public byte[] RepBaseDatos { get; set; }

        public DateTime? RepFechaActualizacion { get; set; }
    }
}
