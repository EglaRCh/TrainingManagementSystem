using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TrainingManagementAPI.Data;
using TrainingManagementAPI.Models;

namespace TrainingManagementAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GoalsController : ControllerBase
{
    private readonly AppDbContext _db;
    public GoalsController(AppDbContext db) => _db = db;

    [HttpGet("by-trainee/{traineeId:int}")]
    public async Task<IActionResult> GetByTrainee(int traineeId) =>
        Ok(await _db.Goals.Where(g => g.TraineeId == traineeId)
                          .OrderByDescending(g => g.StartDate)
                          .AsNoTracking()
                          .ToListAsync());

    [HttpGet("active/{traineeId:int}")]
    public async Task<IActionResult> GetActive(int traineeId) =>
        Ok(await _db.Goals.AsNoTracking()
                          .FirstOrDefaultAsync(g => g.TraineeId == traineeId && g.IsActive));

    [HttpPost]
    public async Task<IActionResult> Create(Goal input)
    {
        // regla simple: al crear uno activo, desactiva los previos
        if (input.IsActive)
        {
            var prev = _db.Goals.Where(g => g.TraineeId == input.TraineeId && g.IsActive);
            await prev.ForEachAsync(g => g.IsActive = false);
        }

        _db.Goals.Add(input);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetByTrainee), new { traineeId = input.TraineeId }, input);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, Goal input)
    {
        if (id != input.Id) return BadRequest();
        _db.Entry(input).State = EntityState.Modified;
        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var entity = await _db.Goals.FindAsync(id);
        if (entity is null) return NotFound();
        _db.Goals.Remove(entity);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
// Nota: para simplificar, no se incluyen DTOs ni validaciones avanzadas
// (se asume que el cliente envía datos correctos y completos)