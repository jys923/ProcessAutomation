﻿// <auto-generated />
using System;
using EFCoreSample.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace EFCoreSample.Migrations
{
    [DbContext(typeof(EFCoreSampleDbContext))]
    partial class EFCoreSampleDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.2")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("EFCoreSample.Entities.MotorModule", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime(6)");

                    b.Property<int>("DataFlag")
                        .HasColumnType("int");

                    b.Property<string>("Detail")
                        .HasMaxLength(100)
                        .HasColumnType("varchar(100)");

                    b.Property<string>("MotorModuleSn")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.HasKey("Id");

                    b.ToTable("MotorModules");
                });

            modelBuilder.Entity("EFCoreSample.Entities.Probe", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime(6)");

                    b.Property<int>("DataFlag")
                        .HasColumnType("int");

                    b.Property<string>("Detail")
                        .HasMaxLength(100)
                        .HasColumnType("varchar(100)");

                    b.Property<int>("MotorModuleId")
                        .HasColumnType("int");

                    b.Property<string>("ProbeSn")
                        .IsRequired()
                        .HasColumnType("varchar(255)");

                    b.Property<int>("TransducerModuleId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("MotorModuleId");

                    b.HasIndex("ProbeSn")
                        .IsUnique();

                    b.HasIndex("TransducerModuleId");

                    b.ToTable("Probes");
                });

            modelBuilder.Entity("EFCoreSample.Entities.ProbeView", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime(6)");

                    b.Property<int>("DataFlag")
                        .HasColumnType("int");

                    b.Property<string>("Detail")
                        .HasMaxLength(100)
                        .HasColumnType("varchar(100)");

                    b.Property<string>("MotorModuleSn")
                        .IsRequired()
                        .HasColumnType("varchar(255)");

                    b.Property<string>("ProbeSN")
                        .IsRequired()
                        .HasColumnType("varchar(255)");

                    b.Property<string>("TransducerModuleSN")
                        .IsRequired()
                        .HasColumnType("varchar(255)");

                    b.Property<string>("TransducerSN")
                        .IsRequired()
                        .HasColumnType("varchar(255)");

                    b.HasKey("Id");

                    b.HasIndex("MotorModuleSn")
                        .IsUnique();

                    b.HasIndex("ProbeSN")
                        .IsUnique();

                    b.HasIndex("TransducerModuleSN")
                        .IsUnique();

                    b.HasIndex("TransducerSN")
                        .IsUnique();

                    b.ToTable("ProbeViews");
                });

            modelBuilder.Entity("EFCoreSample.Entities.Test", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<int>("CategoryId")
                        .HasColumnType("int");

                    b.Property<string>("ChangedImg")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("ChangedImgMetadata")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime(6)");

                    b.Property<int>("DataFlag")
                        .HasColumnType("int");

                    b.Property<string>("Detail")
                        .HasMaxLength(100)
                        .HasColumnType("varchar(100)");

                    b.Property<int>("Method")
                        .HasColumnType("int");

                    b.Property<string>("OriginalImg")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<int>("Result")
                        .HasColumnType("int");

                    b.Property<int>("TestTypeId")
                        .HasColumnType("int");

                    b.Property<int>("TesterId")
                        .HasColumnType("int");

                    b.Property<int>("TransducerModuleId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("CategoryId");

                    b.HasIndex("TestTypeId");

                    b.HasIndex("TesterId");

                    b.HasIndex("TransducerModuleId");

                    b.ToTable("Tests");
                });

            modelBuilder.Entity("EFCoreSample.Entities.TestCategory", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime(6)");

                    b.Property<int>("DataFlag")
                        .HasColumnType("int");

                    b.Property<string>("Detail")
                        .HasMaxLength(100)
                        .HasColumnType("varchar(100)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(10)
                        .HasColumnType("varchar(10)");

                    b.HasKey("Id");

                    b.ToTable("TestCategories");
                });

            modelBuilder.Entity("EFCoreSample.Entities.TestType", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime(6)");

                    b.Property<int>("DataFlag")
                        .HasColumnType("int");

                    b.Property<string>("Detail")
                        .HasMaxLength(100)
                        .HasColumnType("varchar(100)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(10)
                        .HasColumnType("varchar(10)");

                    b.Property<int>("Threshold")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("TestTypes");
                });

            modelBuilder.Entity("EFCoreSample.Entities.Tester", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime(6)");

                    b.Property<int>("DataFlag")
                        .HasColumnType("int");

                    b.Property<string>("Detail")
                        .HasMaxLength(100)
                        .HasColumnType("varchar(100)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(10)
                        .HasColumnType("varchar(10)");

                    b.Property<int>("PcNo")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("Testers");
                });

            modelBuilder.Entity("EFCoreSample.Entities.TransducerModule", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime(6)");

                    b.Property<int>("DataFlag")
                        .HasColumnType("int");

                    b.Property<string>("Detail")
                        .HasMaxLength(100)
                        .HasColumnType("varchar(100)");

                    b.Property<string>("TransducerModuleSn")
                        .IsRequired()
                        .HasColumnType("varchar(255)");

                    b.Property<int>("TransducerModuleTypeId")
                        .HasColumnType("int");

                    b.Property<string>("TransducerSn")
                        .IsRequired()
                        .HasColumnType("varchar(255)");

                    b.HasKey("Id");

                    b.HasIndex("TransducerModuleSn")
                        .IsUnique();

                    b.HasIndex("TransducerModuleTypeId");

                    b.HasIndex("TransducerSn")
                        .IsUnique();

                    b.ToTable("TransducerModules");
                });

            modelBuilder.Entity("EFCoreSample.Entities.TransducerModuleType", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("Code")
                        .IsRequired()
                        .HasMaxLength(10)
                        .HasColumnType("varchar(10)");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime(6)");

                    b.Property<int>("DataFlag")
                        .HasColumnType("int");

                    b.Property<string>("Detail")
                        .HasMaxLength(100)
                        .HasColumnType("varchar(100)");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasMaxLength(10)
                        .HasColumnType("varchar(10)");

                    b.HasKey("Id");

                    b.ToTable("TransducerModuleTypes");
                });

            modelBuilder.Entity("EFCoreSample.Entities.Probe", b =>
                {
                    b.HasOne("EFCoreSample.Entities.MotorModule", "MotorModule")
                        .WithMany()
                        .HasForeignKey("MotorModuleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("EFCoreSample.Entities.TransducerModule", "TransducerModule")
                        .WithMany()
                        .HasForeignKey("TransducerModuleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("MotorModule");

                    b.Navigation("TransducerModule");
                });

            modelBuilder.Entity("EFCoreSample.Entities.Test", b =>
                {
                    b.HasOne("EFCoreSample.Entities.TestCategory", "Category")
                        .WithMany()
                        .HasForeignKey("CategoryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("EFCoreSample.Entities.TestType", "TestType")
                        .WithMany()
                        .HasForeignKey("TestTypeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("EFCoreSample.Entities.Tester", "Tester")
                        .WithMany()
                        .HasForeignKey("TesterId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("EFCoreSample.Entities.TransducerModule", "TransducerModule")
                        .WithMany()
                        .HasForeignKey("TransducerModuleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Category");

                    b.Navigation("TestType");

                    b.Navigation("Tester");

                    b.Navigation("TransducerModule");
                });

            modelBuilder.Entity("EFCoreSample.Entities.TransducerModule", b =>
                {
                    b.HasOne("EFCoreSample.Entities.TransducerModuleType", "TransducerModuleType")
                        .WithMany()
                        .HasForeignKey("TransducerModuleTypeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("TransducerModuleType");
                });
#pragma warning restore 612, 618
        }
    }
}
