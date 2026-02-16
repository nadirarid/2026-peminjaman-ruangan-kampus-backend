using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PeminjamanRuangan.Core.DTOs;
using PeminjamanRuangan.Core.Models;
using PeminjamanRuangan.Infrastructure.Data;

namespace PeminjamanRuangan.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RuanganController : ControllerBase
    {
        private readonly AppDbContext _context;
        
        public RuanganController(AppDbContext context)
        {
            _context = context;
        }
        
        // GET: api/ruangan
        [HttpGet]
        public async Task<ActionResult<IEnumerable<FormDtoPeminjaman>>> GetSemuaRuangan(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? search = null)
        {
            var query = _context.Ruangan.AsQueryable();
            
            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(r => 
                    r.IdRuangan.Contains(search) || 
                    r.NamaRuangan.Contains(search));
            }
            
            var totalItems = await query.CountAsync();
            
            var ruangan = await query
                .OrderBy(r => r.IdRuangan)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(r => new FormDtoPeminjaman
                {
                    IdPeminjaman = r.Id,
                    IdRuangan = r.IdRuangan,
                    NamaRuangan = r.NamaRuangan,
                    Kapasitas = r.Kapasitas,
                    Status = r.Status,
                    CreatedAt = r.CreatedAt
                })
                .ToListAsync();
            
            Response.Headers.Add("X-Total-Count", totalItems.ToString());
            Response.Headers.Add("X-Total-Pages", ((int)Math.Ceiling(totalItems / (double)pageSize)).ToString());
            Response.Headers.Add("X-Current-Page", page.ToString());
            
            return Ok(ruangan);
        }
        
        // GET: api/ruangan/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<FormDtoPeminjaman>> GetRuanganById(int id)
        {
            var ruangan = await _context.Ruangan
                .Where(r => r.Id == id)
                .Select(r => new FormDtoPeminjaman
                {
                    IdPeminjaman = r.Id,
                    IdRuangan = r.IdRuangan,
                    NamaRuangan = r.NamaRuangan,
                    Kapasitas = r.Kapasitas,
                    Status = r.Status,
                    CreatedAt = r.CreatedAt
                })
                .FirstOrDefaultAsync();
            
            if (ruangan == null)
            {
                return NotFound(new { message = $"Ruangan dengan ID {id} tidak ditemukan" });
            }
            
            return Ok(ruangan);
        }
        
        // GET: api/ruangan/kode/{idRuangan}
        [HttpGet("kode/{idRuangan}")]
        public async Task<ActionResult<FormDtoPeminjaman>> GetRuanganByKode(string idRuangan)
        {
            var ruangan = await _context.Ruangan
                .Where(r => r.IdRuangan == idRuangan)
                .Select(r => new FormDtoPeminjaman
                {
                    IdPeminjaman = r.Id,
                    IdRuangan = r.IdRuangan,
                    NamaRuangan = r.NamaRuangan,
                    Kapasitas = r.Kapasitas,
                    Status = r.Status,
                    CreatedAt = r.CreatedAt
                })
                .FirstOrDefaultAsync();
            
            if (ruangan == null)
            {
                return NotFound(new { message = $"Ruangan dengan ID {idRuangan} tidak ditemukan" });
            }
            
            return Ok(ruangan);
        }
        
        // POST: api/ruangan
        [HttpPost]
        public async Task<ActionResult<FormDtoPeminjaman>> BuatRuangan(BuatDtoRuangan buatDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            
            var ruanganExist = await _context.Ruangan
                .AnyAsync(r => r.IdRuangan == buatDto.IdRuangan);
            
            if (ruanganExist)
            {
                return Conflict(new { message = $"Ruangan dengan ID {buatDto.IdRuangan} sudah ada" });
            }
            
            var ruangan = new Ruangan
            {
                IdRuangan = buatDto.IdRuangan,
                NamaRuangan = buatDto.NamaRuangan,
                Kapasitas = buatDto.Kapasitas,
                Status = "Tersedia",
                CreatedAt = DateTime.UtcNow
            };
            
            _context.Ruangan.Add(ruangan);
            await _context.SaveChangesAsync();
            
            var response = new FormDtoPeminjaman
            {
                IdPeminjaman = ruangan.Id,
                IdRuangan = ruangan.IdRuangan,
                NamaRuangan = ruangan.NamaRuangan,
                Kapasitas = ruangan.Kapasitas,
                Status = ruangan.Status,
                CreatedAt = ruangan.CreatedAt
            };
            
            return CreatedAtAction(nameof(GetRuanganById), new { id = ruangan.Id }, response);
        }
        
        // PUT: api/ruangan/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRuangan(int id, UpdateDtoRuangan updateDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            
            var ruangan = await _context.Ruangan.FindAsync(id);
            
            if (ruangan == null)
            {
                return NotFound(new { message = $"Ruangan dengan ID {id} tidak ditemukan" });
            }
            
            var ruanganExist = await _context.Ruangan
                .AnyAsync(r => r.IdRuangan == updateDto.IdRuangan && r.Id != id);
            
            if (ruanganExist)
            {
                return Conflict(new { message = $"Ruangan dengan ID {updateDto.IdRuangan} sudah ada" });
            }
            
            ruangan.IdRuangan = updateDto.IdRuangan;
            ruangan.NamaRuangan = updateDto.NamaRuangan;
            ruangan.Kapasitas = updateDto.Kapasitas;
            ruangan.Status = updateDto.Status;
            ruangan.UpdatedAt = DateTime.UtcNow;
            
            await _context.SaveChangesAsync();
            
            return NoContent();
        }
        
        // PATCH: api/ruangan/{id}/status
        [HttpPatch("{id}/status")]
        public async Task<IActionResult> UpdateStatusRuangan(int id, [FromBody] string status)
        {
            var ruangan = await _context.Ruangan.FindAsync(id);
            
            if (ruangan == null)
            {
                return NotFound(new { message = $"Ruangan dengan ID {id} tidak ditemukan" });
            }
            
            var validStatus = new[] { "Tersedia", "Dipakai", "Perbaikan" };
            if (!validStatus.Contains(status))
            {
                return BadRequest(new { message = $"Status harus diisi salah satu" });
            }
            
            ruangan.Status = status;
            ruangan.UpdatedAt = DateTime.UtcNow;
            
            await _context.SaveChangesAsync();
            
            return Ok(new { message = $"Status ruangan berhasil diupdate" });
        }
        
        // DELETE: api/ruangan/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> HapusRuangan(int id)
        {
            var ruangan = await _context.Ruangan.FindAsync(id);
            
            if (ruangan == null)
            {
                return NotFound(new { message = $"Ruangan dengan ID {id} tidak ditemukan" });
            }
            
            var sedangDipinjam = await _context.Peminjaman
                .AnyAsync(p => p.IdRuangan == id && p.Status == "Diproses");
            
            if (sedangDipinjam)
            {
                return BadRequest(new { message = "Ruangan tidak dapat dihapus karena sedang dalam proses peminjaman" });
            }
            
            ruangan.IsDeleted = true;
            ruangan.DeletedAt = DateTime.UtcNow;
            ruangan.Status = "Dihapus";
            
            await _context.SaveChangesAsync();
            
            return Ok(new { message = "Ruangan berhasil dihapus" });
        }
    }
}