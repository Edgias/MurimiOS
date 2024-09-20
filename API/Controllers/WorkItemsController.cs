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
    public class WorkItemsController : ControllerBase
    {
        private readonly IAppLogger<WorkItemsController> _logger;
        private readonly IAsyncRepository<WorkItem> _repository;
        private readonly IMapper<WorkItem, WorkItemRequest, WorkItemResponse> _mapper;

        public WorkItemsController(IAppLogger<WorkItemsController> logger,
            IAsyncRepository<WorkItem> repository,
            IMapper<WorkItem, WorkItemRequest, WorkItemResponse> mapper)
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
                PaginatedResponse<WorkItemResponse> response = new()
                {
                    Data = workItems.Select(wi => _mapper.Map(wi)),
                    Total = await _repository.CountAsync(new WorkItemSpecification(searchQuery))
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
                PaginatedResponse<WorkItemResponse> response = new()
                {
                    Data = workItems.Select(wi => _mapper.Map(wi)),
                    Total = await _repository.CountAsync(new WorkItemSpecification(seasonId, searchQuery))
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
        public async Task<IActionResult> Post(WorkItemRequest request)
        {
            try
            {
                WorkItem workItem = _mapper.Map(request);

                workItem = await _repository.AddAsync(workItem);

                return CreatedAtAction(nameof(GetById), new { id = workItem.Id }, _mapper.Map(workItem));
            }

            catch (DataStoreException e)
            {
                _logger.LogError(e.Message, e, request);
                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }
        }

        [HttpPut("work-items/{id}")]
        public async Task<IActionResult> Put(Guid id, WorkItemRequest request)
        {
            try
            {
                WorkItem workItem = await _repository.GetByIdAsync(id);

                if (workItem == null)
                {
                    return NotFound();
                }

                _mapper.Map(workItem, request);

                await _repository.UpdateAsync(workItem);

                return Ok();
            }

            catch (DataStoreException e)
            {
                _logger.LogError(e.Message, e, request);
                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }
        }

        [HttpPatch("work-items/{id}/change-status")]
        public async Task<IActionResult> ChangeStatus(Guid id)
        {
            try
            {
                WorkItem workItem = await _repository.GetByIdAsync(id);

                if (workItem == null)
                {
                    return NotFound();
                }

                workItem.ChangeStatus();

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
