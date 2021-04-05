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
    [Route("v1.0")]
    public class FieldsController : ControllerBase
    {
        private readonly IAppLogger<FieldsController> _logger;
        private readonly IAsyncRepository<Field> _repository;
        private readonly IMapper<Field, FieldRequestApiModel, FieldApiModel> _mapper;

        public FieldsController(IAppLogger<FieldsController> logger,
            IAsyncRepository<Field> repository,
            IMapper<Field, FieldRequestApiModel, FieldApiModel> mapper)
        {
            _logger = logger;
            _repository = repository;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            IReadOnlyList<Field> fields = await _repository.GetAllAsync();

            if (fields.Any())
            {
                return Ok(fields.Select(c => _mapper.Map(c)));
            }

            return NoContent();
        }

        [HttpGet("{skip}/{take}/{searchQuery?}")]
        public async Task<IActionResult> Get(int skip, int take, string searchQuery = null)
        {
            IReadOnlyList<Field> fields = await _repository.GetAsync(new FieldSpecification(skip, take, searchQuery));

            if (fields.Any())
            {
                ApiResponse<FieldApiModel> response = new ApiResponse<FieldApiModel>
                {
                    Data = fields.Select(c => _mapper.Map(c)),
                    Count = await _repository.CountAsync(new FieldSpecification(searchQuery))
                };

                return Ok(response);
            }

            return NoContent();
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            Field field = await _repository.GetByIdAsync(id);

            if (field == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map(field));
        }

        [HttpPost]
        public async Task<IActionResult> Post(FieldRequestApiModel apiModel)
        {
            try
            {
                Field field = _mapper.Map(apiModel);

                field = await _repository.AddAsync(field);

                return CreatedAtAction(nameof(GetById), new { id = field.Id }, _mapper.Map(field));
            }

            catch (DataStoreException e)
            {
                _logger.LogError(e.Message, e, apiModel);
                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(Guid id, FieldRequestApiModel apiModel)
        {
            try
            {
                Field field = await _repository.GetByIdAsync(id);

                if (field == null)
                {
                    return NotFound();
                }

                _mapper.Map(field, apiModel);

                await _repository.UpdateAsync(field);

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
                Field field = await _repository.GetByIdAsync(id);

                if (field == null)
                {
                    return NotFound();
                }

                field.ChangeStatus(userId);

                await _repository.UpdateAsync(field);

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
                Field field = await _repository.GetByIdAsync(id);

                if (field == null)
                {
                    return NotFound();
                }

                await _repository.DeleteAsync(field);

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
