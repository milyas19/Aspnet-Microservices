using Dapper;
using Discount.API.Entities;
using Microsoft.Extensions.Configuration;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Discount.API.Repositories
{
    public class DiscountRepository : IDiscountRepository
    {
        private readonly IConfiguration _configuration;

        public DiscountRepository(IConfiguration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public async Task<Coupon> GetDiscount(string productName)
        {
            using var connection = new NpgsqlConnection
                (_configuration.GetValue<string>("DatabaseSettings:ConnectionString"));

            var coupon = await connection.QueryFirstOrDefaultAsync<Coupon>
                ("Select * from Coupon Where ProductName = @ProductName", new
                {
                    ProductName = productName
                });
            if (coupon == null)
            {
                return new Coupon
                {
                    ProductName = "No Discount",
                    Description = "No Discount Description",
                    Amount = 0
                };
            }
            return coupon;
        }

        public async Task<bool> CreateDiscount(Coupon coupon)
        {
            using var connection = new NpgsqlConnection
               (_configuration.GetValue<string>("DatabaseSettings:ConnectionString"));

            var affected = await connection
                .ExecuteAsync("Insert into Coupon (ProductName, Description, Amount) Values (@ProductName, @Description, @Amount)",
                new
                {
                    ProductName = coupon.ProductName,
                    Description = coupon.Description,
                    Amount = coupon.Amount
                });
            if (affected == 0)
            {
                return false;
            }
            return true;
        }

        public async Task<bool> UpdateDiscount(Coupon coupon)
        {
            using var connection = new NpgsqlConnection
               (_configuration.GetValue<string>("DatabaseSettings:ConnectionString"));

            var affected = await connection
                .ExecuteAsync("Update Coupon SET ProductName = @ProductName, Description = @Description, Amount = @Amount WHERE Id = @Id",
                new
                {
                    Id = coupon.Id,
                    ProductName = coupon.ProductName,
                    Description = coupon.Description,
                    Amount = coupon.Amount
                });
            if (affected == 0)
            {
                return false;
            }
            return true;
        }

        public async Task<bool> DeleteDiscount(string productName)
        {
            using var connection = new NpgsqlConnection
               (_configuration.GetValue<string>("DatabaseSettings:ConnectionString"));

            var affected = await connection.ExecuteAsync("Delete from Coupon Where ProductName = @ProductName",
                new
                {
                    ProductName = productName
                });

            if (affected == 0)
                return false;

            return true;
        }
    }
}
