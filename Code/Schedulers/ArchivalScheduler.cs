using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace DataRooms.UI.Code.Schedulers
{
    public class ArchivalScheduler : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            FileArchival fileArchival = new FileArchival();
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }
            await fileArchival.ArchiveFiles();
        }
    }
}