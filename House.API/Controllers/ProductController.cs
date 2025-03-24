using House.API.Filter;
using House.Model;
using House.Model.API;
using House.Model.API.Product;
using House.Model.DB;
using House.Model.Enums;
using House.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Web.Mvc;

namespace House.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ProductController : BaseController
    {
        private readonly ProductService _productService;

        public ProductController(
            IHttpContextAccessor accessor,
            IOptions<AppSettings> appSettings,
            ProductService productService) : base(accessor, appSettings)
        {
            _productService = productService;
        }

        public ResNewBaseBodyModel<List<ProductModel>> Query([FromBody] ReqQueryProductModel model) 
        {
            return _productService.Query(model);
        }

        public ProductModel Get([FromBody] ProductModel model) 
        {
            return _productService.Get(model);
        }

        [APIAuthorizationAttribute(PolicyEnum.FreeMember)]
        public ResBaseBodyModel Create([FromBody] ReqCreateProductModel model)
        {
            return _productService.Create(model);
        }

        [APIAuthorizationAttribute(PolicyEnum.FreeMember)]
        public ResBaseBodyModel Update([FromBody] ProductModel model)
        {
            return _productService.Update(model);
        }

        [APIAuthorizationAttribute(PolicyEnum.FreeMember)]
        public ResBaseBodyModel Delete([FromBody] ProductModel model)
        {
            return _productService.Delete(model);
        }
    }
}
