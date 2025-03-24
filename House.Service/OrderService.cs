using Dapper;
using House.DAL.Dapper;
using House.Model;
using House.Model.API;
using House.Model.API.Order;
using House.Model.API.Product;
using House.Model.DB;
using House.Model.Enums;
using House.Model.Exceptions;
using House.Model.Extensions;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;

namespace House.Service
{
    public class OrderService : BaseService
    {
        private readonly OrderDao _orderDao;

        public OrderService(IOptions<AppSettings> appSettings,
            OrderDao orderDao) : base(appSettings)
        {
            _orderDao = orderDao;
        }

        public ResNewBaseBodyModel<List<OrderModel>> Query(ReqQueryOrderModel model)
        {
            var list = _orderDao.Query(model);
            return new ResNewBaseBodyModel<List<OrderModel>>(list);
        }

        public OrderModel Get(OrderModel model)
        {
            return _orderDao.Get(model.id);
        }

        public ResBaseBodyModel Create(ReqCreateOrderModel model)
        {
            //var res = new OrderModel();
            //model.CopyProp(res);
            //res.id = _orderDao.Create(model);
            //return res;
            var id = _orderDao.Create(model);
            if (id == 0)
                throw new APIException(ErrorCodeEnum.db_insert);

            return new ResBaseBodyModel();
        }

        public ResBaseBodyModel Update(OrderModel model)
        {
            if (_orderDao.Update(model) == 0)
                throw new APIException(ErrorCodeEnum.db_update);

            return new ResBaseBodyModel();
        }

        public ResBaseBodyModel Delete(OrderModel model)
        {
            if (_orderDao.Delete(model.id) == 0)
                throw new APIException(ErrorCodeEnum.db_delete);

            return new ResBaseBodyModel();
        }

        public ResNewBaseBodyModel<List<OrderDetailModel>> QueryOrderDetail(OrderModel model) 
        {
            var list = _orderDao.QueryOrderDetail(model);
            return new ResNewBaseBodyModel<List<OrderDetailModel>>(list);
        }
    }
}
