﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Wedding_Planner.Persistance;

namespace WeddingPlanner.Migrations
{
    [DbContext(typeof(WeddingPlannerDbContext))]
    [Migration("20181029194056_weddingguests")]
    partial class weddingguests
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.1.4-rtm-31024")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("Wedding_Planner.Models.User", b =>
                {
                    b.Property<int>("UserId")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("CreatedAt");

                    b.Property<string>("Email")
                        .IsRequired();

                    b.Property<string>("FirstName")
                        .IsRequired();

                    b.Property<string>("LastName")
                        .IsRequired();

                    b.Property<string>("Password")
                        .IsRequired();

                    b.Property<DateTime>("UpdatedAt");

                    b.HasKey("UserId");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("Wedding_Planner.Models.UserWedding_Xrf", b =>
                {
                    b.Property<int>("UserWedding_XrfId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("State");

                    b.Property<int>("UserId");

                    b.Property<int>("WeddingId");

                    b.HasKey("UserWedding_XrfId");

                    b.HasIndex("UserId");

                    b.HasIndex("WeddingId");

                    b.ToTable("UserWedding_Xrf");
                });

            modelBuilder.Entity("Wedding_Planner.Models.Wedding", b =>
                {
                    b.Property<int>("WeddingId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Address")
                        .IsRequired();

                    b.Property<int>("CreatorId");

                    b.Property<DateTime>("Date");

                    b.Property<string>("Wedder1")
                        .IsRequired();

                    b.Property<string>("Wedder2")
                        .IsRequired();

                    b.HasKey("WeddingId");

                    b.HasIndex("CreatorId");

                    b.ToTable("Weddings");
                });

            modelBuilder.Entity("Wedding_Planner.Models.UserWedding_Xrf", b =>
                {
                    b.HasOne("Wedding_Planner.Models.User", "User")
                        .WithMany("WeddingGuests")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Wedding_Planner.Models.Wedding", "Wedding")
                        .WithMany("WeddingGuests")
                        .HasForeignKey("WeddingId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Wedding_Planner.Models.Wedding", b =>
                {
                    b.HasOne("Wedding_Planner.Models.User", "Creator")
                        .WithMany()
                        .HasForeignKey("CreatorId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
