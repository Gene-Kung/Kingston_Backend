using House.DAL.Dapper;
using House.Model;
using House.Model.DB;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using House.Model.API;
using House.Model.Enums;
using House.Model.Exceptions;
using House.CBL;
using House.Model.Extensions;

namespace House.Service
{
    public class UserService : BaseService
    {
        private UserDao _userDao;
        private readonly SecurityService _securityService;

        public UserService(IOptions<AppSettings> appSettings, SecurityService securityService, UserDao userDao) : base(appSettings)
        {
            _userDao = userDao;
            _securityService = securityService;
        }

        public ResNewBaseBodyModel<List<UserModel>> Query(UserModel userModel)
        {
            var list = _userDao.Query(userModel.email, userModel.name);
            return new ResNewBaseBodyModel<List<UserModel>>(list);
        }

        public UserModel Get(UserModel userModel)
        {
            return _userDao.Get(userModel.id);
        }

        public ResBaseBodyModel Create(ReqCreateUserModel model)
        {
            var userModel = new UserModel();
            model.CopyProp<UserModel>(userModel);
            var defaultPwd = "123456";
            userModel.password = defaultPwd.HashPassword();
            userModel.id = _userDao.Create(userModel);
            if (userModel.id == 0)
                throw new APIException(ErrorCodeEnum.db_insert);
            return new ResBaseBodyModel();
        }

        public ResBaseBodyModel Update(UserModel userModel)
        {
            if (_userDao.Update(userModel) == 0)
                throw new APIException(ErrorCodeEnum.db_update);
         
            return new ResBaseBodyModel();
        }

        public ResBaseBodyModel Delete(UserModel userModel)
        {
            if (_userDao.Delete(userModel.id) == 0)
                throw new APIException(ErrorCodeEnum.db_delete);

            return new ResBaseBodyModel();
        }

        public ResNewBaseBodyModel<UserModel> Login(UserModel model)
        {
            var user = _userDao.Login(model.email);
            
            if (user == null) 
                throw new APIException(ErrorCodeEnum.not_found_email);

            if (user.password != model.password.HashPassword())
                throw new APIException(ErrorCodeEnum.pwd);

            user.token = GenToken(user.id);

            if (_userDao.Update(user) == 0)
                throw new APIException(ErrorCodeEnum.db_update);

            return new ResNewBaseBodyModel<UserModel>(user);
        }

        public ResNewBaseBodyModel<UserModel> Logout(UserModel model)
        {
            var user = _userDao.Login(model.email);

            if (user == null)
                throw new APIException(ErrorCodeEnum.not_found_email);

            if (user.password != model.password.HashPassword())
                throw new APIException(ErrorCodeEnum.pwd);

            return new ResNewBaseBodyModel<UserModel>(user);
        }

        public string GenToken(long memberID, List<string> roleIDs = null)
        {
            if(roleIDs == null)
                roleIDs = new List<string>() { Role.FreeMember.GetHashCodeString() };

            return _securityService.GenToken(memberID, roleIDs);
        }
    }
}
