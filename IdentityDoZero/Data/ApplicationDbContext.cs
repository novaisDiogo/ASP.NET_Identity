using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using IdentityDoZero.Models;
using System.Linq;

namespace IdentityDoZero.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser> //Para o identity funcionar necessida herdar de IdentityDbContext//Parametro generico para funcionar a custumização do AspNetUsers
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<IdentityDoZero.Models.TesteIdentity> TesteIdentity { get; set; }

        //Função para Remover os Cascade das ForeignKey das tabelas
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            foreach (var foreignKey in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
            {
                foreignKey.DeleteBehavior = DeleteBehavior.Restrict;
            }
        }
    }
}
