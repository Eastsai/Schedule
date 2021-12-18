using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using NLog;
using Topshelf;

namespace ScheduleService
{    
    public class Program
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        private static string ServiceName = ConfigurationManager.AppSettings["ServiceName"].ToString();
        
        public  static void Main(string[] args)
        {
            logger.Trace("Begin...");
            var rc = HostFactory.Run(x =>
            {
                x.Service<MainService>(s =>
                {
                    s.ConstructUsing(name => new MainService());
                    s.WhenStarted(tc => tc.Start());
                    s.WhenStopped(tc => tc.Stop());
                    s.WhenCustomCommandReceived((tc, hc, command) => tc.ExecuteCustomCommand(command));
                });              
                x.SetDescription($"{ServiceName} Host");
                x.SetDisplayName(ServiceName);
                x.SetServiceName(ServiceName);
                x.RunAsLocalSystem();
                x.StartAutomatically();
            });

            logger.Trace($"Run {ServiceName}");            
            Console.WriteLine($"Run {ServiceName}");
            var exitCode = (int)Convert.ChangeType(rc, rc.GetTypeCode());
            
        }
    }
}
