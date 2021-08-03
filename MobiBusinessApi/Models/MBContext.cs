namespace MovilBusinessApiCore.Models
{
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class MBContext : DbContext
    {
        public MBContext(DbContextOptions<MBContext> options)
            : base(options)
        {
        }

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

    }
}
