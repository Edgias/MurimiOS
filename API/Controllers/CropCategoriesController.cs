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
    [Route("v1.0/crop-categories")]
    public class CropCategoriesController : ControllerBase
    {
        private readonly IAppLogger<CropCategoriesController> _logger;
        private readonly IAsyncRepository<CropCategory> _repository;
        private readonly IMapper<CropCategory, CropCategoryRequestApiModel, CropCategoryApiModel> _mapper;

        public CropCategoriesController(IAppLogger<CropCategoriesController> logger,
            IAsyncRepository<CropCategory> repository,
            IMapper<CropCategory, CropCategoryRequestApiModel, CropCategoryApiModel> mapper)
        {
            _logger = logger;
            _repository = repository;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            IReadOnlyList<CropCategory> cropCategories = await _repository.GetAllAsync();

            if(cropCategories.Any())
            {
                return Ok(cropCategories.Select(cc => _mapper.Map(cc)));
            }

            return NoContent();
        }

        [HttpGet("{skip}/{take}/{searchQuery?}")]
        public async Task<IActionResult> Get(int skip, int take, string searchQuery = null)
        {
            IReadOnlyList<CropCategory> cropCategories = await _repository.GetAsync(new CropCategorySpecification(skip, take, searchQuery));

            if (cropCategories.Any())
            {
                ApiResponse<CropCategoryApiModel> response = new ApiResponse<CropCategoryApiModel>
                {
                    Data = cropCategories.Select(cc => _mapper.Map(cc)),
                    Count = await _repository.CountAsync(new CropCategorySpecification(searchQuery))
                };

                return Ok(response);
            }

            return NoContent();
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            CropCategory cropCategory = await _repository.GetByIdAsync(id);

            if(cropCategory == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map(cropCategory));
        }

        [HttpPost]
        public async Task<IActionResult> Post(CropCategoryRequestApiModel apiModel)
        {
            try
            {
                CropCategory cropCategory = _mapper.Map(apiModel);

                cropCategory = await _repository.AddAsync(cropCategory);

                return CreatedAtAction(nameof(GetById), new { id = cropCategory.Id }, _mapper.Map(cropCategory));
            }

            catch(DataStoreException e)
            {
                _logger.LogError(e.Message, e, apiModel);
                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(Guid id, CropCategoryRequestApiModel apiModel)
        {
            try
            {
                CropCategory cropCategory = await _repository.GetByIdAsync(id);

                if(cropCategory == null)
                {
                    return NotFound();
                }

                _mapper.Map(cropCategory, apiModel);

                await _repository.UpdateAsync(cropCategory);

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
                CropCategory cropCategory = await _repository.GetByIdAsync(id);

                if (cropCategory == null)
                {
                    return NotFound();
                }

                cropCategory.ChangeStatus(userId);

                await _repository.UpdateAsync(cropCategory);

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
                CropCategory cropCategory = await _repository.GetByIdAsync(id);

                if (cropCategory == null)
                {
                    return NotFound();
                }

                await _repository.DeleteAsync(cropCategory);

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
