using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZF.Repository.Domain;
using Infrastructure;
using Infrastructure.Cache;
using System.Diagnostics;
namespace ZF.App
{
    public class AreaApp : BaseApp<Area>
    {
        public static IEnumerable<Area> list { get;private set; }
        public void GetAll()
        {
            
            IEnumerable<Area> areaList = Repository.ExecuteQuery<Area>("select * from Area where level < 4", new string[] { });
            //Stopwatch sw = new Stopwatch();
            //sw.Start();
            //省份
            IEnumerable<Area> newList = areaList.Where(r => r.Level == 1);
            foreach (var item in newList)
            {
                //市
                item.Childrens.AddRange(areaList.Where(r => r.Level == 2 && r.Parent_Id == item.Id));
                foreach (var area in item.Childrens)
                {
                    //区域
                     area.Childrens.AddRange(areaList.Where(r => r.Level == 3 && r.Parent_Id == area.Id));
                     //foreach (var jiedao in area.Childrens)
                     //{
                     //    //街道
                     //    jiedao.Childrens.AddRange(areaList.Where(r => r.Level == 4 && r.Parent_Id == jiedao.Id));
                     //}
                }
            }
            //sw.Stop();
            //long ticks = sw.ElapsedTicks;
            list = newList;
        }

        //提供整个省份数据
        public IEnumerable<Area> GetProvince(int id)
        {
            if (id == 0)
                return Repository.GetWhere(r => r.Level == 1);
            var province = list.Where(r => r.Id == id);
            province = province.FirstOrDefault().Childrens;
            return province;
        }
    }
}
