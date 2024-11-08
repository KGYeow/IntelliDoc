﻿using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace IntelliDoc_API.Models
{
    public partial class IntelliDocDBContext : DbContext
    {
        public IntelliDocDBContext()
        {
        }

        public IntelliDocDBContext(DbContextOptions<IntelliDocDBContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Document> Documents { get; set; } = null!;
        public virtual DbSet<DocumentRelationship> DocumentRelationships { get; set; } = null!;
        public virtual DbSet<DocumentUserAction> DocumentUserActions { get; set; } = null!;
        public virtual DbSet<DocumentVersionHistory> DocumentVersionHistories { get; set; } = null!;
        public virtual DbSet<Notification> Notifications { get; set; } = null!;
        public virtual DbSet<Page> Pages { get; set; } = null!;
        public virtual DbSet<RoleAccessPage> RoleAccessPages { get; set; } = null!;
        public virtual DbSet<User> Users { get; set; } = null!;
        public virtual DbSet<UserManualDocument> UserManualDocuments { get; set; } = null!;
        public virtual DbSet<UserRole> UserRoles { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Server=ARTHURKG;Database=IntelliDocDB;Trusted_Connection=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Document>(entity =>
            {
                entity.ToTable("Document");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.Category).IsUnicode(false);

                entity.Property(e => e.CreatedById).HasColumnName("CreatedByID");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.Description).IsUnicode(false);

                entity.Property(e => e.ModifiedById).HasColumnName("ModifiedByID");

                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");

                entity.Property(e => e.Name).IsUnicode(false);

                entity.Property(e => e.Type)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.HasOne(d => d.CreatedBy)
                    .WithMany(p => p.DocumentCreatedBies)
                    .HasForeignKey(d => d.CreatedById)
                    .HasConstraintName("FK_Document_CreatedByUser");

                entity.HasOne(d => d.ModifiedBy)
                    .WithMany(p => p.DocumentModifiedBies)
                    .HasForeignKey(d => d.ModifiedById)
                    .HasConstraintName("FK_Document_ModifiedByUser");
            });

            modelBuilder.Entity<DocumentRelationship>(entity =>
            {
                entity.ToTable("DocumentRelationship");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.DocumentMainId).HasColumnName("DocumentMainID");

                entity.Property(e => e.DocumentRelatedId).HasColumnName("DocumentRelatedID");

                entity.HasOne(d => d.DocumentMain)
                    .WithMany(p => p.DocumentRelationshipDocumentMains)
                    .HasForeignKey(d => d.DocumentMainId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_DocumentRelationship_DocumentMain");

                entity.HasOne(d => d.DocumentRelated)
                    .WithMany(p => p.DocumentRelationshipDocumentRelateds)
                    .HasForeignKey(d => d.DocumentRelatedId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_DocumentRelationship_DocumentRelated");
            });

            modelBuilder.Entity<DocumentUserAction>(entity =>
            {
                entity.HasKey(e => new { e.UserId, e.DocumentId });

                entity.ToTable("DocumentUserAction");

                entity.Property(e => e.UserId).HasColumnName("UserID");

                entity.Property(e => e.DocumentId).HasColumnName("DocumentID");

                entity.HasOne(d => d.Document)
                    .WithMany(p => p.DocumentUserActions)
                    .HasForeignKey(d => d.DocumentId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_DocumentUserAction_Document");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.DocumentUserActions)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_DocumentUserAction_User");
            });

            modelBuilder.Entity<DocumentVersionHistory>(entity =>
            {
                entity.ToTable("DocumentVersionHistory");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.ArchivedDate).HasColumnType("datetime");

                entity.Property(e => e.DocumentId).HasColumnName("DocumentID");

                entity.Property(e => e.ModifiedById).HasColumnName("ModifiedByID");

                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");

                entity.HasOne(d => d.Document)
                    .WithMany(p => p.DocumentVersionHistories)
                    .HasForeignKey(d => d.DocumentId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_DocumentVersionHistory_Document");

                entity.HasOne(d => d.ModifiedBy)
                    .WithMany(p => p.DocumentVersionHistories)
                    .HasForeignKey(d => d.ModifiedById)
                    .HasConstraintName("FK_DocumentVersionHistory_User");
            });

            modelBuilder.Entity<Notification>(entity =>
            {
                entity.ToTable("Notification");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.Description).IsUnicode(false);

                entity.Property(e => e.Title).IsUnicode(false);

                entity.Property(e => e.UserId).HasColumnName("UserID");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Notifications)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Notification_User");
            });

            modelBuilder.Entity<Page>(entity =>
            {
                entity.ToTable("Page");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.AccessName).IsUnicode(false);

                entity.Property(e => e.Name)
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<RoleAccessPage>(entity =>
            {
                entity.ToTable("RoleAccessPage");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.PageId).HasColumnName("PageID");

                entity.Property(e => e.UserRoleId).HasColumnName("UserRoleID");

                entity.HasOne(d => d.Page)
                    .WithMany(p => p.RoleAccessPages)
                    .HasForeignKey(d => d.PageId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_RoleAccessPage_Page");

                entity.HasOne(d => d.UserRole)
                    .WithMany(p => p.RoleAccessPages)
                    .HasForeignKey(d => d.UserRoleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_RoleAccessPage_UserRole");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("User");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.Email).HasMaxLength(256);

                entity.Property(e => e.Password).HasMaxLength(100);

                entity.Property(e => e.UserRoleId).HasColumnName("UserRoleID");

                entity.Property(e => e.Username).HasMaxLength(100);

                entity.HasOne(d => d.UserRole)
                    .WithMany(p => p.Users)
                    .HasForeignKey(d => d.UserRoleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_User_UserRole");
            });

            modelBuilder.Entity<UserManualDocument>(entity =>
            {
                entity.ToTable("UserManualDocument");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.Name).IsUnicode(false);

                entity.Property(e => e.Type)
                    .HasMaxLength(10)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<UserRole>(entity =>
            {
                entity.ToTable("UserRole");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.Name)
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
