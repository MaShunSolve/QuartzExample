using System;
using System.Threading.Tasks;
using Quartz;
using Quartz.Impl;

namespace QuartzExample
{
    public class Program : IJob
    {
        static async Task Main(string[] args)
        {
            Quartz.Logging.LogProvider.IsDisabled = true;
            StdSchedulerFactory factory = new StdSchedulerFactory();
            IScheduler scheduler = await factory.GetScheduler();
            await scheduler.Start();//Scheduler Start
            Console.WriteLine("新增排程           |\n每五秒執行一次     |");
            await scheduler.ScheduleJob(Test1Job("1"), Test1Trigger());
            //await scheduler.ScheduleJob(Test3Job(), Test3Trigger());
            //await scheduler.ScheduleJob(Test2Job(), Test2Trigger());
            await Task.Delay(TimeSpan.FromSeconds(10));//10秒後
            Console.WriteLine("更改排程           |\n每一秒執行一次     |");
            ModifyJob(scheduler, Test1Job("2"), Test1Trigger(), TimeSpan.FromSeconds(1));
            await Task.Delay(TimeSpan.FromSeconds(10));//10秒後
            Console.WriteLine("刪除排程           |");
            DeleteJob(scheduler, Test1Job("1"));//刪除
            await Task.Delay(TimeSpan.FromSeconds(10));//10秒後
            Console.WriteLine("重啟排程           |\n每五秒執行一次     |");
            CreateJob(scheduler, Test1Job("3"), Test1Trigger());//新增
            Console.Read();
        }
        //更改週期
        public static void ModifyJob(IScheduler scheduler, IJobDetail job, ISimpleTrigger trigger, TimeSpan time)
        {
            ISimpleTrigger simpleTrigger = trigger;
            trigger.RepeatInterval = time;
            scheduler.RescheduleJob(trigger.Key, simpleTrigger);
        }
        /// <summary>
        /// 刪除Job
        /// </summary>
        /// <param name="scheduler"></param>
        /// <param name="job"></param>
        public static void DeleteJob(IScheduler scheduler, IJobDetail job)
        { 
            scheduler.DeleteJob(job.Key);
        }
        /// <summary>
        /// 新增Job
        /// </summary>
        /// <param name="scheduler"></param>
        /// <param name="job"></param>
        public static void CreateJob(IScheduler scheduler, IJobDetail job, ITrigger trigger)
        {
            scheduler.ScheduleJob(Test1Job("3"), Test1Trigger());
        }
        #region Trigger
        public static ISimpleTrigger Test1Trigger()
        {
            //DateTime startTime = Convert.ToDateTime("");//起始時間
            //DateTime endTime = Convert.ToDateTime("");//結束時間
            ISimpleTrigger trigger = TriggerBuilder.Create()
                                .WithIdentity("123", "Trigger群組1")
                                .StartNow()
                                //.StartAt(startTime)
                                //.EndAt(endTime)
                                .WithSchedule(SimpleScheduleBuilder.RepeatSecondlyForever(5))
                                .Build()
                                as ISimpleTrigger; 
            return trigger;
        }
        public static ITrigger Test2Trigger()
        {
            //DateTime startTime = Convert.ToDateTime("");//起始時間
            //DateTime endTime = Convert.ToDateTime("");//結束時間
            ITrigger trigger = TriggerBuilder.Create()
                                .WithIdentity("456", "Trigger群組2")
                                .StartNow()
                                //.StartAt(startTime)
                                //.EndAt(endTime)
                                .WithSchedule(SimpleScheduleBuilder.RepeatSecondlyForever(5))
                                .Build();
            return trigger;
        }
        public static ITrigger Test3Trigger()
        {
            //DateTime startTime = Convert.ToDateTime("");//起始時間
            //DateTime endTime = Convert.ToDateTime("");//結束時間
            ITrigger trigger = TriggerBuilder.Create()
                                .WithIdentity("123","Trigger群組3")
                                .StartNow()
                                //.StartAt(startTime)
                                //.EndAt(endTime)
                                .WithSchedule(SimpleScheduleBuilder.RepeatSecondlyForever(5))
                                .Build();
            return trigger;
        }
        #endregion
        #region JobDetail
        public static IJobDetail Test1Job(string type)
        {
            string message = "";
            switch (type)
            {
                case "1":
                    message = "我跳進來ㄌ";
                    break;
                case "2":
                    message = "";
                    break;
                case "3":
                    message = "我又跳進來ㄌ";
                    break;
            }
            IJobDetail job = JobBuilder.Create<Program>()
                            .WithIdentity("1", "Job群組1")
                            .UsingJobData("ID", "JOB1")
                            .UsingJobData("type",type)
                            .UsingJobData("message",message)
                            .Build();
            return job;
        }
        public static IJobDetail Test2Job()
        {
            IJobDetail job = JobBuilder.Create<Program>()
                            .WithIdentity("2", "Job群組1")
                            .UsingJobData("ID", "JOB2")
                            .Build();
            return job;
        }
        public static IJobDetail Test3Job()
        {
            IJobDetail job = JobBuilder.Create<Program>()
                            .WithIdentity("3", "Job群組2")
                            .UsingJobData("ID", "JOB3")
                            .Build();
            return job;
        }
        #endregion
        public async Task Execute(IJobExecutionContext context)
        {
            JobDataMap dataMap = context.JobDetail.JobDataMap;
            string triggerKey = context.Trigger.Key.ToString();//trigger Key
            string trriggerGroup = context.Trigger.Key.Group;//trigger Group
            string jobkey = context.JobDetail.Key.ToString();//Job Key
            string jobgroup = context.JobDetail.Key.Group;//Job group

            string message = dataMap.GetString("message");
            string id = dataMap.GetString("ID");
            string type = dataMap.GetString("type");
            if (type == "2")
            {
                Console.WriteLine($"ID 為 : {id}, Job Key 為 : {jobkey}, Job Group 為 : {jobgroup}\n Trigger Key 為 : {triggerKey}, Trigger Group 為 : {trriggerGroup}\n--------------------------------------------------------");
            }
            else if (type == "1")
            {
                //Console.WriteLine($"{message} ID 為 : {id}, Job Key 為 : {jobkey}, Job Group 為 : {jobgroup}\n Trigger Key 為 : {triggerKey}, Trigger Group 為 : {trriggerGroup}\n--------------------------------------------------------");
                Console.WriteLine($"         {message}|\n                   |我又跳出去ㄌ{DateTime.Now.ToString("HH:mm:ss")}");
            }
            else if (type == "3")
            {
                Console.WriteLine($"       {message}|\n                   |我又跳出去ㄌ{DateTime.Now.ToString("HH:mm:ss")}");
            }
        }
    }
}
