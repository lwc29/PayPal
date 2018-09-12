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
using CompanyPay;
namespace ZF.App
{
    /// <summary>
    /// 资金账户
    /// </summary>
    public class SmsApp : BaseApp<Sms>
    {
        public bool Valid(string code, string phone)
        { 
            var endTime = DateTime.Now.ToLocalTime().AddMinutes(-5);
            var sms =  Repository.GetWhere(n => n.CreateTime >= endTime && n.Tel == phone && n.Code == code).FirstOrDefault();
            if (sms != null && sms.IsSuc == true)
            {
                Task.Factory.StartNew(() =>
                {
                    sms.IsSuc = false;
                    Repository.Update(sms, true);
                });
                return true;
            }
            return false;
        }

        public Sms Send(Sms input)
        {
            CompanyPay.SmsResult result = null;
            if(input.Type == 1)
               result = CompanyPay.SmsTool.sendSms(input.Tel, input.Code, input.Code, "SMS_143718854");
            else //您的验证码${code}，该验证码5分钟内有效，请勿泄漏于他人！
               result = CompanyPay.SmsTool.sendSms(input.Tel, input.Code, input.Code, "SMS_143713969");
            input.IsSuc = result.IsSuc;
            input.Message =string.Format("【{0}】,{1}" ,result.Code,result.Message);
            input.BizId = result.BizId;
            return input;
        }
         
    }
}
