using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace DataRooms.UI.Code.Schedulers
{
    public class DeletionScheduler : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            FileDeletion fileDeletion = new FileDeletion();
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }
            await fileDeletion.DeleteFiles();
        }
    }
}