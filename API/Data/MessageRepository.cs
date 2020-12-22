using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class MessageRepository : IMessageRepository
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        public MessageRepository(DataContext context, IMapper mapper)
        {
            _mapper = mapper;
            _context = context;
        }

        public void AddMessage(Message message)
        {
            _context.Messages.Add(message);
        }

        public void DeleteMessage(Message message)
        {
            _context.Messages.Remove(message);
        }

        public async Task<Message> GetMessage(int _id)
        {
            return await _context.Messages
            .Include(u => u.Sender) // including the entities of sender and recipet
            .Include(u => u.Recipent)
            .SingleOrDefaultAsync(x => x.Id == _id);
        }

        public async Task<PagedList<MessageDto>> GetMessagesForUser(messageParams messageParams)
        {
            var query = _context.Messages.OrderByDescending(m => m.MessageSent)
            .ProjectTo<MessageDto>(_mapper.ConfigurationProvider)
            .AsQueryable();

            query = messageParams.Container switch
            {
                "Inbox" => query.Where(u => u.RecipentUsername == messageParams.Username && u.RecipentDeleted == false),
                "Outbox" => query.Where(u => u.SenderUsername == messageParams.Username && u.SenderDeleted == false),
                _ => query.Where(u => u.RecipentUsername == messageParams.Username 
                    && u.RecipentDeleted == false && u.DateRead == null) // unread messages
            };
            
            return await PagedList<MessageDto>.CreateAsync(query, messageParams.PageNumber, messageParams.PageSize);
        }

        public async Task<IEnumerable<MessageDto>> GetMessageThread(string currentUsername, string RecipientUsername)
        {
            var messages = await _context.Messages
            .Where(m => m.Recipent.UserName == currentUsername && m.RecipentDeleted == false
                    && m.Sender.UserName == RecipientUsername
                    || m.Recipent.UserName == RecipientUsername
                    && m.Sender.UserName == currentUsername && m.SenderDeleted == false
            ).OrderBy(m => m.MessageSent)
            .ProjectTo<MessageDto>(_mapper.ConfigurationProvider)
            .ToListAsync();

            var UnreadMessages = messages.Where(m=> m.DateRead == null && m.RecipentUsername == currentUsername).ToList();
            if(UnreadMessages.Any())
            {
                foreach (var message in UnreadMessages)
                {
                    message.DateRead = DateTime.UtcNow;
                }
            } 

            return messages;
        }

        public void AddGroup(Group group)
        {
            _context.Groups.Add(group);
        }

        public async Task<Connection> GetConnection(string connectionId)
        {
            return await _context.Connections.FindAsync(connectionId);
        }

        public async Task<Group> GetMessageGroup(string groupName)
        {
            return await _context.Groups.Include(x => x.Connections).FirstOrDefaultAsync(x => x.Name == groupName);
        }

        public void RemoveConnection(Connection connection)
        {
           _context.Connections.Remove(connection);
        }

        public async Task<Group> GetGroupForConnections(string connectionId)
        {
            return await _context.Groups.Include(c => c.Connections)
            .Where(c => c.Connections.Any(x => x.ConnectionId == connectionId)).FirstOrDefaultAsync();
        }
    }
}