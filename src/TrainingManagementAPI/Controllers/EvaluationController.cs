using TrainingManagementAPI.DTOs;
using TrainingManagementAPI.Models;
using TrainingManagementAPI.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]
public class EvaluationsController : ControllerBase
{
    private readonly AppDbContext _db;
    public EvaluationsController(AppDbContext db) => _db = db;

    // GET: list con opcional traineeId + paginado
    [HttpGet]
    public async Task<ActionResult<IEnumerable<EvaluationReadDto>>> GetAll(
        [FromQuery] int? traineeId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken ct = default)
    {
        page = Math.Clamp(page, 1, 10_000);
        pageSize = Math.Clamp(pageSize, 1, 100);

        var q = _db.Evaluations.AsNoTracking().AsQueryable();
        if (traineeId.HasValue) q = q.Where(e => e.TraineeId == traineeId.Value);

        var items = await q
            .OrderByDescending(e => e.Date)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(e => new EvaluationReadDto(
                e.Id, e.TraineeId, e.Date, e.WaistCm, e.ArmCm, e.WeightKg, e.BodyFatPct, e.Notes, e.CreatedAt
            ))
            .ToListAsync(ct);

        return Ok(items);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<EvaluationReadDto>> GetById(int id, CancellationToken ct)
    {
        var e = await _db.Evaluations.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, ct);
        if (e is null) return NotFound();

        var dto = new EvaluationReadDto(
            e.Id, e.TraineeId, e.Date, e.WaistCm, e.ArmCm, e.WeightKg, e.BodyFatPct, e.Notes, e.CreatedAt
        );
        return Ok(dto);
    }

    [HttpPost]
    public async Task<ActionResult<EvaluationReadDto>> Create([FromBody] EvaluationCreateDto dto, CancellationToken ct)
    {
        // Regla de dominio
        if (dto.Date.Date > DateTime.UtcNow.Date)
            return BadRequest("La fecha no puede ser en el futuro.");

        // Verificar FK
        var traineeExists = await _db.Trainees.AnyAsync(t => t.Id == dto.TraineeId, ct);
        if (!traineeExists) return BadRequest("El ID del Trainee no existe.");

        var entity = new Evaluation
        {
            TraineeId  = dto.TraineeId,
            Date       = dto.Date,
            WaistCm    = dto.WaistCm,
            ArmCm      = dto.ArmCm,
            WeightKg   = dto.WeightKg,
            BodyFatPct = dto.BodyFatPct,
            Notes      = dto.Notes,
            CreatedAt  = DateTime.UtcNow
        };

        _db.Evaluations.Add(entity);
        await _db.SaveChangesAsync(ct);

        var read = new EvaluationReadDto(
            entity.Id, entity.TraineeId, entity.Date, entity.WaistCm, entity.ArmCm,
            entity.WeightKg, entity.BodyFatPct, entity.Notes, entity.CreatedAt
        );

        return CreatedAtAction(nameof(GetById), new { id = entity.Id }, read);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] EvaluationUpdateDto dto, CancellationToken ct)
    {
        var entity = await _db.Evaluations.FirstOrDefaultAsync(e => e.Id == id, ct);
        if (entity is null) return NotFound();

        if (dto.Date.Date > DateTime.UtcNow.Date)
            return BadRequest("La fecha no puede ser en el futuro.");

        // Asignar sólo campos permitidos por DTO (mitiga overposting)
        entity.Date       = dto.Date;
        entity.WaistCm    = dto.WaistCm;
        entity.ArmCm      = dto.ArmCm;
        entity.WeightKg   = dto.WeightKg;
        entity.BodyFatPct = dto.BodyFatPct;
        entity.Notes      = dto.Notes;

        await _db.SaveChangesAsync(ct);
        return NoContent();
    }
}
