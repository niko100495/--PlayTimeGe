using System.Collections.Generic;
using WebApi.Helpers;
using System.Linq;
using WebApi.Entities;
using WebApi.Dtos;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;

namespace WebApi.Services
{
    public interface IMagazineService
    {
        //Magazine Authenticate(string FirstName, string LastName, string password);
        IEnumerable<Magazine> GetAll();
        Magazine GetById(int id);
        Magazine Create(MagazineDto Magazine);
        void Update(MagazineDto magazineParam);
        void Delete(int id);
        Trade createTrade(TradeDto trade);
        string GetTodaySum();
    }
    public class MagazineService : IMagazineService
    {
        private DataContext _context;
        private IMapper _mapper;
        public MagazineService(DataContext context, IMapper mapper)
        {
            _context = context;
             _mapper = mapper;
            
        }

        public Magazine Create(MagazineDto Magazine) 
        {
            var magazine = _mapper.Map<Magazine>(Magazine);
            _context.Add(magazine);
            _context.SaveChanges();
            return magazine;
            
        }

        public void Delete(int id)
        {
             var magazine = _context.Magazines.Find(id);
            if (magazine != null)
            {
                _context.Magazines.Remove(magazine);
                _context.SaveChanges();
            }
        }

        public IEnumerable<Magazine> GetAll()
        {
            return _context.Magazines;
        }

        public Magazine GetById(int id)
        {
            return _context.Magazines.Find(id);
        }

        public void Update(MagazineDto magazineParam)
        {
            var magazine = _context.Magazines.Find(magazineParam.Id);

            magazine.Id = magazineParam.Id;
            magazine.Name = magazineParam.Name;
            magazine.Price = magazineParam.Price;
            magazine.Quantity = magazineParam.Quantity;
            magazine.Type = magazineParam.Type;

            _context.Magazines.Update(magazine);
            _context.SaveChanges();
        }

         public Trade createTrade(TradeDto trade) 
        {
            var tradeEF = _mapper.Map<Trade>(trade);
            tradeEF.BuyDate = DateTime.Now.ToString();
            if (tradeEF.QuantityProduct>0)
            {    
                var findProduct = _context.Magazines.Find(tradeEF.MagazineId);
                findProduct.Id = findProduct.Id;
                findProduct.Name = findProduct.Name;
                findProduct.Price = findProduct.Price;
                findProduct.Quantity = findProduct.Quantity - tradeEF.QuantityProduct;
                 _context.Magazines.Update(findProduct);
                _context.Trades.Add(tradeEF);
                _context.SaveChanges();
            }
            return tradeEF;
            
        }
        public string GetTodaySum()
        {
            var getMagazine =   from T in _context.Trades      
                                join M in _context.Magazines on T.MagazineId equals M.Id 
                                select new {
                                    
                                    Price = M.Price,
                                    Quantity = T.QuantityProduct,
                                    date = T.BuyDate
                                };
            List<float> sum = new List<float>();
            var getMagazineList = getMagazine.ToList();
            for (int i = 0; i < getMagazineList.Count; i++)
            {
                if(DateTime.Today.Date == DateTime.Parse(getMagazineList[i].date).Date ){
                    sum.Add(getMagazineList[i].Quantity * getMagazineList[i].Price);
                }
            }
            float finishDaySum = sum.Sum();
            return finishDaySum.ToString();
        }
    }
}