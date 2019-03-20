using System.Collections;
using System.Collections.Generic;
using WebApi.Helpers;
using System.Linq;
using WebApi.Entities;
using WebApi.Dtos;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using WebApi.Dtos.ForRelationship;
using System.Threading;
using System.Globalization;

namespace WebApi.Services
{
    public interface IConsoleService
    {
        //Console Authenticate(string FirstName, string LastName, string password);
        IEnumerable<Console> GetAllForPromotion();
        IEnumerable<GeTAllConsoleRelation> GetAll();
        Console GetById(int id);
        Console Create(ConsoleDto Console);
        void Update(ConsoleDto consoleParam);
        void Delete(int id);
        ConsolePlay CreateConsolePlay(ConsolePlayDto consolePlayDto);
        ConsolePlay ConsoleFinish(int consolePlayId);
        ConsolePlay ConsolePause(int consolePlayId);
        ConsolePlay ConsoleContinue(int consolePlayId);
        string GetTodaySum();
        List<ConsoleHistories> GetConsoleHistories();
    }
    public class ConsoleService : IConsoleService
    {
        private DataContext _context;
        private IMapper _mapper;
        public ConsoleService(DataContext context, IMapper mapper)
        {
            _context = context;
             _mapper = mapper;
            
        }

        public Console Create(ConsoleDto Console) 
        {
            var console = _mapper.Map<Console>(Console);
            console.CurrentPrice = Console.StandartPrice;
            _context.Add(console);
            _context.SaveChanges();
            return console;
            
        }

        public void Delete(int id)
        {
             var console = _context.Consoles.Find(id);
            if (console != null)
            {
                _context.Consoles.Remove(console);
                _context.SaveChanges();
            }
        }
         public IEnumerable<Console> GetAllForPromotion(){
             return _context.Consoles;
         }
        public IEnumerable<GeTAllConsoleRelation> GetAll()
        {
        var getConsoleBusy =    from C in _context.Consoles      
                                join PT in _context.ConsolePlays on C.Id equals PT.ConsoleId 
                                where PT.PlayFinish == "Waiting"
                                select new GeTAllConsoleRelation {
                                    actionId = PT.Id,
                                    CurrentPrice = C.CurrentPrice,
                                    CurrentTime = 16*3600,
                                    DefaultPrice = C.StandartPrice,
                                    fixedTimer = PT.isFixedTime,
                                    id = C.Id,
                                    maxTime = System.Int32.Parse(PT.FixedTime),
                                    maxUser = PT.Gamer,
                                    name = C.ConsoleName,
                                    state = "busy",
                                    timerActive = true,
                                    type = C.Type,
                                    users = PT.UserId,
                                    startTime =PT.PlayStart,
                                    ConsoleNumber = C.ConsoleNumber
                                    
                            };
                          
          
        var getConsoleFree =    from C in _context.Consoles      
                                select new GeTAllConsoleRelation {
                                    actionId = 0,
                                    CurrentPrice = C.CurrentPrice,
                                    CurrentTime = 0,
                                    DefaultPrice = C.StandartPrice,
                                    fixedTimer =false,
                                    id = C.Id,
                                    maxTime = 0,
                                    maxUser = 0,
                                    name = C.ConsoleName,
                                    state = "free",
                                    timerActive = false,
                                    type = C.Type,
                                    users = 0,
                                    ConsoleNumber = C.ConsoleNumber
                                    
                            };
    
            
            var getConsoleFreeList = getConsoleFree.ToList();
            var getConsoleBusyList = getConsoleBusy.ToList();
            
                for (int i = 0; i <getConsoleFreeList.Count-1; i++)
                {
                    for (int j = 0; j < getConsoleBusyList.Count-1; j++)
                    {
                        if(getConsoleFreeList[i].id==getConsoleBusyList[j].id)
                        {
                            getConsoleFreeList.Remove(getConsoleFreeList[i]);
                        }
                        
                    }
                }
                for (int i = 0; i <  getConsoleBusyList.Count; i++)
                {
                    if (getConsoleBusyList[i].fixedTimer == true)
                    {
                        getConsoleBusyList[i].CurrentTime = System.Convert.ToInt32(getConsoleBusyList[i].maxTime - ((System.DateTime.Now - System.DateTime.Parse(getConsoleBusyList[i].startTime)).TotalSeconds));
                    }
                    else {
                         getConsoleBusyList[i].CurrentTime = System.Convert.ToInt32(System.Math.Round((System.DateTime.Now - System.DateTime.Parse(getConsoleBusyList[i].startTime)).TotalSeconds,3));
                    }
                }

               
            
            

                var getAllConsole = getConsoleBusyList.Concat(getConsoleFree).ToList();
                
                return getAllConsole.GroupBy(x => x.id).Select(y => y.First());
           
        }

        public Console GetById(int id)
        {
            return _context.Consoles.Find(id);
        }

        public void Update(ConsoleDto consoleParam)
        {
            var console = _context.Consoles.Find(consoleParam.Id);

            console.Id = consoleParam.Id;
            console.ConsoleName = consoleParam.ConsoleName;
            console.ConsoleNumber = consoleParam.ConsoleNumber;
            console.Type = consoleParam.Type;
            console.StandartPrice = consoleParam.StandartPrice;
            console.CurrentPrice = consoleParam.StandartPrice;
            //console.PromotionId = consoleParam.PromotionId;

            _context.Consoles.Update(console);
            _context.SaveChanges();
        }
        public ConsolePlay CreateConsolePlay(ConsolePlayDto consolePlayDto)
        {
            var consolePlay = _mapper.Map<ConsolePlay>(consolePlayDto);
            consolePlay.PlayStart = System.DateTime.Now.ToString();
            consolePlay.PlayFinish = "Waiting";
            if (consolePlay.Gamer == 0)
            {
                consolePlay.Gamer = 2;
            }
            consolePlay.Price = (_context.Consoles.Find(consolePlay.ConsoleId).StandartPrice*consolePlay.Gamer)/2;


        var GetPromotion =    from PC in _context.PromotionConsoles      
                                join P in _context.Promotions on PC.promotionId equals P.Id 
                                join C in _context.Consoles on PC.consoleId equals C.Id 
                                where PC.consoleId == consolePlayDto.ConsoleId
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
                            select new PromotionRelation {
                                    PromotionId = g.FirstOrDefault().PromotionId,
                                    PromotionName = g.FirstOrDefault().PromotionName,
                                    PromotionDesc = g.FirstOrDefault().PromotionDesc,
                                    Price = g.FirstOrDefault().Price,
                                    AllTime = g.FirstOrDefault().AllTime,
                                    DateStart = g.FirstOrDefault().DateStart,
                                    DateEnd = g.FirstOrDefault().DateEnd,
                                    HourStart = g.FirstOrDefault().HourStart,
                                    HourEnd = g.FirstOrDefault().HourEnd,
                                    Console = g.Select(x=> new ConsoleProm {
                                        ConsoleId = x.ConsoleId,
                                        ConsoleName = x.ConsoleName,
                                        ConsoleNumber = x.ConsoleNumber,
                                        Type = x.Type,
                                        StandartPrice = x.StandartPrice,
                                        CurrentPrice = x.CurrentPrice,
                                    }).ToList()
                                };
                          
          
                var resultList = GetPromotion.ToList();
            if (resultList.Count > 0)
            {
                for (int i = 0; i < resultList.Count; i++)
                {
                    
                    System.DateTime DateTimeStart = new System.DateTime();
                    System.DateTime DateTimeEnd = new System.DateTime();
                    List<System.DateTime> DateTimeList = new List<System.DateTime>();
                    if (resultList[i].AllTime == true)
                    {
                        if (System.DateTime.Parse(resultList[i].HourStart).TimeOfDay > System.DateTime.Parse(resultList[i].HourEnd).TimeOfDay)
                                {
                                    DateTimeStart =  System.DateTime.Now.Date + System.DateTime.Parse(resultList[i].HourStart).TimeOfDay;
                                    DateTimeEnd =  System.DateTime.Now.Date + System.DateTime.Parse(resultList[i].HourEnd).TimeOfDay;
                                    DateTimeList.Add(DateTimeStart);
                                    DateTimeList.Add(System.DateTime.Parse("23:59:59"));
                                    DateTimeList.Add(System.DateTime.Parse("00:00:00"));
                                    DateTimeList.Add(DateTimeEnd);
                                }
                                else
                                {
                                    DateTimeStart =  System.DateTime.Now.Date + System.DateTime.Parse(resultList[i].HourStart).TimeOfDay;
                                    DateTimeEnd =  System.DateTime.Now.Date + System.DateTime.Parse(resultList[i].HourEnd).TimeOfDay;
                                }
                    }
                    else {
                         if (System.DateTime.ParseExact(resultList[i].DateStart,"d/M/yyyy",  null).Date <=  System.DateTime.Now
                            &&
                            System.DateTime.ParseExact(resultList[i].DateEnd,"d/M/yyyy",  null).Date >=  System.DateTime.Now
                        )
                        {
                           
                                
                                if (System.DateTime.Parse(resultList[i].HourStart).TimeOfDay > System.DateTime.Parse(resultList[i].HourEnd).TimeOfDay)
                                {
                                    DateTimeStart =  System.DateTime.Now.Date + System.DateTime.Parse(resultList[i].HourStart).TimeOfDay;
                                    DateTimeEnd =  System.DateTime.Now.Date + System.DateTime.Parse(resultList[i].HourEnd).TimeOfDay;
                                    DateTimeList.Add(DateTimeStart);
                                    DateTimeList.Add(System.DateTime.Parse("23:59:59"));
                                    DateTimeList.Add(System.DateTime.Parse("00:00:00"));
                                    DateTimeList.Add(DateTimeEnd);
                                }
                                else
                                {
                                    DateTimeStart =  System.DateTime.Now.Date + System.DateTime.Parse(resultList[i].HourStart).TimeOfDay;
                                    DateTimeEnd =  System.DateTime.Now.Date + System.DateTime.Parse(resultList[i].HourEnd).TimeOfDay;
                                }
                            
                            
                        }
                        //DateTimeStart =  System.DateTime.ParseExact(resultList[i].DateStart,"d/M/yyyy",  null).Date + System.DateTime.Parse(resultList[i].HourStart).TimeOfDay;
                        //DateTimeEnd =  System.DateTime.ParseExact(resultList[i].DateEnd,"d/M/yyyy",  null).Date + System.DateTime.Parse(resultList[i].HourEnd).TimeOfDay;
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
                            consolePlay.Price = (resultList[i].Price * consolePlay.Gamer)/2;
                        }
                    }
                    else {
                        if (   
                        DateTimeStart <= System.DateTime.Now
                        &&
                        DateTimeEnd >= System.DateTime.Now
                        )
                        {
                            consolePlay.Price = (resultList[i].Price * consolePlay.Gamer)/2;
                        }
                    }
                   
                }
            }
            _context.Add(consolePlay);
            _context.SaveChanges();
            return consolePlay;
        }
        public ConsolePlay ConsoleFinish(int consolePlayId) {
            var finishConsole = _context.ConsolePlays.Find(consolePlayId);
            finishConsole.PlayFinish = System.DateTime.Now.ToString();
            finishConsole.PauseFinish = System.DateTime.Now.ToString();
            finishConsole.Paid =  System.Math.Round((
                    ( System.DateTime.Parse(finishConsole.PauseStart) -  System.DateTime.Parse(finishConsole.PlayStart)
                    ).TotalSeconds - finishConsole.PauseDuration) * (finishConsole.Price/3600), 2);   
                  
            if (finishConsole.isFixedTime == true)
            {
                var LiveTimeFixed = (System.DateTime.Now -   System.DateTime.Parse(finishConsole.PlayStart)).TotalSeconds;
                var checkDBTimeFixed = double.Parse(finishConsole.FixedTime) - LiveTimeFixed;
                if (checkDBTimeFixed < 1200)
                {
                    finishConsole.PlayFinish = System.DateTime.Now.ToString();
                    finishConsole.PauseFinish = System.DateTime.Now.ToString();
                    finishConsole.Paid =  System.Math.Round(
                        System.Int32.Parse(finishConsole.FixedTime) * (finishConsole.Price/3600), 2);
                }            
            }
            _context.Update(finishConsole);
            _context.SaveChanges();
            return finishConsole;
        }
        public ConsolePlay ConsolePause(int consolePlayId) {
            
            var finishConsole = _context.ConsolePlays.Find(consolePlayId);
            finishConsole.PauseStart = System.DateTime.Now.ToString();
            _context.Update(finishConsole);
            _context.SaveChanges();
            return finishConsole;
        }
         public ConsolePlay ConsoleContinue(int consolePlayId) {
            var finishConsole = _context.ConsolePlays.Find(consolePlayId);
            finishConsole.PauseFinish = System.DateTime.Now.ToString();
            finishConsole.PauseDuration +=  System.Convert.ToInt32(System.Math.Round((System.DateTime.Now - System.DateTime.Parse(finishConsole.PauseStart)).TotalSeconds,3));
            _context.Update(finishConsole);
            _context.SaveChanges();
            return finishConsole;
        }
        public string GetTodaySum()
        {
            var getConsole =   from CP in _context.ConsolePlays      
                                join C in _context.Consoles on CP.ConsoleId equals C.Id 
                                select new {
                                    ConsoleId = C.Id,
                                    UserId =CP.UserId,
                                    Gamer = CP.Gamer,
                                    Price = CP.Price,
                                    isFixedTime = CP.isFixedTime,
                                    FixedTime = CP.FixedTime,
                                    PauseStart = CP.PauseStart,
                                    PauseFinish = CP.PauseFinish,
                                    PauseDuration = CP.PauseDuration,
                                    PlayStart = CP.PlayStart,
                                    PlayFinish = CP.PlayFinish == "Waiting" ? "11/11/2018 00:00:00 AM" : CP.PlayFinish,
                                    Paid = CP.Paid,
                                    
                                };
            List<double> sum = new List<double>();
            var getConsoleList = getConsole.ToList();
            for (int i = 0; i < getConsoleList.Count; i++)
            {
                if(System.DateTime.Today.Date == System.DateTime.Parse(getConsoleList[i].PlayFinish).Date ){
                    sum.Add(getConsoleList[i].Paid);
                }
            }
            double finishDaySum = sum.Sum();
            return finishDaySum.ToString();
        }
        
        public List<ConsoleHistories> GetConsoleHistories()
        {
            var getConsoleHistories =   from CP in _context.ConsolePlays      
                                        join C in _context.Consoles on CP.ConsoleId equals C.Id
                                        join U in _context.Users on CP.UserId equals U.Id 
                                        select new ConsoleHistories {
                                            ActionId = CP.Id,
                                            ConsoleId = C.Id,
                                            ConsoleName = C.ConsoleName,
                                            ConsoleNumber = C.ConsoleNumber,
                                            ConsoleType = C.Type,
                                            ConsoleStandartPrice = C.StandartPrice,
                                            ConsoleCurrentPrice = C.CurrentPrice,
                                            UserId =CP.UserId,
                                            UserAvatarId =U.AvatarId,
                                            UserFirstName =U.FirstName,
                                            UserLastName =U.LastName,
                                            UserEmail =U.Email,
                                            UserNumber =U.Number,
                                            UserFK_Role =U.FK_Role,
                                            Gamer = CP.Gamer,
                                            Price = CP.Price,
                                            isFixedTime = CP.isFixedTime,
                                            FixedTime = CP.FixedTime,
                                            PauseStart = CP.PauseStart,
                                            PauseFinish = CP.PauseFinish,
                                            PauseDuration = CP.PauseDuration,
                                            PlayStart = CP.PlayStart,
                                            PlayFinish = CP.PlayFinish,
                                            Paid = CP.Paid,
                                        };
            return getConsoleHistories.OrderByDescending(d => d.ActionId).ToList();
        }



    }
}