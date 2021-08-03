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
            ReplicacionesSuscriptoresCambios = new HashSet<ReplicacionesSuscriptoresCambios>();
            ReplicacionesTablas = new HashSet<ReplicacionesTablas>();
            ReplicacionesTablas1 = new HashSet<ReplicacionesTablas>();
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

        public virtual UsuarioSistema UsuarioSistema { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ReplicacionesSuscriptoresCambios> ReplicacionesSuscriptoresCambios { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ReplicacionesTablas> ReplicacionesTablas { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ReplicacionesTablas> ReplicacionesTablas1 { get; set; }
    }
}
