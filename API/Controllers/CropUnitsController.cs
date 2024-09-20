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
    [Route("v1.0/crop-units")]
    public class CropUnitsController : ControllerBase
    {
        private readonly IAppLogger<CropUnitsController> _logger;
        private readonly IAsyncRepository<CropUnit> _repository;
        private readonly IMapper<CropUnit, CropUnitRequest, CropUnitResponse> _mapper;

        public CropUnitsController(IAppLogger<CropUnitsController> logger,
            IAsyncRepository<CropUnit> repository,
            IMapper<CropUnit, CropUnitRequest, CropUnitResponse> mapper)
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
                PaginatedResponse<CropUnitResponse> response = new()
                {
                    Data = cropUnits.Select(cu => _mapper.Map(cu)),
                    Total = await _repository.CountAsync(new CropUnitSpecification(searchQuery))
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
        public async Task<IActionResult> Post(CropUnitRequest request)
        {
            try
            {
                CropUnit cropUnit = _mapper.Map(request);

                cropUnit = await _repository.AddAsync(cropUnit);

                return CreatedAtAction(nameof(GetById), new { id = cropUnit.Id }, _mapper.Map(cropUnit));
            }

            catch (DataStoreException e)
            {
                _logger.LogError(e.Message, e, request);
                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(Guid id, CropUnitRequest request)
        {
            try
            {
                CropUnit cropUnit = await _repository.GetByIdAsync(id);

                if (cropUnit == null)
                {
                    return NotFound();
                }

                _mapper.Map(cropUnit, request);

                await _repository.UpdateAsync(cropUnit);

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
                CropUnit cropUnit = await _repository.GetByIdAsync(id);

                if (cropUnit == null)
                {
                    return NotFound();
                }

                cropUnit.ChangeStatus();

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
