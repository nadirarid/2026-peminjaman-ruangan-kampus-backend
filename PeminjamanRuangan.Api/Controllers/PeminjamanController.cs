using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PeminjamanRuangan.Core.Models;
using PeminjamanRuangan.Infrastructure.Data;

namespace PeminjamanRuangan.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PeminjamanController : ControllerBase
    {
        private readonly AppDbContext _context;
        
        public PeminjamanController(AppDbContext context)
        {
            _context = context;
        }
        
        // GET: api/peminjaman
        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetSemuaPeminjaman(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? status = null)
        {
            var query = _context.Peminjaman
                .Include(p => p.Ruangan)
                .AsQueryable();
            
            if (!string.IsNullOrWhiteSpace(status))
            {
                query = query.Where(p => p.Status == status);
            }
            
            var totalItems = await query.CountAsync();
            
            var peminjaman = await query
                .OrderByDescending(p => p.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(p => new
                {
                    p.IdPeminjaman,
                    p.IdRuangan,
                    NamaRuangan = p.Ruangan != null ? p.Ruangan.NamaRuangan : "",
                    p.NamaUser,
                    p.IdUser,
                    p.Keterangan,
                    p.StartTime,
                    p.EndTime,
                    p.Status,
                    p.CreatedAt
                })
                .ToListAsync();
            
            Response.Headers.Add("X-Total-Count", totalItems.ToString());
            
            return Ok(peminjaman);
        }
        
        // GET: api/peminjaman/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<object>> GetPeminjamanById(int id)
        {
            var peminjaman = await _context.Peminjaman
                .Include(p => p.Ruangan)
                .Where(p => p.IdPeminjaman == id)
                .Select(p => new
                {
                    p.IdPeminjaman,
                    p.IdRuangan,
                    NamaRuangan = p.Ruangan != null ? p.Ruangan.NamaRuangan : "",
                    p.NamaUser,
                    p.IdUser,
                    p.Keterangan,
                    p.StartTime,
                    p.EndTime,
                    p.Status,
                    p.CreatedAt
                })
                .FirstOrDefaultAsync();
            
            if (peminjaman == null)
            {
                return NotFound(new { message = $"Peminjaman dengan ID {id} tidak ditemukan" });
            }
            
            return Ok(peminjaman);
        }

        // GET: api/peminjaman/cek-ketersediaan
        [HttpGet("cek-ketersediaan")]
        public async Task<IActionResult> CekKetersediaan(
            [FromQuery] int idRuangan,
            [FromQuery] DateTime startTime,
            [FromQuery] DateTime endTime)
        {
            var ruangan = await _context.Ruangan.FindAsync(idRuangan);
            if (ruangan == null)
            {
                return NotFound(new { message = "Ruangan tidak ditemukan" });
            }
            
            if (ruangan.Status != "Tersedia")
            {
                return Ok(new { 
                    tersedia = false, 
                    alasan = $"Ruangan sedang {ruangan.Status.ToLower()}" 
                });
            }
            
            var bentrok = await _context.Peminjaman
                .AnyAsync(p => p.IdRuangan == idRuangan &&
                            p.Status == "Disetujui" &&
                            p.StartTime < endTime &&
                            p.EndTime > startTime);
            
            if (bentrok)
            {
                return Ok(new { 
                    tersedia = false, 
                    alasan = "Maaf, ruangan sudah terjadwal" 
                });
            }
            
            return Ok(new { tersedia = true, alasan = "Ruangan tersedia" });
        }
        
        // POST: api/peminjaman
        [HttpPost]
        public async Task<ActionResult<object>> BuatPeminjaman(Peminjaman peminjaman)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var durasi = peminjaman.EndTime - peminjaman.StartTime;
  
            if (durasi.TotalHours > 8)
            {
                return BadRequest(new { message = "Durasi peminjaman maksimal 8 jam" });
            }
            
            var ruangan = await _context.Ruangan.FindAsync(peminjaman.IdRuangan);
            if (ruangan == null)
            {
                return BadRequest(new { message = "Ruangan tidak ditemukan" });
            }
            
            if (ruangan.Status != "Tersedia")
            {
                return BadRequest(new { message = "Ruangan sedang tidak tersedia" });
            }
            
            var bentrok = await _context.Peminjaman
                .AnyAsync(p => p.IdRuangan == peminjaman.IdRuangan &&
                               p.Status == "Disetujui" &&
                               p.StartTime < peminjaman.EndTime &&
                               p.EndTime > peminjaman.StartTime);
            
            if (bentrok)
            {
                return BadRequest(new { message = "Maaf, ruangan sudah terjadwal" });
            }
            
            peminjaman.CreatedAt = DateTime.UtcNow;
            peminjaman.Status = "Diproses";
            
            _context.Peminjaman.Add(peminjaman);
            await _context.SaveChangesAsync();
            
            ruangan.Status = "Dipakai";
            ruangan.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            
            return CreatedAtAction(nameof(GetPeminjamanById), new { id = peminjaman.IdPeminjaman }, peminjaman);
        }
        
        // PATCH: api/peminjaman/{id}/setujui
        [HttpPatch("{id}/setujui")]
        public async Task<IActionResult> SetujuiPeminjaman(int id)
        {
            var peminjaman = await _context.Peminjaman.FindAsync(id);
            
            if (peminjaman == null)
            {
                return NotFound(new { message = "Peminjaman tidak ditemukan" });
            }
            
            peminjaman.Status = "Disetujui";
            await _context.SaveChangesAsync();
            
            return Ok(new { message = "Peminjaman disetujui" });
        }
        
        // PATCH: api/peminjaman/{id}/tolak
        [HttpPatch("{id}/tolak")]
        public async Task<IActionResult> TolakPeminjaman(int id)
        {
            var peminjaman = await _context.Peminjaman.FindAsync(id);
            
            if (peminjaman == null)
            {
                return NotFound(new { message = "Peminjaman tidak ditemukan" });
            }
            
            peminjaman.Status = "Ditolak";
            
            var ruangan = await _context.Ruangan.FindAsync(peminjaman.IdRuangan);
            if (ruangan != null)
            {
                ruangan.Status = "Tersedia";
                ruangan.UpdatedAt = DateTime.UtcNow;
            }
            
            await _context.SaveChangesAsync();
            
            return Ok(new { message = "Peminjaman ditolak" });
        }
        
        // PATCH: api/peminjaman/{id}/selesai
        [HttpPatch("{id}/selesai")]
        public async Task<IActionResult> SelesaikanPeminjaman(int id)
        {
            var peminjaman = await _context.Peminjaman.FindAsync(id);
            
            if (peminjaman == null)
            {
                return NotFound(new { message = "Peminjaman tidak ditemukan" });
            }
            
            peminjaman.Status = "Selesai";
            
            var ruangan = await _context.Ruangan.FindAsync(peminjaman.IdRuangan);
            if (ruangan != null)
            {
                ruangan.Status = "Tersedia";
                ruangan.UpdatedAt = DateTime.UtcNow;
            }
            
            await _context.SaveChangesAsync();
            
            return Ok(new { message = "Peminjaman selesai" });
        }
    }
}