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
    [Route("v1.0/work-item-statuses")]
    public class WorkItemStatusesController : ControllerBase
    {
        private readonly IAppLogger<WorkItemStatusesController> _logger;
        private readonly IAsyncRepository<WorkItemStatus> _repository;
        private readonly IMapper<WorkItemStatus, WorkItemStatusRequest, WorkItemStatusResponse> _mapper;

        public WorkItemStatusesController(IAppLogger<WorkItemStatusesController> logger,
            IAsyncRepository<WorkItemStatus> repository,
            IMapper<WorkItemStatus, WorkItemStatusRequest, WorkItemStatusResponse> mapper)
        {
            _logger = logger;
            _repository = repository;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            IReadOnlyList<WorkItemStatus> workItemStatuses = await _repository.GetAllAsync();

            if (workItemStatuses.Any())
            {
                return Ok(workItemStatuses.Select(cu => _mapper.Map(cu)));
            }

            return NoContent();
        }

        [HttpGet("{skip}/{take}/{searchQuery?}")]
        public async Task<IActionResult> Get(int skip, int take, string searchQuery = null)
        {
            IReadOnlyList<WorkItemStatus> workItemStatuses = await _repository.GetAsync(new WorkItemStatusSpecification(skip, take, searchQuery));

            if (workItemStatuses.Any())
            {
                PaginatedResponse<WorkItemStatusResponse> response = new()
                {
                    Data = workItemStatuses.Select(cu => _mapper.Map(cu)),
                    Total = await _repository.CountAsync(new WorkItemStatusSpecification(searchQuery))
                };

                return Ok(response);
            }

            return NoContent();
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            WorkItemStatus workItemStatus = await _repository.GetByIdAsync(id);

            if (workItemStatus == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map(workItemStatus));
        }

        [HttpPost]
        public async Task<IActionResult> Post(WorkItemStatusRequest request)
        {
            try
            {
                WorkItemStatus workItemStatus = _mapper.Map(request);

                workItemStatus = await _repository.AddAsync(workItemStatus);

                return CreatedAtAction(nameof(GetById), new { id = workItemStatus.Id }, _mapper.Map(workItemStatus));
            }

            catch (DataStoreException e)
            {
                _logger.LogError(e.Message, e, request);
                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(Guid id, WorkItemStatusRequest request)
        {
            try
            {
                WorkItemStatus workItemStatus = await _repository.GetByIdAsync(id);

                if (workItemStatus == null)
                {
                    return NotFound();
                }

                _mapper.Map(workItemStatus, request);

                await _repository.UpdateAsync(workItemStatus);

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
                WorkItemStatus workItemStatus = await _repository.GetByIdAsync(id);

                if (workItemStatus == null)
                {
                    return NotFound();
                }

                workItemStatus.ChangeStatus();

                await _repository.UpdateAsync(workItemStatus);

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
                WorkItemStatus workItemStatus = await _repository.GetByIdAsync(id);

                if (workItemStatus == null)
                {
                    return NotFound();
                }

                await _repository.DeleteAsync(workItemStatus);

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
