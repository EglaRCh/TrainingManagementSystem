using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TrainingManagementAPI.Data;
using TrainingManagementAPI.Models;

namespace TrainingManagementAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TraineesController : ControllerBase
{
    private readonly AppDbContext _db;
    public TraineesController(AppDbContext db) => _db = db;

    [HttpGet]
    public async Task<IActionResult> GetAll() =>
        Ok(await _db.Trainees.AsNoTracking().ToListAsync());

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var trainee = await _db.Trainees.FindAsync(id);
        return trainee is null ? NotFound() : Ok(trainee);
    }

    [HttpPost]
    public async Task<IActionResult> Create(Trainee input)
    {
        _db.Trainees.Add(input);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = input.Id }, input);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, Trainee input)
    {
        if (id != input.Id) return BadRequest("Id de ruta y body no coinciden.");
        _db.Entry(input).State = EntityState.Modified;
        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var entity = await _db.Trainees.FindAsync(id);
        if (entity is null) return NotFound();
        _db.Trainees.Remove(entity);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
// Nota: para simplificar, no se incluyen DTOs ni validaciones avanzadas
// (se asume que el cliente envía datos correctos y completos)