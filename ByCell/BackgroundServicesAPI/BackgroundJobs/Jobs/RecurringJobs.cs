using BackgroundServicesAPI.BackgroundJobs.Services.RecurringJobs;
using Hangfire;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BackgroundServicesAPI.BackgroundJobs.Jobs
{
    public static class RecurringJobs
    {
        //2 saniye de bir çalışacak
        [AutomaticRetry(Attempts = 5)]
        public static void SendMailFromDatabase()
        {
            RecurringJob.RemoveIfExists(nameof(SendUserMail));
            RecurringJob.AddOrUpdate<SendUserMail>(
                nameof(SendUserMail),
                job => job.Process(),
                "0-59/2 * * * * *",
                TimeZoneInfo.Local
                );
        }
    }
}
