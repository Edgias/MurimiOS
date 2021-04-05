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
    [Route("v1.0/ownership-types")]
    public class OwnershipTypesController : ControllerBase
    {
        private readonly IAppLogger<OwnershipTypesController> _logger;
        private readonly IAsyncRepository<OwnershipType> _repository;
        private readonly IMapper<OwnershipType, OwnershipTypeRequestApiModel, OwnershipTypeApiModel> _mapper;

        public OwnershipTypesController(IAppLogger<OwnershipTypesController> logger,
            IAsyncRepository<OwnershipType> repository,
            IMapper<OwnershipType, OwnershipTypeRequestApiModel, OwnershipTypeApiModel> mapper)
        {
            _logger = logger;
            _repository = repository;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            IReadOnlyList<OwnershipType> ownershipTypes = await _repository.GetAllAsync();

            if (ownershipTypes.Any())
            {
                return Ok(ownershipTypes.Select(c => _mapper.Map(c)));
            }

            return NoContent();
        }

        [HttpGet("{skip}/{take}/{searchQuery?}")]
        public async Task<IActionResult> Get(int skip, int take, string searchQuery = null)
        {
            IReadOnlyList<OwnershipType> ownershipTypes = await _repository.GetAsync(new OwnershipTypeSpecification(skip, take, searchQuery));

            if (ownershipTypes.Any())
            {
                ApiResponse<OwnershipTypeApiModel> response = new ApiResponse<OwnershipTypeApiModel>
                {
                    Data = ownershipTypes.Select(c => _mapper.Map(c)),
                    Count = await _repository.CountAsync(new OwnershipTypeSpecification(searchQuery))
                };

                return Ok(response);
            }

            return NoContent();
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            OwnershipType ownershipType = await _repository.GetByIdAsync(id);

            if (ownershipType == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map(ownershipType));
        }

        [HttpPost]
        public async Task<IActionResult> Post(OwnershipTypeRequestApiModel apiModel)
        {
            try
            {
                OwnershipType ownershipType = _mapper.Map(apiModel);

                ownershipType = await _repository.AddAsync(ownershipType);

                return CreatedAtAction(nameof(GetById), new { id = ownershipType.Id }, _mapper.Map(ownershipType));
            }

            catch (DataStoreException e)
            {
                _logger.LogError(e.Message, e, apiModel);
                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(Guid id, OwnershipTypeRequestApiModel apiModel)
        {
            try
            {
                OwnershipType ownershipType = await _repository.GetByIdAsync(id);

                if (ownershipType == null)
                {
                    return NotFound();
                }

                _mapper.Map(ownershipType, apiModel);

                await _repository.UpdateAsync(ownershipType);

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
                OwnershipType ownershipType = await _repository.GetByIdAsync(id);

                if (ownershipType == null)
                {
                    return NotFound();
                }

                ownershipType.ChangeStatus(userId);

                await _repository.UpdateAsync(ownershipType);

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
                OwnershipType ownershipType = await _repository.GetByIdAsync(id);

                if (ownershipType == null)
                {
                    return NotFound();
                }

                await _repository.DeleteAsync(ownershipType);

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
