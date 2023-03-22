using DataRooms.UI.Code.Schedulers;
using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace DataRooms.UI
{
    public  class Scheduler
    {
        public async void Start()
        {
            try
            {
                int archivalHour = Convert.ToInt32(ConfigurationManager.AppSettings["ArchivalHour"]);
                int archivalMinute = Convert.ToInt32(ConfigurationManager.AppSettings["ArchivalMinute"]);
                int deleteHour = Convert.ToInt32(ConfigurationManager.AppSettings["DeleteHour"]);
                int deleteMinute = Convert.ToInt32(ConfigurationManager.AppSettings["DeleteMinute"]);
                int intervalHoursArchival = Convert.ToInt32(ConfigurationManager.AppSettings["ArchivalInterval"]);
                int intervalHourDeletion = Convert.ToInt32(ConfigurationManager.AppSettings["DeleteInterval"]);
                // construct a scheduler factory
                ISchedulerFactory schedFact = new StdSchedulerFactory();

                // get a scheduler
                IScheduler sched = await schedFact.GetScheduler();
                await sched.Start();

                IJobDetail jobofArchival = JobBuilder.Create<ArchivalScheduler>()
                    .WithIdentity("myJob1", "group1")
                    .Build();
                IJobDetail jobofDeletion = JobBuilder.Create<DeletionScheduler>()
                    .WithIdentity("myJob2", "group2")
                    .Build();
                IJobDetail jobofADSync = JobBuilder.Create<ADSyncScheduler>()
                   .WithIdentity("myJob3", "group3")
                   .Build();

                ITrigger trigger1 = TriggerBuilder.Create()
                   .WithDailyTimeIntervalSchedule
                   (s =>
                       s.WithIntervalInMinutes(intervalHoursArchival)
                           .OnEveryDay()
                           .StartingDailyAt(TimeOfDay.HourAndMinuteOfDay(archivalHour, archivalMinute))
                   )
                   .Build();

                ITrigger trigger2 = TriggerBuilder.Create()
                   .WithDailyTimeIntervalSchedule
                   (s =>
                       s.WithIntervalInMinutes(intervalHourDeletion)
                           .OnEveryDay()
                           .StartingDailyAt(TimeOfDay.HourAndMinuteOfDay(deleteHour, deleteMinute))
                   )
                   .Build();

                ITrigger trigger3 = TriggerBuilder.Create()
                   .WithDailyTimeIntervalSchedule
                   (s =>
                       s.WithIntervalInMinutes(60)
                           .OnEveryDay()
                           .StartingDailyAt(TimeOfDay.HourAndMinuteOfDay(09, 00))
                   )
                   .Build();

                await sched.ScheduleJob(jobofArchival, trigger1);
                await sched.ScheduleJob(jobofDeletion, trigger2);
                await sched.ScheduleJob(jobofADSync, trigger3);
            }
            catch (ArgumentException e)
            {
                //Log.Error(e);
            }
        }
    }
}