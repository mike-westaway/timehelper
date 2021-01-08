using TimeHelper.Data;
using TimeHelper.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TimeHelper.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AssociationsController : ControllerBase
    {
        private readonly TimeHelperDataContext _context;
        public AssociationsController(TimeHelperDataContext context)
        {
            _context = context;
        }
        // GET: api/Associations
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Association>>> GetAssociation()
        {
            
            return await _context.Association.Include(a => a.Project).ToListAsync();
        }


        // GET api/<AssociationsController>/5
        [HttpGet("GetAssociationByid")]
        public async Task<ActionResult<Association>> GetAssociationById(int id)
        {
            var association = await _context.Association.FindAsync(id);

            if (association == null)
            {
                return NotFound();
            }

            return association;
        }

        // POST api/<AssociationsController>
        [HttpPost]
        public async Task<ActionResult<Association>> PostProject(Association association)
        {
            _context.Association.Add(association);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetAssociationById", new { id = association.AssociationId }, association);
        }

        // PUT api/<AssociationsController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAssocation(int id, Association assocation)
        {
            if (id != assocation.AssociationId)
            {
                return BadRequest();
            }

            _context.Entry(assocation).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!associationExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }


        // DELETE: api/Associations/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Association>> DeleteAssociation(int id)
        {
            var association = await _context.Association.FindAsync(id);
            if (association == null)
            {
                return NotFound();
            }

            _context.Association.Remove(association);
            await _context.SaveChangesAsync();

            return association;
        }

        private bool associationExists(int id)
        {
            return _context.Association.Any(e => e.AssociationId == id);
        }
    }
}
