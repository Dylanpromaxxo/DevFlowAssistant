using DevFlowAssistant.Application.Interfaces;
using DevFlowAssistant.Application.Services.Interface;
using DevFlowAssistant.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevFlowAssistant.Application.Services.implementation
{
    public class WorkflowService : IWorkflowService
    {
        private readonly IWorkflowRepository _workflowRepository;

        public WorkflowService(IWorkflowRepository workflowRepository)
        {
            _workflowRepository = workflowRepository;
        }

        public Task CreateAsync(string name, string? Description)
        {

            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("El nombre del workflow es obligatorio.");


            var workflow = new Workflow
            {
                Name = name.Trim(),
                Description = Description?.Trim()   ,
                IsActive = 1,
                CreatedAt = DateTime.Now.ToString(),
                UpdatedAt = null
            };
            return _workflowRepository.AddAsync(workflow);
        }

        public async Task<List<Workflow>> GetAllAsync()
        {
            return await _workflowRepository.GetAllAsync();
        }
    }
}
