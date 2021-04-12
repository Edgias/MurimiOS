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
    [Route("v1.0")]
    public class CropVarietiesController : ControllerBase
    {
        private readonly IAppLogger<CropVarietiesController> _logger;
        private readonly IAsyncRepository<CropVariety> _repository;
        private readonly IMapper<CropVariety, CropVarietyRequest, CropVarietyResponse> _mapper;

        public CropVarietiesController(IAppLogger<CropVarietiesController> logger,
            IAsyncRepository<CropVariety> repository,
            IMapper<CropVariety, CropVarietyRequest, CropVarietyResponse> mapper)
        {
            _logger = logger;
            _repository = repository;
            _mapper = mapper;
        }

        [HttpGet("crop-varieties")]
        public async Task<IActionResult> Get()
        {
            IReadOnlyList<CropVariety> cropVarieties = await _repository.GetAllAsync();

            if (cropVarieties.Any())
            {
                return Ok(cropVarieties.Select(cv => _mapper.Map(cv)));
            }

            return NoContent();
        }

        [HttpGet("crop-varieties/{skip}/{take}/{searchQuery?}")]
        public async Task<IActionResult> Get(int skip, int take, string searchQuery = null)
        {
            IReadOnlyList<CropVariety> cropVarieties = await _repository.GetAsync(new CropVarietySpecification(skip, take, searchQuery));

            if (cropVarieties.Any())
            {
                PaginatedResponse<CropVarietyResponse> response = new()
                {
                    Data = cropVarieties.Select(cv => _mapper.Map(cv)),
                    Total = await _repository.CountAsync(new CropVarietySpecification(searchQuery))
                };

                return Ok(response);
            }

            return NoContent();
        }

        [HttpGet("crops/{cropId}/varieties")]
        public async Task<IActionResult> Get(Guid cropId)
        {
            IReadOnlyList<CropVariety> cropVarieties = await _repository.GetAsync(new CropVarietySpecification(cropId));

            if (cropVarieties.Any())
            {
                return Ok(cropVarieties.Select(cv => _mapper.Map(cv)));
            }

            return NoContent();
        }

        [HttpGet("crops/{cropId}/varieties/{skip}/{take}/{searchQuery?}")]
        public async Task<IActionResult> Get(Guid cropId, int skip, int take, string searchQuery = null)
        {
            IReadOnlyList<CropVariety> cropVarieties = await _repository.GetAsync(new CropVarietySpecification(cropId, skip, take, searchQuery));

            if (cropVarieties.Any())
            {
                PaginatedResponse<CropVarietyResponse> response = new()
                {
                    Data = cropVarieties.Select(cv => _mapper.Map(cv)),
                    Total = await _repository.CountAsync(new CropVarietySpecification(cropId, searchQuery))
                };

                return Ok(response);
            }

            return NoContent();
        }

        [HttpGet("crop-varieties/{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            CropVariety cropVariety = await _repository.GetByIdAsync(id);

            if (cropVariety == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map(cropVariety));
        }

        [HttpPost("crop-varieties")]
        public async Task<IActionResult> Post(CropVarietyRequest request)
        {
            try
            {
                CropVariety cropVariety = _mapper.Map(request);

                cropVariety = await _repository.AddAsync(cropVariety);

                return CreatedAtAction(nameof(GetById), new { id = cropVariety.Id }, _mapper.Map(cropVariety));
            }

            catch (DataStoreException e)
            {
                _logger.LogError(e.Message, e, request);
                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }
        }

        [HttpPut("crop-varieties/{id}")]
        public async Task<IActionResult> Put(Guid id, CropVarietyRequest request)
        {
            try
            {
                CropVariety cropVariety = await _repository.GetByIdAsync(id);

                if (cropVariety == null)
                {
                    return NotFound();
                }

                _mapper.Map(cropVariety, request);

                await _repository.UpdateAsync(cropVariety);

                return Ok();
            }

            catch (DataStoreException e)
            {
                _logger.LogError(e.Message, e, request);
                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }
        }

        [HttpPatch("crop-varieties/{id}/change-status")]
        public async Task<IActionResult> ChangeStatus(Guid id)
        {
            try
            {
                CropVariety cropVariety = await _repository.GetByIdAsync(id);

                if (cropVariety == null)
                {
                    return NotFound();
                }

                cropVariety.ChangeStatus();

                await _repository.UpdateAsync(cropVariety);

                return Ok();
            }

            catch (DataStoreException e)
            {
                _logger.LogError(e.Message, e, id);
                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }
        }

        [HttpDelete("crop-varieties/{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                CropVariety cropVariety = await _repository.GetByIdAsync(id);

                if (cropVariety == null)
                {
                    return NotFound();
                }

                await _repository.DeleteAsync(cropVariety);

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
