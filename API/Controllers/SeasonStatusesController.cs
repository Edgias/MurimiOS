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
    [Route("v1.0/season-statuses")]
    public class SeasonStatusesController : ControllerBase
    {
        private readonly IAppLogger<SeasonStatusesController> _logger;
        private readonly IAsyncRepository<SeasonStatus> _repository;
        private readonly IMapper<SeasonStatus, SeasonStatusRequestApiModel, SeasonStatusApiModel> _mapper;

        public SeasonStatusesController(IAppLogger<SeasonStatusesController> logger,
            IAsyncRepository<SeasonStatus> repository,
            IMapper<SeasonStatus, SeasonStatusRequestApiModel, SeasonStatusApiModel> mapper)
        {
            _logger = logger;
            _repository = repository;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            IReadOnlyList<SeasonStatus> seasonStatuses = await _repository.GetAllAsync();

            if (seasonStatuses.Any())
            {
                return Ok(seasonStatuses.Select(cu => _mapper.Map(cu)));
            }

            return NoContent();
        }

        [HttpGet("{skip}/{take}/{searchQuery?}")]
        public async Task<IActionResult> Get(int skip, int take, string searchQuery = null)
        {
            IReadOnlyList<SeasonStatus> seasonStatuses = await _repository.GetAsync(new SeasonStatusSpecification(skip, take, searchQuery));

            if (seasonStatuses.Any())
            {
                ApiResponse<SeasonStatusApiModel> response = new ApiResponse<SeasonStatusApiModel>
                {
                    Data = seasonStatuses.Select(cu => _mapper.Map(cu)),
                    Count = await _repository.CountAsync(new SeasonStatusSpecification(searchQuery))
                };

                return Ok(response);
            }

            return NoContent();
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            SeasonStatus seasonStatus = await _repository.GetByIdAsync(id);

            if (seasonStatus == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map(seasonStatus));
        }

        [HttpPost]
        public async Task<IActionResult> Post(SeasonStatusRequestApiModel apiModel)
        {
            try
            {
                SeasonStatus seasonStatus = _mapper.Map(apiModel);

                seasonStatus = await _repository.AddAsync(seasonStatus);

                return CreatedAtAction(nameof(GetById), new { id = seasonStatus.Id }, _mapper.Map(seasonStatus));
            }

            catch (DataStoreException e)
            {
                _logger.LogError(e.Message, e, apiModel);
                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(Guid id, SeasonStatusRequestApiModel apiModel)
        {
            try
            {
                SeasonStatus seasonStatus = await _repository.GetByIdAsync(id);

                if (seasonStatus == null)
                {
                    return NotFound();
                }

                _mapper.Map(seasonStatus, apiModel);

                await _repository.UpdateAsync(seasonStatus);

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
                SeasonStatus seasonStatus = await _repository.GetByIdAsync(id);

                if (seasonStatus == null)
                {
                    return NotFound();
                }

                seasonStatus.ChangeStatus(userId);

                await _repository.UpdateAsync(seasonStatus);

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
                SeasonStatus seasonStatus = await _repository.GetByIdAsync(id);

                if (seasonStatus == null)
                {
                    return NotFound();
                }

                await _repository.DeleteAsync(seasonStatus);

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
