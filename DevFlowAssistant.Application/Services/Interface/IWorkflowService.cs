using DevFlowAssistant.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevFlowAssistant.Application.Services.Interface
{
    public interface IWorkflowService
    {
        Task<List<Workflow>> GetAllAsync();
        Task CreateAsync(string name , string? Description);
    }

}
