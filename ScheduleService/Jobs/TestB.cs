using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog;

namespace ScheduleService
{
    public class TestB
    {
        Logger logger = LogManager.GetCurrentClassLogger();
        public TestB()
        {
            logger.Trace("new TestB");
        }

        public void TestBMethod1()
        {
            Console.WriteLine("TestBMethod1");
            logger.Trace("TestBMethod1");
        }
    }
}
