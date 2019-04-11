using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using WebApi.Dtos.ForRelationship;
using WebApi.Entities;
using WebApi.Helpers;

namespace WebApi.Services
{
    public interface IUserService
    {
        User Authenticate(string FirstName, string LastName, string password);
        //IEnumerable<User> GetAll();
        IEnumerable<UserRelation> GetAll();
        IEnumerable<Roles> GetRoles();
        User GetById(int id);
        void  GetByIdStaffWorkGeneratedMoney(int id , out string getConsoleMoneySum, out string getMagazineMoneySum);
        StaffHistory GetByIdStaff(int id);
        User Create(User user, string password);
        void Update(User user, string password = null);
        void Delete(int id);

        StaffHistory getLastRecord(int userId);
        StaffHistory Enter(int userId, int count);

        StaffHistory Exit(int userId);
        List<StaticRelation> UserStatic(int userId);
        //PagedList<StaticRelation> UserStatic(UserParams userParams,int userId);
        List<StaticRelationConsole> UserStaticConsole(int userId);
        User getActionUser();
        List<string> getLiveWorkInfo(int userId, bool clickExit = false);
    }

    public class UserService : IUserService
    {
        private DataContext _context;

        public UserService(DataContext context)
        {
            _context = context;
        }

        public User Authenticate(string FirstName, string LastName, string password)
        {
            if (string.IsNullOrEmpty(FirstName) || string.IsNullOrEmpty(LastName) || string.IsNullOrEmpty(password))
                return null;

            var user = _context.Users.SingleOrDefault(x => x.FirstName == FirstName && x.LastName == LastName);
            
            if (user == null)
                return null;

           
            if (!VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
                return null;

           
            return user;
        }

       
        public IEnumerable<UserRelation> GetAll()
        {

            
           IEnumerable<UserRelation> model = null;
 
            model=  (
                        from u in _context.Users
                        join r in _context.Role
                        on u.FK_Role equals r.Id
                        
                        select new UserRelation 
                        {
                            Id = u.Id,
                            AvatarId = u.AvatarId,
                            FirstName = u.FirstName,
                            LastName = u.LastName,
                            Email = u.Email,
                            CreateDate = u.CreateDate,
                            Number = u.Number,
                            FK_Role = r.StatusRole,
                            UserActive = false
                           
                        }
                    );

            return model;
        }
        

        public IEnumerable<Roles> GetRoles() {
            return _context.Role;
        }
        public User GetById(int id)
        {
            return _context.Users.Find(id);
        }
         public  void  GetByIdStaffWorkGeneratedMoney(int id , out string getConsoleMoneySum, out string getMagazineMoneySum)
        {
            var getConsoleMoney =  from CP in _context.ConsolePlays
                                    where CP.UserId == id
                                    select new  
                                        { ConsolePrice = CP.Paid };
             var getMagazineMoney =  from CP in _context.Trades
                                    join M in _context.Magazines on CP.MagazineId equals M.Id 
                                    where CP.UserId == id
                                    select new  
                                        {   MagazinePrice = M.Price,
                                            Quantity =CP.QuantityProduct
                                         };

            List<float> sum = new List<float>();
            var getMagazineList = getMagazineMoney.ToList();
            for (int i = 0; i < getMagazineList.Count; i++)
            {
               
                    sum.Add(getMagazineList[i].Quantity * getMagazineList[i].MagazinePrice);
                
            }
                
            getMagazineMoneySum =  sum.Sum().ToString();
            getConsoleMoneySum = getConsoleMoney.Sum(item => item.ConsolePrice).ToString();

        }
         public StaffHistory GetByIdStaff(int id)
        {
            
            return  _context.StaffHistories.Where(u=> u.UserId ==id )
                  .OrderByDescending(u => u.Id).FirstOrDefault();;
        }

        public User Create(User user, string password)
        {
            
            if (string.IsNullOrWhiteSpace(password))
                throw new AppException("Password is required");

            if (_context.Users.Any(x => x.FirstName == user.FirstName && x.LastName==user.LastName))
                throw new AppException("Username \"" + user.FirstName + "," + user.LastName + "\" is already taken");

            byte[] passwordHash, passwordSalt;
            CreatePasswordHash(password, out passwordHash, out passwordSalt);

            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;
            
            user.CreateDate = DateTime.Now.ToString();

            _context.Users.Add(user);
            _context.SaveChanges();

            return user;
        }

        public void Update(User userParam, string password = null)
        {
            var user = _context.Users.Find(userParam.Id);

            if (user == null)
                throw new AppException("User not found");

            if (userParam.FirstName != user.FirstName && userParam.LastName != user.LastName)
            {
                
                if (_context.Users.Any(x => x.FirstName == userParam.FirstName && x.LastName == userParam.LastName))
                    throw new AppException("Username " + user.FirstName + "," + user.LastName + " is already taken");
            }

            
            user.AvatarId = userParam.AvatarId;
            user.FirstName = userParam.FirstName;
            user.LastName = userParam.LastName;
            user.Email = userParam.Email;
            user.Number = userParam.Number;
            user.FK_Role = userParam.FK_Role;

            
            if (!string.IsNullOrWhiteSpace(password))
            {
                byte[] passwordHash, passwordSalt;
                CreatePasswordHash(password, out passwordHash, out passwordSalt);

                user.PasswordHash = passwordHash;
                user.PasswordSalt = passwordSalt;
            }

            _context.Users.Update(user);
            _context.SaveChanges();
        }

        public void Delete(int id)
        {
            var user = _context.Users.Find(id);
            if (user != null)
            {
                _context.Users.Remove(user);
                _context.SaveChanges();
            }
        }

      

        private static void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            if (password == null) throw new ArgumentNullException("password");
            if (string.IsNullOrWhiteSpace(password)) throw new ArgumentException("Value cannot be empty or whitespace only string.", "password");

            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        private static bool VerifyPasswordHash(string password, byte[] storedHash, byte[] storedSalt)
        {
            if (password == null) throw new ArgumentNullException("password");
            if (string.IsNullOrWhiteSpace(password)) throw new ArgumentException("Value cannot be empty or whitespace only string.", "password");
            if (storedHash.Length != 64) throw new ArgumentException("Invalid length of password hash (64 bytes expected).", "passwordHash");
            if (storedSalt.Length != 128) throw new ArgumentException("Invalid length of password salt (128 bytes expected).", "passwordHash");

            using (var hmac = new System.Security.Cryptography.HMACSHA512(storedSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                for (int i = 0; i < computedHash.Length; i++)
                {
                    if (computedHash[i] != storedHash[i]) return false;
                }
            }

            return true;
        }
        public StaffHistory Enter(int userId, int count){
            StaffHistory history = new StaffHistory();
            history.UserId=userId;
            history.EnterTime = DateTime.Now.ToString();
            history.ExitTime = "waiting";
            history.CountEnter = count;
            _context.StaffHistories.Add(history);
            _context.SaveChanges();
            return history;

        }

        public StaffHistory getLastRecord(int userId){
             var lastHistory = _context.StaffHistories.Where(u=> u.UserId ==userId )
                  .OrderByDescending(u => u.Id).FirstOrDefault();
            return lastHistory;
        }
         public StaffHistory Exit(int userId){
            
            //var history = _context.StaffHistories.FromSql($"GetLastRecordForExit {userId}").ToList();
            //tu ar imushava $ gauwere @amis nacvlad
            // var history = _context.StaffHistories
            //        .FromSql(    @"select TOP 1 * from StaffHistories 
            //                     where UserId={0} 
            //                     order BY ID DESC", userId)
            //         .ToList();
            var history =  _context.StaffHistories.Where(u=> u.UserId ==userId )
                  .OrderByDescending(u => u.Id).FirstOrDefault();
            
            
            history.ExitTime =  DateTime.Now.ToString();
            _context.StaffHistories.Update(history);
            _context.SaveChanges();
            return history;
        }
        //public PagedList<StaticRelation> UserStatic(UserParams userParams,int userId)
        public List<StaticRelation> UserStatic(int userId)
        {
            var result =    from T in _context.Trades      
                                join M in _context.Magazines on T.MagazineId equals M.Id 
                                join U in _context.Users on T.UserId equals U.Id 
                                where T.UserId == userId
                                select new {
                                    TradeId = T.Id,
                                    TradeQuantity =T.QuantityProduct,
                                    ProductId = M.Id,
                                    ProductName = M.Name,
                                    ProductPrice = M.Price,
                                    QuantityProduct = T.QuantityProduct,
                                    //Sum = M.Price*T.QuantityProduct,
                                    dateDay =  DateTime.Parse(T.BuyDate).Date,
                                    date =  T.BuyDate,
                                    userId = T.UserId,
                                    OrigUserId = U.Id,
                                    UserName = U.FirstName,
                                    UserLastName = U.LastName
                                } into Ttable
                            group Ttable by Ttable.dateDay into g
                            select new StaticRelation {
                                    Date = g.FirstOrDefault().date, 
                                    // AboutUser = g.Select(x=> new {
                                    //     toDaySum = x.Sum,
                                    //     User = x.UserName,
                                    //     Userlast = x.UserLastName,
                                    // }).ToList()
                                    AboutUser = g.GroupBy(d => d.userId)
                                                .Select(
                                                    d => new AboutUser
                                                    {
                                                        Id = g.First().userId,
                                                        SumDay = g.Sum(s => s.ProductPrice*s.QuantityProduct),
                                                        Name = g.First().UserName,
                                                        LastName = g.First().UserLastName,
                                                        DetailDay = g.GroupBy(DD => DD.date)
                                                                    .Select(DD => new DetailDay {
                                                                        ProductId = DD.First().ProductId,
                                                                        ProductName = DD.First().ProductName,
                                                                        ProductPrice = DD.First().ProductPrice,
                                                                        QuantityProduct = DD.First().QuantityProduct,
                                                                        //Sum = DD.First().Sum,
                                                                        DateBuy = DD.First().date
                                                                    }).ToList()
                                                    }).ToList()
                                };
                  
          
                // return PagedList<StaticRelation>.Create(result,userParams.PageNumber, userParams.PageSize);
                return result.OrderByDescending(x=>DateTime.Parse(x.Date)).ToList();
        }
         public List<StaticRelationConsole> UserStaticConsole(int userId)
        {
            var result =    from CP in _context.ConsolePlays      
                                join C in _context.Consoles on CP.ConsoleId equals C.Id 
                                join U in _context.Users on CP.UserId equals U.Id 
                                where CP.UserId == userId
                                select new {
                                    ConsolePlayId = CP.Id,
                                    ConsoleId = CP.Id,
                                    ConsoleName = C.ConsoleName,
                                    ConsoleNumber = C.ConsoleNumber,
                                    ConsoleType = C.Type,
                                    ConsoleStandartPrice = C.StandartPrice,
                                    Gamer = CP.Gamer,
                                    Price = CP.Price,
                                    isFixedTime = CP.isFixedTime,
                                    FixedTime = CP.FixedTime,
                                    PauseStart = CP.PauseStart,
                                    PauseFinish = CP.PauseFinish,
                                    PauseDuration = CP.PauseDuration,
                                    PlayStart = CP.PlayStart,
                                    PlayFinish = CP.PlayFinish == "Waiting" ? "00:00:00" : CP.PlayFinish,
                                    Paid = CP.Paid,
                                    Sum = CP.Paid,
                                    dateDay =  DateTime.Parse(CP.PlayFinish == "Waiting" ? "00:00:00" : CP.PlayFinish).Date,
                                    date =  CP.PlayFinish == "Waiting" ? "00:00:00" : CP.PlayFinish ,
                                    userId = CP.UserId,
                                    OrigUserId = U.Id,
                                    UserName = U.FirstName,
                                    UserLastName = U.LastName,

                                } into CPtable
                            group CPtable by CPtable.dateDay into g
                            select new StaticRelationConsole {
                                    Date = g.FirstOrDefault().date, 
                                    AboutUser = g.GroupBy(d => d.userId)
                                                .Select(
                                                    d => new AboutUserConsole
                                                    {
                                                        UserId = g.First().userId,
                                                        SumDay = Math.Round(g.Sum(s => s.Sum),2),
                                                        Name = g.First().UserName,
                                                        LastName = g.First().UserLastName,
                                                        DetailDay = g.GroupBy(DD => DD.date)
                                                                    .Select(DD => new DetailDayConsole {
                                                                        ConsoleId = DD.First().ConsoleId,
                                                                        ConsoleName = DD.First().ConsoleName,
                                                                        ConsoleNumber = DD.First().ConsoleNumber,
                                                                        ConsoleType = DD.First().ConsoleType,
                                                                        ConsoleStandartPrice = DD.First().ConsoleStandartPrice,
                                                                        PlayStart = DD.First().PlayStart,
                                                                        PlayFinish = DD.First().PlayFinish == "Waiting" ? "00:00:00" : DD.First().PlayFinish ,                                                               
                                                                        Sum = DD.First().Sum,
                                                                    }).ToList()
                                                    }).ToList()
                                };
                          
          
                //return PagedList<StaticRelation>.Create(result,userParams.PageNumber, userParams.PageSize);
                return result.OrderByDescending(x=>DateTime.Parse(x.Date)).ToList();
        }
        public User getActionUser(){
            if (_context.StaffHistories.Count() == 0)
            {
                return null;
            }
            var user =  from SH in _context.StaffHistories
                        join U in _context.Users on SH.UserId equals U.Id 
                        where SH.ExitTime == "waiting"
                        select new User{
                            Id = SH.UserId,
                            FirstName = U.FirstName,
                            LastName = U.LastName
                        };
           
            // var user =   _context.StaffHistories.Join(  _context.Users,
            //                                         sh => sh.UserId, 
            //                                         uu => uu.Id,
            //                                         (sh,uu)=> new {SH = sh, U = uu }
            //                                     )
            //                                 .Where(SH_and_U=>SH_and_U.SH.ExitTime=="waiting")
            //                                 .DefaultIfEmpty()  
            //                                 .First()
            //                                 .U ?? new User();
            if (user.ToList().Count == 0)
            {
                return null;
            }
            return user.ToList().FirstOrDefault();
        }
         public List<string> getLiveWorkInfo(int userId, bool clickExit = false) {

             // If dont want to see the information on the other user, just remove this part
             ///////////////////////////////////////////////////////
            var getUser =   from SH in _context.StaffHistories 
                                where SH.ExitTime == "waiting"
                                select new {
                                    userId = SH.UserId
                                };
            //////////////////////////////////////////////////////////
             var getConsole =   from CP in _context.ConsolePlays 
                                where CP.UserId == getUser.FirstOrDefault().userId
                                select new getLiveWorkInfo {
                                    Paid = CP.Paid.ToString(),
                                    Time = DateTime.Parse(CP.PlayStart),
                                    isFixedTime = CP.isFixedTime,
                                    FixedTime = CP.FixedTime,
                                    Price = CP.Price
                                };
            var getMagazine =   from T in _context.Trades 
                                join M in _context.Magazines on T.MagazineId  equals M.Id
                                where T.UserId == getUser.FirstOrDefault().userId
                                select new getLiveWorkInfo {
                                    Paid =(T.QuantityProduct * M.Price).ToString(),
                                    Time = DateTime.Parse(T.BuyDate)
                                };
            try
            {
               var getEnterTime = DateTime.Parse(_context.StaffHistories
                                        .Where(u=> u.UserId == getUser.FirstOrDefault().userId && u.ExitTime == "waiting" )
                                        .OrderByDescending(u => u.Id)
                                        .FirstOrDefault().EnterTime);
            var getConsoleList = getConsole.ToList();
            var getMagazineList = getMagazine.ToList();
            if(clickExit == true){

                for (int i = 0; i < getConsoleList.Count; i++)
                {
                    if (getConsoleList[i].isFixedTime == true)
                    {
                        getConsoleList[i].Paid =  System.Math.Round(
                        System.Int32.Parse(getConsoleList[i].FixedTime) * (getConsoleList[i].Price/3600), 2).ToString();            
                    }
                }
            }
           
            List<string> Sum = new List<string>();
            Sum.Add((checkTimeForLiveWork(getConsoleList,getEnterTime).Sum(item => Convert.ToDouble(item.Paid))).ToString());
            Sum.Add((checkTimeForLiveWork(getMagazineList,getEnterTime).Sum(item => Convert.ToDouble(item.Paid))).ToString());

            return Sum;
            }
            catch { 
                        List<string> Sum = new List<string>();
                        Sum.Add("0");
                        Sum.Add("0");
                        return Sum;
                    }
         }

         public List<getLiveWorkInfo> checkTimeForLiveWork (List<getLiveWorkInfo> workList, DateTime getEnterTime)
        {
            List<getLiveWorkInfo> live = new List<getLiveWorkInfo>();
            for (int i = 0; i < workList.Count; i++)
            {
                if (getEnterTime < workList[i].Time)
                {
                    live.Add(workList[i]);
                }
            }
            return live;
        }
    }
}