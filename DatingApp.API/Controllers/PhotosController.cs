using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using DatingApp.API.Data;
using DatingApp.API.Dtos;
using DatingApp.API.Helpers;
using DatingApp.API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace DatingApp.API.Controllers
{
    [Route("api/users/{userId}/photos")]
    [ApiController]
    public class PhotosController : ControllerBase
    {
        private readonly Cloudinary cloudinary;
        private readonly IOptions<CloudinarySettings> cloudinaryConfig;
        private readonly IMapper mapper;
        private readonly IPhotosRepository photosRepository;
        private readonly IRepository repository;
        private readonly UserManager<User> userManager;
        private readonly IUsersRepository usersRepository;

        public PhotosController(
            UserManager<User> userManager,
            IOptions<CloudinarySettings> cloudinaryConfig,
            IPhotosRepository photosRepository,
            IUsersRepository usersRepository,
            IRepository repository,
            IMapper mapper)
        {
            this.userManager = userManager;
            this.repository = repository;
            this.photosRepository = photosRepository;
            this.usersRepository = usersRepository;
            this.mapper = mapper;
            this.cloudinaryConfig = cloudinaryConfig;

            var acc = new Account(
                cloudinaryConfig.Value.CloudName,
                cloudinaryConfig.Value.ApiKey,
                cloudinaryConfig.Value.ApiSecret
            );

            cloudinary = new Cloudinary(acc);
        }

        [HttpGet("{id}", Name = "GetPhoto")]
        public async Task<IActionResult> GetPhoto(int id)
        {
            var photoFromRepo = await photosRepository.GetPhoto(id);

            var photo = mapper.Map<PhotoForReturnDto>(photoFromRepo);

            return Ok(photo);
        }


        [HttpPost]
        public async Task<IActionResult> AddPhotoForUser(int userId, [FromForm] PhotoForCreationDto photoForCreationDto)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value)) return Unauthorized();

            var userFromRepo = await usersRepository.GetUser(userId, true);

            var file = photoForCreationDto.File;

            var uploadResult = new ImageUploadResult();

            if (file.Length > 0)
                using (var stream = file.OpenReadStream())
                {
                    var uploadParams = new ImageUploadParams
                    {
                        File = new FileDescription(file.Name, stream),
                        Transformation = new Transformation().Width(500).Height(500).Crop("fill").Gravity("face")
                    };

                    uploadResult = cloudinary.Upload(uploadParams);
                }

            photoForCreationDto.Url = uploadResult.Uri.ToString();
            photoForCreationDto.PublicId = uploadResult.PublicId;

            var photo = mapper.Map<Photo>(photoForCreationDto);

            userFromRepo.Photos.Add(photo);

            if (await repository.SaveAll())
            {
                var photoToReturn = mapper.Map<PhotoForReturnDto>(photo);
                return CreatedAtRoute("GetPhoto", new {id = photo.Id}, photoToReturn);
            }

            return BadRequest("Failed to add the photo");
        }

        [HttpPost("{id}/setMain")]
        public async Task<IActionResult> SetMainPhoto(int userId, int id)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value)) return Unauthorized();

            var userFromRepo = await usersRepository.GetUser(userId, true);

            if (userFromRepo.Photos.All(p => p.Id != id)) return Unauthorized();

            var photoFromRepo = await photosRepository.GetPhoto(id);

            if (photoFromRepo.IsMain) return BadRequest("This is already your main photo");

            var currentMainPhoto = await photosRepository.GetMainPhotoForUser(userId);

            if (currentMainPhoto != null) currentMainPhoto.IsMain = false;

            photoFromRepo.IsMain = true;

            if (await repository.SaveAll()) return NoContent();

            return BadRequest("Failed to set the photo to main");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePhoto(int userId, int id)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value)) return Unauthorized();

            var userFromRepo = await usersRepository.GetUser(userId, true);

            if (userFromRepo.Photos.All(p => p.Id != id)) return Unauthorized();

            var photoFromRepo = await photosRepository.GetPhoto(id);

            if (photoFromRepo.PublicId != null)
            {
                var deleteParams = new DeletionParams(photoFromRepo.PublicId);

                var result = cloudinary.Destroy(deleteParams);

                if (result.Result == "ok") repository.Delete(photoFromRepo);
            }

            if (photoFromRepo.PublicId == null) repository.Delete(photoFromRepo);

            if (await repository.SaveAll()) return Ok();

            return BadRequest("Failed to delete the photo");
        }
    }
}