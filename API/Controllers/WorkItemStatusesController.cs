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
    [Route("v1.0/work-item-statuses")]
    public class WorkItemStatusesController : ControllerBase
    {
        private readonly IAppLogger<WorkItemStatusesController> _logger;
        private readonly IAsyncRepository<WorkItemStatus> _repository;
        private readonly IMapper<WorkItemStatus, WorkItemStatusRequestApiModel, WorkItemStatusApiModel> _mapper;

        public WorkItemStatusesController(IAppLogger<WorkItemStatusesController> logger,
            IAsyncRepository<WorkItemStatus> repository,
            IMapper<WorkItemStatus, WorkItemStatusRequestApiModel, WorkItemStatusApiModel> mapper)
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
                ApiResponse<WorkItemStatusApiModel> response = new ApiResponse<WorkItemStatusApiModel>
                {
                    Data = workItemStatuses.Select(cu => _mapper.Map(cu)),
                    Count = await _repository.CountAsync(new WorkItemStatusSpecification(searchQuery))
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
        public async Task<IActionResult> Post(WorkItemStatusRequestApiModel apiModel)
        {
            try
            {
                WorkItemStatus workItemStatus = _mapper.Map(apiModel);

                workItemStatus = await _repository.AddAsync(workItemStatus);

                return CreatedAtAction(nameof(GetById), new { id = workItemStatus.Id }, _mapper.Map(workItemStatus));
            }

            catch (DataStoreException e)
            {
                _logger.LogError(e.Message, e, apiModel);
                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(Guid id, WorkItemStatusRequestApiModel apiModel)
        {
            try
            {
                WorkItemStatus workItemStatus = await _repository.GetByIdAsync(id);

                if (workItemStatus == null)
                {
                    return NotFound();
                }

                _mapper.Map(workItemStatus, apiModel);

                await _repository.UpdateAsync(workItemStatus);

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
                WorkItemStatus workItemStatus = await _repository.GetByIdAsync(id);

                if (workItemStatus == null)
                {
                    return NotFound();
                }

                workItemStatus.ChangeStatus(userId);

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
