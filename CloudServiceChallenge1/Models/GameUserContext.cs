using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CloudServiceChallenge1.Models
{
    public class GameUserContext:DbContext
    {
        public GameUserContext(DbContextOptions<GameUserContext> options):base(options)
        {

        }
        public DbSet<UserModel> Users { get; set; }

    }
}
