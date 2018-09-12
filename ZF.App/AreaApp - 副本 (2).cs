using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZF.Repository.Domain;
using Infrastructure;
using Infrastructure.Cache;
using System.Diagnostics;
using Newtonsoft.Json;
namespace ZF.App
{
    public class Area2
    {
        public Area2() {
            childs = new List<Area2>();
        }

        public string code { get; set; }

        public string name { get; set; }
        public List<Area2> childs { get; set; }
    }
    public class AreaApp : BaseApp<Area>
    {
        public static IEnumerable<Area2> list2 { get; private set; }
        public static IEnumerable<Area> list { get;private set; }
        public void GetAll()
        {
           var areaList = Repository.GetAll();
           // Stopwatch sw = new Stopwatch();
           // sw.Start();
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
           // sw.Stop();
            //long ticks = sw.ElapsedTicks;
            
            list = newList;
           var bb= com(newList);
           var str = JsonConvert.SerializeObject(bb);

        }
        List<Area2> newList = new List<Area2>();
        private IEnumerable<Area2> com(IEnumerable<Area> list)
        {
          
            foreach (var item in list)
            {
                Area2 area = new Area2() { code = item.Id.ToString(), name = item.Area_name };
                
                if (item.Childrens.Count > 0)
                {
                    //省
                    List<Area2> area1List = new List<Area2>();
                    foreach (var item2 in item.Childrens)
                    {
                        //市
                          Area2 area2 = new Area2() { code = item2.Id.ToString(), name = item2.Area_name };
                          List<Area2> area2List = new List<Area2>();
                          if (item2.Childrens.Count > 0)
                         {
                             foreach (var item3 in item2.Childrens)
                             {
                                 Area2 area3 = new Area2() { code = item3.Id.ToString(), name = item3.Area_name };
                                 area2List.Add(area3);
                             }
                             area2.childs.AddRange(area2List);
                         }
                          area1List.Add(area2);
                    }
                    area.childs.AddRange(area1List);
                }
                newList.Add(area);
            }
            return newList;
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
