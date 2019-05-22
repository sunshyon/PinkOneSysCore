using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aspose.Cells;

namespace Utility
{
    public class ExcelHelper
    {
        /// <summary>
        /// 获取第一张sheet
        /// </summary>
        public static DataTable ImportExcel(string filePath)
        {
            DataTable dt = null;

            Workbook workbook = new Workbook(filePath);

            //获取 sheet 表
            WorksheetCollection wsc = workbook.Worksheets;

            Worksheet worksheet = null;

            Cells cells = null;

            int rowIndex = 0;   //起始行
            int colIndex = 0;   //起始列

            if (wsc.Count > 0)
            {
                for (var i = 0; i < wsc.Count; i++)
                {
                    dt = new DataTable();
                    dt.TableName = "table" + i.ToString();

                    worksheet = wsc[i];

                    cells = worksheet.Cells;

                    dt = cells.ExportDataTableAsString(rowIndex, colIndex, cells.MaxDataRow + 1, cells.MaxDataColumn + 1, true);

                    break;
                }
            }
            wsc.Clear();

            worksheet = null;
            wsc = null;
            workbook = null;

            return dt;
        }

        /// <summary>
        /// 获取所有sheet
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static DataSet ImportExcelFromAllSheet(string filePath)
        {
            DataSet ds = null;
            DataTable dt = null;

            Workbook workbook = new Workbook(filePath);

            //获取 sheet 表
            WorksheetCollection wsc = workbook.Worksheets;

            Worksheet worksheet = null;

            Cells cells = null;

            ds = new DataSet();

            int rowIndex = 0;   //起始行
            int colIndex = 0;   //起始列

            for (int i = 0; i < wsc.Count; i++)
            {
                dt = new DataTable();
                dt.TableName = "table" + i.ToString();

                worksheet = wsc[i];

                //获取每个 sheet 表的所有单元格
                cells = worksheet.Cells;

                dt = cells.ExportDataTableAsString(rowIndex, colIndex, cells.MaxDataRow + 1, cells.MaxDataColumn + 1, true);

                ds.Tables.Add(dt);
            }

            wsc.Clear();

            worksheet = null;
            wsc = null;
            workbook = null;

            return ds;
        }
    }
}
