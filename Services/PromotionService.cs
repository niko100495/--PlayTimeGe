using System.Collections.Generic;
using WebApi.Helpers;
using System.Linq;
using WebApi.Entities;
using WebApi.Dtos;
using AutoMapper;

namespace WebApi.Services
{
    public interface IPromotionService
    {
        
        IEnumerable<Promotion> GetAll();
        PromotionRelation GetById(int id);
        Promotion Create(PromotionDto Promotion);
        void Update(PromotionDto promotionParam, int[] ConsId);
        void Delete(int id);

        List<PromotionConsole> AddPromCons(int PromId, int[] ConsId);
         IEnumerable<PromotionConsole> getAllPromCons();
         List<PromotionRelation> GetPromWithCons();
    }
    public class PromotionService : IPromotionService
    {
        private DataContext _context;
        private IMapper _mapper;
        public PromotionService(DataContext context, IMapper mapper)
        {
            _context = context;
             _mapper = mapper;
            
        }

        public Promotion Create(PromotionDto Promotion) 
        {
            var promotion = _mapper.Map<Promotion>(Promotion);
            
            _context.Add(promotion);
            _context.SaveChanges();
            return promotion;
            
        }

         public List<PromotionConsole> AddPromCons(int PromId, int[] ConsId) 
        {
            //var promotion = _mapper.Map<Promotion>(Promotion);

            List<PromotionConsole> Obj = new List<PromotionConsole>();
            for (int i = 0; i < ConsId.Length; i++)
            {
               Obj.Add(new PromotionConsole() { consoleId = ConsId[i], promotionId = PromId });
            }
            
            _context.PromotionConsoles.AddRange(Obj);
            _context.SaveChanges();
            
          
            return Obj;
            
        }
        public IEnumerable<PromotionConsole> getAllPromCons() 
        {
           return _context.PromotionConsoles;   
        }
        

        public void Delete(int id)
        {
            var promotion = _context.Promotions.Find(id);
            if (promotion != null)
            {
                var forConsole = _context.PromotionConsoles.ToList();
                for (int i = 0; i < forConsole.Count; i++)
                {
                    if (forConsole[i].promotionId == promotion.Id)
                    {
                        _context.PromotionConsoles.Remove(forConsole[i]);
                    }
                }
                _context.Promotions.Remove(promotion);
                _context.SaveChanges();
            }
        }

        public IEnumerable<Promotion> GetAll()
        {
            return _context.Promotions;
        }

        public PromotionRelation GetById(int id)
        {
             var result =   from PC in _context.PromotionConsoles      
                            join P in _context.Promotions on PC.promotionId equals P.Id 
                            join C in _context.Consoles on PC.consoleId equals C.Id 
                            where P.Id == id
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
            //return _context.Promotions.Find(id);
            return result.FirstOrDefault();
        }

        public void Update(PromotionDto promotionParam, int[] ConsId)
        {
            var promotion = _context.Promotions.Find(promotionParam.Id);

            promotion.Id = promotionParam.Id;
            promotion.PromotionName = promotionParam.PromotionName;
            promotion.PromotionDesc = promotionParam.PromotionDesc;
            promotion.Price = promotionParam.Price;
            promotion.AllTime = promotionParam.AllTime;
            promotion.DateStart = promotionParam.DateStart;
            promotion.DateEnd = promotionParam.DateEnd;

            var forConsole = _context.PromotionConsoles.ToList();
            for (int i = 0; i < forConsole.Count; i++)
            {
                if (forConsole[i].promotionId == promotion.Id)
                {
                    _context.PromotionConsoles.Remove(forConsole[i]);
                }
            }
            List<PromotionConsole> Obj = new List<PromotionConsole>();
            for (int i = 0; i < ConsId.Length; i++)
            {
                Obj.Add(new PromotionConsole() { consoleId = ConsId[i], promotionId = promotion.Id });
            }
            _context.PromotionConsoles.AddRange(Obj);
                

            _context.Promotions.Update(promotion);
            _context.SaveChanges();
        }
        public List<PromotionRelation> GetPromWithCons()
        {
             var result =    from PC in _context.PromotionConsoles      
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
                          
          
                
            return result.ToList();
        }
    }
}