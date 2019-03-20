using System;
using System.Collections.Generic;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using WebApi.Dtos;
using WebApi.Helpers;
using WebApi.Services;

namespace WebApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class MagazinesController : ControllerBase
    {
        private IMagazineService _magazineService;
        private IMapper _mapper;
        private readonly AppSettings _appSettings;

        public MagazinesController(
            IMagazineService magazineService,
            IMapper mapper,
            IOptions<AppSettings> appSettings)
        {
            _magazineService = magazineService;
            _mapper = mapper;
            _appSettings = appSettings.Value;
        }

        [HttpPost("create")]
        public IActionResult CreateMagazine([FromBody]MagazineDto magazineDto)
        {
            var magazineDtos = _mapper.Map<MagazineDto>(magazineDto);
            
            var magazine = _magazineService.Create(magazineDtos);
             return Ok(new {
                Id = magazine.Id,
                Name = magazine.Name,
                Price = magazine.Price,
                Quantity = magazine.Quantity,
                Type = magazine.Type 
            });
            
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var magazines =  _magazineService.GetAll();
            //var consoleDtos = _mapper.Map<IList<ConsoleDto>>(consoles);
            return Ok(magazines);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var magazine =  _magazineService.GetById(id);
            var magazineDto = _mapper.Map<MagazineDto>(magazine);
            return Ok(magazineDto);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody]MagazineDto magazineDto)
        {
            var magazine = _mapper.Map<MagazineDto>(magazineDto);
            magazine.Id = id;

            try 
            {
                _magazineService.Update(magazine);
                return Ok();
            } 
            catch(AppException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            _magazineService.Delete(id);
            return Ok();
        }
        [HttpPost("trade")]
        public IActionResult Trade(TradeDto trades)
        {
             var tradeDtos = _mapper.Map<TradeDto>(trades);
            _magazineService.createTrade(tradeDtos);
            var getTodaySum = _magazineService.GetTodaySum();

            return Ok( new {thisDay=getTodaySum});
        }
        [HttpGet("TradeToDay")]
        public IActionResult TradeToday()
        {
            var getTodaySum = _magazineService.GetTodaySum();
            return Ok( new {thisDay=getTodaySum});
        }
    }
}