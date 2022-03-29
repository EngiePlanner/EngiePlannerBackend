﻿// <auto-generated />
using System;
using DataAccessLayer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace DataAccessLayer.Migrations
{
    [DbContext(typeof(EngiePlannerContext))]
    [Migration("20220326191557_AddUserType")]
    partial class AddUserType
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("ProductVersion", "5.0.9")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("BusinessObjectLayer.Entities.AvailabilityEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("AvailableHours")
                        .HasColumnType("int");

                    b.Property<DateTime>("FromDate")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("ToDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("UserUsername")
                        .IsRequired()
                        .HasColumnType("nvarchar(10)");

                    b.HasKey("Id");

                    b.HasIndex("UserUsername");

                    b.ToTable("Availabilities");
                });

            modelBuilder.Entity("BusinessObjectLayer.Entities.DepartmentEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("Id");

                    b.ToTable("Departments");
                });

            modelBuilder.Entity("BusinessObjectLayer.Entities.GroupEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("DepartmentId")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("Id");

                    b.HasIndex("DepartmentId");

                    b.ToTable("Groups");
                });

            modelBuilder.Entity("BusinessObjectLayer.Entities.TaskEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("Duration")
                        .HasColumnType("int");

                    b.Property<DateTime?>("EndDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<DateTime>("PlannedDate")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("StartDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Subteam")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Tasks");
                });

            modelBuilder.Entity("BusinessObjectLayer.Entities.TaskPredecessorMapping", b =>
                {
                    b.Property<int>("TaskId")
                        .HasColumnType("int");

                    b.Property<int>("PredecessorId")
                        .HasColumnType("int");

                    b.HasKey("TaskId", "PredecessorId");

                    b.HasIndex("PredecessorId");

                    b.ToTable("TaskPredecessorsMappings");
                });

            modelBuilder.Entity("BusinessObjectLayer.Entities.UserDepartmentMapping", b =>
                {
                    b.Property<string>("UserUsername")
                        .HasMaxLength(10)
                        .HasColumnType("nvarchar(10)");

                    b.Property<int>("DepartmentId")
                        .HasColumnType("int");

                    b.HasKey("UserUsername", "DepartmentId");

                    b.HasIndex("DepartmentId");

                    b.ToTable("UserDepartmentMappings");
                });

            modelBuilder.Entity("BusinessObjectLayer.Entities.UserEntity", b =>
                {
                    b.Property<string>("Username")
                        .HasMaxLength(10)
                        .HasColumnType("nvarchar(10)");

                    b.Property<string>("DisplayName")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("LeaderUsername")
                        .HasColumnType("nvarchar(10)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<int>("RoleType")
                        .HasColumnType("int");

                    b.HasKey("Username");

                    b.HasIndex("LeaderUsername");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("BusinessObjectLayer.Entities.UserGroupMapping", b =>
                {
                    b.Property<string>("UserUsername")
                        .HasMaxLength(10)
                        .HasColumnType("nvarchar(10)");

                    b.Property<int>("GroupId")
                        .HasColumnType("int");

                    b.HasKey("UserUsername", "GroupId");

                    b.HasIndex("GroupId");

                    b.ToTable("UserGroupMappings");
                });

            modelBuilder.Entity("BusinessObjectLayer.Entities.UserTaskMapping", b =>
                {
                    b.Property<string>("UserUsername")
                        .HasColumnType("nvarchar(10)");

                    b.Property<int>("TaskId")
                        .HasColumnType("int");

                    b.Property<int>("UserType")
                        .HasColumnType("int");

                    b.HasKey("UserUsername", "TaskId", "UserType");

                    b.HasIndex("TaskId");

                    b.ToTable("UserTaskMapping");
                });

            modelBuilder.Entity("BusinessObjectLayer.Entities.AvailabilityEntity", b =>
                {
                    b.HasOne("BusinessObjectLayer.Entities.UserEntity", "User")
                        .WithMany("Availabilities")
                        .HasForeignKey("UserUsername")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("BusinessObjectLayer.Entities.GroupEntity", b =>
                {
                    b.HasOne("BusinessObjectLayer.Entities.DepartmentEntity", "Department")
                        .WithMany()
                        .HasForeignKey("DepartmentId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Department");
                });

            modelBuilder.Entity("BusinessObjectLayer.Entities.TaskPredecessorMapping", b =>
                {
                    b.HasOne("BusinessObjectLayer.Entities.TaskEntity", "Predecessor")
                        .WithMany("Tasks")
                        .HasForeignKey("PredecessorId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("BusinessObjectLayer.Entities.TaskEntity", "Task")
                        .WithMany("Predecessors")
                        .HasForeignKey("TaskId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Predecessor");

                    b.Navigation("Task");
                });

            modelBuilder.Entity("BusinessObjectLayer.Entities.UserDepartmentMapping", b =>
                {
                    b.HasOne("BusinessObjectLayer.Entities.DepartmentEntity", "Department")
                        .WithMany()
                        .HasForeignKey("DepartmentId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("BusinessObjectLayer.Entities.UserEntity", "User")
                        .WithMany("UserDepartments")
                        .HasForeignKey("UserUsername")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Department");

                    b.Navigation("User");
                });

            modelBuilder.Entity("BusinessObjectLayer.Entities.UserEntity", b =>
                {
                    b.HasOne("BusinessObjectLayer.Entities.UserEntity", "Leader")
                        .WithMany()
                        .HasForeignKey("LeaderUsername");

                    b.Navigation("Leader");
                });

            modelBuilder.Entity("BusinessObjectLayer.Entities.UserGroupMapping", b =>
                {
                    b.HasOne("BusinessObjectLayer.Entities.GroupEntity", "Group")
                        .WithMany()
                        .HasForeignKey("GroupId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("BusinessObjectLayer.Entities.UserEntity", "User")
                        .WithMany("UserGroups")
                        .HasForeignKey("UserUsername")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Group");

                    b.Navigation("User");
                });

            modelBuilder.Entity("BusinessObjectLayer.Entities.UserTaskMapping", b =>
                {
                    b.HasOne("BusinessObjectLayer.Entities.TaskEntity", "Task")
                        .WithMany("UserTasks")
                        .HasForeignKey("TaskId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("BusinessObjectLayer.Entities.UserEntity", "User")
                        .WithMany("UserTasks")
                        .HasForeignKey("UserUsername")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Task");

                    b.Navigation("User");
                });

            modelBuilder.Entity("BusinessObjectLayer.Entities.TaskEntity", b =>
                {
                    b.Navigation("Predecessors");

                    b.Navigation("Tasks");

                    b.Navigation("UserTasks");
                });

            modelBuilder.Entity("BusinessObjectLayer.Entities.UserEntity", b =>
                {
                    b.Navigation("Availabilities");

                    b.Navigation("UserDepartments");

                    b.Navigation("UserGroups");

                    b.Navigation("UserTasks");
                });
#pragma warning restore 612, 618
        }
    }
}