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

                    b.Property<int>("Id")
                        .HasColumnType("int");

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

                    b.Property<string>("Subtitulo")
                        .HasMaxLength(255)
                        .HasColumnType("varchar(255)");

                    b.Property<string>("Titulo")
                        .HasMaxLength(255)
                        .HasColumnType("varchar(255)");

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

            modelBuilder.Entity("artstudio.Models.Configurationtext", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("Section")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("varchar(50)");

                    b.Property<string>("TextContent")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("configurationtext", (string)null);
                });

            modelBuilder.Entity("artstudio.Models.Departamento", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("id");

                    b.Property<int>("CodigoDane")
                        .HasColumnType("int")
                        .HasColumnName("codigo_dane");

                    b.Property<string>("Nombre")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("varchar(255)")
                        .HasColumnName("nombre");

                    b.Property<int?>("RegionId")
                        .HasColumnType("int")
                        .HasColumnName("region_id");

                    b.HasKey("Id");

                    b.HasIndex(new[] { "RegionId" }, "region_id");

                    b.ToTable("departamento", (string)null);
                });

            modelBuilder.Entity("artstudio.Models.Efmigrationshistory", b =>
                {
                    b.Property<string>("MigrationId")
                        .HasMaxLength(150)
                        .HasColumnType("varchar(150)");

                    b.Property<string>("ProductVersion")
                        .IsRequired()
                        .HasMaxLength(32)
                        .HasColumnType("varchar(32)");

                    b.HasKey("MigrationId")
                        .HasName("PRIMARY");

                    b.ToTable("__efmigrationshistory", (string)null);
                });

            modelBuilder.Entity("artstudio.Models.Instagramtoken", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("AccessToken")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("varchar(255)");

                    b.Property<DateTime?>("CreatedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp")
                        .HasDefaultValueSql("CURRENT_TIMESTAMP");

                    b.Property<DateTime>("ExpiryDate")
                        .HasColumnType("datetime");

                    b.HasKey("Id");

                    b.ToTable("instagramtokens", (string)null);
                });

            modelBuilder.Entity("artstudio.Models.Municipio", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("id");

                    b.Property<int>("CodigoDane")
                        .HasColumnType("int")
                        .HasColumnName("codigo_dane");

                    b.Property<int?>("DepartamentoId")
                        .HasColumnType("int")
                        .HasColumnName("departamento_id");

                    b.Property<string>("Nombre")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("varchar(255)")
                        .HasColumnName("nombre");

                    b.HasKey("Id");

                    b.HasIndex(new[] { "DepartamentoId" }, "departamento_id");

                    b.ToTable("municipio", (string)null);
                });

            modelBuilder.Entity("artstudio.Models.Order", b =>
                {
                    b.Property<int>("OrderId")
                        .HasColumnType("int");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("varchar(255)");

                    b.Property<string>("ShippingAddress")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("varchar(255)");

                    b.Property<string>("ShippingCity")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("varchar(255)");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("varchar(50)");

                    b.Property<decimal>("Total")
                        .HasPrecision(10, 2)
                        .HasColumnType("decimal(10,2)");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("datetime");

                    b.HasKey("OrderId");

                    b.ToTable("orders", (string)null);
                });

            modelBuilder.Entity("artstudio.Models.Paymenttransaction", b =>
                {
                    b.Property<int>("TransactionId")
                        .HasColumnType("int");

                    b.Property<decimal>("Amount")
                        .HasPrecision(10, 2)
                        .HasColumnType("decimal(10,2)");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime");

                    b.Property<string>("Currency")
                        .IsRequired()
                        .HasMaxLength(10)
                        .HasColumnType("varchar(10)");

                    b.Property<int?>("OrderId")
                        .HasColumnType("int");

                    b.Property<string>("ReferenceCode")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("varchar(100)");

                    b.Property<string>("ResponseMessage")
                        .HasColumnType("text");

                    b.Property<string>("TransactionState")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("varchar(50)");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("datetime");

                    b.HasKey("TransactionId")
                        .HasName("PRIMARY");

                    b.HasIndex(new[] { "OrderId" }, "OrderId");

                    b.ToTable("paymenttransactions", (string)null);
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

                    b.Property<string>("TamanhoPoster")
                        .HasMaxLength(50)
                        .HasColumnType("varchar(50)")
                        .HasColumnName("tamanhoPoster");

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
                        .HasMaxLength(3000)
                        .HasColumnType("varchar(3000)")
                        .HasColumnName("imagenes");

                    b.Property<string>("ListTags")
                        .IsRequired()
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

            modelBuilder.Entity("artstudio.Models.Region", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("id");

                    b.Property<string>("Nombre")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("varchar(255)")
                        .HasColumnName("nombre");

                    b.HasKey("Id");

                    b.ToTable("region", (string)null);
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

            modelBuilder.Entity("Productotag", b =>
                {
                    b.Property<int>("IdProducto")
                        .HasColumnType("int")
                        .HasColumnName("idProducto");

                    b.Property<int>("IdTag")
                        .HasColumnType("int")
                        .HasColumnName("idTag");

                    b.HasKey("IdProducto", "IdTag")
                        .HasName("PRIMARY")
                        .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });

                    b.HasIndex(new[] { "IdTag" }, "idTag");

                    b.ToTable("productotag", (string)null);
                });

            modelBuilder.Entity("artstudio.Models.Departamento", b =>
                {
                    b.HasOne("artstudio.Models.Region", "Region")
                        .WithMany("Departamentos")
                        .HasForeignKey("RegionId")
                        .HasConstraintName("departamento_ibfk_1");

                    b.Navigation("Region");
                });

            modelBuilder.Entity("artstudio.Models.Municipio", b =>
                {
                    b.HasOne("artstudio.Models.Departamento", "Departamento")
                        .WithMany("Municipios")
                        .HasForeignKey("DepartamentoId")
                        .HasConstraintName("municipio_ibfk_1");

                    b.Navigation("Departamento");
                });

            modelBuilder.Entity("artstudio.Models.Paymenttransaction", b =>
                {
                    b.HasOne("artstudio.Models.Order", "Order")
                        .WithMany("Paymenttransactions")
                        .HasForeignKey("OrderId")
                        .HasConstraintName("paymenttransactions_ibfk_1");

                    b.Navigation("Order");
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

            modelBuilder.Entity("Productotag", b =>
                {
                    b.HasOne("artstudio.Models.Producto", null)
                        .WithMany()
                        .HasForeignKey("IdProducto")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("productotag_ibfk_1");

                    b.HasOne("artstudio.Models.Tag", null)
                        .WithMany()
                        .HasForeignKey("IdTag")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("productotag_ibfk_2");
                });

            modelBuilder.Entity("artstudio.Models.Categoria", b =>
                {
                    b.Navigation("Productos");
                });

            modelBuilder.Entity("artstudio.Models.Departamento", b =>
                {
                    b.Navigation("Municipios");
                });

            modelBuilder.Entity("artstudio.Models.Order", b =>
                {
                    b.Navigation("Paymenttransactions");
                });

            modelBuilder.Entity("artstudio.Models.Region", b =>
                {
                    b.Navigation("Departamentos");
                });
#pragma warning restore 612, 618
        }
    }
}
