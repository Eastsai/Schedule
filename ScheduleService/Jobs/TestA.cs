using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog;

namespace ScheduleService
{
    class TestA
    {
        Logger logger = LogManager.GetCurrentClassLogger();
        public TestA() {
            logger.Trace("new TestA");
        }

        public void TestAMethod1() {
            Console.WriteLine("TestAMethod1");
            logger.Trace("TestAMethod1");
        }
    }
}
