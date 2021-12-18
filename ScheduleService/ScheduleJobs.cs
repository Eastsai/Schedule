using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog;
using Quartz;

namespace ScheduleService
{

    [DisallowConcurrentExecution]
    public class TestAJob : IJob
    {
        Logger logger = LogManager.GetCurrentClassLogger();
        public void Execute(IJobExecutionContext context)
        {
            logger.Trace("Execute TestA");
            Console.WriteLine("Execute TestA");
            TestA job = new TestA();
            job.TestAMethod1();
        }
    }

    public class TestBJob : IJob
    {
        Logger logger = LogManager.GetCurrentClassLogger();
        public void Execute(IJobExecutionContext context)
        {
            logger.Trace("Execute TestB");
            Console.WriteLine("Execute TestB");
            TestB job = new TestB();
            job.TestBMethod1();
        }
    }

    public class TestCJob : IJob
    {
        Logger logger = LogManager.GetCurrentClassLogger();
        public void Execute(IJobExecutionContext context)
        {
            logger.Trace("Execute TestC");
            Console.WriteLine("Execute TestC");
            TestC job = new TestC();
            job.TestCMethod1();
        }
    }
}
