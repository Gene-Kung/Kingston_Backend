using House.DAL.Dapper;
using House.Model;
using House.Model.API;
using House.Model.API.Product;
using House.Model.DB;
using House.Model.Enums;
using House.Model.Exceptions;
using House.Model.Extensions;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace House.Service
{
    public class ProductService : BaseService
    {
        private ProductDao _productDao;

        public ProductService(IOptions<AppSettings> appSettings, ProductDao productDao) : base(appSettings)
        {
            _productDao = productDao;
        }

        public ResNewBaseBodyModel<List<ProductModel>> Query(ReqQueryProductModel model)
        {
            var list = _productDao.Query(model);
            return new ResNewBaseBodyModel<List<ProductModel>>(list);
        }

        public ProductModel Get(ProductModel productModel)
        {
            return _productDao.Get(productModel.id);
        }

        public ResBaseBodyModel Create(ReqCreateProductModel model)
        {
            var productModel = new ProductModel();
            model.CopyProp<ProductModel>(productModel);
            productModel.id = _productDao.Create(productModel);
            if (productModel.id == 0)
                throw new APIException(ErrorCodeEnum.db_insert);
            return new ResBaseBodyModel();
        }

        public ResBaseBodyModel Update(ProductModel productModel)
        {
            if (_productDao.Update(productModel) == 0)
                throw new APIException(ErrorCodeEnum.db_update);

            return new ResBaseBodyModel();
        }

        public ResBaseBodyModel Delete(ProductModel productModel)
        {
            if (_productDao.Delete(productModel.id) == 0)
                throw new APIException(ErrorCodeEnum.db_delete);

            return new ResBaseBodyModel();
        }
    }
}
