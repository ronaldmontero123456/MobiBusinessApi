namespace MovilBusinessApiCore.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    

    public partial class ReplicacionesSuscriptores
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ReplicacionesSuscriptores()
        {
            ReplicacionesSuscriptoresCambios = new HashSet<ReplicacionesSuscriptoresCambios>();
        }

        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int RepID { get; set; }

        [Key]
        [Column(Order = 1)]
        public Guid RepSuscriptor { get; set; }

        public DateTime? RepFechaCargaInicial { get; set; }

        public DateTime? RepFechaUltimaSincronizacion { get; set; }

        public short? resEstado { get; set; }

        [StringLength(64)]
        public string UsuInicioSesion { get; set; }

        public DateTime? RepFechaActualizacion { get; set; }

        public short? RsuTipo { get; set; }

        public Guid rowguid { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ReplicacionesSuscriptoresCambios> ReplicacionesSuscriptoresCambios { get; set; }
    }
}
