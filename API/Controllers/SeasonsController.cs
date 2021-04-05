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
    [Route("v1.0/seasons")]
    public class SeasonsController : ControllerBase
    {
        private readonly IAppLogger<SeasonsController> _logger;
        private readonly IAsyncRepository<Season> _repository;
        private readonly IMapper<Season, SeasonRequestApiModel, SeasonApiModel> _mapper;

        public SeasonsController(IAppLogger<SeasonsController> logger,
            IAsyncRepository<Season> repository,
            IMapper<Season, SeasonRequestApiModel, SeasonApiModel> mapper)
        {
            _logger = logger;
            _repository = repository;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            IReadOnlyList<Season> seasons = await _repository.GetAllAsync();

            if (seasons.Any())
            {
                return Ok(seasons.Select(c => _mapper.Map(c)));
            }

            return NoContent();
        }

        [HttpGet("{skip}/{take}/{searchQuery?}")]
        public async Task<IActionResult> Get(int skip, int take, string searchQuery = null)
        {
            IReadOnlyList<Season> seasons = await _repository.GetAsync(new SeasonSpecification(skip, take, searchQuery));

            if (seasons.Any())
            {
                ApiResponse<SeasonApiModel> response = new ApiResponse<SeasonApiModel>
                {
                    Data = seasons.Select(c => _mapper.Map(c)),
                    Count = await _repository.CountAsync(new SeasonSpecification(searchQuery))
                };

                return Ok(response);
            }

            return NoContent();
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            Season season = await _repository.GetByIdAsync(id);

            if (season == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map(season));
        }

        [HttpPost]
        public async Task<IActionResult> Post(SeasonRequestApiModel apiModel)
        {
            try
            {
                Season season = _mapper.Map(apiModel);

                season = await _repository.AddAsync(season);

                return CreatedAtAction(nameof(GetById), new { id = season.Id }, _mapper.Map(season));
            }

            catch (DataStoreException e)
            {
                _logger.LogError(e.Message, e, apiModel);
                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(Guid id, SeasonRequestApiModel apiModel)
        {
            try
            {
                Season season = await _repository.GetByIdAsync(id);

                if (season == null)
                {
                    return NotFound();
                }

                _mapper.Map(season, apiModel);

                await _repository.UpdateAsync(season);

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
                Season season = await _repository.GetByIdAsync(id);

                if (season == null)
                {
                    return NotFound();
                }

                season.ChangeStatus(userId);

                await _repository.UpdateAsync(season);

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
                Season season = await _repository.GetByIdAsync(id);

                if (season == null)
                {
                    return NotFound();
                }

                await _repository.DeleteAsync(season);

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
