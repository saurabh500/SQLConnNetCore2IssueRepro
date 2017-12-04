using Dapper;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Sockets;

using System.Net;

namespace sqltestwebapi.Controllers
{
    [Route("api/[controller]")]
    public class ValuesController : Controller
    {
        // GET api/values
        [HttpGet("dapper")]
        public async Task<IActionResult> GetDapper()
        {
            var connectionString = "Server=tcp:eshopsql-e3h5ug5xd3rfs.database.windows.net,1433;Initial Catalog=orderingdb;Persist Security Info=False;User ID=eshop;Password=Pass@word!;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Max Pool Size=400;Connection Timeout=30;";

            using (var connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    var orders = await connection.QueryAsync<OrderSummary>(@"SELECT o.[Id] as ordernumber,o.[OrderDate] as [date],os.[Name] as [status],SUM(oi.units*oi.unitprice) as total
                     FROM [ordering].[Orders] o
                     LEFT JOIN[ordering].[orderitems] oi ON  o.Id = oi.orderid 
                     LEFT JOIN[ordering].[orderstatus] os on o.OrderStatusId = os.Id                     
                     GROUP BY o.[Id], o.[OrderDate], os.[Name] 
                     ORDER BY o.[Id]");

                    return Ok(orders.Count());

                }
                catch (Exception x)
                {
                    throw new InvalidOperationException(x.ToString());
                }
            }

        }
        [HttpGet("native")]
        public async Task<IActionResult> GetNative()
        {
            var connectionString = "Server=tcp:eshopsql-e3h5ug5xd3rfs.database.windows.net,1433;Initial Catalog=orderingdb;Persist Security Info=False;User ID=eshop;Password=Pass@word!;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Max Pool Size=400;Connection Timeout=30;";

            using (var connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    var command = connection.CreateCommand();
                    command.CommandText = @"SELECT o.[Id] as ordernumber,o.[OrderDate] as [date],os.[Name] as [status],SUM(oi.units*oi.unitprice) as total
                     FROM [ordering].[Orders] o
                     LEFT JOIN[ordering].[orderitems] oi ON  o.Id = oi.orderid 
                     LEFT JOIN[ordering].[orderstatus] os on o.OrderStatusId = os.Id                     
                     GROUP BY o.[Id], o.[OrderDate], os.[Name] 
                     ORDER BY o.[Id]";

                    var reader = command.ExecuteReader(System.Data.CommandBehavior.CloseConnection);
                    var count = 0;
                    while (reader.Read())
                    {
                        count += 1;
                    }

                    return Ok(count);

                }
                catch (Exception x)
                {
                    throw new InvalidOperationException(x.ToString());
                }
            }

            
        }
        
        [HttpGet("sqlnet")]
        public async Task<IActionResult> GetNetwork()
        {
            string serverName = "eshopsql-e3h5ug5xd3rfs.database.windows.net";
            int port = 1433;
            Task<Socket> connectTask = ConnectAsync(serverName, port);
            connectTask.Wait(30 * 1000);
            Socket _socket = connectTask.Result;
            if(_socket !=null && _socket.Connected)
            {
                Console.WriteLine("Socket Connected");
            }
            
            return Ok(0);
        }

        private async Task<Socket> ConnectAsync(string serverName, int port)
        {
            IPAddress[] addresses = await Dns.GetHostAddressesAsync(serverName).ConfigureAwait(false);
            IPAddress targetAddrV4 = Array.Find(addresses, addr => (addr.AddressFamily == AddressFamily.InterNetwork));
            {
                IPAddress targetAddr = targetAddrV4;
                var socket = new Socket(targetAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

                try
                {
                    await socket.ConnectAsync(targetAddr, port).ConfigureAwait(false);
                }
                catch
                {
                    socket.Dispose();
                    throw;
                }
                return socket;
            }
        }
    }

    public class OrderSummary
    {
        public int ordernumber { get; set; }
        public DateTime date { get; set; }
        public string status { get; set; }
        public double total { get; set; }
    }
}
