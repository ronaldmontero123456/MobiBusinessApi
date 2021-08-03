namespace MovilBusinessApiCore.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    

    public partial class Representantes
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Representantes()
        {
            Pedidos = new HashSet<Pedidos>();
        }

        [Key]
        [StringLength(15)]
        public string RepCodigo { get; set; }

        [StringLength(25)]
        public string RepNombre { get; set; }

        [StringLength(25)]
        public string RepCargo { get; set; }

        [StringLength(25)]
        public string RepClasificacion { get; set; }

        [StringLength(20)]
        public string RepTelefono1 { get; set; }

        [StringLength(20)]
        public string RepTelefono2 { get; set; }

        [StringLength(10)]
        public string RepClave { get; set; }

        public bool? RepIndicadorVenta { get; set; }

        public bool? RepInventarioVentas { get; set; }

        public bool? RepPedidos { get; set; }

        public bool? RepCobros { get; set; }

        public bool? RepDevoluciones { get; set; }

        public bool? RepEntrega { get; set; }

        public bool? RepDeposito { get; set; }

        public bool? RepInvestigacion { get; set; }

        public bool? RepM1 { get; set; }

        public bool? RepM2 { get; set; }

        public bool? RepM3 { get; set; }

        public bool? RepM4 { get; set; }

        public bool? RepM5 { get; set; }

        public int? ZonID { get; set; }

        public bool? RepIndicadorSupervisor { get; set; }

        [StringLength(15)]
        public string RepSupervisor { get; set; }

        public int? RepDivision { get; set; }

        public int? AlmID { get; set; }

        public DateTime? RepFechaUltimaActualizacion { get; set; }

        [StringLength(10)]
        public string RepClaveSync { get; set; }

        public bool? RepIndicadorRutaVisitasFecha { get; set; }

        public Guid RepLicencia { get; set; }

        public int? RutID { get; set; }

        [StringLength(15)]
        public string EquID { get; set; }

        public DateTime? RepFechaActualizacion { get; set; }

        [StringLength(64)]
        public string UsuInicioSesion { get; set; }

        public Guid rowguid { get; set; }

        public short? RepEstatus { get; set; }

        public int? VehID { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Pedidos> Pedidos { get; set; }

        public virtual UsuarioSistema UsuarioSistema { get; set; }
    }
}
