using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using House.Service;
using House.API.Filter;
using Microsoft.Extensions.Options;
using House.Model;
using House.Model.API.Order;
using House.Model.DB;
using House.Model.API.Product;
using System.Collections.Generic;
using House.Model.API;
using House.Model.Enums;

namespace House.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class OrderController : BaseController
    {
        private readonly OrderService _orderService;

        public OrderController(
            IHttpContextAccessor accessor,
            IOptions<AppSettings> appSettings,
             OrderService orderService
            ) : base(accessor, appSettings)
        {
            _orderService = orderService;
        }

        
        public ResNewBaseBodyModel<List<OrderModel>> Query([FromBody] ReqQueryOrderModel model)
        {
            return _orderService.Query(model);
        }

        public OrderModel Get([FromBody] OrderModel model)
        {
            return _orderService.Get(model);
        }

        [APIAuthorizationAttribute(PolicyEnum.FreeMember)]
        public ResBaseBodyModel Create([FromBody] ReqCreateOrderModel model)
        {
            return _orderService.Create(model);
        }

        [APIAuthorizationAttribute(PolicyEnum.FreeMember)]
        public ResBaseBodyModel Update([FromBody] OrderModel model)
        {
            return _orderService.Update(model);
        }

        [APIAuthorizationAttribute(PolicyEnum.FreeMember)]
        public ResBaseBodyModel Delete([FromBody] OrderModel model)
        {
            return _orderService.Delete(model);
        }

        public ResNewBaseBodyModel<List<OrderDetailModel>> QueryOrderDetail([FromBody] OrderModel model) 
        {
            return _orderService.QueryOrderDetail(model);
        }
    }
}
