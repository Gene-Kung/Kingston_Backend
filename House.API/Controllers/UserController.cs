using House.API.Filter;
using House.Model;
using House.Model.API;
using House.Model.DB;
using House.Model.Enums;
using House.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Collections.Generic;

namespace House.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UserController : BaseController
    {
        private readonly UserService _userService;

        public UserController(
            IHttpContextAccessor accessor,
            IOptions<AppSettings> appSettings,
            UserService userService) : base(accessor, appSettings)
        {
            _userService = userService;
        }

        [APIAuthorizationAttribute(PolicyEnum.FreeMember)]
        public ResNewBaseBodyModel<List<UserModel>> Query([FromBody] UserModel user) 
        {
            return _userService.Query(user);
        }

        public UserModel Get([FromBody] UserModel user) 
        {
            return _userService.Get(user);
        }

        [APIAuthorizationAttribute(PolicyEnum.FreeMember)]
        public ResBaseBodyModel Create([FromBody] ReqCreateUserModel user)
        {
            return _userService.Create(user);
        }

        [APIAuthorizationAttribute(PolicyEnum.FreeMember)]
        public ResBaseBodyModel Update([FromBody] UserModel user)
        {
            return _userService.Update(user);
        }

        [APIAuthorizationAttribute(PolicyEnum.FreeMember)]
        public ResBaseBodyModel Delete([FromBody] UserModel user)
        {
            return _userService.Delete(user);
        }

        public ResNewBaseBodyModel<UserModel> Login([FromForm] UserModel user)
        {
            return _userService.Login(user);
        }

        public ResNewBaseBodyModel<UserModel> Logout([FromForm] UserModel user)
        {
            return _userService.Logout(user);
        }
    }
}
