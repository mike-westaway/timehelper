using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TimeHelper.Data.Models;

namespace TimeHelper.Data
{
    public class TimeHelperDataContext : DbContext
    {
        public TimeHelperDataContext(DbContextOptions<TimeHelperDataContext> options)
            : base(options)
        {
        }

        public DbSet<Project> Project { get; set; }
        public DbSet<Association> Association { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var project = new Project() { ProjectId = -1, Name = "Alpha", Description = "Project Alpha" };
            modelBuilder.Entity<Project>().HasData(project);

            modelBuilder.Entity<Association>().HasData(
               new Association() { AssociationId = -1, Type = AssociationType.Subject, UserId = "nick@nikkh.net", Criteria = "alpha",   ProjectId = project.ProjectId});
            modelBuilder.Entity<Association>().HasData(
               new Association() { AssociationId = -2, Type = AssociationType.Category, UserId = "nick@nikkh.net", Criteria = "alpha", ProjectId = project.ProjectId });


            project = new Project() { ProjectId = -2, Name = "Beta", Description = "Project Beta" };
            modelBuilder.Entity<Project>().HasData(project);

            modelBuilder.Entity<Association>().HasData(
               new Association() { AssociationId = -3, Type = AssociationType.Subject, UserId = "keith2@nikkh.net", Criteria = "beta", ProjectId = project.ProjectId });
            modelBuilder.Entity<Association>().HasData(
               new Association() { AssociationId = -4, Type = AssociationType.Category, UserId = "keith2@nikkh.net", Criteria = "beta", ProjectId = project.ProjectId });
            modelBuilder.Entity<Association>().HasData(
               new Association() { AssociationId = -5, Type = AssociationType.Subject, UserId = "nick@nikkh.net", Criteria = "beta", ProjectId = project.ProjectId });
            modelBuilder.Entity<Association>().HasData(
               new Association() { AssociationId = -6, Type = AssociationType.Category, UserId = "nick@nikkh.net", Criteria = "beta", ProjectId = project.ProjectId });

            project = new Project() { ProjectId = -3, Name = "Gamma", Description = "Project Gamma" };
            modelBuilder.Entity<Project>().HasData(project);

            modelBuilder.Entity<Association>().HasData(
               new Association() { AssociationId = -7, Type = AssociationType.Subject, UserId = "keith2@nikkh.net", Criteria = "gamma", ProjectId = project.ProjectId });
            modelBuilder.Entity<Association>().HasData(
               new Association() { AssociationId = -8, Type = AssociationType.Category, UserId = "keith2@nikkh.net", Criteria = "gamma", ProjectId = project.ProjectId });
            modelBuilder.Entity<Association>().HasData(
               new Association() { AssociationId = -9, Type = AssociationType.Subject, UserId = "nick@nikkh.net", Criteria = "gamma", ProjectId = project.ProjectId });
            modelBuilder.Entity<Association>().HasData(
               new Association() { AssociationId = -10, Type = AssociationType.Category, UserId = "nick@nikkh.net", Criteria = "gamma", ProjectId = project.ProjectId });

            project = new Project() { ProjectId = -4, Name = "Delta", Description = "Project Delta" };
            modelBuilder.Entity<Project>().HasData(project);

            modelBuilder.Entity<Association>().HasData(
               new Association() { AssociationId = -11, Type = AssociationType.Subject, UserId = "keith2@nikkh.net", Criteria = "delta", ProjectId = project.ProjectId });
            modelBuilder.Entity<Association>().HasData(
               new Association() { AssociationId = -12, Type = AssociationType.Category, UserId = "keith2@nikkh.net", Criteria = "delta", ProjectId = project.ProjectId });
            modelBuilder.Entity<Association>().HasData(
               new Association() { AssociationId = -13, Type = AssociationType.Subject, UserId = "nick@nikkh.net", Criteria = "delta", ProjectId = project.ProjectId });
            modelBuilder.Entity<Association>().HasData(
               new Association() { AssociationId = -14, Type = AssociationType.Category, UserId = "nick@nikkh.net", Criteria = "delta", ProjectId = project.ProjectId });

            project = new Project() { ProjectId = -5, Name = "Epsilon", Description = "Project Epsilon" };
            modelBuilder.Entity<Project>().HasData(project);

            modelBuilder.Entity<Association>().HasData(
               new Association() { AssociationId = -15, Type = AssociationType.Subject, UserId = "keith2@nikkh.net", Criteria = "epsilon", ProjectId = project.ProjectId });
            modelBuilder.Entity<Association>().HasData(
               new Association() { AssociationId = -16, Type = AssociationType.Category, UserId = "keith2@nikkh.net", Criteria = "epsilon", ProjectId = project.ProjectId });
            modelBuilder.Entity<Association>().HasData(
               new Association() { AssociationId = -17, Type = AssociationType.Subject, UserId = "nick@nikkh.net", Criteria = "epsilon", ProjectId = project.ProjectId });
            modelBuilder.Entity<Association>().HasData(
               new Association() { AssociationId = -18, Type = AssociationType.Category, UserId = "nick@nikkh.net", Criteria = "epsilon", ProjectId = project.ProjectId });
        }

    }
}
