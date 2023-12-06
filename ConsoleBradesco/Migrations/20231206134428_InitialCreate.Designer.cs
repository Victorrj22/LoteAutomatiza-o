﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace ConsoleBradesco.Migrations
{
    [DbContext(typeof(ConsultaContext))]
    [Migration("20231206134428_InitialCreate")]
    partial class InitialCreate
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Consulta", b =>
                {
                    b.Property<int>("ConsultaId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ConsultaId"));

                    b.Property<int>("ConsultaTipoId")
                        .HasColumnType("int");

                    b.Property<string>("Document")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("Inicio")
                        .HasColumnType("datetime2");

                    b.Property<int>("TokenAcessoId")
                        .HasColumnType("int");

                    b.HasKey("ConsultaId");

                    b.ToTable("Consultas");
                });
#pragma warning restore 612, 618
        }
    }
}