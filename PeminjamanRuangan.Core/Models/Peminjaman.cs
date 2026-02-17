using System;
using System.ComponentModel.DataAnnotations;

namespace PeminjamanRuangan.Core.Models
{
    public class Peminjaman
    {
        public int IdPeminjaman { get; set; }
        
        [Required]
        public int IdRuangan { get; set; }
        
        [Required]
        [StringLength(100)]
        public string NamaUser { get; set; } = string.Empty;
        
        [Required]
        [StringLength(50)]
        public string IdUser { get; set; } = string.Empty;
        
        [Required]
        [StringLength(100)]
        public string Keterangan { get; set; } = string.Empty;
        
        [Required]
        public DateTime StartTime { get; set; }
        
        [Required]
        public DateTime EndTime { get; set; }
        
        [StringLength(50)]
        public string Status { get; set; } = "Diproses";
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public Ruangan? Ruangan { get; set; }
    }
}