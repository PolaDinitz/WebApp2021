using WebApp2021.DAL;
using WebApp2021.Models;
using WebApp2021.Utils;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;


namespace WebApp2021.BL
{
    public class UserBL
    {
        private readonly AppDbContext db;

        public UserBL(AppDbContext db) => this.db = db;


        public ValueTask<User> GetUser(int id)
        {
            return this.db.Users.FindAsync(id);
        }

        public Task<List<User>> GetUsers(Expression<Func<User,bool>> predicate = null)
        {
            if (predicate != null)
            {
                return this.db.Users.Where(predicate).ToListAsync();
            }

            return this.db.Users.ToListAsync();
        }

        public Task<int> Create(User user)
        {
            this.db.Users.Add(user);
            return this.db.SaveChangesAsync();
        }

        public Task<int> Edit(User user)
        {
            this.db.Users.Update(user);
            return this.db.SaveChangesAsync();
        }

        public async Task<int> Delete(int id)
        {
            User user = await this.GetUser(id);

            user.Events.Clear();

            foreach (var recipe in user.Recipes.ToList())
            {
                recipe.UserId = Constants.SystemUserID;
                this.db.Recipes.Update(recipe);
            }

            this.db.Users.Remove(user);

            return await this.db.SaveChangesAsync();
        }        
    }
} 