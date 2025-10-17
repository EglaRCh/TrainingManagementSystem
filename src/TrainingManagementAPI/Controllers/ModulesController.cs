using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TrainingManagementAPI.Data;
using TrainingManagementAPI.DTOs;
using TrainingManagementAPI.Models;

namespace TrainingManagementAPI.Controllers
{
    // 🧭 [ApiController]: indica que este archivo define un controlador de API REST.
    // ASP.NET Core aplicará validaciones automáticas de modelo y formato JSON.
    [ApiController]

    // 📌 [Route]: define la URL base para este controlador.
    // "api/modules" será la dirección base de los endpoints.
    [Route("api/[controller]")]
    public class ModulesController : ControllerBase
    {
        // 🔹 Dependencia del contexto de base de datos (AppDbContext).
        // Permite acceder a las tablas definidas en EF Core (Modules, Goals, etc.)
        private readonly AppDbContext _db;

        // 📦 Constructor: inyecta el contexto de base de datos cuando se crea el controlador.
        public ModulesController(AppDbContext db)
        {
            _db = db;
        }

        // =====================================================================
        // 1️⃣ [GET] api/modules
        // Este método obtiene todos los módulos o los filtra por GoalId.
        // =====================================================================
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ModuleReadDto>>> GetAll(
            [FromQuery] int? goalId,     // filtro opcional por objetivo
            [FromQuery] int page = 1,    // número de página
            [FromQuery] int pageSize = 20, // tamaño de página
            CancellationToken ct = default)
        {
            // 🧩 Validaciones simples de paginación
            page = Math.Clamp(page, 1, 10000);
            pageSize = Math.Clamp(pageSize, 1, 100);

            // 🔎 AsNoTracking(): indica a EF Core que no rastree los resultados
            // (mejor rendimiento para lecturas, no se modifican).
            var query = _db.Modules.AsNoTracking().AsQueryable();

            // 🎯 Si se pasó un GoalId, filtra los módulos de ese objetivo
            if (goalId.HasValue)
                query = query.Where(m => m.GoalId == goalId.Value);

            // 📊 Ordena por fecha de creación (los más recientes primero)
            var items = await query
                .OrderByDescending(m => m.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                // 🪄 Proyecta a DTO (solo campos que queremos exponer)
                .Select(m => new ModuleReadDto(
                    m.Id,
                    m.GoalId,
                    m.Type,
                    m.DurationWeeks,
                    m.SessionsPerWeek,
                    m.Notes,
                    m.CreatedAt
                ))
                .ToListAsync(ct);

            // 🧾 Devuelve 200 OK con la lista
            return Ok(items);
        }

        // =====================================================================
        // 2️⃣ [GET] api/modules/{id}
        // Obtiene un solo módulo por su Id.
        // =====================================================================
        [HttpGet("{id:int}")]
        public async Task<ActionResult<ModuleReadDto>> GetById(int id, CancellationToken ct)
        {
            // 🔍 Busca el módulo correspondiente
            var m = await _db.Modules.AsNoTracking()
                        .FirstOrDefaultAsync(x => x.Id == id, ct);

            // ❌ Si no existe, devuelve 404 Not Found
            if (m is null) return NotFound();

            // ✅ Si existe, transforma a DTO y devuelve 200 OK
            var dto = new ModuleReadDto(
                m.Id, m.GoalId, m.Type, m.DurationWeeks,
                m.SessionsPerWeek, m.Notes, m.CreatedAt
            );

            return Ok(dto);
        }

        // =====================================================================
        // 3️⃣ [POST] api/modules
        // Crea un nuevo módulo de entrenamiento.
        // =====================================================================
        [HttpPost]
        public async Task<ActionResult<ModuleReadDto>> Create(
            [FromBody] ModuleCreateDto dto,  // 👈 Se usa DTO con Data Annotations
            CancellationToken ct)
        {
            // ⚙️ Validaciones de dominio adicionales
            // Aunque Data Annotations ya hace lo básico, agregamos reglas lógicas:
            if (dto.DurationWeeks < 1 || dto.SessionsPerWeek < 1)
                return BadRequest("La Duración Semanal y Sesiones Semanales deben ser => 1.");

            // Verifica que el GoalId exista (evita error de FK en la BD)
            var goalExists = await _db.Goals.AnyAsync(g => g.Id == dto.GoalId, ct);
            if (!goalExists)
                return BadRequest($"El Objetivo con el ID {dto.GoalId} no existe.");

            // 🧱 Crea la entidad desde el DTO
            var entity = new Module
            {
                GoalId = dto.GoalId,
                Type = dto.Type.Trim(),
                DurationWeeks = dto.DurationWeeks,
                SessionsPerWeek = dto.SessionsPerWeek,
                Notes = dto.Notes,
                CreatedAt = DateTime.UtcNow
            };

            // 🧩 Agrega a la base de datos
            _db.Modules.Add(entity);
            await _db.SaveChangesAsync(ct);

            // 🔁 Convierte la entidad a DTO de lectura
            var readDto = new ModuleReadDto(
                entity.Id, entity.GoalId, entity.Type,
                entity.DurationWeeks, entity.SessionsPerWeek,
                entity.Notes, entity.CreatedAt
            );

            // 📤 Devuelve 201 Created con ubicación y datos
            return CreatedAtAction(nameof(GetById), new { id = entity.Id }, readDto);
        }

        // =====================================================================
        // 4️⃣ [PUT] api/modules/{id}
        // Actualiza parcialmente los datos de un módulo.
        // =====================================================================
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(
            int id,
            [FromBody] ModuleUpdateDto dto, // DTO de actualización
            CancellationToken ct)
        {
            // Busca el módulo
            var entity = await _db.Modules.FirstOrDefaultAsync(m => m.Id == id, ct);
            if (entity is null) return NotFound();

            // ⚙️ Validaciones lógicas
            if (string.IsNullOrWhiteSpace(dto.Type))
                return BadRequest("Type is required.");
            if (dto.DurationWeeks < 1 || dto.SessionsPerWeek < 1)
                return BadRequest("La Duración Semanal y Sesioes por semana debe ser => a 1.");

            // 🔄 Actualiza solo campos permitidos (no se puede cambiar GoalId, CreatedAt)
            entity.Type = dto.Type.Trim();
            entity.DurationWeeks = dto.DurationWeeks;
            entity.SessionsPerWeek = dto.SessionsPerWeek;
            entity.Notes = dto.Notes;

            await _db.SaveChangesAsync(ct);

            // ✅ No hay contenido que devolver (convención REST)
            return NoContent();
        }

        // =====================================================================
        // 5️⃣ [DELETE] api/modules/{id}
        // Elimina un módulo por su Id.
        // =====================================================================
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id, CancellationToken ct)
        {
            var entity = await _db.Modules.FindAsync([id], ct);
            if (entity is null) return NotFound();

            _db.Modules.Remove(entity);
            await _db.SaveChangesAsync(ct);

            // ✅ 204 No Content = eliminado con éxito
            return NoContent();
        }
    }
}
