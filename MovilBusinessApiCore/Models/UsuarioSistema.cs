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
        public UsuarioSistema(){}

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
