using Consul;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Service2.Common
{
    public static class ConsulHelper
    {
        public static void ConsulRegist(this IConfiguration configuration)
        {
            ConsulClient client = new ConsulClient(c =>
            {
                c.Address = new Uri(configuration["Consul:Client:Address"]);
                c.Datacenter = configuration["Consul:Client:Datacenter"];
            });
            string ip = configuration["Consul:IP"];
            int port = int.Parse(configuration["Consul:Port"]);
            int weight = string.IsNullOrWhiteSpace(configuration["Consul:Weight"]) ? 1 : int.Parse(configuration["Consul:Weight"]);

            client.Agent.ServiceRegister(new AgentServiceRegistration()
            {
                ID = "service" + Guid.NewGuid(),//唯一的
                Name = configuration["Consul:ServiceName"],//服务名称
                Address = ip,//
                Port = port,//
                Tags = new string[] { weight.ToString() }
                ,//标签
                Check = new AgentServiceCheck()
                {
                    Interval = TimeSpan.FromSeconds(12),//间隔12s一次
                    HTTP = $"http://{ip}:{port}{configuration["Consul:HeartCheck:HTTP"]}",
                    Timeout = TimeSpan.FromSeconds(Convert.ToDouble(configuration["Consul:HeartCheck:Timeout"])),//检测等待时间
                    DeregisterCriticalServiceAfter = TimeSpan.FromSeconds(Convert.ToDouble(configuration["Consul:HeartCheck:DeregisterCriticalServiceAfter"]))//失败后多久移除
                }
            });
            //命令行参数获取
            string ss = $"http://{ip}:{port}{configuration["Consul:HeartCheck:HTTP"]}";
            Console.WriteLine($"http://{ip}:{port}{configuration["Consul:HeartCheck:HTTP"]}");
        }
    }
}
