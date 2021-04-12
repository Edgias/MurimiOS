using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Murimi.API.Interfaces;
using Murimi.API.Models.Requests;
using Murimi.API.Models.Responses;
using Murimi.ApplicationCore.Entities;
using Murimi.ApplicationCore.Exceptions;
using Murimi.ApplicationCore.Interfaces;
using Murimi.ApplicationCore.SharedKernel;
using Murimi.ApplicationCore.Specifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Murimi.API.Controllers
{
    [ApiController]
    [Route("v1.0/work-item-categories")]
    public class WorkItemCategoriesController : ControllerBase
    {
        private readonly IAppLogger<WorkItemCategoriesController> _logger;
        private readonly IAsyncRepository<WorkItemCategory> _repository;
        private readonly IMapper<WorkItemCategory, WorkItemCategoryRequest, WorkItemCategoryResponse> _mapper;

        public WorkItemCategoriesController(IAppLogger<WorkItemCategoriesController> logger,
            IAsyncRepository<WorkItemCategory> repository,
            IMapper<WorkItemCategory, WorkItemCategoryRequest, WorkItemCategoryResponse> mapper)
        {
            _logger = logger;
            _repository = repository;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            IReadOnlyList<WorkItemCategory> workItemCategories = await _repository.GetAllAsync();

            if (workItemCategories.Any())
            {
                return Ok(workItemCategories.Select(wic => _mapper.Map(wic)));
            }

            return NoContent();
        }

        [HttpGet("{skip}/{take}/{searchQuery?}")]
        public async Task<IActionResult> Get(int skip, int take, string searchQuery = null)
        {
            IReadOnlyList<WorkItemCategory> workItemCategories = await _repository.GetAsync(new WorkItemCategorySpecification(skip, take, searchQuery));

            if (workItemCategories.Any())
            {
                PaginatedResponse<WorkItemCategoryResponse> response = new()
                {
                    Data = workItemCategories.Select(wic => _mapper.Map(wic)),
                    Total = await _repository.CountAsync(new WorkItemCategorySpecification(searchQuery))
                };

                return Ok(response);
            }

            return NoContent();
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            WorkItemCategory workItemCategory = await _repository.GetByIdAsync(id);

            if (workItemCategory == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map(workItemCategory));
        }

        [HttpPost]
        public async Task<IActionResult> Post(WorkItemCategoryRequest request)
        {
            try
            {
                WorkItemCategory workItemCategory = _mapper.Map(request);

                workItemCategory = await _repository.AddAsync(workItemCategory);

                return CreatedAtAction(nameof(GetById), new { id = workItemCategory.Id }, _mapper.Map(workItemCategory));
            }

            catch (DataStoreException e)
            {
                _logger.LogError(e.Message, e, request);
                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(Guid id, WorkItemCategoryRequest request)
        {
            try
            {
                WorkItemCategory workItemCategory = await _repository.GetByIdAsync(id);

                if (workItemCategory == null)
                {
                    return NotFound();
                }

                _mapper.Map(workItemCategory, request);

                await _repository.UpdateAsync(workItemCategory);

                return Ok();
            }

            catch (DataStoreException e)
            {
                _logger.LogError(e.Message, e, request);
                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }
        }

        [HttpPatch("{id}/change-status")]
        public async Task<IActionResult> ChangeStatus(Guid id)
        {
            try
            {
                WorkItemCategory workItemCategory = await _repository.GetByIdAsync(id);

                if (workItemCategory == null)
                {
                    return NotFound();
                }

                workItemCategory.ChangeStatus();

                await _repository.UpdateAsync(workItemCategory);

                return Ok();
            }

            catch (DataStoreException e)
            {
                _logger.LogError(e.Message, e, id);
                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                WorkItemCategory workItemCategory = await _repository.GetByIdAsync(id);

                if (workItemCategory == null)
                {
                    return NotFound();
                }

                await _repository.DeleteAsync(workItemCategory);

                return Ok(id);
            }

            catch (DataStoreException e)
            {
                _logger.LogError(e.Message, e, id);
                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }
        }
    }
}
