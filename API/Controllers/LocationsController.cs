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
    public class LocationsController : ControllerBase
    {
        private readonly IAppLogger<LocationsController> _logger;
        private readonly IAsyncRepository<Location> _repository;
        private readonly IMapper<Location, LocationRequestApiModel, LocationApiModel> _mapper;

        public LocationsController(IAppLogger<LocationsController> logger,
            IAsyncRepository<Location> repository,
            IMapper<Location, LocationRequestApiModel, LocationApiModel> mapper)
        {
            _logger = logger;
            _repository = repository;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            IReadOnlyList<Location> locations = await _repository.GetAllAsync();

            if (locations.Any())
            {
                return Ok(locations.Select(c => _mapper.Map(c)));
            }

            return NoContent();
        }

        [HttpGet("{skip}/{take}/{searchQuery?}")]
        public async Task<IActionResult> Get(int skip, int take, string searchQuery = null)
        {
            IReadOnlyList<Location> locations = await _repository.GetAsync(new LocationSpecification(skip, take, searchQuery));

            if (locations.Any())
            {
                ApiResponse<LocationApiModel> response = new ApiResponse<LocationApiModel>
                {
                    Data = locations.Select(c => _mapper.Map(c)),
                    Count = await _repository.CountAsync(new LocationSpecification(searchQuery))
                };

                return Ok(response);
            }

            return NoContent();
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            Location location = await _repository.GetByIdAsync(id);

            if (location == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map(location));
        }

        [HttpPost]
        public async Task<IActionResult> Post(LocationRequestApiModel apiModel)
        {
            try
            {
                Location location = _mapper.Map(apiModel);

                location = await _repository.AddAsync(location);

                return CreatedAtAction(nameof(GetById), new { id = location.Id }, _mapper.Map(location));
            }

            catch (DataStoreException e)
            {
                _logger.LogError(e.Message, e, apiModel);
                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(Guid id, LocationRequestApiModel apiModel)
        {
            try
            {
                Location location = await _repository.GetByIdAsync(id);

                if (location == null)
                {
                    return NotFound();
                }

                _mapper.Map(location, apiModel);

                await _repository.UpdateAsync(location);

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
                Location location = await _repository.GetByIdAsync(id);

                if (location == null)
                {
                    return NotFound();
                }

                location.ChangeStatus(userId);

                await _repository.UpdateAsync(location);

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
                Location location = await _repository.GetByIdAsync(id);

                if (location == null)
                {
                    return NotFound();
                }

                await _repository.DeleteAsync(location);

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
