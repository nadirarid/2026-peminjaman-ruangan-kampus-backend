using System;
using System.ComponentModel.DataAnnotations;

namespace PeminjamanRuangan.Core.Models
{
    public class Ruangan
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(50)]
        public string IdRuangan { get; set; } = string.Empty;
        
        [Required]
        [StringLength(100)]
        public string NamaRuangan { get; set; } = string.Empty;
        
        public int Kapasitas { get; set; }
        
        [StringLength(50)]
        public string Status { get; set; } = "Tersedia";
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }
    }
}