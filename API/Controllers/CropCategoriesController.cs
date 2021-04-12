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
    [Route("v1.0/crop-categories")]
    public class CropCategoriesController : ControllerBase
    {
        private readonly IAppLogger<CropCategoriesController> _logger;
        private readonly IAsyncRepository<CropCategory> _repository;
        private readonly IMapper<CropCategory, CropCategoryRequest, CropCategoryApiModel> _mapper;

        public CropCategoriesController(IAppLogger<CropCategoriesController> logger,
            IAsyncRepository<CropCategory> repository,
            IMapper<CropCategory, CropCategoryRequest, CropCategoryApiModel> mapper)
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
                PaginatedResponse<CropCategoryApiModel> response = new()
                {
                    Data = cropCategories.Select(cc => _mapper.Map(cc)),
                    Total = await _repository.CountAsync(new CropCategorySpecification(searchQuery))
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
        public async Task<IActionResult> Post(CropCategoryRequest request)
        {
            try
            {
                CropCategory cropCategory = _mapper.Map(request);

                cropCategory = await _repository.AddAsync(cropCategory);

                return CreatedAtAction(nameof(GetById), new { id = cropCategory.Id }, _mapper.Map(cropCategory));
            }

            catch(DataStoreException e)
            {
                _logger.LogError(e.Message, e, request);
                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(Guid id, CropCategoryRequest request)
        {
            try
            {
                CropCategory cropCategory = await _repository.GetByIdAsync(id);

                if(cropCategory == null)
                {
                    return NotFound();
                }

                _mapper.Map(cropCategory, request);

                await _repository.UpdateAsync(cropCategory);

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
                CropCategory cropCategory = await _repository.GetByIdAsync(id);

                if (cropCategory == null)
                {
                    return NotFound();
                }

                cropCategory.ChangeStatus();

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
