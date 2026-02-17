using System;

namespace PeminjamanRuangan.Core.DTOs
{
    public class FormDtoPeminjaman
    {
        public int IdPeminjaman { get; set; }
        public string IdRuangan { get; set; } = string.Empty;
        public string NamaRuangan { get; set; } = string.Empty;
        public int Kapasitas { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}