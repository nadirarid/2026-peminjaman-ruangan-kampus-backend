using Microsoft.EntityFrameworkCore;
using PeminjamanRuangan.Core.Models;

namespace PeminjamanRuangan.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) 
            : base(options)
        {
        }

        public DbSet<Ruangan> Ruangan { get; set; }
        public DbSet<Peminjaman> Peminjaman { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Konfigurasi Ruangan
            modelBuilder.Entity<Ruangan>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.IdRuangan).IsUnique();
                entity.Property(e => e.IdRuangan).IsRequired().HasMaxLength(50);
                entity.Property(e => e.NamaRuangan).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Status).HasMaxLength(50).HasDefaultValue("Tersedia");
                entity.Property(e => e.IsDeleted).HasDefaultValue(false);
                entity.HasQueryFilter(e => !e.IsDeleted); // Soft delete filter
            });

            // Konfigurasi Peminjaman
            modelBuilder.Entity<Peminjaman>(entity =>
            {
                entity.HasKey(e => e.IdPeminjaman);
                entity.Property(e => e.NamaUser).IsRequired().HasMaxLength(100);
                entity.Property(e => e.IdUser).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Keterangan).HasMaxLength(100);
                entity.Property(e => e.Status).HasMaxLength(50).HasDefaultValue("Diproses");

                entity.HasOne(e => e.Ruangan)
                      .WithMany()
                      .HasForeignKey(e => e.IdRuangan)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // Data Seeding
            modelBuilder.Entity<Ruangan>().HasData(
                new Ruangan 
                { 
                    Id = 1, 
                    IdRuangan = "A001", 
                    NamaRuangan = "Lab Jaringan", 
                    Kapasitas = 30, 
                    Status = "Tersedia",
                    CreatedAt = new DateTime(2026, 2, 17, 0, 0, 0, DateTimeKind.Utc)
                },
                new Ruangan 
                { 
                    Id = 2, 
                    IdRuangan = "A002", 
                    NamaRuangan = "Lab Database", 
                    Kapasitas = 30, 
                    Status = "Tersedia",
                    CreatedAt = new DateTime(2026, 2, 17, 0, 0, 0, DateTimeKind.Utc)
                },
                new Ruangan 
                { 
                    Id = 3, 
                    IdRuangan = "A003", 
                    NamaRuangan = "Lab Perangkat Lunak", 
                    Kapasitas = 30, 
                    Status = "Tersedia",
                    CreatedAt = new DateTime(2026, 2, 17, 0, 0, 0, DateTimeKind.Utc)
                }
            );
        }
    }
}