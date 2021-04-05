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
    [Route("v1.0/crops")]
    public class CropsController : ControllerBase
    {
        private readonly IAppLogger<CropsController> _logger;
        private readonly IAsyncRepository<Crop> _repository;
        private readonly IMapper<Crop, CropRequestApiModel, CropApiModel> _mapper;

        public CropsController(IAppLogger<CropsController> logger,
            IAsyncRepository<Crop> repository,
            IMapper<Crop, CropRequestApiModel, CropApiModel> mapper)
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
                ApiResponse<CropApiModel> response = new ApiResponse<CropApiModel>
                {
                    Data = crops.Select(c => _mapper.Map(c)),
                    Count = await _repository.CountAsync(new CropSpecification(searchQuery))
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
        public async Task<IActionResult> Post(CropRequestApiModel apiModel)
        {
            try
            {
                Crop crop = _mapper.Map(apiModel);

                crop = await _repository.AddAsync(crop);

                return CreatedAtAction(nameof(GetById), new { id = crop.Id }, _mapper.Map(crop));
            }

            catch (DataStoreException e)
            {
                _logger.LogError(e.Message, e, apiModel);
                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(Guid id, CropRequestApiModel apiModel)
        {
            try
            {
                Crop crop = await _repository.GetByIdAsync(id);

                if (crop == null)
                {
                    return NotFound();
                }

                _mapper.Map(crop, apiModel);

                await _repository.UpdateAsync(crop);

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
                Crop crop = await _repository.GetByIdAsync(id);

                if (crop == null)
                {
                    return NotFound();
                }

                crop.ChangeStatus(userId);

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
