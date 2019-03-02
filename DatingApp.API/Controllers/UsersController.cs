using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using DatingApp.API.Data;
using DatingApp.API.Dtos;
using DatingApp.API.Helpers;
using DatingApp.API.Models;
using Microsoft.AspNetCore.Mvc;

namespace DatingApp.API.Controllers
{
    [ServiceFilter(typeof(LogUserActivity))]
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IMapper mapper;
        private readonly IRepository repository;
        private readonly IUsersRepository usersRepository;

        public UsersController(
            IRepository repository,
            IUsersRepository usersRepository,
            IMapper mapper)
        {
            this.repository = repository;
            this.usersRepository = usersRepository;
            this.mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetUsers([FromQuery] UserParams userParams)
        {
            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var userFromRepo = await usersRepository.GetUser(currentUserId, true);

            userParams.UserId = currentUserId;

            if (string.IsNullOrEmpty(userParams.Gender))
                userParams.Gender = userFromRepo.Gender == "male" ? "female" : "male";

            var users = await usersRepository.GetUsers(userParams);

            var usersToReturn = mapper.Map<IEnumerable<UserForListDto>>(users);

            Response.AddPagination(users.CurrentPage, users.PageSize, users.TotalCount, users.TotalPages);

            return Ok(usersToReturn);
        }

        [HttpGet("{id}", Name = "GetUser")]
        public async Task<IActionResult> GetUser(int id)
        {
            var isCurrentUser = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value) == id;

            var user = await usersRepository.GetUser(id, isCurrentUser);

            var userToReturn = mapper.Map<UserForDetailedDto>(user);

            return Ok(userToReturn);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, UserForUpdateDto userForUpdateDto)
        {
            if (id != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value)) return Unauthorized();

            var userFromRepo = await usersRepository.GetUser(id, true);

            mapper.Map(userForUpdateDto, userFromRepo);

            return await repository.SaveAll() ? (IActionResult) NoContent() : BadRequest("No changes were detected");
        }

        [HttpPost("{id}/like/{recipientId}")]
        public async Task<IActionResult> LikeUser(int id, int recipientId)
        {
            if (id != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value)) return Unauthorized();

            var like = await usersRepository.GetLike(id, recipientId);

            var likedUser = usersRepository.GetUser(recipientId, false).Result.KnownAs;

            if (like != null) return BadRequest("You already like " + likedUser);

            if (await usersRepository.GetUser(recipientId, false) == null) return NotFound();

            like = new Like
            {
                LikerId = id,
                LikedId = recipientId
            };

            repository.Add(like);

            if (await repository.SaveAll()) return Ok();

            return BadRequest("Failed to like user" + likedUser);
        }
    }
}