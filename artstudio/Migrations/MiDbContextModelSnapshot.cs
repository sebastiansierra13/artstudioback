﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using artstudio.Models;

#nullable disable

namespace artstudio.Migrations
{
    [DbContext(typeof(MiDbContext))]
    partial class MiDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .UseCollation("utf8mb4_0900_ai_ci")
                .HasAnnotation("ProductVersion", "6.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            MySqlModelBuilderExtensions.HasCharSet(modelBuilder, "utf8mb4");

            modelBuilder.Entity("artstudio.Models.Admin", b =>
                {
                    b.Property<string>("User")
                        .HasColumnType("varchar(255)")
                        .HasColumnName("user");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("varchar(255)")
                        .HasColumnName("password");

                    b.HasKey("User")
                        .HasName("PRIMARY");

                    b.ToTable("admin", (string)null);
                });

            modelBuilder.Entity("artstudio.Models.Banner", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("id");

                    b.Property<int?>("Posicion")
                        .HasColumnType("int")
                        .HasColumnName("posicion");

                    b.Property<string>("Url")
                        .HasMaxLength(3000)
                        .HasColumnType("varchar(3000)")
                        .HasColumnName("url");

                    b.HasKey("Id");

                    b.ToTable("banners", (string)null);
                });

            modelBuilder.Entity("artstudio.Models.Categoria", b =>
                {
                    b.Property<int>("IdCategoria")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("idCategoria");

                    b.Property<string>("ImagenCategoria")
                        .HasMaxLength(3000)
                        .HasColumnType("varchar(3000)")
                        .HasColumnName("imagenCategoria");

                    b.Property<string>("NombreCategoria")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("varchar(255)")
                        .HasColumnName("nombreCategoria");

                    b.HasKey("IdCategoria")
                        .HasName("PRIMARY");

                    b.ToTable("categorias", (string)null);
                });

            modelBuilder.Entity("artstudio.Models.Precio", b =>
                {
                    b.Property<int>("IdPrecio")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("idPrecio");

                    b.Property<decimal>("PrecioMarco")
                        .HasPrecision(10, 2)
                        .HasColumnType("decimal(10,2)")
                        .HasColumnName("precioMarco");

                    b.Property<decimal>("PrecioPoster")
                        .HasPrecision(10, 2)
                        .HasColumnType("decimal(10,2)")
                        .HasColumnName("precioPoster");

                    b.Property<string>("TamañoPoster")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("varchar(255)")
                        .HasColumnName("tamañoPoster");

                    b.HasKey("IdPrecio")
                        .HasName("PRIMARY");

                    b.ToTable("precios", (string)null);
                });

            modelBuilder.Entity("artstudio.Models.Producto", b =>
                {
                    b.Property<int>("IdProducto")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("idProducto");

                    b.Property<int>("CantVendido")
                        .HasColumnType("int")
                        .HasColumnName("cantVendido");

                    b.Property<string>("DescripcionProducto")
                        .HasColumnType("text")
                        .HasColumnName("descripcionProducto");

                    b.Property<bool?>("Destacado")
                        .HasColumnType("tinyint(1)")
                        .HasColumnName("destacado");

                    b.Property<int>("IdCategoria")
                        .HasColumnType("int")
                        .HasColumnName("idCategoria");

                    b.Property<string>("Imagenes")
                        .IsRequired()
                        .HasColumnType("json")
                        .HasColumnName("imagenes");

                    b.Property<string>("ListPrecios")
                        .HasMaxLength(255)
                        .HasColumnType("varchar(255)")
                        .HasColumnName("listPrecios");

                    b.Property<string>("ListTags")
                        .HasMaxLength(255)
                        .HasColumnType("varchar(255)")
                        .HasColumnName("listTags");

                    b.Property<string>("NombreProducto")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("varchar(255)")
                        .HasColumnName("nombreProducto");

                    b.Property<int?>("Posicion")
                        .HasColumnType("int")
                        .HasColumnName("posicion");

                    b.HasKey("IdProducto")
                        .HasName("PRIMARY");

                    b.HasIndex(new[] { "IdCategoria" }, "idCategoria");

                    b.ToTable("productos", (string)null);
                });

            modelBuilder.Entity("artstudio.Models.Sort", b =>
                {
                    b.Property<int>("IdSort")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("idSort");

                    b.Property<string>("DescripcionSort")
                        .HasColumnType("text");

                    b.Property<string>("NombreSort")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("varchar(255)")
                        .HasColumnName("nombreSort");

                    b.HasKey("IdSort")
                        .HasName("PRIMARY");

                    b.ToTable("sort", (string)null);
                });

            modelBuilder.Entity("artstudio.Models.Tag", b =>
                {
                    b.Property<int>("IdTag")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("idTag");

                    b.Property<string>("DescripcionTag")
                        .HasColumnType("text");

                    b.Property<string>("NombreTag")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("varchar(255)")
                        .HasColumnName("nombreTag");

                    b.HasKey("IdTag")
                        .HasName("PRIMARY");

                    b.ToTable("tags", (string)null);
                });

            modelBuilder.Entity("artstudio.Models.Producto", b =>
                {
                    b.HasOne("artstudio.Models.Categoria", "IdCategoriaNavigation")
                        .WithMany("Productos")
                        .HasForeignKey("IdCategoria")
                        .IsRequired()
                        .HasConstraintName("productos_ibfk_1");

                    b.Navigation("IdCategoriaNavigation");
                });

            modelBuilder.Entity("artstudio.Models.Categoria", b =>
                {
                    b.Navigation("Productos");
                });
#pragma warning restore 612, 618
        }
    }
}
