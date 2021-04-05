using Edgias.Agrik.API.Interfaces;
using Edgias.Agrik.API.Models.Form;
using Edgias.Agrik.API.Models.View;
using Edgias.Agrik.ApplicationCore.Entities;
using Edgias.Agrik.ApplicationCore.Exceptions;
using Edgias.Agrik.ApplicationCore.Interfaces;
using Edgias.Agrik.ApplicationCore.Specifications;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Edgias.Agrik.API.Controllers
{
    [ApiController]
    [Route("v1.0/work-item-categories")]
    public class WorkItemCategoriesController : ControllerBase
    {
        private readonly IAppLogger<WorkItemCategoriesController> _logger;
        private readonly IAsyncRepository<WorkItemCategory> _repository;
        private readonly IMapper<WorkItemCategory, WorkItemCategoryRequestApiModel, WorkItemCategoryApiModel> _mapper;

        public WorkItemCategoriesController(IAppLogger<WorkItemCategoriesController> logger,
            IAsyncRepository<WorkItemCategory> repository,
            IMapper<WorkItemCategory, WorkItemCategoryRequestApiModel, WorkItemCategoryApiModel> mapper)
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
                ApiResponse<WorkItemCategoryApiModel> response = new ApiResponse<WorkItemCategoryApiModel>
                {
                    Data = workItemCategories.Select(wic => _mapper.Map(wic)),
                    Count = await _repository.CountAsync(new WorkItemCategorySpecification(searchQuery))
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
        public async Task<IActionResult> Post(WorkItemCategoryRequestApiModel apiModel)
        {
            try
            {
                WorkItemCategory workItemCategory = _mapper.Map(apiModel);

                workItemCategory = await _repository.AddAsync(workItemCategory);

                return CreatedAtAction(nameof(GetById), new { id = workItemCategory.Id }, _mapper.Map(workItemCategory));
            }

            catch (DataStoreException e)
            {
                _logger.LogError(e.Message, e, apiModel);
                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(Guid id, WorkItemCategoryRequestApiModel apiModel)
        {
            try
            {
                WorkItemCategory workItemCategory = await _repository.GetByIdAsync(id);

                if (workItemCategory == null)
                {
                    return NotFound();
                }

                _mapper.Map(workItemCategory, apiModel);

                await _repository.UpdateAsync(workItemCategory);

                return Ok();
            }

            catch (DataStoreException e)
            {
                _logger.LogError(e.Message, e, apiModel);
                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }
        }

        [HttpPatch("{id}/{userId}/change-status")]
        public async Task<IActionResult> ChangeStatus(Guid id, string userId)
        {
            try
            {
                WorkItemCategory workItemCategory = await _repository.GetByIdAsync(id);

                if (workItemCategory == null)
                {
                    return NotFound();
                }

                workItemCategory.ChangeStatus(userId);

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
