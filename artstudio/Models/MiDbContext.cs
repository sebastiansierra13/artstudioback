using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace artstudio.Models
{
    public partial class MiDbContext : DbContext
    {
        public MiDbContext()
        {
        }

        public MiDbContext(DbContextOptions<MiDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Admin> Admins { get; set; } = null!;
        public virtual DbSet<Banner> Banners { get; set; } = null!;
        public virtual DbSet<Categoria> Categorias { get; set; } = null!;
        public virtual DbSet<Departamento> Departamentos { get; set; } = null!;
        public virtual DbSet<Efmigrationshistory> Efmigrationshistories { get; set; } = null!;
        public virtual DbSet<Instagramtoken> Instagramtokens { get; set; } = null!;
        public virtual DbSet<Municipio> Municipios { get; set; } = null!;
        public virtual DbSet<Precio> Precios { get; set; } = null!;
        public virtual DbSet<Producto> Productos { get; set; } = null!;
        public virtual DbSet<Region> Regions { get; set; } = null!;
        public virtual DbSet<Sort> Sorts { get; set; } = null!;
        public virtual DbSet<Tag> Tags { get; set; } = null!;


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseMySql("server=localhost;database=art_studio;user=root;password=12345", Microsoft.EntityFrameworkCore.ServerVersion.Parse("8.1.0-mysql"));
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.UseCollation("utf8mb4_0900_ai_ci")
                .HasCharSet("utf8mb4");

            modelBuilder.Entity<Admin>(entity =>
            {
                entity.HasKey(e => e.User)
                    .HasName("PRIMARY");

                entity.ToTable("admin");

                entity.Property(e => e.User).HasColumnName("user");

                entity.Property(e => e.Password)
                    .HasMaxLength(255)
                    .HasColumnName("password");
            });

            modelBuilder.Entity<Banner>(entity =>
            {
                entity.ToTable("banners");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Posicion).HasColumnName("posicion");

                entity.Property(e => e.Url)
                    .HasMaxLength(3000)
                    .HasColumnName("url");
            });

            modelBuilder.Entity<Categoria>(entity =>
            {
                entity.HasKey(e => e.IdCategoria)
                    .HasName("PRIMARY");

                entity.ToTable("categorias");

                entity.Property(e => e.IdCategoria).HasColumnName("idCategoria");

                entity.Property(e => e.ImagenCategoria)
                    .HasMaxLength(3000)
                    .HasColumnName("imagenCategoria");

                entity.Property(e => e.NombreCategoria)
                    .HasMaxLength(255)
                    .HasColumnName("nombreCategoria");
            });

            modelBuilder.Entity<Departamento>(entity =>
            {
                entity.ToTable("departamento");

                entity.HasIndex(e => e.RegionId, "region_id");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CodigoDane).HasColumnName("codigo_dane");

                entity.Property(e => e.Nombre)
                    .HasMaxLength(255)
                    .HasColumnName("nombre");

                entity.Property(e => e.RegionId).HasColumnName("region_id");

                entity.HasOne(d => d.Region)
                    .WithMany(p => p.Departamentos)
                    .HasForeignKey(d => d.RegionId)
                    .HasConstraintName("departamento_ibfk_1");
            });

            modelBuilder.Entity<Efmigrationshistory>(entity =>
            {
                entity.HasKey(e => e.MigrationId)
                    .HasName("PRIMARY");

                entity.ToTable("__efmigrationshistory");

                entity.Property(e => e.MigrationId).HasMaxLength(150);

                entity.Property(e => e.ProductVersion).HasMaxLength(32);
            });

            modelBuilder.Entity<Instagramtoken>(entity =>
            {
                entity.ToTable("instagramtokens");

                entity.Property(e => e.AccessToken).HasMaxLength(255);

                entity.Property(e => e.CreatedAt)
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.ExpiryDate).HasColumnType("datetime");
            });



            modelBuilder.Entity<Municipio>(entity =>
            {
                entity.ToTable("municipio");

                entity.HasIndex(e => e.DepartamentoId, "departamento_id");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CodigoDane).HasColumnName("codigo_dane");

                entity.Property(e => e.DepartamentoId).HasColumnName("departamento_id");

                entity.Property(e => e.Nombre)
                    .HasMaxLength(255)
                    .HasColumnName("nombre");

                entity.HasOne(d => d.Departamento)
                    .WithMany(p => p.Municipios)
                    .HasForeignKey(d => d.DepartamentoId)
                    .HasConstraintName("municipio_ibfk_1");
            });

            modelBuilder.Entity<Precio>(entity =>
            {
                entity.HasKey(e => e.IdPrecio)
                    .HasName("PRIMARY");

                entity.ToTable("precios");

                entity.Property(e => e.IdPrecio).HasColumnName("idPrecio");

                entity.Property(e => e.PrecioMarco)
                    .HasPrecision(10, 2)
                    .HasColumnName("precioMarco");

                entity.Property(e => e.PrecioPoster)
                    .HasPrecision(10, 2)
                    .HasColumnName("precioPoster");

                entity.Property(e => e.TamanhoPoster)
                    .HasMaxLength(50)
                    .HasColumnName("tamanhoPoster");
            });

            modelBuilder.Entity<Producto>(entity =>
            {
                entity.HasKey(e => e.IdProducto)
                    .HasName("PRIMARY");

                entity.ToTable("productos");

                entity.HasIndex(e => e.IdCategoria, "idCategoria");

                entity.Property(e => e.IdProducto).HasColumnName("idProducto");

                entity.Property(e => e.CantVendido).HasColumnName("cantVendido");

                entity.Property(e => e.DescripcionProducto)
                    .HasColumnType("text")
                    .HasColumnName("descripcionProducto");

                entity.Property(e => e.Destacado).HasColumnName("destacado");

                entity.Property(e => e.IdCategoria).HasColumnName("idCategoria");

                entity.Property(e => e.Imagenes)
                    .HasMaxLength(3000)
                    .HasColumnName("imagenes");

                entity.Property(e => e.ListTags)
                    .HasMaxLength(255)
                    .HasColumnName("listTags");

                entity.Property(e => e.NombreProducto)
                    .HasMaxLength(255)
                    .HasColumnName("nombreProducto");

                entity.Property(e => e.Posicion).HasColumnName("posicion");

                entity.HasOne(d => d.IdCategoriaNavigation)
                    .WithMany(p => p.Productos)
                    .HasForeignKey(d => d.IdCategoria)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("productos_ibfk_1");
            });

            modelBuilder.Entity<Region>(entity =>
            {
                entity.ToTable("region");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Nombre)
                    .HasMaxLength(255)
                    .HasColumnName("nombre");
            });

            modelBuilder.Entity<Sort>(entity =>
            {
                entity.HasKey(e => e.IdSort)
                    .HasName("PRIMARY");

                entity.ToTable("sort");

                entity.Property(e => e.IdSort).HasColumnName("idSort");

                entity.Property(e => e.DescripcionSort).HasColumnType("text");

                entity.Property(e => e.NombreSort)
                    .HasMaxLength(255)
                    .HasColumnName("nombreSort");
            });

            modelBuilder.Entity<Tag>(entity =>
            {
                entity.HasKey(e => e.IdTag)
                    .HasName("PRIMARY");

                entity.ToTable("tags");

                entity.Property(e => e.IdTag).HasColumnName("idTag");

                entity.Property(e => e.DescripcionTag).HasColumnType("text");

                entity.Property(e => e.NombreTag)
                    .HasMaxLength(255)
                    .HasColumnName("nombreTag");
            });

            OnModelCreatingPartial(modelBuilder);
        }



        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
