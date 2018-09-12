using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZF.Repository.Domain;
using Infrastructure;
using Infrastructure.Cache;
using System.Diagnostics;
using System.Web.Http;
using ZF.App.Request;
using ZF.App.Response;
using System.Data.SqlClient;
using System.IO;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using Infrastructure;
using System.Linq.Expressions;
using System.IO;
using System.Transactions;
using NPOI.HSSF.Util;
namespace ZF.App
{
    /// <summary>
    /// 体现审核
    /// </summary>
    public class TakeCheckApp : BaseApp<TakeCheck>
    {
        public IEnumerable<TakeCheckOut> GetTakeCheckList(StoreCondition input)
        {
            var sqlParamters = new List<SqlParameter>();
            sqlParamters.Add(new SqlParameter("@pageindex", input.Page));
            sqlParamters.Add(new SqlParameter("@pagesize", input.Limit));
            sqlParamters.Add(new SqlParameter("@name", input.Name ?? ""));
            sqlParamters.Add(new SqlParameter("@Start", input.Start ?? ""));
            sqlParamters.Add(new SqlParameter("@End", input.End ?? ""));

            var r = Repository.ExecuteQuery<TakeCheckOut>("EXEC sp_GetPageTakeCheck @pageindex,@pagesize,@name,@Start,@End", sqlParamters.ToArray());

            return r;
        }

        public void Update(TakeCheck input)
        {
            var model = Repository.Get(input.Id);
            if (model == null)
                throw new Exception("该记录不存在");

            if (model.ExportCount == 0)
                throw new Exception("该记录还未打款");
            Repository.Update(model, () => new TakeCheck() { ConfirmTime = DateTime.Now }, true);
        }

        public string ExportExcel(StoreCondition input, string path)
        {
            if (!Directory.Exists(path))
            { Directory.CreateDirectory(path); }

            var sqlParamters = new List<SqlParameter>();
            sqlParamters.Add(new SqlParameter("@name", input.Name ?? ""));
            sqlParamters.Add(new SqlParameter("@Start", input.Start ?? ""));
            sqlParamters.Add(new SqlParameter("@End", input.End ?? ""));

            var dt = Repository.ExecuteQuery<TakeCheckOut>("EXEC sp_GetExportTakeCheck @name,@Start,@End", sqlParamters.ToArray());

            //创建EXCEL工作薄
            HSSFWorkbook workBook = null;
            using (FileStream fs = File.Open(path + "/takelist.xls", FileMode.Open, System.IO.FileAccess.Read, FileShare.Read))
            {
                workBook = new HSSFWorkbook(fs);
                fs.Close();
            }

            //创建sheet文件表
            Sheet sheet = workBook.GetSheetAt(0);//workBook..CreateSheet("提现列表");

            int r = 0;
            Dictionary<string, object> dict = new Dictionary<string, object>();
            //创建表头
            if (dt.Count() > 0)
            {
                Expression<Func<TakeCheck>> properties = (() => new TakeCheck()
                {
                    Id = 0,
                    StoreName = "",
                    TakeOut = 0,
                    Account = "",
                    Bank = "",
                    BankNo = "",
                    CreateTime = DateTime.Now,
                    ExportCount = 0,
                    ConfirmTime = DateTime.Now
                });
                dict = properties.GetPropertyWithValue();
                //Cell cell = header.CreateCell(r);
                // cell.SetCellValue("头部");
                //foreach (var item in dict.Keys)
                //{
                //    string colunname = GetColumnName(item);
                //    if (colunname != "")
                //    {
                //        Cell cell = header.CreateCell(r);
                //        cell.SetCellValue(colunname);
                //        r++;
                //    }
                //}
            }
            else
            {
                Row header = sheet.CreateRow(2);
                Cell cell = header.CreateCell(0);
                cell.SetCellValue("数据为空");
            }



            #region 设置字体
            Font font = workBook.CreateFont();//创建字体样式
            font.Color = HSSFColor.RED.index;//设置字体颜色
            CellStyle style = workBook.CreateCellStyle();//创建单元格样式
            style.SetFont(font);//设置单元格样式中的字体样式
            #endregion

            Row row;

            Dictionary<string, string> di;
            //数据
            for (var i = 0; i < dt.Count(); i++)
            {
                row = sheet.CreateRow(i + 1);
                r = 0;
                TakeCheckOut model = dt.ElementAt(i);
                di = ExpressionHelper.GetProperties<TakeCheckOut>(model);
                foreach (var item in dict.Keys)
                {
                    var cell = row.CreateCell(r);
                    object obj = di[item];
                    if (obj != null)
                        cell.SetCellValue(obj.ToString());
                    else
                        cell.SetCellValue(string.Empty);
                    if (item == "ExportCount")
                    {
                        var v = 0;
                        if (int.TryParse(obj.ToString(), out v) && v > 0)
                            cell.CellStyle = style;//为单元格设置显示样式 
                    }
                    if (item == "ConfirmTime")
                        cell.CellStyle = style;
                    r++;
                }
            }

            //转为字节数组
            var stream = new MemoryStream();
            workBook.Write(stream);

            path += System.Guid.NewGuid().GetHashCode().ToString("x") + ".xls";
            using (FileStream fs = new FileStream(path, FileMode.Create, FileAccess.Write))
            {
                byte[] data = stream.ToArray();
                fs.Write(data, 0, data.Length);
                fs.Flush();
                data = null;
            }
            stream.Close();
            stream.Dispose();
            if (File.Exists(path))
            {
                Update(dt);
                return path;
            }
            else
                return string.Empty;

        }

        public void Update(IEnumerable<TakeCheckOut> dt)
        {
            if (dt.Count() > 0)
            {
                using (var tran = new TransactionScope())
                {
                    foreach (var item in dt)
                    {
                        UnitWork.RegisterDirty<TakeCheck>(item.MapTo<TakeCheck>(), () => new TakeCheck { ExportCount = item.ExportCount + 1 });
                    }
                    UnitWork.Commit();
                    tran.Complete();
                }

            }
        }


        public bool ImportExcel(Stream stream, out string msg)
        {
            HSSFWorkbook hssfworkbook = new HSSFWorkbook(stream);
            List<TakeCheck> list = new List<TakeCheck>();
            using (Sheet sheet = hssfworkbook.GetSheetAt(0))
            {
                TakeCheck takecheck;

                int r = 0;
                int v = 0;
                //总条目
                int rowCount = sheet.LastRowNum;
                Expression<Func<TakeCheck>> properties = (() => new TakeCheck()
                {
                    Id = 0,
                    StoreName = "",
                    Account = "",
                    TakeOut = 0,
                    Bank = "",
                    BankNo = ""
                });
                var dict = properties.GetPropertyWithValue();

                Row row;
                Dictionary<string, string> di = null;

                for (int i = sheet.FirstRowNum + 2; i <= rowCount; i++)
                {
                    row = sheet.GetRow(i);
                    if (row != null)
                    {
                        r = 0;
                        foreach (var item in dict.Keys)
                        {
                            Cell cell = row.GetCell(r);
                            if (cell != null)
                            {
                                if (item == "Id")
                                {
                                    if (int.TryParse(cell.ToString(), out v))
                                    {
                                        takecheck = Repository.Get(v);
                                        if (takecheck == null)
                                        {
                                            msg = "第" + i + "记录不存在";
                                            return false;
                                        }
                                        if (takecheck.ConfirmTime.HasValue)
                                            continue;

                                        list.Add(takecheck);
                                        di = ExpressionHelper.GetProperties<TakeCheck>(takecheck);
                                    }
                                    else
                                    {
                                        msg = "第" + i + "行序号有误";
                                        return false;
                                    }
                                }
                                else
                                {
                                    if (cell.ToString() != di[item])
                                    {
                                        msg = "第" + i + "行数据有误";
                                        return false;
                                    }

                                }
                            }
                            r++;
                        }
                    }
                }

            }

            if (list.Count > 0)
            {
                using (var tran = new TransactionScope())
                {
                    foreach (var item in list)
                    {
                        UnitWork.RegisterDirty(item, () => new TakeCheck { ConfirmTime = DateTime.Now });
                    }
                    UnitWork.Commit();
                    tran.Complete();
                }
            }
            msg = "ok";
            return true;
        }

        /// <summary>
        /// 根据字段名称获取对应的列名
        /// </summary>
        //public string GetColumnName(string columnName)
        //{
        //    var convertColumnName = string.Empty;
        //    switch (columnName)
        //    {
        //        case "StoreName":
        //            convertColumnName = "商家名称";
        //            break;
        //        case "TakeOut":
        //            convertColumnName = "提现金额";
        //            break;
        //        case "BankNo":
        //            convertColumnName = "银行卡号";
        //            break;
        //        case "Account":
        //            convertColumnName = "姓名";
        //            break;
        //        case "Bank":
        //            convertColumnName = "开户行";
        //            break;
        //        case "CreateTime":
        //            convertColumnName = "开户时间";
        //            break;
        //        case "ExportCount":
        //            convertColumnName = "导出次数";
        //            break;
        //    }
        //    return convertColumnName;
        //}

    }
}
