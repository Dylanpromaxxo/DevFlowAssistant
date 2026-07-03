using DevFlowAssistant.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevFlowAssistant.Application.Interfaces
{
    public interface IWorkflowRepository
    {
        Task<List<Workflow>> GetAllAsync();
        Task<Workflow?> GetByIdAsync(int id);
        Task AddAsync(Workflow workflow);
        Task UpdateAsync(Workflow workflow);
        Task Delete(int id);
    }
}
