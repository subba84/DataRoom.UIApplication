using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace DataRooms.UI.Code.Schedulers
{
    public class ADSyncScheduler : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            await ADHelper.ADSync(0);
        }
    }
}