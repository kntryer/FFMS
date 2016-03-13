using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using Microsoft.Win32;
using System.Windows.Forms;

namespace FFMS
{
    /// <summary> 
    /// DataTable导出到Excel 
    /// 整理:dongVi 
    /// </summary> 
    public class ExportExcel
    {
        private ExportExcel()
        {

        }
        /// <summary> 
        /// 导出Excel 
        /// </summary> 
        /// <param name="dt">要导出的DataTable</param> 
        public static void ExportToExcel(System.Windows.Controls.DataGrid dataGrid)
        {

            System.Data.DataTable dt = ((DataView)dataGrid.ItemsSource).Table;

            if (dt == null) return;

            Microsoft.Office.Interop.Excel.Application xlApp = new Microsoft.Office.Interop.Excel.Application();
            if (xlApp == null)
            {
                // lblMsg.Text = "无法创建Excel对象，可能您的电脑未安装Excel"; 
                System.Windows.MessageBox.Show("无法创建Excel对象，可能您的电脑未安装Excel");
                return;
            }
            Microsoft.Win32.SaveFileDialog saveDia = new Microsoft.Win32.SaveFileDialog();
            saveDia.Filter = "Excel|*.xlsx";
            saveDia.Title = "导出为Excel文件";
            if (saveDia.ShowDialog() == true && !string.Empty.Equals(saveDia.FileName))
            {
                Microsoft.Office.Interop.Excel.Workbooks workbooks = xlApp.Workbooks;
                Microsoft.Office.Interop.Excel.Workbook workbook = workbooks.Add(Microsoft.Office.Interop.Excel.XlWBATemplate.xlWBATWorksheet);
                Microsoft.Office.Interop.Excel.Worksheet worksheet = (Microsoft.Office.Interop.Excel.Worksheet)workbook.Worksheets[1];//取得sheet1 
                Microsoft.Office.Interop.Excel.Range range = null;
                long totalCount = dt.Rows.Count;
                long rowRead = 0;
                float percent = 0;
                string fileName = saveDia.FileName;
                //写入标题 
                for (int i = 0; i < dataGrid.Columns.Count - 1; i++)
                {
                    //worksheet.Cells[1, i + 1] = dt.Columns[i].ColumnName;
                    worksheet.Cells[1, i + 1] = dataGrid.Columns[i].Header.ToString();
                    range = (Microsoft.Office.Interop.Excel.Range)worksheet.Cells[1, i + 1];

                    //range.Interior.ColorIndex = 15;//背景颜色 
                    range.Font.Bold = true;//粗体 
                    range.Font.Size = 14;//字体大小 
                    range.HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;//居中 
                    //加边框 
                    //range.BorderAround(Microsoft.Office.Interop.Excel.XlLineStyle.xlContinuous, Microsoft.Office.Interop.Excel.XlBorderWeight.xlThin, Microsoft.Office.Interop.Excel.XlColorIndex.xlColorIndexAutomatic, null);
                    //range.ColumnWidth = 4.63;//设置列宽 
                    range.EntireColumn.AutoFit();//自动调整列宽 
                    range.EntireRow.AutoFit();//自动调整行高 
                }
                //写入内容 
                for (int r = 0; r < dt.DefaultView.Count; r++)
                {
                    for (int i = 0; i < dt.Columns.Count; i++)
                    {
                        worksheet.Cells[r + 2, i + 1] = dt.DefaultView[r][i];
                        range = (Microsoft.Office.Interop.Excel.Range)worksheet.Cells[r + 2, i + 1];
                        range.Font.Size = 13;//字体大小 
                        range.HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;//居中 
                        //加边框 
                        //range.BorderAround(Microsoft.Office.Interop.Excel.XlLineStyle.xlContinuous, Microsoft.Office.Interop.Excel.XlBorderWeight.xlThin, Microsoft.Office.Interop.Excel.XlColorIndex.xlColorIndexAutomatic, null);
                        range.EntireColumn.AutoFit();//自动调整列宽 
                    }
                    rowRead++;
                    percent = ((float)(100 * rowRead)) / totalCount;
                    System.Windows.Forms.Application.DoEvents();
                }
                range.Borders[Microsoft.Office.Interop.Excel.XlBordersIndex.xlInsideHorizontal].Weight = Microsoft.Office.Interop.Excel.XlBorderWeight.xlThin;
                if (dt.Columns.Count > 1)
                {
                    range.Borders[Microsoft.Office.Interop.Excel.XlBordersIndex.xlInsideVertical].Weight = Microsoft.Office.Interop.Excel.XlBorderWeight.xlThin;
                }
                try
                {
                    workbook.Saved = true;
                    workbook.SaveCopyAs(fileName);
                }
                catch (Exception ex)
                {
                    //lblMsg.Text = "导出文件时出错,文件可能正被打开！\n" + ex.Message; 
                    System.Windows.MessageBox.Show("导出文件时出错,文件可能正被打开！\n" + ex.Message);
                    return;
                }

                workbooks.Close();
                if (xlApp != null)
                {
                    xlApp.Workbooks.Close();
                    xlApp.Quit();
                    int generation = System.GC.GetGeneration(xlApp);
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(xlApp);
                    xlApp = null;
                    System.GC.Collect(generation);
                }
                GC.Collect();//强行销毁 
                #region 强行杀死最近打开的Excel进程
                System.Diagnostics.Process[] excelProc = System.Diagnostics.Process.GetProcessesByName("EXCEL");
                System.DateTime startTime = new DateTime();
                int m, killId = 0;
                for (m = 0; m < excelProc.Length; m++)
                {
                    if (startTime < excelProc[m].StartTime)
                        if (startTime < excelProc[m].StartTime)
                        {
                            startTime = excelProc[m].StartTime;
                            killId = m;
                        }
                }
                if (excelProc[killId].HasExited == false)
                {
                    excelProc[killId].Kill();
                }
                #endregion
                System.Windows.MessageBox.Show("导出成功!");
            }
        }
    }
}
