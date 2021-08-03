namespace MovilBusinessApiCore.Models
{
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    

    public partial class ReplicacionesSuscriptoresCambios
    {
        [Key]
        public Guid RscKey { get; set; }

        public int RepID { get; set; }

        public Guid RepSuscriptor { get; set; }

        [StringLength(64)]
        public string RscTabla { get; set; }

        public Guid RscTablarowguid { get; set; }

        [StringLength(10)]
        public string RscTipTran { get; set; }

        [StringLength(64)]
        public string UsuInicioSesion { get; set; }

        public string RscScript { get; set; }

        [JsonIgnore] public DateTime? RscFechaActualizacion { get; set; }

        [JsonIgnore] public virtual Replicaciones Replicaciones { get; set; }

        [JsonIgnore] public virtual ReplicacionesSuscriptores ReplicacionesSuscriptores { get; set; }
    }

    public partial class ReplicacionesSuscriptoresCambiosV8
    {
        [Key]
        public Guid RscKey { get; set; }

        public string RscScript { get; set; }

        [JsonIgnore] public virtual Replicaciones Replicaciones { get; set; }

        [JsonIgnore] public virtual ReplicacionesSuscriptores ReplicacionesSuscriptores { get; set; }
    }
}
