namespace MovilBusinessApiCore.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    

    public partial class TransaccionesImagenesTablas
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(15)]
        public string RepCodigo { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(64)]
        public string RepTabla { get; set; }

        [Key]
        [Column(Order = 2)]
        [StringLength(64)]
        public string RepTablaKey { get; set; }

        [Key]
        [Column(Order = 3)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int TraPosicion { get; set; }

        [Column(TypeName = "image")]
        public byte[] TraImagen { get; set; }

        [StringLength(50)]
        public string TraFormato { get; set; }

        [StringLength(50)]
        public string TraTamano { get; set; }

        public Guid rowguid { get; set; }

        [StringLength(64)]
        public string UsuInicioSesion { get; set; }

        public DateTime? TraFechaActualizacion { get; set; }
    }
}
