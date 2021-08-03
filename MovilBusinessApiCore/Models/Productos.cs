namespace MovilBusinessApiCore.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    

    public partial class Productos
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ProID { get; set; }

        public int? LinID { get; set; }

        [StringLength(200)]
        public string ProDescripcion { get; set; }

        public decimal? ProPrecio { get; set; }

        public decimal? ProPrecio2 { get; set; }

        public decimal? ProPrecio3 { get; set; }

        public decimal? ProPrecioMin { get; set; }

        [StringLength(100)]
        public string ProCodigo { get; set; }

        [StringLength(120)]
        public string ProReferencia { get; set; }

        public decimal? ProUnidades { get; set; }

        public decimal? ProCantidad { get; set; }

        public int? Cat1ID { get; set; }

        public int? Cat2ID { get; set; }

        public int? Cat3ID { get; set; }

        [StringLength(200)]
        public string ProDescripcion1 { get; set; }

        [StringLength(200)]
        public string ProDescripcion2 { get; set; }

        [StringLength(200)]
        public string ProDescripcion3 { get; set; }

        [StringLength(100)]
        public string ProDatos1 { get; set; }

        [StringLength(100)]
        public string ProDatos2 { get; set; }

        [StringLength(100)]
        public string ProDatos3 { get; set; }

        public decimal? ProItbis { get; set; }

        public decimal? ProSelectivo { get; set; }

        public decimal? ProAdValorem { get; set; }

        public bool? ProIndicadorDetalle { get; set; }

        [StringLength(10)]
        public string ProCodigoDescuento { get; set; }

        public decimal? ProCargoDistribucion { get; set; }

        [StringLength(10)]
        public string SecCodigo { get; set; }

        [StringLength(64)]
        public string UsuInicioSesion { get; set; }

        public DateTime? ProFechaActualizacion { get; set; }

        [StringLength(15)]
        public string UnmCodigo { get; set; }

        [StringLength(50)]
        public string ProImg { get; set; }

        public Guid rowguid { get; set; }

        public decimal? ProPeso { get; set; }

        public decimal? ProVolumen { get; set; }

        [StringLength(2000)]
        public string ProGrupoProductos { get; set; }

        [StringLength(1000)]
        public string ProLotes { get; set; }

        public decimal? ProHolgura { get; set; }

        [StringLength(5000)]
        public string ProInventarios { get; set; }

        [StringLength(5000)]
        public string ProListaPrecios { get; set; }
    }
}
