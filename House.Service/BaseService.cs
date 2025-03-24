using House.DAL;
using House.Model;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace House.Service
{
    public class BaseService
    {
        protected readonly Logger _logger;
        protected readonly AppSettings _appSettings;

        public BaseService(IOptions<AppSettings> appSettings) 
        {
            _appSettings = appSettings.Value;

            //Log 寫入檔案
            //_logger = LogManager.GetLogger("InsertFile");

            //Log 寫入資料庫
            _logger = LogManager.GetLogger("insert_info_log");
        }
    }
}
