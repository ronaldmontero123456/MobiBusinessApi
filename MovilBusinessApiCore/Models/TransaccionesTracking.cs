namespace MovilBusinessApiCore.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    

    [Table("TransaccionesTracking")]
    public partial class TransaccionesTracking
    {
        [Key]
        public Guid rowguid { get; set; }

        [Required]
        [StringLength(20)]
        public string RepCodigo { get; set; }

        public int TraID { get; set; }

        [Required]
        [StringLength(50)]
        public string TraEstado { get; set; }

        [Required]
        [StringLength(100)]
        public string TraKey { get; set; }

        [Required]
        [StringLength(150)]
        public string TraMensaje { get; set; }

        public DateTime? TraFecha { get; set; }

        [StringLength(64)]
        public string UsuInicioSesion { get; set; }

        public DateTime? TraFechaActualizacion { get; set; }
    }
}
