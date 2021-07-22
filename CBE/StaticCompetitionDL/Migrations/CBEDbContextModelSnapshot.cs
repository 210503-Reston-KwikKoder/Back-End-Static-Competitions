﻿// <auto-generated />
using System;
using StaticCompetitionDL;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace StaticCompetitionDL.Migrations
{
    [DbContext(typeof(StaticCompetitionDbContext))]
    partial class StaticCompetitionDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("ProductVersion", "5.0.7")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("StaticCompetitionModels.Category", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("Name")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.ToTable("Categories");
                });

            modelBuilder.Entity("StaticCompetitionModels.Competition", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("CategoryId")
                        .HasColumnType("int");

                    b.Property<string>("CompetitionName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("EndDate")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("StartDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("TestAuthor")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("TestString")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("UserCreatedId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("CategoryId");

                    b.HasIndex("UserCreatedId");

                    b.ToTable("Competitions");
                });

            modelBuilder.Entity("StaticCompetitionModels.CompetitionStat", b =>
                {
                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.Property<int>("CompetitionId")
                        .HasColumnType("int");

                    b.Property<double>("Accuracy")
                        .HasColumnType("float");

                    b.Property<double>("WPM")
                        .HasColumnType("float");

                    b.Property<int>("rank")
                        .HasColumnType("int");

                    b.HasKey("UserId", "CompetitionId");

                    b.HasIndex("CompetitionId");

                    b.ToTable("CompetitionStats");
                });

            modelBuilder.Entity("StaticCompetitionModels.LiveCompStat", b =>
                {
                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.Property<int>("LiveCompetitionId")
                        .HasColumnType("int");

                    b.Property<int>("Losses")
                        .HasColumnType("int");

                    b.Property<double>("WLRatio")
                        .HasColumnType("float");

                    b.Property<int>("Wins")
                        .HasColumnType("int");

                    b.HasKey("UserId", "LiveCompetitionId");

                    b.HasIndex("LiveCompetitionId");

                    b.ToTable("LiveCompStats");
                });

            modelBuilder.Entity("StaticCompetitionModels.LiveCompetition", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("LiveCompetitions");
                });

            modelBuilder.Entity("StaticCompetitionModels.LiveCompetitionTest", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("CategoryId")
                        .HasColumnType("int");

                    b.Property<DateTime>("DateCreated")
                        .HasColumnType("datetime2");

                    b.Property<int>("LiveCompetitionId")
                        .HasColumnType("int");

                    b.Property<string>("TestAuthor")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("TestString")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("CategoryId");

                    b.HasIndex("LiveCompetitionId");

                    b.ToTable("LiveCompetitionTests");
                });

            modelBuilder.Entity("StaticCompetitionModels.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Auth0Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("Revapoints")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("Auth0Id")
                        .IsUnique()
                        .HasFilter("[Auth0Id] IS NOT NULL");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("StaticCompetitionModels.UserQueue", b =>
                {
                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.Property<int>("LiveCompetitionId")
                        .HasColumnType("int");

                    b.Property<DateTime>("EnterTime")
                        .HasColumnType("datetime2");

                    b.HasKey("UserId", "LiveCompetitionId");

                    b.HasIndex("LiveCompetitionId");

                    b.ToTable("UserQueues");
                });

            modelBuilder.Entity("StaticCompetitionModels.Competition", b =>
                {
                    b.HasOne("StaticCompetitionModels.Category", "Category")
                        .WithMany()
                        .HasForeignKey("CategoryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("StaticCompetitionModels.User", "User")
                        .WithMany()
                        .HasForeignKey("UserCreatedId");

                    b.Navigation("Category");

                    b.Navigation("User");
                });

            modelBuilder.Entity("StaticCompetitionModels.CompetitionStat", b =>
                {
                    b.HasOne("StaticCompetitionModels.Competition", "Competition")
                        .WithMany()
                        .HasForeignKey("CompetitionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("StaticCompetitionModels.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Competition");

                    b.Navigation("User");
                });

            modelBuilder.Entity("StaticCompetitionModels.LiveCompStat", b =>
                {
                    b.HasOne("StaticCompetitionModels.LiveCompetition", "LiveCompetition")
                        .WithMany()
                        .HasForeignKey("LiveCompetitionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("StaticCompetitionModels.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("LiveCompetition");

                    b.Navigation("User");
                });

            modelBuilder.Entity("StaticCompetitionModels.LiveCompetitionTest", b =>
                {
                    b.HasOne("StaticCompetitionModels.Category", "Category")
                        .WithMany()
                        .HasForeignKey("CategoryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("StaticCompetitionModels.LiveCompetition", "LiveCompetition")
                        .WithMany("LiveCompetitionTests")
                        .HasForeignKey("LiveCompetitionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Category");

                    b.Navigation("LiveCompetition");
                });

            modelBuilder.Entity("StaticCompetitionModels.UserQueue", b =>
                {
                    b.HasOne("StaticCompetitionModels.LiveCompetition", "LiveCompetition")
                        .WithMany()
                        .HasForeignKey("LiveCompetitionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("StaticCompetitionModels.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("LiveCompetition");

                    b.Navigation("User");
                });

            modelBuilder.Entity("StaticCompetitionModels.LiveCompetition", b =>
                {
                    b.Navigation("LiveCompetitionTests");
                });
#pragma warning restore 612, 618
        }
    }
}
