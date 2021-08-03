namespace MovilBusinessApiCore.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("ClientesReplicacionesUrl")]
    public partial class ClientesReplicacionesUrl
    {
        [Key]
        [StringLength(10)]
        public string CliUrlKey { get; set; }

        [Required]
        [StringLength(200)]
        public string CliNombre { get; set; }

        [Required]
        [StringLength(200)]
        public string CliUrl { get; set; }

        public DateTime? CliFechaActualizacion { get; set; }

        [StringLength(64)]
        public string UsuInicioSesion { get; set; }

        public Guid rowguid { get; set; }

        [StringLength(8)]
        public string CliReplicacionVersion { get; set; }
    }
}
