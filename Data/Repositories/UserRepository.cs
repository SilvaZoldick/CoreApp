using Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Data.Repositories
{
    public class UserRepository
    {
        private readonly DataContext data;
        public UserRepository(DataContext data) 
        {
            this.data = data;
        }

        public IEnumerable<User> Get()
        {
            return data.AspNetUsers;
        }
        public User GetById(int id)
        {
            var user = data.AspNetUsers.Find(id);
            return user;
        }
        public void Insert(User user)
        {
            data.AspNetUsers.AddAsync(user);
            data.SaveChangesAsync();
        }
        public void Update(User user)
        {
            data.Entry(user).State = EntityState.Modified;
            data.SaveChanges();
        }
        public void Delete(User user) 
        {
            data.AspNetUsers.Remove(user);
        }
    }
}
