using DevFlowAssistant.Application.Interfaces;
using DevFlowAssistant.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevFlowAssistant.Infrastructure.Repositories
{
    public class WorkflowRepository : IWorkflowRepository
    {
        private readonly AppDbContext _context;
        public WorkflowRepository(AppDbContext context)
        {

            _context = context;
        }


        public async Task AddAsync(Workflow workflow)
        {
            _context.Workflows.Add(workflow);
            await _context.SaveChangesAsync();
        }

        public async Task Delete(int id)
        {
            var workflow = await _context.Workflows.FindAsync(id);

            if (workflow is null)
            {
                return;
            }

            _context.Workflows.Remove(workflow);
            await _context.SaveChangesAsync();


        }

        public async Task<List<Workflow>> GetAllAsync()
        {
            return await _context.Workflows.OrderByDescending(c=>c.CreatedAt).ToListAsync();
        }

        public async Task<Workflow?> GetByIdAsync(int id)
        {

            return await _context.Workflows.Include(w => w.WorkflowActions)
                .Include(l => l.ExecutionLogs)
                .FirstOrDefaultAsync(w=> w.Id == id );

           

        }

        public async Task UpdateAsync(Workflow workflow)
        {
            _context.Workflows.Update(workflow);

             await _context.SaveChangesAsync();
        }
    }
}
