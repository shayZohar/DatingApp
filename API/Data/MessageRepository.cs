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
            var query = _context.Messages.OrderByDescending(m => m.MessageSent).AsQueryable();

            query = messageParams.Container switch
            {
                "Inbox" => query.Where(u => u.Recipent.UserName == messageParams.Username && u.RecipentDeleted == false),
                "Outbox" => query.Where(u => u.Sender.UserName == messageParams.Username && u.SenderDeleted == false),
                _ => query.Where(u => u.Recipent.UserName == messageParams.Username 
                    && u.RecipentDeleted == false && u.DateRead == null) // unread messages
            };

            //projecting
            var messages = query.ProjectTo<MessageDto>(_mapper.ConfigurationProvider);
            return await PagedList<MessageDto>.CreateAsync(messages, messageParams.PageNumber, messageParams.PageSize);
        }

        public async Task<IEnumerable<MessageDto>> GetMessageThread(string currentUsername, string RecipientUsername)
        {
            var messages = await _context.Messages
            .Include(u => u.Sender).ThenInclude(p => p.Photos)
            .Include(u => u.Recipent).ThenInclude(p => p.Photos)
            .Where(m => m.Recipent.UserName == currentUsername && m.RecipentDeleted == false
                    && m.Sender.UserName == RecipientUsername
                    || m.Recipent.UserName == RecipientUsername
                    && m.Sender.UserName == currentUsername && m.SenderDeleted == false
            ).OrderBy(m => m.MessageSent)
            .ToListAsync();

            var UnreadMessages = messages.Where(m=> m.DateRead == null && m.Recipent.UserName == currentUsername).ToList();
            if(UnreadMessages.Any())
            {
                foreach (var message in UnreadMessages)
                {
                    message.DateRead = DateTime.UtcNow;
                }

                await _context.SaveChangesAsync();
            } 

            return _mapper.Map<IEnumerable<MessageDto>>(messages);
        }

        public async Task<bool> SaveAllAsync()
        {
            return await _context.SaveChangesAsync() > 0;
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