using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DatingApp.API.Helpers;
using DatingApp.API.Models;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.API.Data
{
    public class UsersRepository : IUsersRepository
    {
        private readonly DataContext context;

        public UsersRepository(DataContext context)
        {
            this.context = context;
        }

        public async Task<PagedList<User>> GetUsers(UserParams userParams)
        {
            var users = context.Users.Include(p => p.Photos).OrderByDescending(u => u.LastActive).AsQueryable();

            users = users.Where(u => u.Id != userParams.UserId);

            if (!(userParams.Likers || userParams.Liked)) users = users.Where(u => u.Gender == userParams.Gender);

            if (userParams.Likers)
            {
                var userLikers = await GetUserLikes(userParams.UserId, userParams.Likers);
                users = users.Where(u => userLikers.Contains(u.Id));
            }

            if (userParams.Liked)
            {
                var userLiked = await GetUserLikes(userParams.UserId, userParams.Likers);
                users = users.Where(u => userLiked.Contains(u.Id));
            }

            var minDateOfBirth = DateTime.Today.AddYears(-Math.Clamp(userParams.MaxAge, 0, 99) - 1);
            var maxDateOfBirth = DateTime.Today.AddYears(-Math.Clamp(userParams.MinAge, 0, 99));

            users = users.Where(u => u.DateOfBirth >= minDateOfBirth && u.DateOfBirth <= maxDateOfBirth);

            if (!string.IsNullOrEmpty(userParams.OrderBy))
                switch (userParams.OrderBy)
                {
                    case "created":
                        users = users.OrderByDescending(u => u.Created);
                        break;
                    default:
                        users = users.OrderByDescending(u => u.LastActive);
                        break;
                }

            return await PagedList<User>.CreateAsync(users, userParams.PageNumber, userParams.PageSize);
        }

        public async Task<User> GetUser(int id, bool isCurrentUser)
        {
            var query = context.Users.Include(p => p.Photos).AsQueryable();

            if (isCurrentUser) query = query.IgnoreQueryFilters();

            var user = await query.FirstOrDefaultAsync(u => u.Id == id);

            return user;
        }

        public async Task<Like> GetLike(int userId, int recipientId)
        {
            return await context.Likes.FirstOrDefaultAsync(u => u.LikerId == userId && u.LikedId == recipientId);
        }

        private async Task<IEnumerable<int>> GetUserLikes(int id, bool likers)
        {
            var user = await context.Users.Include(x => x.Likers).Include(x => x.Liked)
                .FirstOrDefaultAsync(u => u.Id == id);

            return likers
                ? user.Likers.Where(u => u.LikedId == id).Select(i => i.LikerId)
                : user.Liked.Where(u => u.LikerId == id).Select(i => i.LikedId);
        }
    }
}