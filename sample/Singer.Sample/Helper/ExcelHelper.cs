using NPOI.HSSF.UserModel;
using NPOI.HSSF.Util;
using NPOI.SS.UserModel;
using Singer.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Singer.Sample.Helper
{
    /// <summary>
    /// Excel操作帮助类
    /// </summary>
    public class ExcelHelper
    {
        /// <summary> Excel样式 </summary>
        public enum XlsStyle
        {
            /// <summary> 默认 </summary>
            Default,
            /// <summary> 表头 </summary>
            Header,
            /// <summary> 链接 </summary>
            Url,
            /// <summary> 日期 </summary>
            DateTime,
            /// <summary> 数字 </summary>
            Number,
            /// <summary> 金额 </summary>
            Money,
            /// <summary> 百分比 </summary>
            Percent
        }

        #region 打开excel文件选择框
        public static string OpenExcelFileDialog()
        {
            var ofd = new Microsoft.Win32.OpenFileDialog()
            {
                DefaultExt = "xls",
                Filter = "excel files(*.xls)|*.xls|All files(*.*)|*.*",
                FilterIndex = 1
            };

            if (ofd.ShowDialog() != true)
                return null;
            return ofd.FileName;
        }
        #endregion

        #region Excel导入(读取Excel数据到DataTable)
        public static DResult<DataTable> ImportExcelFile(string filepath)
        {
            DataTable dt = new DataTable();
            HSSFWorkbook hssfworkbook = null;

            #region//初始化信息
            try
            {
                using (var file = new FileStream(filepath, FileMode.Open, FileAccess.Read))
                {
                    hssfworkbook = new HSSFWorkbook(file);
                }
            }
            catch (Exception)
            {
                return new DResult<DataTable>("Excel文件异常");
            }
            #endregion

            #region //读取Excel到Dt
            var sheet = hssfworkbook.GetSheetAt(0);
            System.Collections.IEnumerator rows = sheet.GetRowEnumerator();

            for (int j = 0; j < (sheet.GetRow(0).LastCellNum); j++)
            {
                dt.Columns.Add(Convert.ToChar(((int)'A') + j).ToString());
            }
            while (rows.MoveNext())
            {
                HSSFRow row = (HSSFRow)rows.Current;
                DataRow dr = dt.NewRow();
                for (int i = 0; i < row.LastCellNum; i++)
                {
                    var cell = row.GetCell(i);
                    if (cell == null)
                    {
                        dr[i] = "";
                    }
                    else
                    {
                        if (cell.CellType == NPOI.SS.UserModel.CellType.Numeric)
                        {
                            if (HSSFDateUtil.IsCellDateFormatted(cell))
                            {
                                dr[i] = cell.DateCellValue;
                            }
                            else
                            {
                                dr[i] = cell.NumericCellValue;
                            }
                        }
                        else if (cell.CellType == NPOI.SS.UserModel.CellType.Boolean)
                        {
                            dr[i] = cell.BooleanCellValue;
                        }
                        else
                        {
                            dr[i] = cell.StringCellValue;
                        }
                    }
                }
                dt.Rows.Add(dr);
            }
            #endregion

            return new DResult<DataTable>(dt) { Message = filepath };
        }
        #endregion

        #region 保存excel文件选择框
        public static string SaveExcelFileDialog(string excelName)
        {
            var sfd = new Microsoft.Win32.SaveFileDialog()
            {
                FileName = excelName,
                DefaultExt = "xls",
                Filter = "excel files(*.xls)|*.xls|All files(*.*)|*.*",
                FilterIndex = 1
            };


            if (sfd.ShowDialog() != true)
                return null;


            return sfd.FileName;
        }
        #endregion

        #region Excel导出
        /// <summary> 创建workbook样式 </summary>
        /// <param name="wb"></param>
        /// <param name="str"></param>
        /// <returns></returns>
        public static ICellStyle GetCellStyle(IWorkbook wb, XlsStyle str)
        {
            var cellStyle = wb.CreateCellStyle();
            //定义几种字体  
            //也可以一种字体，写一些公共属性，然后在下面需要时加特殊的  
            var heaerFont = wb.CreateFont();
            heaerFont.FontHeightInPoints = 12;
            heaerFont.Boldweight = 600;
            heaerFont.FontName = "微软雅黑";

            var font = wb.CreateFont();
            font.FontHeightInPoints = 10;
            font.FontName = "微软雅黑";

            var fontcolorblue = wb.CreateFont();
            fontcolorblue.Color = HSSFColor.Black.Index;
            fontcolorblue.IsItalic = true; //下划线  
            fontcolorblue.FontName = "微软雅黑";
            //边框  
            cellStyle.BorderBottom =
                cellStyle.BorderLeft = cellStyle.BorderRight = cellStyle.BorderTop = BorderStyle.Thin;
            //边框颜色  
            cellStyle.BottomBorderColor = HSSFColor.Black.Index;
            cellStyle.TopBorderColor = HSSFColor.Black.Index;

            //背景图形，我没有用到过。感觉很丑 
            cellStyle.FillForegroundColor = HSSFColor.White.Index;
            cellStyle.FillBackgroundColor = HSSFColor.Blue.Index;

            //水平对齐  
            cellStyle.Alignment = HorizontalAlignment.Left;

            //垂直对齐  
            cellStyle.VerticalAlignment = VerticalAlignment.Center;

            //自动换行  
            //cellStyle.WrapText = true;

            //缩进;当设置为1时，前面留的空白太大了。希旺官网改进。或者是我设置的不对  
            cellStyle.Indention = 0;

            //上面基本都是设共公的设置  
            //下面列出了常用的字段类型  
            switch (str)
            {
                case XlsStyle.Header:
                    // cellStyle.FillPattern = FillPatternType.LEAST_DOTS; 
                    cellStyle.FillForegroundColor = 41;
                    cellStyle.FillPattern = FillPattern.SolidForeground;
                    cellStyle.Alignment = HorizontalAlignment.Center;
                    cellStyle.SetFont(heaerFont);
                    break;
                case XlsStyle.DateTime:
                    var datastyle = wb.CreateDataFormat();

                    cellStyle.DataFormat = datastyle.GetFormat("yyyy/mm/dd");
                    cellStyle.SetFont(font);
                    break;
                case XlsStyle.Number:
                    cellStyle.DataFormat = HSSFDataFormat.GetBuiltinFormat("0.00");
                    cellStyle.SetFont(font);
                    break;
                case XlsStyle.Money:
                    var format = wb.CreateDataFormat();
                    cellStyle.DataFormat = format.GetFormat("￥#,##0");
                    cellStyle.SetFont(font);
                    break;
                case XlsStyle.Url:
                    fontcolorblue.Underline = FontUnderlineType.Single;
                    cellStyle.SetFont(fontcolorblue);
                    break;
                case XlsStyle.Percent:
                    cellStyle.DataFormat = HSSFDataFormat.GetBuiltinFormat("0.00%");
                    cellStyle.SetFont(font);
                    break;
                //case XlsStyle.中文大写:
                //    IDataFormat format1 = wb.CreateDataFormat();
                //    cellStyle.DataFormat = format1.GetFormat("[DbNum2][$-804]0");
                //    cellStyle.SetFont(font);
                //    break;
                //case XlsStyle.科学计数法:
                //    cellStyle.DataFormat = HSSFDataFormat.GetBuiltinFormat("0.00E+00");
                //    cellStyle.SetFont(font);
                //    break;
                case XlsStyle.Default:
                    cellStyle.SetFont(font);
                    break;
            }
            return cellStyle;
        }
        private static bool IsNumber(object value)
        {
            if (value == null) return false;
            return
                new[] { typeof(int), typeof(double), typeof(long), typeof(decimal), typeof(float), typeof(byte) }.Contains
                    (value.GetType());
        }

        public static bool WriteExcel(DataTable dt, string filepath)
        {
            if (string.IsNullOrEmpty(filepath) || null == dt)
                return true;
            var book = new HSSFWorkbook();
            var sheet = book.CreateSheet("Sheet1");

            var headerStyle = GetCellStyle(book, XlsStyle.Header);
            var style = GetCellStyle(book, XlsStyle.Default);

            var row = sheet.CreateRow(0);
            for (var i = 0; i < dt.Columns.Count; i++)
            {
                sheet.AutoSizeColumn(i);
                var cell = row.CreateCell(i);
                cell.CellStyle = headerStyle;
                cell.SetCellValue(dt.Columns[i].ColumnName);
            }

            for (var i = 0; i < dt.Rows.Count; i++)
            {
                row = sheet.CreateRow(i + 1);
                var cellItems = dt.Rows[i].ItemArray;
                for (var j = 0; j < dt.Columns.Count; j++)
                {
                    if (cellItems.Length <= j)
                        break;
                    var value = cellItems[j];
                    if (IsNumber(value))
                    {
                        var cell = row.CreateCell(j, CellType.Numeric);
                        cell.CellStyle = style;
                        cell.SetCellValue(value == null ? 0D : Convert.ToDouble(value));
                    }
                    else
                    {
                        var cell = row.CreateCell(j);
                        cell.CellStyle = style;
                        cell.SetCellValue((value ?? string.Empty).ToString());
                    }
                }
            }
            // 写入到客户端  
            using (var ms = new MemoryStream())
            {
                book.Write(ms);
                using (var fs = new FileStream(filepath, FileMode.Create, FileAccess.Write))
                {
                    var data = ms.ToArray();
                    fs.Write(data, 0, data.Length);
                    fs.Flush();
                }
            }
            return true;
        }

        #endregion

        #region list转datatable
        public static DataTable ListToDataTable<T>(IEnumerable<T> c)
        {
            var props = typeof(T).GetProperties();
            var dt = new DataTable();
            dt.Columns.AddRange(props.Select(p => new DataColumn(p.Name, p.PropertyType)).ToArray());
            if (c.Count() > 0)
            {
                for (int i = 0; i < c.Count(); i++)
                {
                    ArrayList tempList = new ArrayList();
                    foreach (PropertyInfo item in props)
                    {
                        object obj = item.GetValue(c.ElementAt(i), null);
                        tempList.Add(obj);
                    }
                    dt.LoadDataRow(tempList.ToArray(), true);
                }
            }
            return dt;
        }
        #endregion  
    }
}
