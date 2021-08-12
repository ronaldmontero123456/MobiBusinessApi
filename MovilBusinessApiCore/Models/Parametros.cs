namespace MovilBusinessApiCore.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;


    public partial class Parametros
    {
        //[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ParKey { get; set; }

        [Key]
        [StringLength(15)]
        public string ParReferencia { get; set; }

        [StringLength(50)]
        public string ParDescripcion { get; set; }

        [StringLength(50)]
        public string ParValor { get; set; }

        [StringLength(64)]
        public string UsuInicioSesion { get; set; }

        public DateTime? ParFechaActualizacion { get; set; }

        public Guid rowguid { get; set; }
    }
}
