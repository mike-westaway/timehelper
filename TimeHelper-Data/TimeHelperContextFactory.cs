using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace TimeHelper.Data
{



public class TimeHelperContextFactory : IDesignTimeDbContextFactory<TimeHelperDataContext>
{
    public TimeHelperDataContext CreateDbContext(string[] args)
    {
            var currentDirectory = Directory.GetCurrentDirectory();
            IConfiguration config = new ConfigurationBuilder()
               .SetBasePath(Directory.GetCurrentDirectory())
               .AddJsonFile("appsettings.json")
               .Build();

            var connectionString = config.GetConnectionString(nameof(TimeHelperDataContext));
            var optionsBuilder = new DbContextOptionsBuilder<TimeHelperDataContext>();
            optionsBuilder.UseSqlServer(connectionString).EnableSensitiveDataLogging();

        return new TimeHelperDataContext(optionsBuilder.Options);
    }
}}