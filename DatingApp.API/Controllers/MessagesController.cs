using System;
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
    [Route("api/users/{userId}/[controller]")]
    [ApiController]
    public class MessagesController : ControllerBase
    {
        private readonly IMapper mapper;
        private readonly IMessagesRepository messagesRepository;
        private readonly IRepository repository;
        private readonly IUsersRepository usersRepository;

        public MessagesController(
            IRepository repository,
            IMessagesRepository messagesRepository,
            IUsersRepository usersRepository,
            IMapper mapper)
        {
            this.repository = repository;
            this.messagesRepository = messagesRepository;
            this.usersRepository = usersRepository;
            this.mapper = mapper;
        }

        [HttpGet("{id}", Name = "GetMessage")]
        public async Task<IActionResult> GetMessage(int userId, int id)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value)) return Unauthorized();

            var messageFromRepo = await messagesRepository.GetMessage(id);

            if (messageFromRepo == null) return NotFound();

            return Ok(messageFromRepo);
        }

        [HttpGet]
        public async Task<IActionResult> GetMessagesForUser(int userId,
            [FromQuery] MessageParams messageParams)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value)) return Unauthorized();

            messageParams.UserId = userId;

            var messagesFromRepo = await messagesRepository.GetMessagesForUser(messageParams);

            var messages = mapper.Map<IEnumerable<MessageToReturnDto>>(messagesFromRepo);

            Response.AddPagination(messagesFromRepo.CurrentPage, messagesFromRepo.PageSize,
                messagesFromRepo.TotalCount, messagesFromRepo.TotalPages);

            return Ok(messages);
        }

        [HttpGet("thread/{recipientId}")]
        public async Task<IActionResult> GetMessageThread(int userId, int recipientId)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value)) return Unauthorized();

            var messagesFromRepo = await messagesRepository.GetMessageThread(userId, recipientId);

            var messageThread = mapper.Map<IEnumerable<MessageToReturnDto>>(messagesFromRepo);

            return Ok(messageThread);
        }

        [HttpPost]
        public async Task<IActionResult> CreateMessage(int userId, MessageForCreationDto messageForCreationDto)
        {
            var sender = await usersRepository.GetUser(userId, false);

            if (sender.Id != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value)) return Unauthorized();

            messageForCreationDto.SenderId = userId;

            var recipient = await usersRepository.GetUser(messageForCreationDto.RecipientId, false);

            if (recipient == null) return BadRequest("Could not find the user");

            var message = mapper.Map<Message>(messageForCreationDto);

            repository.Add(message);

            if (await repository.SaveAll())
            {
                var messageToReturn = mapper.Map<MessageToReturnDto>(message);
                return CreatedAtRoute("GetMessage", new {id = message.Id}, messageToReturn);
            }

            return BadRequest("Error creating the message");
        }

        [HttpPost("{id}")]
        public async Task<IActionResult> DeleteMessage(int id, int userId)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value)) return Unauthorized();

            var messageFromRepo = await messagesRepository.GetMessage(id);

            if (messageFromRepo.RecipientId == userId) messageFromRepo.RecipientHidden = true;

            if (messageFromRepo.SenderId == userId) repository.Delete(messageFromRepo);

            if (await repository.SaveAll()) return NoContent();

            return BadRequest("Error deleting the message");
        }

        [HttpPost("{id}/read")]
        public async Task<IActionResult> MarkMessageAsRead(int userId, int id)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value)) return Unauthorized();

            var message = await messagesRepository.GetMessage(id);

            if (message.RecipientId != userId) return Unauthorized();

            message.IsRead = true;
            message.DateRead = DateTime.Now;

            await repository.SaveAll();

            return NoContent();
        }
    }
}