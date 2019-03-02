using System.Linq;
using System.Threading.Tasks;
using DatingApp.API.Models;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.API.Data
{
    public class PhotosRepository : IPhotosRepository
    {
        private readonly DataContext context;

        public PhotosRepository(DataContext context)
        {
            this.context = context;
        }

        public async Task<Photo> GetPhoto(int id)
        {
            var photo = await context.Photos.IgnoreQueryFilters().FirstOrDefaultAsync(p => p.Id == id);

            return photo;
        }

        public async Task<Photo> GetMainPhotoForUser(int userId)
        {
            return await context.Photos.Where(u => u.UserId == userId).FirstOrDefaultAsync(p => p.IsMain);
        }
    }
}