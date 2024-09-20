using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Murimi.API.Interfaces;
using Murimi.API.Models.Requests;
using Murimi.API.Models.Responses;
using Edgias.MurimiOS.Domain.Entities;
using Edgias.MurimiOS.Domain.Exceptions;
using Edgias.MurimiOS.Domain.Interfaces;
using Edgias.MurimiOS.Domain.SharedKernel;
using Edgias.MurimiOS.Domain.Specifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Murimi.API.Controllers
{
    [ApiController]
    [Route("v1.0")]
    public class WorkItemSubCategoriesController : ControllerBase
    {
        private readonly IAppLogger<WorkItemSubCategoriesController> _logger;
        private readonly IAsyncRepository<WorkItemSubCategory> _repository;
        private readonly IMapper<WorkItemSubCategory, WorkItemSubCategoryRequest, WorkItemSubCategoryResponse> _mapper;

        public WorkItemSubCategoriesController(IAppLogger<WorkItemSubCategoriesController> logger,
            IAsyncRepository<WorkItemSubCategory> repository,
            IMapper<WorkItemSubCategory, WorkItemSubCategoryRequest, WorkItemSubCategoryResponse> mapper)
        {
            _logger = logger;
            _repository = repository;
            _mapper = mapper;
        }

        [HttpGet("work-item-sub-categories")]
        public async Task<IActionResult> Get()
        {
            IReadOnlyList<WorkItemSubCategory> workItemSubCategories = await _repository.GetAllAsync();

            if (workItemSubCategories.Any())
            {
                return Ok(workItemSubCategories.Select(wisc => _mapper.Map(wisc)));
            }

            return NoContent();
        }

        [HttpGet("work-item-categories/{workItemCategoryId}/sub-categories")]
        public async Task<IActionResult> Get(Guid workItemCategoryId)
        {
            IReadOnlyList<WorkItemSubCategory> workItemSubCategories = await _repository.GetAsync(new WorkItemSubCategorySpecification(workItemCategoryId));

            if (workItemSubCategories.Any())
            {
                return Ok(workItemSubCategories.Select(wisc => _mapper.Map(wisc)));
            }

            return NoContent();
        }

        [HttpGet("work-item-sub-categories/{skip}/{take}/{searchQuery?}")]
        public async Task<IActionResult> Get(int skip, int take, string searchQuery = null)
        {
            IReadOnlyList<WorkItemSubCategory> workItemSubCategories = 
                await _repository.GetAsync(new WorkItemSubCategorySpecification(skip, take, searchQuery));

            if (workItemSubCategories.Any())
            {
                PaginatedResponse<WorkItemSubCategoryResponse> response = new()
                {
                    Data = workItemSubCategories.Select(wisc => _mapper.Map(wisc)),
                    Total = await _repository.CountAsync(new WorkItemSubCategorySpecification(searchQuery))
                };

                return Ok(response);
            }

            return NoContent();
        }

        [HttpGet("work-item-categories/{workItemCategoryId}/sub-categories/{skip}/{take}/{searchQuery?}")]
        public async Task<IActionResult> Get(Guid workItemCategoryId, int skip, int take, string searchQuery = null)
        {
            IReadOnlyList<WorkItemSubCategory> workItemSubCategories = 
                await _repository.GetAsync(new WorkItemSubCategorySpecification(workItemCategoryId, skip, take, searchQuery));

            if (workItemSubCategories.Any())
            {
                PaginatedResponse<WorkItemSubCategoryResponse> response = new()
                {
                    Data = workItemSubCategories.Select(wisc => _mapper.Map(wisc)),
                    Total = await _repository.CountAsync(new WorkItemSubCategorySpecification(workItemCategoryId, searchQuery))
                };

                return Ok(response);
            }

            return NoContent();
        }

        [HttpGet("work-item-sub-categories/{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            WorkItemSubCategory workItemSubCategory = await _repository.GetByIdAsync(id);

            if (workItemSubCategory == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map(workItemSubCategory));
        }

        [HttpPost("work-item-sub-categories")]
        public async Task<IActionResult> Post(WorkItemSubCategoryRequest request)
        {
            try
            {
                WorkItemSubCategory workItemSubCategory = _mapper.Map(request);

                workItemSubCategory = await _repository.AddAsync(workItemSubCategory);

                return CreatedAtAction(nameof(GetById), new { id = workItemSubCategory.Id }, _mapper.Map(workItemSubCategory));
            }

            catch (DataStoreException e)
            {
                _logger.LogError(e.Message, e, request);
                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }
        }

        [HttpPut("work-item-sub-categories/{id}")]
        public async Task<IActionResult> Put(Guid id, WorkItemSubCategoryRequest request)
        {
            try
            {
                WorkItemSubCategory workItemSubCategory = await _repository.GetByIdAsync(id);

                if (workItemSubCategory == null)
                {
                    return NotFound();
                }

                _mapper.Map(workItemSubCategory, request);

                await _repository.UpdateAsync(workItemSubCategory);

                return Ok();
            }

            catch (DataStoreException e)
            {
                _logger.LogError(e.Message, e, request);
                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }
        }

        [HttpPatch("work-item-sub-categories/{id}/change-status")]
        public async Task<IActionResult> ChangeStatus(Guid id)
        {
            try
            {
                WorkItemSubCategory workItemSubCategory = await _repository.GetByIdAsync(id);

                if (workItemSubCategory == null)
                {
                    return NotFound();
                }

                workItemSubCategory.ChangeStatus();

                await _repository.UpdateAsync(workItemSubCategory);

                return Ok();
            }

            catch (DataStoreException e)
            {
                _logger.LogError(e.Message, e, id);
                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }
        }

        [HttpDelete("work-item-sub-categories/{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                WorkItemSubCategory workItemSubCategory = await _repository.GetByIdAsync(id);

                if (workItemSubCategory == null)
                {
                    return NotFound();
                }

                await _repository.DeleteAsync(workItemSubCategory);

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
