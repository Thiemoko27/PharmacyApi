using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PharmacyApi.Data;
using PharmacyApi.Models;

namespace PharmacyApi.Controllers;

[Route("[controller]")]
[ApiController]

public class DrugController : ControllerBase
{
    private readonly DataBaseContext _context;

    public DrugController(DataBaseContext context) {
        _context = context;
    }

    [HttpGet]
    public IEnumerable<Drug> GetAll() {
        return _context.Drugs.ToList();
    }

    [HttpGet("{id}")]
    public IActionResult GetById(int id) {
        var drug = _context.Drugs.Find(id);
        
        if(drug == null) {
            return NotFound();
        }

        return Ok(drug);
    }

    [HttpPost]
    [Authorize(Policy = "Admin")]
    public IActionResult Create([FromBody] Drug drug) {
        _context.Drugs.Add(drug);
        _context.SaveChanges();

        return CreatedAtAction(nameof(GetById), new{id = drug.Id}, drug);
    }

    [HttpPut("{id}")]
    public IActionResult Update([FromBody] Drug updatedDrug, int id) {
        var drug = _context.Drugs.Find(id);

        if(drug == null) {
            return NotFound();
        }

        drug.Name = updatedDrug.Name;
        drug.Number = updatedDrug.Number;
        drug.Price = updatedDrug.Price;
        drug.Stock = updatedDrug.Stock;

        _context.Drugs.Update(drug);
        _context.SaveChanges();

        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize(Policy = "Admin")]
    public IActionResult Delete(int id) {
        var drug = _context.Drugs.Find(id);

        if(drug == null) {
            return NotFound();
        }

        _context.Drugs.Remove(drug);
        _context.SaveChanges();

        return NoContent();
    }
}