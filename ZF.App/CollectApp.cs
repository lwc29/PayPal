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
namespace ZF.App
{
    //收款账号
    public class CollectApp : BaseApp<Collect>
    {
       public StoreApp _app { get; set; }
        
        //各商家手续费设置
       public bool Update(CollectIn input)
        {
            var config = _app.Get(input.Id);
            if (config == null)
                throw new Exception("当前数据不存在");
            //手续费设置
            if (input.ALIPer > 0 || input.WXPer > 0 || input.BANKPer > 0 || input.Type > 0)
            {
                //if (config.CollectId == 0)
                //{
                //    Collect collect = new Collect() { Account = input.Account, WX = "", ALI ="", BANK = "", BANKType = "", CreateTime = DateTime.Now };
                //    if (input.ALIPer > 0)
                //    {
                //        collect.ALIPer = input.ALIPer;
                //    }
                //    else if (input.WXPer > 0)
                //    {
                //        collect.WXPer = input.WXPer;
                //    }
                //    else if (input.BANKPer> 0 && !string.IsNullOrEmpty(input.BANKType))
                //    {
                //        collect.BANKPer = input.BANKPer;
                //    }
                //    else
                //        return false;

                //    Repository.Insert(collect, true);
                //    config.CollectId = collect.Id;
                //    _app.Update(config);
                //}
                //else
                //{
                    var model = Repository.Get(config.CollectId);
                    if (config == null)
                        throw new Exception("当前数据不存在");
                    if (input.ALIPer > 0 || input.Type == 1)
                    {
                        Repository.Update(r => r.Id == model.Id, r => new Collect() { Account = input.Account, ALIPer = input.ALIPer });
                    }
                    else if (input.WXPer > 0 || input.Type == 2)
                    {
                        Repository.Update(r => r.Id == model.Id, r => new Collect() { Account = input.Account, WXPer = input.WXPer });
                    }
                    else if (input.BANKPer > 0 || input.Type == 3)
                    {
                        Repository.Update(r => r.Id == model.Id, r => new Collect() { Account = input.Account, BANKPer = input.BANKPer });
                    }
                    else
                        return false;
 
                //}
            }//账号设置
            else  
            {
                //if (config.CollectId == 0)
                //{
                //    Collect collect = new Collect() { Account = input.Account, WX = "", ALI = "", BANK = "", BANKType = "", CreateTime = DateTime.Now };
                //    if (!string.IsNullOrEmpty(input.WX))
                //    {
                //        collect.WX = input.WX;
                //    }
                //    else if (!string.IsNullOrEmpty(input.ALI))
                //    {
                //        collect.ALI = input.ALI;
                //    }
                //    else if (!string.IsNullOrEmpty(input.BANK) && !string.IsNullOrEmpty(input.BANKType))
                //    {
                //        collect.BANK = input.BANK;
                //        collect.BANKType = input.BANKType;
                //    }
                //    else
                //        return false;

                //    Repository.Insert(collect, true);
                //    config.CollectId = collect.Id;
                //    _app.Update(config);
                //}
                //else
                //{
                    var model = Repository.Get(config.CollectId);
                    if (config == null)
                        throw new Exception("当前数据不存在");
 
                    if (!string.IsNullOrEmpty(input.WX))
                    {
                        Repository.Update(r => r.Id == model.Id, r => new Collect() { Account = input.Account, WX = input.WX });
                    }
                    else if (!string.IsNullOrEmpty(input.ALI))
                    {
                        Repository.Update(r => r.Id == model.Id, r => new Collect() { Account = input.Account, ALI = input.ALI });
                    }
                    else if (!string.IsNullOrEmpty(input.BANK) && !string.IsNullOrEmpty(input.BANKType))
                    {
                        Repository.Update(r => r.Id == model.Id, r => new Collect() { Account = input.Account, BANKType = input.BANKType, BANK = input.BANK });
                    }
                    else
                        return false;
 
                //}
            }
 
            return true;
        }
         
    }
}
