using BackgroundJobs.Services.FireAndForgetJobs;
using Hangfire;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackgroundJobs.Jobs
{
    public static class FireAndForgetJobs
    {
        [AutomaticRetry(Attempts = 5)]
        public static void SendMailRegisterJobs(string emailAdress)
        {
            Hangfire.BackgroundJob.Enqueue<SendRegisteredUserMail>
            (
                job => job.Process(emailAdress)
            );
        }

    }
}
