namespace MovilBusinessApiCore.Models
{
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    
    using System.Linq;

    [Table("UsuarioSistema")]
    public partial class UsuarioSistema
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public UsuarioSistema()
        {
            Estados = new HashSet<Estados>();
            Parametros = new HashSet<Parametros>();
            Pedidos = new HashSet<Pedidos>();
            PedidosDetalle = new HashSet<PedidosDetalle>();
            PedidosDetalleConfirmados = new HashSet<PedidosDetalleConfirmados>();
            PedidosPresentacion = new HashSet<PedidosPresentacion>();
            Presupuestos = new HashSet<Presupuestos>();
            Presupuestos1 = new HashSet<Presupuestos>();
            PresupuestosHistoricos = new HashSet<PresupuestosHistoricos>();
            Replicaciones = new HashSet<Replicaciones>();
            ReplicacionesTablas = new HashSet<ReplicacionesTablas>();
            Representantes = new HashSet<Representantes>();
            RepresentantesDetalle = new HashSet<RepresentantesDetalle>();
            RepresentantesParametros = new HashSet<RepresentantesParametros>();
            TransaccionesFirmas = new HashSet<TransaccionesFirmas>();
            UsosMultiples = new HashSet<UsosMultiples>();
        }

        [Key]
        [StringLength(64)]
        public string UsuInicioSesion { get; set; }

        [StringLength(11)]
        public string UsuCedula { get; set; }

        [StringLength(32)]
        public string UsuClave { get; set; }

        [StringLength(150)]
        public string UsuNombres { get; set; }

        [StringLength(150)]
        public string UsuApellidos { get; set; }

        [StringLength(150)]
        public string UsuInstitucion { get; set; }

        [StringLength(150)]
        public string UsuDepartamento { get; set; }

        public bool? UsuEstatus { get; set; }

        [StringLength(250)]
        public string UsuCorreoElectronico { get; set; }

        public int? CliID { get; set; }

        public DateTime? UsuFechaActualizacion { get; set; }

        public int? RolID { get; set; }

        [StringLength(15)]
        public string RepCodigo { get; set; }

        public bool? UsuFiltrarClientes { get; set; }

        public Guid rowguid { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Estados> Estados { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Parametros> Parametros { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Pedidos> Pedidos { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PedidosDetalle> PedidosDetalle { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PedidosDetalleConfirmados> PedidosDetalleConfirmados { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PedidosPresentacion> PedidosPresentacion { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Presupuestos> Presupuestos { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Presupuestos> Presupuestos1 { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PresupuestosHistoricos> PresupuestosHistoricos { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Replicaciones> Replicaciones { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ReplicacionesTablas> ReplicacionesTablas { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Representantes> Representantes { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<RepresentantesDetalle> RepresentantesDetalle { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<RepresentantesParametros> RepresentantesParametros { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TransaccionesFirmas> TransaccionesFirmas { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<UsosMultiples> UsosMultiples { get; set; }

        public static bool Exists(string usuInicioSesion, string usuClave, MBContext db)
        {
            if(string.IsNullOrEmpty(usuInicioSesion) || string.IsNullOrEmpty(usuClave))
            {
                return false;
            }

            foreach (UsuarioSistema user in db.UsuarioSistema.AsNoTracking().ToList())
            {
                if (user.UsuInicioSesion.Trim().ToUpper() == usuInicioSesion.Trim().ToUpper() && user.UsuClave.Trim().ToUpper() == usuClave.Trim().ToUpper())
                {
                    return true;
                }
            }

            return false;
        }
    }
}
