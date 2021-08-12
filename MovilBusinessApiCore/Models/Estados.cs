namespace MovilBusinessApiCore.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    

    public partial class Estados
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(100)]
        public string EstTabla { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(2)]
        public string EstEstado { get; set; }

        [StringLength(50)]
        public string EstDescripcion { get; set; }

        [StringLength(10)]
        public string estOpciones { get; set; }

        public Guid rowguid { get; set; }

        [StringLength(64)]
        public string UsuInicioSesion { get; set; }

        public DateTime? EstFechaActualizacion { get; set; }

        [StringLength(100)]
        public string EstSiguientesEstados { get; set; }
    }
}
