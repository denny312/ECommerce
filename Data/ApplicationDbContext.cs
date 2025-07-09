using ECommerce.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Scaffolding.Internal;
using System;
using System.Collections.Generic;
namespace ECommerce.Data
{
    public partial class ApplicationDbContext : IdentityDbContext<IdentityUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<EfmigrationsHistory> EfmigrationsHistories { get; set; }
        public virtual DbSet<Ordini> Ordinis { get; set; }
        public virtual DbSet<OrdiniRasoi> OrdiniRasois { get; set; }
        public virtual DbSet<Rasoi> Rasois { get; set; }
        public virtual DbSet<Recensioni> Recensionis { get; set; }
        public virtual DbSet<Utenti> Utentis { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var configuration = new ConfigurationBuilder()
                    .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                    .Build();

                var connectionString = configuration.GetConnectionString("DefaultConnection");

                if (string.IsNullOrEmpty(connectionString))
                {
                    throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
                }

                optionsBuilder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
            }
        }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder); // <<------- CHIAMATA OBBLIGATORIA

            modelBuilder
                .UseCollation("utf8mb4_0900_ai_ci")
                .HasCharSet("utf8mb4");

            modelBuilder.Entity<EfmigrationsHistory>(entity =>
            {
                entity.HasKey(e => e.MigrationId).HasName("PRIMARY");

                entity.ToTable("__EFMigrationsHistory");

                entity.Property(e => e.MigrationId).HasMaxLength(150);
                entity.Property(e => e.ProductVersion).HasMaxLength(32);
            });

            modelBuilder.Entity<Ordini>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PRIMARY");

                entity.ToTable("ordini");

                entity.HasIndex(e => e.UtenteId, "utente_id");

                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Data)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP")
                    .HasColumnType("timestamp")
                    .HasColumnName("data");
                entity.Property(e => e.UtenteId).HasColumnName("utente_id");

                entity.HasOne(d => d.Utente).WithMany(p => p.Ordinis)
                    .HasForeignKey(d => d.UtenteId)
                    .HasConstraintName("ordini_ibfk_1");
            });

            modelBuilder.Entity<OrdiniRasoi>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PRIMARY");

                entity.ToTable("ordini_rasoi");

                entity.HasIndex(e => e.OrdineId, "ordine_id");

                entity.HasIndex(e => e.RasoioId, "rasoio_id");

                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.OrdineId).HasColumnName("ordine_id");
                entity.Property(e => e.Quantita).HasColumnName("quantita");
                entity.Property(e => e.RasoioId).HasColumnName("rasoio_id");

                entity.HasOne(d => d.Ordine).WithMany(p => p.OrdiniRasois)
                    .HasForeignKey(d => d.OrdineId)
                    .HasConstraintName("ordini_rasoi_ibfk_1");

                entity.HasOne(d => d.Rasoio).WithMany(p => p.OrdiniRasois)
                    .HasForeignKey(d => d.RasoioId)
                    .HasConstraintName("ordini_rasoi_ibfk_2");
            });

            modelBuilder.Entity<Rasoi>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PRIMARY");

                entity.ToTable("rasoi");

                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Marca)
                    .HasMaxLength(100)
                    .HasColumnName("marca");
                entity.Property(e => e.Modello)
                    .HasMaxLength(100)
                    .HasColumnName("modello");
                entity.Property(e => e.Prezzo)
                    .HasPrecision(10, 2)
                    .HasColumnName("prezzo");
                entity.Property(e => e.Tipo)
                    .HasColumnType("enum('manuale','elettrico')")
                    .HasColumnName("tipo");
            });

            modelBuilder.Entity<Recensioni>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PRIMARY");

                entity.ToTable("recensioni");

                entity.HasIndex(e => e.RasoioId, "rasoio_id");

                entity.HasIndex(e => e.UtenteId, "utente_id");

                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Commento)
                    .HasColumnType("text")
                    .HasColumnName("commento");
                entity.Property(e => e.RasoioId).HasColumnName("rasoio_id");
                entity.Property(e => e.UtenteId).HasColumnName("utente_id");
                entity.Property(e => e.Voto).HasColumnName("voto");

                entity.HasOne(d => d.Rasoio).WithMany(p => p.Recensionis)
                    .HasForeignKey(d => d.RasoioId)
                    .HasConstraintName("recensioni_ibfk_2");

                entity.HasOne(d => d.Utente).WithMany(p => p.Recensionis)
                    .HasForeignKey(d => d.UtenteId)
                    .HasConstraintName("recensioni_ibfk_1");
            });

            modelBuilder.Entity<Utenti>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PRIMARY");

                entity.ToTable("utenti");

                entity.HasIndex(e => e.Email, "email").IsUnique();

                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Email)
                    .HasMaxLength(100)
                    .HasColumnName("email");
                entity.Property(e => e.Nome)
                    .HasMaxLength(100)
                    .HasColumnName("nome");
                entity.Property(e => e.Password)
                    .HasMaxLength(100)
                    .HasColumnName("password");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }

}

