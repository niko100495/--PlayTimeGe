using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using System.IdentityModel.Tokens.Jwt;
using WebApi.Helpers;
using Microsoft.Extensions.Options;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using WebApi.Services;
using WebApi.Dtos;
using WebApi.Entities;
using WebApi.Dtos.ForRelationship;
using System.Linq;

namespace WebApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class UsersController : ControllerBase
    {
        private IUserService _userService;
        private IMapper _mapper;
        private readonly AppSettings _appSettings;

        public UsersController(
            IUserService userService,
            IMapper mapper,
            IOptions<AppSettings> appSettings)
        {
            _userService = userService;
            _mapper = mapper;
            _appSettings = appSettings.Value;
        }

        [AllowAnonymous]
        [HttpPost("authenticate")]
        public IActionResult Authenticate([FromBody]UserDto userDto)
        {
            var user = _userService.Authenticate(userDto.FirstName, userDto.LastName, userDto.Password);
            
            if(user == null)
                return BadRequest(new { message = "Username or password is incorrect" });
            var role = _userService.GetRoles();

            var roles =  role.Where(c => c.Id == user.FK_Role);
                                   
            var userRelation = _mapper.Map<UserRelation>(user);
            userRelation.FK_Role = roles.ToArray()[0].StatusRole;
            
            if (userRelation == null)
                return BadRequest(new { message = "Username or password is incorrect" });

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[] 
                {
                    new Claim(ClaimTypes.Name, userRelation.Id.ToString()),
                    new Claim(ClaimTypes.Name, userRelation.FirstName),
                    new Claim(ClaimTypes.Role, userRelation.FK_Role)
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            return Ok(new {
                Id = userRelation.Id,
                avatarId = userRelation.AvatarId,
                FirstName = userRelation.FirstName,
                LastName = userRelation.LastName,
                Email = userRelation.Email,
                Number = userRelation.Number,
                FK_Role= userRelation.FK_Role,
                CreateDate = userRelation.CreateDate,
                Token = tokenString,
                totalDay = Math.Round((DateTime.Now - DateTime.Parse(userRelation.CreateDate)).TotalDays)
            });
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public IActionResult Register([FromBody]UserDto userDto)
        {
            var user = _mapper.Map<User>(userDto);

            try 
            {
                _userService.Create(user, userDto.Password);
                return Ok();
            } 
            catch(AppException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        [Authorize(Roles = "Developer, Administrator")]
        [HttpGet]
        public IActionResult GetAll()
        {
            var users =  _userService.GetAll().ToList();
            
            for (int i = 0; i < users.Count; i++)
            {
               var userHistory =  _userService.getLastRecord(users[i].Id);
               if (userHistory != null)
               {    
                    if (userHistory.ExitTime == "waiting")
                    {
                        users[i].UserActive = true;
                    }
               }
            }
            return Ok(users);
        }
        
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var user =  _userService.GetById(id);
           
            
            var CheckWork = _userService.GetByIdStaff(id);
            string HelpCheck = "";
            if (CheckWork != null)
            {
                HelpCheck = CheckWork.ExitTime;
            }


             string getConsoleMoneySum;
             string getMagazineMoneySum;
            _userService.GetByIdStaffWorkGeneratedMoney(id, out  getConsoleMoneySum, out getMagazineMoneySum);
            
            
            var userDto = _mapper.Map<UserDto>(user);
            return Ok(new {
                    
                    AvatarId =userDto.AvatarId,
                    FirstName =userDto.FirstName,
                    LastName =userDto.LastName,
                    Email =userDto.Email,
                    Number =userDto.Number,
                    Password =userDto.Password,
                    FK_Role =userDto.FK_Role,
                    userActive =  HelpCheck == "waiting" ? true : false,
                    WorkTime = CheckWork == null ? 0 : CheckWork.CountEnter,
                    ConsoleGenetateMoney = getConsoleMoneySum,
                    MagazineGenetateMoney = getMagazineMoneySum,
                } );
        }
        [Authorize(Roles = "Developer, Administrator")]
        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody]UserDto userDto)
        {
            var user = _mapper.Map<User>(userDto);
            user.Id = id;

            try 
            {
                _userService.Update(user, userDto.Password);
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
            _userService.Delete(id);
            return Ok();
        }
        
        [HttpPost("enter")]
        public IActionResult EnterUser(){
            var getActionUser = _userService.getActionUser();
            if (getActionUser != null)
            {
                return  Ok( new {Message = getActionUser.FirstName +" "+ getActionUser.LastName + " is Active"});
            }
            var lastRecord = _userService.getLastRecord(Int32.Parse(User.Identity.Name));
            var EnterUser = new StaffHistory();
            if(lastRecord != null){
                EnterUser =  _userService.Enter(Int32.Parse(User.Identity.Name), lastRecord.CountEnter+1);
            }
            else {

                EnterUser =  _userService.Enter(Int32.Parse(User.Identity.Name),1);
            }
                
            
            return Ok(
                new {
                Id = EnterUser.Id,
                UserId = EnterUser.UserId,
                Enter = EnterUser.EnterTime,
                Exit = EnterUser.ExitTime,
                CountEnter = EnterUser.CountEnter
                }
            );    
        }
        [HttpPost("exit")]
        public IActionResult ExitUser(){
            var userConsole = _userService.getLiveWorkInfo(Int32.Parse(User.Identity.Name), true);
            var ExitUser = _userService.Exit(Int32.Parse(User.Identity.Name));
            
             return Ok(
                new {
                Id = ExitUser.Id,
                UserId = ExitUser.UserId,
                Enter = ExitUser.EnterTime,
                Exit = ExitUser.ExitTime,
                CountEnter = ExitUser.CountEnter,
                ConsoleSum = float.Parse(userConsole.ToList()[0]),
                MagazineSum = float.Parse(userConsole.ToList()[1])
                }
            );    
        }
        //public IActionResult UserStatic(int userId,[FromQuery]UserParams userParams)
        [HttpGet("UserStatic/{userId}")]
        public IActionResult UserStatic(int userId)
        {   
            if (userId == 0)
            {
               return BadRequest(new { message = "User Id is Empty" });
            }
             //var getRecord = _userService.UserStatic(userParams, userId);
            var getRecordTrade = _userService.UserStatic(userId);
            var getRecordConsole = _userService.UserStaticConsole(userId);
            var getCommonStatic = new List<CommonStatic>();
            var getCommonStaticHelp = new List<CommonStatic>();
            for (int i = 0; i < getRecordConsole.Count; i++)
            {
                for (int j = 0; j < getRecordTrade.Count; j++)
                {
                    if (DateTime.Parse(getRecordConsole[i].Date).Date == DateTime.Parse(getRecordTrade[j].Date).Date)
                    {
                        getCommonStatic.Add( 
                            new CommonStatic{
                                Date = getRecordConsole[i].Date,
                                AboutUserConsole = getRecordConsole[i].AboutUser,
                                AboutUserTrade = getRecordTrade[j].AboutUser
                        }
                            
                        );
                    }
                    
                }
                    try {
                        if (getCommonStatic[i].AboutUserConsole==null)
                        {
                            continue;
                        }
                    }
                    catch {
                         getCommonStatic.Add( 
                            new CommonStatic{
                                Date = getRecordConsole[i].Date,
                                AboutUserConsole = getRecordConsole[i].AboutUser
                        });
                    }
            }
           for (int i = 0; i < getRecordTrade.Count; i++)
            {
                bool check = false;
                for (int j = 0; j < getCommonStatic.Count; j++)
                {
                    if (DateTime.Parse(getRecordTrade[i].Date).Date == DateTime.Parse(getCommonStatic[j].Date).Date)
                    {     
                        check = true;
                    }
                }
                    if (check == false)
                    {
                        
                        getCommonStatic.Add( 
                            new CommonStatic{
                                Date = getRecordTrade[i].Date,
                                AboutUserTrade = getRecordTrade[i].AboutUser,
                                AboutUserConsole = null
                        });
                    }
            }
        
               
                getCommonStatic.Sort((x, y) => DateTime.Parse(y.Date).CompareTo( DateTime.Parse(x.Date)));
                
            
            //Respone.AddPagination(getRecord.CurrentPage, getRecord.PageSize,getRecord.TotalCount, getRecord.TotalPages);
            return Ok(new {
                getCommonStatic
                //userTrade = getRecordTrade,
                //userConsole = getRecordConsole
                //getRecord 
            });
        }
        [HttpGet("UserStaticConsole/{userId}")]
        public IActionResult UserStaticConsole(int userId)
        {   
            if (userId == 0)
            {
               return BadRequest(new { message = "User Id is Empty" });
            }
            // var getRecord = _userService.UserStatic(userId, userParams);
            var getRecord = _userService.UserStaticConsole(userId);
            //Respone.AddPagination(getRecord.CurrentPage, getRecord.PageSize,getRecord.TotalCount, getRecord.TotalPages);
            return Ok(getRecord);
        }
        [HttpGet("userWorkInfo")]
        public IActionResult userWorkInfo()
        {   
            var userConsole = _userService.getLiveWorkInfo(Int32.Parse(User.Identity.Name));
            
            return Ok(new {
                ConsoleSum = float.Parse(userConsole.ToList()[0]),
                MagazineSum = float.Parse(userConsole.ToList()[1])
            });
        }

    }
}
