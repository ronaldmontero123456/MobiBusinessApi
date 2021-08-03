namespace MovilBusinessApiCore.Models
{
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    

    public partial class ReplicacionesTablas
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int RepID { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(50)]
        public string RepTabla { get; set; }

        [StringLength(2000)]
        public string RepColumnas { get; set; }

        [StringLength(2000)]
        public string RepCriterio { get; set; }

        public string RepScriptCreacion { get; set; }

        [StringLength(64)]
        public string UsuInicioSesion { get; set; }

        public DateTime? RepFechaActualizacion { get; set; }

        public Guid rowguid { get; set; }

        [JsonIgnore]public virtual Replicaciones Replicaciones { get; set; }

        [JsonIgnore] public virtual Replicaciones Replicaciones1 { get; set; }

        [JsonIgnore] public virtual UsuarioSistema UsuarioSistema { get; set; }
    }
}
