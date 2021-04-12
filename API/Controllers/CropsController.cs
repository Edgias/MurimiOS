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
    [Route("v1.0/crops")]
    public class CropsController : ControllerBase
    {
        private readonly IAppLogger<CropsController> _logger;
        private readonly IAsyncRepository<Crop> _repository;
        private readonly IMapper<Crop, CropRequest, CropResponse> _mapper;

        public CropsController(IAppLogger<CropsController> logger,
            IAsyncRepository<Crop> repository,
            IMapper<Crop, CropRequest, CropResponse> mapper)
        {
            _logger = logger;
            _repository = repository;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            IReadOnlyList<Crop> crops = await _repository.GetAllAsync();

            if (crops.Any())
            {
                return Ok(crops.Select(c => _mapper.Map(c)));
            }

            return NoContent();
        }

        [HttpGet("{skip}/{take}/{searchQuery?}")]
        public async Task<IActionResult> Get(int skip, int take, string searchQuery = null)
        {
            IReadOnlyList<Crop> crops = await _repository.GetAsync(new CropSpecification(skip, take, searchQuery));

            if (crops.Any())
            {
                PaginatedResponse<CropResponse> response = new()
                {
                    Data = crops.Select(c => _mapper.Map(c)),
                    Total = await _repository.CountAsync(new CropSpecification(searchQuery))
                };

                return Ok(response);
            }

            return NoContent();
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            Crop crop = await _repository.GetByIdAsync(id);

            if (crop == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map(crop));
        }

        [HttpPost]
        public async Task<IActionResult> Post(CropRequest request)
        {
            try
            {
                Crop crop = _mapper.Map(request);

                crop = await _repository.AddAsync(crop);

                return CreatedAtAction(nameof(GetById), new { id = crop.Id }, _mapper.Map(crop));
            }

            catch (DataStoreException e)
            {
                _logger.LogError(e.Message, e, request);
                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(Guid id, CropRequest request)
        {
            try
            {
                Crop crop = await _repository.GetByIdAsync(id);

                if (crop == null)
                {
                    return NotFound();
                }

                _mapper.Map(crop, request);

                await _repository.UpdateAsync(crop);

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
                Crop crop = await _repository.GetByIdAsync(id);

                if (crop == null)
                {
                    return NotFound();
                }

                crop.ChangeStatus();

                await _repository.UpdateAsync(crop);

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
                Crop crop = await _repository.GetByIdAsync(id);

                if (crop == null)
                {
                    return NotFound();
                }

                await _repository.DeleteAsync(crop);

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
