using System;
using System.Collections;
using System.Collections.Generic;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using WebApi.Dtos;
using WebApi.Helpers;
using System.Linq;
using WebApi.Services;

namespace WebApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class PromotionsController : ControllerBase
    {
        private DataContext _context;
        private IPromotionService _promotionService;
        private IConsoleService _consoleService;
        private IMapper _mapper;
        private readonly AppSettings _appSettings;

        public PromotionsController(
        
            IPromotionService promotionService,
            IMapper mapper,
            IOptions<AppSettings> appSettings,
            IConsoleService consoleService,
            DataContext context
            )
        {
            _promotionService = promotionService;
            _mapper = mapper;
            _appSettings = appSettings.Value;
            _consoleService = consoleService;
            _context = context;
        }

        [HttpPost("create")]
        public IActionResult CreatePromotion([FromBody]PromotionDto promotionDto)
        {
            var promotionDtos = _mapper.Map<PromotionDto>(promotionDto);
            
            var promotion = _promotionService.Create(promotionDtos);

            var promotionToConsoles = _promotionService.AddPromCons(promotion.Id, promotionDtos.Consoles);

            
             return Ok(new {
                Id = promotion.Id,
                PromotionName = promotion.PromotionName,
                PromitionDesc = promotion.PromotionDesc,
                Type = promotion.Price,
                AllTime = promotion.AllTime,
                DateStart = promotion.DateStart,
                DateEnd= promotion.DateEnd,
                HourStart = promotion.HourStart,
                HourEnd = promotion.HourEnd,
                PromCons = promotionToConsoles
            });
            
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var promotions =  _promotionService.GetAll();
            var console =  _consoleService.GetAllForPromotion();
            var consProm = _promotionService.getAllPromCons();
            var consPromNew = _promotionService.GetPromWithCons();
            var promotionDtos = _mapper.Map<IList<PromotionDto>>(promotions);
            var consoleDtos = _mapper.Map<IList<ConsoleDto>>(console);
            var consPromDtos = _mapper.Map<IList<PromConsDto>>(consProm);
            
            // List<ConsoleDto> p = new List<ConsoleDto>();
            // List<PromotionRelation> getPromo = new List<PromotionRelation>();
            // var result =    from PC in _context.PromotionConsoles      
            //                 join P in _context.Promotions on PC.promotionId equals P.Id 
            //                 join C in _context.Consoles on PC.consoleId equals C.Id 
            //                 select new {
            //                     PromotionId = PC.promotionId,
            //                     PromotionName = P.PromotionName,
            //                     PromotionDesc = P.PromotionDesc,
            //                     Price = P.Price,
            //                     AllTime = P.AllTime,
            //                     DateStart = P.DateStart,
            //                     DateEnd = P.DateEnd,
            //                     HourStart = P.HourStart,
            //                     HourEnd = P.HourEnd,
            //                     ConsoleId = PC.consoleId,
            //                     ConsoleName = C.ConsoleName,
            //                     ConsoleNumber = C.ConsoleNumber,
            //                     Type = C.Type,
            //                     StandartPrice = C.StandartPrice,
            //                     CurrentPrice = C.CurrentPrice
            //                 } into PCtable
            //                 group PCtable by PCtable.PromotionId into g
            //                 select new {
            //                         PromotionId = g.FirstOrDefault().PromotionId,
            //                         PromotionName = g.FirstOrDefault().PromotionName,
            //                         PromotionDesc = g.FirstOrDefault().PromotionDesc,
            //                         Price = g.FirstOrDefault().Price,
            //                         AllTime = g.FirstOrDefault().AllTime,
            //                         DateStart = g.FirstOrDefault().DateStart,
            //                         DateEnd = g.FirstOrDefault().DateEnd,
            //                         HourStart = g.FirstOrDefault().HourStart,
            //                         HourEnd = g.FirstOrDefault().HourEnd,
            //                         Console = g.Select(x=> new {
            //                             consoleId = x.ConsoleId,
            //                             ConsoleName = x.ConsoleName,
            //                             ConsoleNumber = x.ConsoleNumber,
            //                             Type = x.Type,
            //                             StandartPrice = x.StandartPrice,
            //                             CurrentPrice = x.CurrentPrice,
            //                         }).ToList()
            //                     };
                          
          
            //     var resultList = result.ToList();
             
           

                
           
            
            
            return Ok(new{promotionDtos,consoleDtos,consPromDtos});
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var promotion =  _promotionService.GetById(id);
            //var promotionDto = _mapper.Map<PromotionDto>(promotion);
            return Ok(promotion);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody]PromotionDto promotionDto)
        {
            var promotion = _mapper.Map<PromotionDto>(promotionDto);
            promotion.Id = id;

            try 
            {
                _promotionService.Update(promotion, promotionDto.Consoles);
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
            _promotionService.Delete(id);
            return Ok();
        }
        
    }
}

