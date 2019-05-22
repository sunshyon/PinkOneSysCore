using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utility
{
    public class AdoNetHelper
    {
        private string _connStr = "";
        private SqlConnection _sCnn;
        public AdoNetHelper(string connStr)
        {
            _connStr = connStr;
            _sCnn = new SqlConnection(_connStr);
        }
        /// <summary>
        /// 执行增、删、改语句
        /// </summary>
        public int ExecuteNonQueryCmd(string sql)
        {
            SqlCommand Scmd = new SqlCommand(sql, _sCnn);
            _sCnn.Open();
            int res = Scmd.ExecuteNonQuery();
            _sCnn.Close();
            return res;
        }

        /// <summary>
        /// 执行查询命令语句
        /// </summary>
        public SqlDataReader ExecuteReaderCmd(string sql)
        {
            SqlCommand Scmd = new SqlCommand(sql, _sCnn);
            _sCnn.Open();
            return Scmd.ExecuteReader(CommandBehavior.CloseConnection);
        }

      
    }
}
