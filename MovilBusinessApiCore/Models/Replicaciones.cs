namespace MovilBusinessApiCore.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    

    public partial class Replicaciones
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Replicaciones()
        {
        }

        [Key]
        public int RepID { get; set; }

        [StringLength(50)]
        public string RepNombre { get; set; }

        public short? RepEstado { get; set; }

        [StringLength(64)]
        public string UsuInicioSesion { get; set; }

        public DateTime? RepFechaActualizacion { get; set; }

        public Guid rowguid { get; set; }
    }
}
