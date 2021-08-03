namespace MovilBusinessApiCore.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    

    [Table("Empresa")]
    public partial class Empresa
    {
        [Key]
        public int EmpID { get; set; }

        [StringLength(100)]
        public string EmpNombre { get; set; }

        [StringLength(100)]
        public string EmpDireccion { get; set; }

        [StringLength(100)]
        public string EmpDireccion2 { get; set; }

        [StringLength(100)]
        public string EmpDireccion3 { get; set; }

        [StringLength(100)]
        public string EmpRNC { get; set; }

        [StringLength(15)]
        public string EmpTelefono { get; set; }

        [StringLength(15)]
        public string EmpFax { get; set; }

        [Column(TypeName = "image")]
        public byte[] EmpLogo { get; set; }

        [StringLength(4)]
        public string SocCodigo { get; set; }

        [StringLength(4)]
        public string OrvCodigo { get; set; }

        [StringLength(4)]
        public string OfvCodigo { get; set; }

        [StringLength(64)]
        public string UsuInicioSesion { get; set; }

        public DateTime? empFechaActualizacion { get; set; }

        public Guid? rowguid { get; set; }

        [StringLength(5)]
        public string empmdkey { get; set; }
    }
}
