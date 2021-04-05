using Murimi.API.Interfaces;
using Murimi.API.Models.Request;
using Murimi.API.Models.Response;
using Murimi.ApplicationCore.Entities;
using Murimi.ApplicationCore.Exceptions;
using Murimi.ApplicationCore.Interfaces;
using Murimi.ApplicationCore.Specifications;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Murimi.API.Controllers
{
    [ApiController]
    [Route("v1.0/crop-units")]
    public class CropUnitsController : ControllerBase
    {
        private readonly IAppLogger<CropUnitsController> _logger;
        private readonly IAsyncRepository<CropUnit> _repository;
        private readonly IMapper<CropUnit, CropUnitRequestApiModel, CropUnitApiModel> _mapper;

        public CropUnitsController(IAppLogger<CropUnitsController> logger,
            IAsyncRepository<CropUnit> repository,
            IMapper<CropUnit, CropUnitRequestApiModel, CropUnitApiModel> mapper)
        {
            _logger = logger;
            _repository = repository;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            IReadOnlyList<CropUnit> cropUnits = await _repository.GetAllAsync();

            if (cropUnits.Any())
            {
                return Ok(cropUnits.Select(cu => _mapper.Map(cu)));
            }

            return NoContent();
        }

        [HttpGet("{skip}/{take}/{searchQuery?}")]
        public async Task<IActionResult> Get(int skip, int take, string searchQuery = null)
        {
            IReadOnlyList<CropUnit> cropUnits = await _repository.GetAsync(new CropUnitSpecification(skip, take, searchQuery));

            if (cropUnits.Any())
            {
                ApiResponse<CropUnitApiModel> response = new ApiResponse<CropUnitApiModel>
                {
                    Data = cropUnits.Select(cu => _mapper.Map(cu)),
                    Count = await _repository.CountAsync(new CropUnitSpecification(searchQuery))
                };

                return Ok(response);
            }

            return NoContent();
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            CropUnit cropUnit = await _repository.GetByIdAsync(id);

            if (cropUnit == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map(cropUnit));
        }

        [HttpPost]
        public async Task<IActionResult> Post(CropUnitRequestApiModel apiModel)
        {
            try
            {
                CropUnit cropUnit = _mapper.Map(apiModel);

                cropUnit = await _repository.AddAsync(cropUnit);

                return CreatedAtAction(nameof(GetById), new { id = cropUnit.Id }, _mapper.Map(cropUnit));
            }

            catch (DataStoreException e)
            {
                _logger.LogError(e.Message, e, apiModel);
                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(Guid id, CropUnitRequestApiModel apiModel)
        {
            try
            {
                CropUnit cropUnit = await _repository.GetByIdAsync(id);

                if (cropUnit == null)
                {
                    return NotFound();
                }

                _mapper.Map(cropUnit, apiModel);

                await _repository.UpdateAsync(cropUnit);

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
                CropUnit cropUnit = await _repository.GetByIdAsync(id);

                if (cropUnit == null)
                {
                    return NotFound();
                }

                cropUnit.ChangeStatus(userId);

                await _repository.UpdateAsync(cropUnit);

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
                CropUnit cropUnit = await _repository.GetByIdAsync(id);

                if (cropUnit == null)
                {
                    return NotFound();
                }

                await _repository.DeleteAsync(cropUnit);

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
