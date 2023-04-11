using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreAPIs.Models
{
    public class ResultModel
    {
        public int code { get; set; } = 500;

        /// <summary>
        /// 信息
        /// </summary>
        public string msg { get; set; }

        /// <summary>
        /// 详情
        /// </summary>
        public string detail { get; set; }

        /// <summary>
        /// 时间戳
        /// </summary>
        public string timestamp { get; set; } = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
    }
}
