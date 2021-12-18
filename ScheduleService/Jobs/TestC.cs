using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog;

namespace ScheduleService
{
    public class TestC
    {
        Logger logger = LogManager.GetCurrentClassLogger();
        public TestC()
        {
            logger.Trace("new TestC");
        }

        public void TestCMethod1()
        {
            Console.WriteLine("TestCMethod1");
            logger.Trace("TestCMethod1");
        }
    }
}
