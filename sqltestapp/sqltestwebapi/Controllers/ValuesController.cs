using Dapper;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

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
    }

    public class OrderSummary
    {
        public int ordernumber { get; set; }
        public DateTime date { get; set; }
        public string status { get; set; }
        public double total { get; set; }
    }
}
