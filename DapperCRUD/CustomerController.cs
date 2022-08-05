using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;

namespace DapperCRUD
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly IConfiguration _config;
        public CustomerController(IConfiguration config)
        {
            _config = config;
        }

        [HttpGet]

        public async Task<ActionResult<List<Customer>>> GetAllCustomers()
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            IEnumerable<Customer> customer = await SelectAllCustomers(connection);
            return Ok(customer);
        }

        [HttpGet("{customerId}")]

        public async Task<ActionResult<List<Customer>>> GetCustomer(int customerId)
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            var customer = await connection.QueryFirstAsync<Customer>("select * from customers where Id = @Id",
                new { Id = customerId});
            return Ok(customer);
        }

        [HttpPost]
        public async Task<ActionResult<List<Customer>>> CreateCustomer(Customer customer)
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            await connection.ExecuteAsync(
                "insert into customers (name, firstname, lastname, location) values (@Name, @FirstName, @LastName, @Location)", customer);
            return Ok(await SelectAllCustomers(connection));
        }

        [HttpPut]
        public async Task<ActionResult<List<Customer>>> UpdateCustomer(Customer customer)
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            await connection.ExecuteAsync(
                "update customers set name = @Name, firstname = @FirstName, lastname = @LastName, location = @Location where id = @Id", customer);
            return Ok(await SelectAllCustomers(connection));
        }

        [HttpDelete("{customerId}")]
        public async Task<ActionResult<List<Customer>>> DeleteCustomer(int customerId)
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            await connection.ExecuteAsync("delete from customers where id = @Id", new { Id = customerId });
            return Ok(await SelectAllCustomers(connection));
        }
        private static async Task<IEnumerable<Customer>> SelectAllCustomers(SqlConnection connection)
        {
            return await connection.QueryAsync<Customer>("select * from customers");
        }
    }
}
