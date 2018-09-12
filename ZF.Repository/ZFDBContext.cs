using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using ZF.Repository.Domain;
using ZF.Repository.Mapping;

namespace ZF.Repository
{
    public class ZFDBContext : DbContext
    {
        static ZFDBContext()
        {
            Database.SetInitializer<ZFDBContext>(null);
        }

        public ZFDBContext()
            : base("ZF")
        {
            // 关闭语义可空判断
            //Configuration.UseDatabaseNullSemantics = true;

            //与变量的值为null比较
            //ef判断为null的时候，不能用变量比较：https://stackoverflow.com/questions/682429/how-can-i-query-for-null-values-in-entity-framework?utm_medium=organic&utm_source=google_rich_qa&utm_campaign=google_rich_qa
            (this as IObjectContextAdapter).ObjectContext.ContextOptions.UseCSharpNullComparisonBehavior = true;
            Database.Log = s => System.Diagnostics.Debug.WriteLine(s);
        }


        public ZFDBContext(string nameOrConnectionString)
            : base(nameOrConnectionString)
        { }

        public DbSet<Store> Stores { get; set; }

        public DbSet<StoreType> StoreTypes { get; set; }

        public DbSet<Area> Areas { get; set; }

        public DbSet<User> Users { get; set; }

        public DbSet<Module> Modules { get; set; }

        public DbSet<ModuleElement> ModuleElements { get; set; }

        public DbSet<Collect> Collects { get; set; }

        public DbSet<Order> Orders { get; set; }

        public DbSet<Account> Accounts { get; set; }

        public DbSet<TipsFlow> TipsFlows { get; set; }

        public DbSet<TakeList> TakeLists { get; set; }

        public DbSet<TakeCheck> TakeChecks { get; set; }

        public DbSet<Settings> Settings { get; set; }

        public DbSet<PointDetail> PointDetails { get; set; }

        public DbSet<Role> Roles { get; set; }

        public DbSet<Sms> Smss { get; set; }

        //public DbSet<RoleRelation> RoleRelations { get; set; }

        public DbSet<RoleAndMoudule> RoleAndMoudules { get; set; }

        public DbSet<Feedback> Feedbacks { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.Add(new StoreMap());
            modelBuilder.Configurations.Add(new AreaMap());
            modelBuilder.Configurations.Add(new StoreTypeMap());
            modelBuilder.Configurations.Add(new UserMap());
            modelBuilder.Configurations.Add(new ModuleMap());
            modelBuilder.Configurations.Add(new ModuleElementMap());
            modelBuilder.Configurations.Add(new CollectMap());
            modelBuilder.Configurations.Add(new OrderMap());
            modelBuilder.Configurations.Add(new AccountMap());
            modelBuilder.Configurations.Add(new TipsFlowMap());
            modelBuilder.Configurations.Add(new TakeListMap());
            modelBuilder.Configurations.Add(new TakeCheckMap());
            modelBuilder.Configurations.Add(new SettingsMap());
            modelBuilder.Configurations.Add(new PointDetailMap ());
            modelBuilder.Configurations.Add(new RoleMap());
            //modelBuilder.Configurations.Add(new RoleRelationMap());
            modelBuilder.Configurations.Add(new RoleAndMouduleMap());
            modelBuilder.Configurations.Add(new SmsMap());
            modelBuilder.Configurations.Add(new FeedbackMap());
            //不映射到数据库中
            modelBuilder.Entity<Area>().Ignore(p => p.Childrens);
            modelBuilder.Entity<Sms>().Ignore(p => p.Type); 
        }
    }
}
