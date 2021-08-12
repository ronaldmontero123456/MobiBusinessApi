namespace MovilBusinessApiCore.Models
{
    using Microsoft.EntityFrameworkCore;

    public partial class MBContext : DbContext
    {
        public MBContext(DbContextOptions<MBContext> options) : base(options) { }

        public virtual DbSet<Empresa> Empresa { get; set; }
        public virtual DbSet<Estados> Estados { get; set; }
        public virtual DbSet<Parametros> Parametros { get; set; }
        public virtual DbSet<Pedidos> Pedidos { get; set; }
        public virtual DbSet<PedidosAdicionales> PedidosAdicionales { get; set; }
        public virtual DbSet<PedidosBackOrder> PedidosBackOrder { get; set; }
        public virtual DbSet<PedidosConfirmados> PedidosConfirmados { get; set; }
        public virtual DbSet<PedidosDescuentos> PedidosDescuentos { get; set; }
        public virtual DbSet<PedidosDetalle> PedidosDetalle { get; set; }
        public virtual DbSet<PedidosDetalleConfirmados> PedidosDetalleConfirmados { get; set; }
        public virtual DbSet<PedidosDetalleHistoricos> PedidosDetalleHistoricos { get; set; }
        public virtual DbSet<PedidosHistoricos> PedidosHistoricos { get; set; }
        public virtual DbSet<PedidosPresentacion> PedidosPresentacion { get; set; }
        public virtual DbSet<PedidosSugeridos> PedidosSugeridos { get; set; }
        public virtual DbSet<Presupuestos> Presupuestos { get; set; }
        public virtual DbSet<PresupuestosDiarios> PresupuestosDiarios { get; set; }
        public virtual DbSet<PresupuestosHistoricos> PresupuestosHistoricos { get; set; }
        public virtual DbSet<PresupuestosProductosExcluir> PresupuestosProductosExcluir { get; set; }
        public virtual DbSet<Productos> Productos { get; set; }
        public virtual DbSet<Replicaciones> Replicaciones { get; set; }
        public virtual DbSet<ReplicacionesSuscriptores> ReplicacionesSuscriptores { get; set; }
        public virtual DbSet<ReplicacionesSuscriptoresBaseDatos> ReplicacionesSuscriptoresBaseDatos { get; set; }
        public virtual DbSet<ReplicacionesSuscriptoresCambios> ReplicacionesSuscriptoresCambios { get; set; }
        public virtual DbSet<ReplicacionesSuscriptoresConflictos> ReplicacionesSuscriptoresConflictos { get; set; }
        public virtual DbSet<ReplicacionesSuscriptoresSincronizaciones> ReplicacionesSuscriptoresSincronizaciones { get; set; }
        public virtual DbSet<ReplicacionesTablas> ReplicacionesTablas { get; set; }
        public virtual DbSet<Representantes> Representantes { get; set; }
        public virtual DbSet<RepresentantesDetalle> RepresentantesDetalle { get; set; }
        public virtual DbSet<RepresentantesParametros> RepresentantesParametros { get; set; }
        public virtual DbSet<RepresentantesSecuencias> RepresentantesSecuencias { get; set; }
        public virtual DbSet<TransaccionesFirmas> TransaccionesFirmas { get; set; }
        public virtual DbSet<TransaccionesImagenes> TransaccionesImagenes { get; set; }
        public virtual DbSet<TransaccionesImagenesTablas> TransaccionesImagenesTablas { get; set; }
        public virtual DbSet<TransaccionesTracking> TransaccionesTracking { get; set; }
        public virtual DbSet<UsosMultiples> UsosMultiples { get; set; }
        public virtual DbSet<UsuarioSistema> UsuarioSistema { get; set; }
        public virtual DbSet<UsuariosSistemaParametros> UsuariosSistemaParametros { get; set; }
        public virtual DbSet<PedidosDescuentosConfirmados> PedidosDescuentosConfirmados { get; set; }
        public virtual DbSet<PedidosDescuentosHistoricos> PedidosDescuentosHistoricos { get; set; }
        public virtual DbSet<ClientesReplicacionesUrl> ClientesReplicacionesUrl { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Estados>()
            .HasKey(o => new { o.EstTabla, o.EstEstado });

            modelBuilder.Entity<Pedidos>().
                HasKey(p => new { p.RepCodigo, p.PedSecuencia });

            modelBuilder.Entity<PedidosAdicionales>().
                HasKey(p => new
                {
                    p.RepCodigo,
                    p.PedSecuencia
                });

            modelBuilder.Entity<PedidosBackOrder>().
                HasKey(p => new
                {
                    p.RepCodigo,
                    p.CliID
                });

            modelBuilder.Entity<PedidosConfirmados>().
                HasKey(p => new
                {
                    p.RepCodigo,
                    p.PedSecuencia
                });

            modelBuilder.Entity<PedidosDescuentos>().
                HasKey(p => new
                {
                    p.RepCodigo,
                    p.PedSecuencia
                });

            modelBuilder.Entity<PedidosDetalle>().
                HasKey(p => new
                {
                    p.RepCodigo,
                    p.PedSecuencia
                });

            modelBuilder.Entity<PedidosDetalleConfirmados>().
                HasKey(p => new
                {
                    p.RepCodigo,
                    p.PedSecuencia
                });

            modelBuilder.Entity<PedidosDetalleHistoricos>().
                HasKey(p => new
                {
                    p.RepCodigo,
                    p.PedSecuencia
                });

            modelBuilder.Entity<PedidosHistoricos>().
                HasKey(p => new
                {
                    p.RepCodigo,
                    p.PedSecuencia
                });

            modelBuilder.Entity<PedidosPresentacion>().
                HasKey(p => new
                {
                    p.RepCodigo,
                    p.PedSecuencia
                });

            modelBuilder.Entity<PedidosSugeridos>().
                HasKey(p => new
                {
                    p.RepCodigo,
                    p.CliID
                });

            modelBuilder.Entity<Presupuestos>().
                HasKey(p => new
                {
                    p.RepCodigo,
                    p.PreTipo
                });

            modelBuilder.Entity<PresupuestosDiarios>().
                HasKey(p => new
                {
                    p.RepCodigo,
                    p.PreTipo
                });

            modelBuilder.Entity<PresupuestosHistoricos>().
                HasKey(p => new
                {
                    p.RepCodigo,
                    p.PreTipo
                });


            modelBuilder.Entity<PresupuestosProductosExcluir>().
                HasKey(p => new
                {
                    p.PreAnio,
                    p.PreMes,
                    p.PreTipo,
                    p.Proid,
                });


            modelBuilder.Entity<ReplicacionesSuscriptores>().
                HasKey(p => new
                {
                    p.RepID,
                    p.RepSuscriptor,
                });

            modelBuilder.Entity<ReplicacionesSuscriptoresBaseDatos>().
                HasKey(p => new
                {
                    p.RepID,
                    p.RepSuscriptor,
                    p.rsbSecuencia,
                });


            modelBuilder.Entity<ReplicacionesSuscriptoresSincronizaciones>().
                HasKey(p => new
                {
                    p.RepID,
                    p.RepSuscriptor,
                    p.RssSecuencia,
                });

            modelBuilder.Entity<ReplicacionesTablas>()
                .HasKey(p => new
                {
                    p.RepID,
                    p.RepTabla,
                });


            modelBuilder.Entity<RepresentantesParametros>()
                .HasKey(p => new
                {
                    p.RepCodigo,
                    p.ParReferencia,
                });

            modelBuilder.Entity<RepresentantesSecuencias>()
                .HasKey(p => new
                {
                    p.RepCodigo,
                    p.RepTabla,
                });

            modelBuilder.Entity<TransaccionesFirmas>()
                .HasKey(p => new
                {
                    p.RepCodigo,
                    p.TitID,
                    p.TraSecuencia,
                });

            modelBuilder.Entity<TransaccionesImagenes>()
                .HasKey(p => new
                {
                    p.RepCodigo,
                    p.TitID,
                    p.TraSecuencia,
                    p.triposicion,
                });

            modelBuilder.Entity<TransaccionesImagenesTablas>()
                .HasKey(p => new
                {
                    p.RepCodigo,
                    p.RepTabla,
                    p.RepTablaKey,
                    p.TraPosicion,
                });

            modelBuilder.Entity<UsosMultiples>()
                .HasKey(p => new
                {
                    p.CodigoGrupo,
                    p.CodigoUso,
                });

            modelBuilder.Entity<UsuariosSistemaParametros>()
                .HasKey(p => new
                {
                    p.UsuInicioSesionPK,
                    p.ParReferencia,
                });

            modelBuilder.Entity<PedidosDescuentosConfirmados>().
                HasKey(p => new
                {
                    p.RepCodigo,
                    p.PedSecuencia,
                    p.PedPosicion,
                    p.DesID,
                });

            modelBuilder.Entity<PedidosDescuentosHistoricos>().
                HasKey(p => new
                {
                    p.RepCodigo,
                    p.PedSecuencia,
                    p.PedPosicion,
                    p.DesID,
                });
        }
    }
}
