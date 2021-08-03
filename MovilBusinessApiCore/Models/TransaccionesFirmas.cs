namespace MovilBusinessApiCore.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    

    public partial class TransaccionesFirmas
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(15)]
        public string RepCodigo { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int TitID { get; set; }

        [Key]
        [Column(Order = 2)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int TraSecuencia { get; set; }

        [StringLength(50)]
        public string TraNombre { get; set; }

        [Column(TypeName = "image")]
        public byte[] TraFirma { get; set; }

        public Guid rowguid { get; set; }

        [StringLength(64)]
        public string UsuInicioSesion { get; set; }

        public DateTime? TraFechaActualizacion { get; set; }

        public virtual UsuarioSistema UsuarioSistema { get; set; }
    }
}
