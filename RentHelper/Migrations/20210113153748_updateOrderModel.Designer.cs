﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using RentHelper.Database;

namespace RentHelper.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20210113153748_updateOrderModel")]
    partial class updateOrderModel
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.10")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("RentHelper.Models.CartItem", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<Guid>("ProductId")
                        .HasColumnType("char(36)");

                    b.Property<Guid>("UserId")
                        .HasColumnType("char(36)");

                    b.HasKey("Id");

                    b.HasIndex("ProductId");

                    b.HasIndex("UserId");

                    b.ToTable("cartitems");
                });

            modelBuilder.Entity("RentHelper.Models.EmailVerifyCode", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<DateTime>("CreateTime")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Email")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("VerityCode")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.HasKey("Id");

                    b.ToTable("verifycodes");
                });

            modelBuilder.Entity("RentHelper.Models.Note", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<string>("Message")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<Guid>("OrderId")
                        .HasColumnType("char(36)");

                    b.Property<Guid>("SenderId")
                        .HasColumnType("char(36)");

                    b.Property<string>("SenderName")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<DateTime>("createTime")
                        .HasColumnType("datetime(6)");

                    b.HasKey("Id");

                    b.HasIndex("OrderId");

                    b.ToTable("notes");
                });

            modelBuilder.Entity("RentHelper.Models.Order", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<Guid?>("Lender")
                        .HasColumnType("char(36)");

                    b.Property<DateTime?>("OrderTime")
                        .HasColumnType("datetime(6)");

                    b.Property<DateTime?>("PayTime")
                        .HasColumnType("datetime(6)");

                    b.Property<DateTime?>("ProductArrive")
                        .HasColumnType("datetime(6)");

                    b.Property<DateTime?>("ProductGetBack")
                        .HasColumnType("datetime(6)");

                    b.Property<Guid?>("ProductId")
                        .HasColumnType("char(36)");

                    b.Property<DateTime?>("ProductSend")
                        .HasColumnType("datetime(6)");

                    b.Property<DateTime?>("ProductSendBack")
                        .HasColumnType("datetime(6)");

                    b.Property<int>("TradeMethod")
                        .HasColumnType("int");

                    b.Property<int>("TradeQuantity")
                        .HasColumnType("int");

                    b.Property<string>("p_Address")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<int>("p_Deposit")
                        .HasColumnType("int");

                    b.Property<string>("p_Desc")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<int>("p_Rent")
                        .HasColumnType("int");

                    b.Property<string>("p_RentMethod")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("p_Title")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("p_Type")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("p_Type1")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("p_Type2")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<decimal>("p_WeightPrice")
                        .HasColumnType("decimal(65,30)");

                    b.Property<bool>("p_isExchange")
                        .HasColumnType("tinyint(1)");

                    b.Property<bool>("p_isRent")
                        .HasColumnType("tinyint(1)");

                    b.Property<bool>("p_isSale")
                        .HasColumnType("tinyint(1)");

                    b.Property<Guid>("p_ownerId")
                        .HasColumnType("char(36)");

                    b.Property<int>("p_salePrice")
                        .HasColumnType("int");

                    b.Property<int>("status")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("ProductId");

                    b.ToTable("orders");
                });

            modelBuilder.Entity("RentHelper.Models.OrderExchangeItem", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<Guid>("OrderId")
                        .HasColumnType("char(36)");

                    b.Property<Guid>("WishItemId")
                        .HasColumnType("char(36)");

                    b.Property<int>("packageQuantity")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("OrderId");

                    b.HasIndex("WishItemId");

                    b.ToTable("orderExchangeItems");
                });

            modelBuilder.Entity("RentHelper.Models.Picture", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<DateTime>("DateTime")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Desc")
                        .HasColumnType("varchar(1500) CHARACTER SET utf8mb4")
                        .HasMaxLength(1500);

                    b.Property<Guid?>("OrderId")
                        .HasColumnType("char(36)");

                    b.Property<string>("Path")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<Guid>("ProductId")
                        .HasColumnType("char(36)");

                    b.HasKey("Id");

                    b.HasIndex("OrderId");

                    b.HasIndex("ProductId");

                    b.ToTable("pictures");
                });

            modelBuilder.Entity("RentHelper.Models.Product", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<string>("Address")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<int>("Deposit")
                        .HasColumnType("int");

                    b.Property<string>("Description")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<int>("Rent")
                        .HasColumnType("int");

                    b.Property<string>("RentMethod")
                        .IsRequired()
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("Type1")
                        .IsRequired()
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("Type2")
                        .IsRequired()
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<Guid>("UserId")
                        .HasColumnType("char(36)");

                    b.Property<decimal>("WeightPrice")
                        .HasColumnType("decimal(65,30)");

                    b.Property<decimal>("amount")
                        .HasColumnType("decimal(65,30)");

                    b.Property<DateTime>("createTime")
                        .HasColumnType("datetime(6)");

                    b.Property<bool>("isExchange")
                        .HasColumnType("tinyint(1)");

                    b.Property<bool>("isRent")
                        .HasColumnType("tinyint(1)");

                    b.Property<bool>("isSale")
                        .HasColumnType("tinyint(1)");

                    b.Property<int>("salePrice")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("products");
                });

            modelBuilder.Entity("RentHelper.Models.User", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<string>("Address")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("NickName")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("Phone")
                        .IsRequired()
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("dTK")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.HasKey("Id");

                    b.ToTable("users");
                });

            modelBuilder.Entity("RentHelper.Models.WishItem", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<string>("ExchangeItem")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<int>("RequestQuantity")
                        .HasColumnType("int");

                    b.Property<Guid>("UserId")
                        .HasColumnType("char(36)");

                    b.Property<decimal>("WeightPoint")
                        .HasColumnType("decimal(65,30)");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("wishItems");
                });

            modelBuilder.Entity("RentHelper.Models.CartItem", b =>
                {
                    b.HasOne("RentHelper.Models.Product", "Product")
                        .WithMany()
                        .HasForeignKey("ProductId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("RentHelper.Models.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("RentHelper.Models.Note", b =>
                {
                    b.HasOne("RentHelper.Models.Order", "Order")
                        .WithMany("notes")
                        .HasForeignKey("OrderId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("RentHelper.Models.Order", b =>
                {
                    b.HasOne("RentHelper.Models.Product", "Product")
                        .WithMany()
                        .HasForeignKey("ProductId");
                });

            modelBuilder.Entity("RentHelper.Models.OrderExchangeItem", b =>
                {
                    b.HasOne("RentHelper.Models.Order", "Order")
                        .WithMany("OrderExchangeItems")
                        .HasForeignKey("OrderId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("RentHelper.Models.WishItem", "WishItem")
                        .WithMany()
                        .HasForeignKey("WishItemId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("RentHelper.Models.Picture", b =>
                {
                    b.HasOne("RentHelper.Models.Order", null)
                        .WithMany("Pics")
                        .HasForeignKey("OrderId");

                    b.HasOne("RentHelper.Models.Product", "Product")
                        .WithMany("Pics")
                        .HasForeignKey("ProductId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("RentHelper.Models.Product", b =>
                {
                    b.HasOne("RentHelper.Models.User", "User")
                        .WithMany("Products")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("RentHelper.Models.WishItem", b =>
                {
                    b.HasOne("RentHelper.Models.User", "User")
                        .WithMany("WishItems")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
