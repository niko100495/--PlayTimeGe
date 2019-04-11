using System;
using System.Collections.Generic;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using WebApi.Dtos;
using WebApi.Dtos.ForRelationship;
using WebApi.Helpers;
using WebApi.Services;
using System.Linq;

namespace WebApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class ConsolesController : ControllerBase
    {
        private DataContext _context;
        private IConsoleService _consoleService;
        private IMapper _mapper;
        private readonly AppSettings _appSettings;

        public ConsolesController(
            IConsoleService consoleService,
            IMapper mapper,
            IOptions<AppSettings> appSettings,
            DataContext context
            )
        {
            _consoleService = consoleService;
            _mapper = mapper;
            _appSettings = appSettings.Value;
            _context = context;
        }

        [HttpPost("create")]
        public IActionResult CreateConsole([FromBody]ConsoleDto consoleDto)
        {
            var consoleDtos = _mapper.Map<ConsoleDto>(consoleDto);
            
            var console = _consoleService.Create(consoleDtos);
             return Ok(new {
                Id = console.Id,
                ConsoleName = console.ConsoleName,
                ConsoleNumber = console.ConsoleNumber,
                Type = console.Type,
                StandartPrice = console.StandartPrice,
                CurrentPrice = console.CurrentPrice,
                //PromotionId= console.PromotionId
                
            });
            
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var consoles =  _consoleService.GetAll();
            var consoleDtos = _mapper.Map<IList<GeTAllConsoleRelation>>(consoles);
            var SortConsole = new List<GeTAllConsoleRelation>();
            var tempForSort =  new List<GeTAllConsoleRelation>();

            List<ConsoleDto> p = new List<ConsoleDto>();
            List<PromotionRelation> getPromo = new List<PromotionRelation>();
            var Promotion =    from PC in _context.PromotionConsoles      
                            join P in _context.Promotions on PC.promotionId equals P.Id 
                            join C in _context.Consoles on PC.consoleId equals C.Id 
                            select new {
                                PromotionId = PC.promotionId,
                                PromotionName = P.PromotionName,
                                PromotionDesc = P.PromotionDesc,
                                Price = P.Price,
                                AllTime = P.AllTime,
                                DateStart = P.DateStart,
                                DateEnd = P.DateEnd,
                                HourStart = P.HourStart,
                                HourEnd = P.HourEnd,
                                ConsoleId = PC.consoleId,
                                ConsoleName = C.ConsoleName,
                                ConsoleNumber = C.ConsoleNumber,
                                Type = C.Type,
                                StandartPrice = C.StandartPrice,
                                CurrentPrice = C.CurrentPrice
                            } into PCtable
                            group PCtable by PCtable.PromotionId into g
                            select new {
                                    PromotionId = g.FirstOrDefault().PromotionId,
                                    PromotionName = g.FirstOrDefault().PromotionName,
                                    PromotionDesc = g.FirstOrDefault().PromotionDesc,
                                    Price = g.FirstOrDefault().Price,
                                    AllTime = g.FirstOrDefault().AllTime,
                                    DateStart = g.FirstOrDefault().DateStart,
                                    DateEnd = g.FirstOrDefault().DateEnd,
                                    HourStart = g.FirstOrDefault().HourStart,
                                    HourEnd = g.FirstOrDefault().HourEnd,
                                    Console = g.Select(x=> new {
                                        consoleId = x.ConsoleId,
                                        ConsoleName = x.ConsoleName,
                                        ConsoleNumber = x.ConsoleNumber,
                                        Type = x.Type,
                                        StandartPrice = x.StandartPrice,
                                        CurrentPrice = x.CurrentPrice,
                                    }).ToList()
                                };
                          

            var PromotionList = Promotion.ToList();

            if (PromotionList.Count>0)
            {
                
                for (int i = 0; i < consoleDtos.Count; i++)
                {
                    for (int j = 0; j < PromotionList.Count; j++)
                    {
                        DateTime DateTimeStart = new DateTime();
                        DateTime DateTimeEnd= new DateTime();
                        List<System.DateTime> DateTimeList = new List<System.DateTime>();
                            if (PromotionList[j].AllTime == true)
                            {
                                if (System.DateTime.Parse(PromotionList[j].HourStart).TimeOfDay > System.DateTime.Parse(PromotionList[j].HourEnd).TimeOfDay)
                                {
                                    DateTimeStart =  System.DateTime.Now.Date + System.DateTime.Parse(PromotionList[j].HourStart).TimeOfDay;
                                    DateTimeEnd =  System.DateTime.Now.Date + System.DateTime.Parse(PromotionList[j].HourEnd).TimeOfDay;
                                    DateTimeList.Add(DateTimeStart);
                                    DateTimeList.Add(System.DateTime.Parse("23:59:59"));
                                    DateTimeList.Add(System.DateTime.Parse("00:00:00"));
                                    DateTimeList.Add(DateTimeEnd);
                                }
                                else
                                {
                                    DateTimeStart =  System.DateTime.Now.Date + System.DateTime.Parse(PromotionList[j].HourStart).TimeOfDay;
                                    DateTimeEnd =  System.DateTime.Now.Date + System.DateTime.Parse(PromotionList[j].HourEnd).TimeOfDay;
                                }
                            }
                            else {
                                if (System.DateTime.ParseExact(PromotionList[j].DateStart,"d/M/yyyy",  null).Date <=  System.DateTime.Now
                                    &&
                                    System.DateTime.ParseExact(PromotionList[j].DateEnd,"d/M/yyyy",  null).Date >=  System.DateTime.Now
                                )
                                {
                                 
                                
                                    if (System.DateTime.Parse(PromotionList[j].HourStart).TimeOfDay > System.DateTime.Parse(PromotionList[j].HourEnd).TimeOfDay)
                                    {
                                        DateTimeStart =  System.DateTime.Now.Date + System.DateTime.Parse(PromotionList[j].HourStart).TimeOfDay;
                                        DateTimeEnd =  System.DateTime.Now.Date + System.DateTime.Parse(PromotionList[j].HourEnd).TimeOfDay;
                                        DateTimeList.Add(DateTimeStart);
                                        DateTimeList.Add(System.DateTime.Parse("23:59:59"));
                                        DateTimeList.Add(System.DateTime.Parse("00:00:00"));
                                        DateTimeList.Add(DateTimeEnd);
                                    }
                                    else
                                    {
                                        DateTimeStart =  System.DateTime.Now.Date + System.DateTime.Parse(PromotionList[j].HourStart).TimeOfDay;
                                        DateTimeEnd =  System.DateTime.Now.Date + System.DateTime.Parse(PromotionList[j].HourEnd).TimeOfDay;
                                    }
                            
                            
                                }
                            }
                            if (DateTimeList.Count>0)
                            {
                                if (
                                    DateTimeList[0] <= System.DateTime.Now
                                    &&
                                    DateTimeList[1] >= System.DateTime.Now
                                    ||
                                    DateTimeList[2] <= System.DateTime.Now
                                    &&
                                    DateTimeList[3] >= System.DateTime.Now
                                )
                                {
                                   for (int k = 0; k < PromotionList[j].Console.Count; k++)
                                    {
                                        
                                        if (    consoleDtos[i].id == PromotionList[j].Console[k].consoleId )
                                        {
                                            consoleDtos[i].Promotion =  PromotionList[j].PromotionName;
                                            consoleDtos[i].PromotionPrice = PromotionList[j].Price.ToString();
                                        }
                                        
                                        
                                    }
                                }
                            }
                            else {
                                if (   
                                DateTimeStart <= System.DateTime.Now
                                &&
                                DateTimeEnd >= System.DateTime.Now
                                )
                                {
                                    for (int k = 0; k < PromotionList[j].Console.Count; k++)
                                    {
                                        
                                        if (    consoleDtos[i].id == PromotionList[j].Console[k].consoleId )
                                        {
                                            consoleDtos[i].Promotion =  PromotionList[j].PromotionName;
                                            consoleDtos[i].PromotionPrice = PromotionList[j].Price.ToString();
                                        }
                                        
                                        
                                    }
                                }
                            }
                            if (   
                                    DateTimeStart <= System.DateTime.Now
                                    &&
                                    DateTimeEnd >= System.DateTime.Now
                            )
                            {
                               
                                for (int k = 0; k < PromotionList[j].Console.Count; k++)
                                {
                                    
                                    if (    consoleDtos[i].id == PromotionList[j].Console[k].consoleId )
                                    {
                                        consoleDtos[i].Promotion =  PromotionList[j].PromotionName;
                                        consoleDtos[i].PromotionPrice = PromotionList[j].Price.ToString();
                                    }
                                    
                                    
                                }
                            
                            }
                            
                    }
                }
            }
           





            for (int i = 0; i < consoleDtos.Count; i++)
            {
                if (consoleDtos[i].fixedTimer == true)
                {
                    SortConsole.Add(consoleDtos[i]);
                    SortConsole.Sort((x, y) => x.CurrentTime.CompareTo(y.CurrentTime));
                }
                if (consoleDtos[i].fixedTimer == false )
                {
                    tempForSort.Add(consoleDtos[i]);
                    tempForSort.Sort((x, y) => y.CurrentTime.CompareTo(x.CurrentTime));
                }


            }
            SortConsole.AddRange(tempForSort);
            return Ok(SortConsole);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var console =  _consoleService.GetById(id);
            var consoleDto = _mapper.Map<ConsoleDto>(console);
            return Ok(consoleDto);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody]ConsoleDto consoleDto)
        {
            //var console = _mapper.Map<Console, ConsoleDto>(consoleDto);
            var console = _mapper.Map<ConsoleDto>(consoleDto);
            console.Id = id;

            try 
            {
                _consoleService.Update(console);
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
            _consoleService.Delete(id);
            return Ok();
        }
        [HttpPost("ConsolePlay")]
        public IActionResult ConsolePlay([FromBody]ConsolePlayDto consoleplay)
        {
             var consolePlayDtos = _mapper.Map<ConsolePlayDto>(consoleplay);
            
            var consolePlay = _consoleService.CreateConsolePlay(consolePlayDtos);
             return Ok(new {
            
                Id = consolePlay.Id,
                ConsoleId = consolePlay.ConsoleId,
                UserId = consolePlay.UserId,
                Gamer = consolePlay.Gamer,
                Price = consolePlay.Price,
                isFixedTime = consolePlay.isFixedTime,
                FixedTime = consolePlay.FixedTime,
                PlayStart = consolePlay.PlayStart,
                PlayFinish = consolePlay.PlayFinish    
            });        
        }
        
        [HttpPost("ConsoleFinish/{consolePlayId}")]
        public IActionResult ConsoleFinish(int consolePlayId)
        {
            var consoleFinish = _consoleService.ConsoleFinish(consolePlayId);

            
             return Ok(new {
                Id = consoleFinish.Id,
                ConsoleId = consoleFinish.ConsoleId,
                UserId = consoleFinish.UserId,
                Gamer = consoleFinish.Gamer,
                Price = consoleFinish.Price,
                isFixedTime = consoleFinish.isFixedTime,
                FixedTime = consoleFinish.FixedTime,
                PlayStart = consoleFinish.PlayStart,
                PlayFinish = consoleFinish.PlayFinish,
                usedTime = (DateTime.Parse(consoleFinish.PlayFinish).Subtract(DateTime.Parse(consoleFinish.PlayStart))),
                Paid =   consoleFinish.Paid   
            });        
        }

        [HttpPost("ConsolePause/{consolePlayId}")]
        public IActionResult ConsolePause(int consolePlayId)
        {
            if (consolePlayId == 0)
            {
                return BadRequest(new { message = "consolePlayId is null" });
            }
            var consolePause = _consoleService.ConsolePause(consolePlayId);
            TimeSpan time = TimeSpan.FromSeconds(consolePause.PauseDuration);
            var Paid =  System.Math.Round((
                    ( System.DateTime.Parse(consolePause.PauseStart) -  System.DateTime.Parse(consolePause.PlayStart)
                    ).TotalSeconds - consolePause.PauseDuration) * (consolePause.Price/3600), 2);   
                  
            var usedTimer = (DateTime.Parse(consolePause.PauseStart).Subtract(DateTime.Parse(consolePause.PlayStart))).Subtract(time);
            if (consolePause.isFixedTime == true)
            {
                var LiveTimeFixed = (System.DateTime.Now -   System.DateTime.Parse(consolePause.PlayStart)).TotalSeconds;
                var checkDBTimeFixed = double.Parse(consolePause.FixedTime) - LiveTimeFixed;
                if (checkDBTimeFixed < 1200)
                {
                    Paid =  System.Math.Round(
                        System.Int32.Parse(consolePause.FixedTime)  * (consolePause.Price/3600), 2);
                } 
                var k = TimeSpan.FromSeconds(Convert.ToDouble(consolePause.FixedTime));           
                if (usedTimer >= TimeSpan.FromSeconds(Convert.ToDouble(consolePause.FixedTime)))
                {
                    usedTimer = TimeSpan.FromSeconds(Convert.ToDouble(consolePause.FixedTime));
                }
            }
             return Ok(new {
                
                ConsoleId = consolePause.ConsoleId,
                Price = consolePause.Price,
                usedTime = usedTimer,
                Paid =   Paid   
            });        
        }
        [HttpPost("ConsoleContinue/{consolePlayId}")]
        public IActionResult ConsoleContinue(int consolePlayId)
        {
            var consoleFinish = _consoleService.ConsoleContinue(consolePlayId);

            
             return Ok(new {
                Id = consoleFinish.Id,
                ConsoleId = consoleFinish.ConsoleId,
                UserId = consoleFinish.UserId,
                Gamer = consoleFinish.Gamer,
                Price = consoleFinish.Price,
                isFixedTime = consoleFinish.isFixedTime,
                FixedTime = consoleFinish.FixedTime,
                PlayStart = consoleFinish.PlayStart,
                PlayFinish = consoleFinish.PlayFinish,
                Pause = consoleFinish.PauseStart,
                Continue = consoleFinish.PauseFinish,
                DurationPause = consoleFinish.PauseDuration
            });        
        }
        [HttpGet("ConsolePlayToDay")]
        public IActionResult ConsolePlayToDay()
        {
            var getTodaySum = _consoleService.GetTodaySum();
            return Ok( 
                  new {thisDay=getTodaySum}
                );
        }

        [HttpGet("ConsoleHistories")]
        public IActionResult ConsoleHistories()
        {
            var getConsoleHistories = _consoleService.GetConsoleHistories();
            return Ok( 
                  getConsoleHistories
                );
        }
    }
}