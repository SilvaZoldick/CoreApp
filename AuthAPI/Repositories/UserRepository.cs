using System.Linq;
using System.Threading.Tasks;
using AuthAPI.Data;
using AuthAPI.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace AuthAPI.Repositories
{
    public static class UserRepository
    {
        public static async Task<User> GetAsync(User model)
        {           
            return await APIContext.SqlServer.AspNetUsers.Where(
                    x => x.UserName == model.UserName && 
                         x.PasswordHash == model.PasswordHash.GetHashCode().ToString()                       
                    ).FirstOrDefaultAsync();
        }
        public static async void InsertAsync(User model)
        {
            User user = new User
            {
                UserName = model.UserName,
                PasswordHash = model.PasswordHash.GetHashCode().ToString()
            };
            
            await APIContext.SqlServer.AspNetUsers.AddAsync(user);
            await APIContext.SqlServer.SaveChangesAsync();
        }
    }
}