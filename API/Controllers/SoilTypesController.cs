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
    [Route("v1.0/soil-types")]
    public class SoilTypesController : ControllerBase
    {
        private readonly IAppLogger<SoilTypesController> _logger;
        private readonly IAsyncRepository<SoilType> _repository;
        private readonly IMapper<SoilType, SoilTypeRequestApiModel, SoilTypeApiModel> _mapper;

        public SoilTypesController(IAppLogger<SoilTypesController> logger,
            IAsyncRepository<SoilType> repository,
            IMapper<SoilType, SoilTypeRequestApiModel, SoilTypeApiModel> mapper)
        {
            _logger = logger;
            _repository = repository;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            IReadOnlyList<SoilType> soilTypes = await _repository.GetAllAsync();

            if (soilTypes.Any())
            {
                return Ok(soilTypes.Select(c => _mapper.Map(c)));
            }

            return NoContent();
        }

        [HttpGet("{skip}/{take}/{searchQuery?}")]
        public async Task<IActionResult> Get(int skip, int take, string searchQuery = null)
        {
            IReadOnlyList<SoilType> soilTypes = await _repository.GetAsync(new SoilTypeSpecification(skip, take, searchQuery));

            if (soilTypes.Any())
            {
                ApiResponse<SoilTypeApiModel> response = new ApiResponse<SoilTypeApiModel>
                {
                    Data = soilTypes.Select(c => _mapper.Map(c)),
                    Count = await _repository.CountAsync(new SoilTypeSpecification(searchQuery))
                };

                return Ok(response);
            }

            return NoContent();
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            SoilType soilType = await _repository.GetByIdAsync(id);

            if (soilType == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map(soilType));
        }

        [HttpPost]
        public async Task<IActionResult> Post(SoilTypeRequestApiModel apiModel)
        {
            try
            {
                SoilType soilType = _mapper.Map(apiModel);

                soilType = await _repository.AddAsync(soilType);

                return CreatedAtAction(nameof(GetById), new { id = soilType.Id }, _mapper.Map(soilType));
            }

            catch (DataStoreException e)
            {
                _logger.LogError(e.Message, e, apiModel);
                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(Guid id, SoilTypeRequestApiModel apiModel)
        {
            try
            {
                SoilType soilType = await _repository.GetByIdAsync(id);

                if (soilType == null)
                {
                    return NotFound();
                }

                _mapper.Map(soilType, apiModel);

                await _repository.UpdateAsync(soilType);

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
                SoilType soilType = await _repository.GetByIdAsync(id);

                if (soilType == null)
                {
                    return NotFound();
                }

                soilType.ChangeStatus(userId);

                await _repository.UpdateAsync(soilType);

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
                SoilType soilType = await _repository.GetByIdAsync(id);

                if (soilType == null)
                {
                    return NotFound();
                }

                await _repository.DeleteAsync(soilType);

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
