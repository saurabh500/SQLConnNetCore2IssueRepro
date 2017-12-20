using Dapper;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Sockets;

using System.Net;
using System.Diagnostics;

namespace sqltestwebapi.Controllers
{
    [Route("api/[controller]")]
    public class ValuesController : Controller
    {
        // GET api/values
        [HttpGet("dapper")]
        public async Task<IActionResult> GetDapper()
        {
            var connectionString = "Server=tcp:ss-desktop2.redmond.corp.microsoft.com,1433;Initial Catalog=fortune;Persist Security Info=False;User ID=saurabh;Password=HappyPass321;MultipleActiveResultSets=False;Max Pool Size=400;Connection Timeout=30;";

            using (var connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    var orders = await connection.QueryAsync<Fortune>(@"SELECT * from FORTUNE");

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
            var connectionString = "Server=tcp:sqlloadtest.database.windows.net,1433;Initial Catalog=fortune;Persist Security Info=False;User ID=saurabh;Password=HappyPass321;MultipleActiveResultSets=False;Max Pool Size=400;Connection Timeout=30;";

            using (var connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    var command = connection.CreateCommand();
                    command.CommandText = @"SELECT * from FORTUNE";

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

        [HttpGet("sqlasync")]
        public async Task<IActionResult> GetSqlAsync()
        {
            var connectionString = "Server=tcp:sqlloadtest.database.windows.net,1433;Initial Catalog=fortune;Persist Security Info=False;User ID=saurabh;Password=HappyPass321;MultipleActiveResultSets=False;Max Pool Size=400;Connection Timeout=30;";

            using (var connection = new SqlConnection(connectionString))
            {
                try
                {
                    await connection.OpenAsync();

                    var command = connection.CreateCommand();
                    command.CommandText = @"SELECT * from FORTUNE";

                    using (var reader = await command.ExecuteReaderAsync(System.Data.CommandBehavior.CloseConnection))
                    {
                        var count = 0;
                        while (await reader.ReadAsync())
                        {
                            count += 1;
                        }
                        return Ok(count);
                    }
                    

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
            Stopwatch st = new Stopwatch();
            st.Start();
            Task<Socket> connectTask = ConnectAsync(serverName, port);
            Socket _socket = await connectTask;
            st.Stop();

            if (_socket != null && _socket.Connected)
            {
                Console.WriteLine("Socket Connected " + st.ElapsedMilliseconds);
            }
            _socket.Dispose();
            return Ok(0);
        }

        private async Task<Socket> ConnectAsync(string serverName, int port)
        {
            Stopwatch st = new Stopwatch();
            st.Start();
            IPAddress[] addresses = Dns.GetHostAddresses(serverName);
            //IPAddress[] addresses = await Dns.GetHostAddressesAsync(serverName).ConfigureAwait(false);
            st.Stop();
            Console.WriteLine("DNS Resolution Time " + st.ElapsedMilliseconds);
            IPAddress targetAddrV4 = Array.Find(addresses, addr => (addr.AddressFamily == AddressFamily.InterNetwork));
            {
                IPAddress targetAddr = targetAddrV4;
                var socket = new Socket(targetAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

                try
                {
                    socket.Connect(targetAddr, port);
                    //await socket.ConnectAsync(targetAddr, port).ConfigureAwait(false);
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

    public class Fortune
    {
        public int Id { get; set; }
        public string Message { get; set; }
    }
}
