using System.ComponentModel.DataAnnotations;

namespace PeminjamanRuangan.Core.DTOs
{
    public class BuatDtoRuangan
    {
        [Required]
        [StringLength(50)]
        public string IdRuangan { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string NamaRuangan { get; set; } = string.Empty;

        [Required]
        [Range(1, 1000)]
        public int Kapasitas { get; set; }
    }
}