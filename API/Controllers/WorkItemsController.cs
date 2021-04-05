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
    [Route("v1.0")]
    public class WorkItemsController : ControllerBase
    {
        private readonly IAppLogger<WorkItemsController> _logger;
        private readonly IAsyncRepository<WorkItem> _repository;
        private readonly IMapper<WorkItem, WorkItemRequestApiModel, WorkItemApiModel> _mapper;

        public WorkItemsController(IAppLogger<WorkItemsController> logger,
            IAsyncRepository<WorkItem> repository,
            IMapper<WorkItem, WorkItemRequestApiModel, WorkItemApiModel> mapper)
        {
            _logger = logger;
            _repository = repository;
            _mapper = mapper;
        }

        [HttpGet("work-items")]
        public async Task<IActionResult> Get()
        {
            IReadOnlyList<WorkItem> workItems = await _repository.GetAllAsync();

            if (workItems.Any())
            {
                return Ok(workItems.Select(c => _mapper.Map(c)));
            }

            return NoContent();
        }

        [HttpGet("seasons/{seasonId}/work-items")]
        public async Task<IActionResult> Get(Guid seasonId)
        {
            IReadOnlyList<WorkItem> workItems = await _repository.GetAsync(new WorkItemSpecification(seasonId));

            if (workItems.Any())
            {
                return Ok(workItems.Select(wi => _mapper.Map(wi)));
            }

            return NoContent();
        }

        [HttpGet("work-items/{skip}/{take}/{searchQuery?}")]
        public async Task<IActionResult> Get(int skip, int take, string searchQuery = null)
        {
            IReadOnlyList<WorkItem> workItems = await _repository.GetAsync(new WorkItemSpecification(skip, take, searchQuery));

            if (workItems.Any())
            {
                ApiResponse<WorkItemApiModel> response = new ApiResponse<WorkItemApiModel>
                {
                    Data = workItems.Select(wi => _mapper.Map(wi)),
                    Count = await _repository.CountAsync(new WorkItemSpecification(searchQuery))
                };

                return Ok(response);
            }

            return NoContent();
        }

        [HttpGet("seasons/{seasonId}/work-items/{skip}/{take}/{searchQuery?}")]
        public async Task<IActionResult> Get(Guid seasonId, int skip, int take, string searchQuery = null)
        {
            IReadOnlyList<WorkItem> workItems = await _repository.GetAsync(new WorkItemSpecification(seasonId, skip, take, searchQuery));

            if (workItems.Any())
            {
                ApiResponse<WorkItemApiModel> response = new ApiResponse<WorkItemApiModel>
                {
                    Data = workItems.Select(wi => _mapper.Map(wi)),
                    Count = await _repository.CountAsync(new WorkItemSpecification(seasonId, searchQuery))
                };

                return Ok(response);
            }

            return NoContent();
        }

        [HttpGet("work-items/{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            WorkItem workItem = await _repository.GetByIdAsync(id);

            if (workItem == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map(workItem));
        }

        [HttpPost("work-items")]
        public async Task<IActionResult> Post(WorkItemRequestApiModel apiModel)
        {
            try
            {
                WorkItem workItem = _mapper.Map(apiModel);

                workItem = await _repository.AddAsync(workItem);

                return CreatedAtAction(nameof(GetById), new { id = workItem.Id }, _mapper.Map(workItem));
            }

            catch (DataStoreException e)
            {
                _logger.LogError(e.Message, e, apiModel);
                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }
        }

        [HttpPut("work-items/{id}")]
        public async Task<IActionResult> Put(Guid id, WorkItemRequestApiModel apiModel)
        {
            try
            {
                WorkItem workItem = await _repository.GetByIdAsync(id);

                if (workItem == null)
                {
                    return NotFound();
                }

                _mapper.Map(workItem, apiModel);

                await _repository.UpdateAsync(workItem);

                return Ok();
            }

            catch (DataStoreException e)
            {
                _logger.LogError(e.Message, e, apiModel);
                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }
        }

        [HttpPatch("work-items/{id}/{userId}/change-status")]
        public async Task<IActionResult> ChangeStatus(Guid id, string userId)
        {
            try
            {
                WorkItem workItem = await _repository.GetByIdAsync(id);

                if (workItem == null)
                {
                    return NotFound();
                }

                workItem.ChangeStatus(userId);

                await _repository.UpdateAsync(workItem);

                return Ok();
            }

            catch (DataStoreException e)
            {
                _logger.LogError(e.Message, e, id);
                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }
        }

        [HttpDelete("work-items/{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                WorkItem workItem = await _repository.GetByIdAsync(id);

                if (workItem == null)
                {
                    return NotFound();
                }

                await _repository.DeleteAsync(workItem);

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
