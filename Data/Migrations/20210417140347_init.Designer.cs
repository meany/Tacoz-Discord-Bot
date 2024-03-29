﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using dm.TCZ.Data;

namespace dm.TCZ.Data.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20210417140347_init")]
    partial class init
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .UseIdentityColumns()
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("ProductVersion", "5.0.1");

            modelBuilder.Entity("dm.TCZ.Data.Models.Price", b =>
                {
                    b.Property<int>("PriceId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .UseIdentityColumn();

                    b.Property<int>("CircMarketCapUSD")
                        .HasColumnType("int");

                    b.Property<DateTime>("Date")
                        .HasColumnType("datetime2");

                    b.Property<int>("FullMarketCapUSD")
                        .HasColumnType("int");

                    b.Property<Guid>("Group")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("LpRawTacoz")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("LpRawTezos")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("MarketCapUSDChange")
                        .HasColumnType("int");

                    b.Property<decimal>("MarketCapUSDChangePct")
                        .HasColumnType("decimal(12,8)");

                    b.Property<decimal>("PriceBTC")
                        .HasColumnType("decimal(16,8)");

                    b.Property<decimal>("PriceBTC120k")
                        .HasColumnType("decimal(16,8)");

                    b.Property<int>("PriceBTCChange")
                        .HasColumnType("int");

                    b.Property<decimal>("PriceBTCChangePct")
                        .HasColumnType("decimal(12,8)");

                    b.Property<decimal>("PriceETH")
                        .HasColumnType("decimal(16,8)");

                    b.Property<decimal>("PriceETH120k")
                        .HasColumnType("decimal(16,8)");

                    b.Property<int>("PriceETHChange")
                        .HasColumnType("int");

                    b.Property<decimal>("PriceETHChangePct")
                        .HasColumnType("decimal(12,8)");

                    b.Property<decimal>("PriceTCZForOneXTZ")
                        .HasColumnType("decimal(16,8)");

                    b.Property<decimal>("PriceUSD")
                        .HasColumnType("decimal(11,6)");

                    b.Property<decimal>("PriceUSD120k")
                        .HasColumnType("decimal(11,6)");

                    b.Property<int>("PriceUSDChange")
                        .HasColumnType("int");

                    b.Property<decimal>("PriceUSDChangePct")
                        .HasColumnType("decimal(12,8)");

                    b.Property<decimal>("PriceXTZForOneTCZ")
                        .HasColumnType("decimal(16,8)");

                    b.Property<int>("Source")
                        .HasColumnType("int");

                    b.Property<int>("VolumeUSD")
                        .HasColumnType("int");

                    b.Property<int>("VolumeUSDChange")
                        .HasColumnType("int");

                    b.Property<decimal>("VolumeUSDChangePct")
                        .HasColumnType("decimal(12,8)");

                    b.HasKey("PriceId");

                    b.HasIndex("Date");

                    b.HasIndex("Group");

                    b.ToTable("Prices");
                });

            modelBuilder.Entity("dm.TCZ.Data.Models.Request", b =>
                {
                    b.Property<int>("RequestId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .UseIdentityColumn();

                    b.Property<DateTime>("Date")
                        .HasColumnType("datetime2");

                    b.Property<int>("Response")
                        .HasColumnType("int");

                    b.Property<int>("Type")
                        .HasColumnType("int");

                    b.Property<string>("User")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("RequestId");

                    b.HasIndex("Date");

                    b.HasIndex("Response", "Type");

                    b.ToTable("Requests");
                });
#pragma warning restore 612, 618
        }
    }
}
