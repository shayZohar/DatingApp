using System;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.SignalR;

namespace API.SignalR
{
    // This classs is for the direct conversation between two members
    public class MessageHub : Hub
    {
        private readonly IMessageRepository _messageRepository;
        private readonly IMapper _mapper;
        private readonly IUserRepository _userRepository;
        private readonly IHubContext<PresenceHub> _presenceHub;
        private readonly PresenceTracker _presenceTracker;
        public MessageHub(IMessageRepository messageRepository, IMapper mapper,
        IUserRepository userRepository, IHubContext<PresenceHub> presenceHub, PresenceTracker presenceTracker)
        {
            _presenceTracker = presenceTracker;
            _userRepository = userRepository;
            _mapper = mapper;
            _messageRepository = messageRepository;
            _presenceHub = presenceHub;
        }

    public override async Task OnConnectedAsync()
    {
        var httpContext = Context.GetHttpContext();
        // getting hold of the other user
        var otherUser = httpContext.Request.Query["user"].ToString(); // we can get it when we establish this hub connection
        var groupName = GetGroupName(Context.User.GetUsername(), otherUser); // naming the per group
        await Groups.AddToGroupAsync(Context.ConnectionId, groupName); // adding to the hub's Groups
        var group = await AddToGroup(groupName);
        await Clients.Group(groupName).SendAsync("UpdatedGroup", group); // Sending updated group to client
        var messages = await _messageRepository.GetMessageThread(Context.User.GetUsername(), otherUser);
        await Clients.Caller.SendAsync("ReceiveMessageThread", messages);
    }

    public override async Task OnDisconnectedAsync(Exception ex)
    {
        var group = await RemoveFromMessageGroup();
        await Clients.Group(group.Name).SendAsync("UpdatedGroup", group);
        await base.OnDisconnectedAsync(ex);
    }

    // sending message via hub 
    public async Task SendMessage(CreateMessageDto createMessageDto)
    {
        var username = Context.User.GetUsername();
        if (username == createMessageDto.RecipientUsername.ToLower())
            throw new HubException("You cannot send message to yourself");

        var sender = await _userRepository.GetUserByUserName(username);
        var recipient = await _userRepository.GetUserByUserName(createMessageDto.RecipientUsername);
        if (recipient == null) throw new HubException("Not found user");

        var message = new Message
        {
            Sender = sender,
            Recipent = recipient,
            SenderUsername = sender.UserName,
            RecipentUsername = recipient.UserName,
            Content = createMessageDto.Content
        };
        var groupName = GetGroupName(sender.UserName, recipient.UserName);
        var group = await _messageRepository.GetMessageGroup(groupName);

        // to update the date read to now if the user is connected to this specific hub so it wont be marked as "Unread"
        if (group.Connections.Any(x => x.Username == recipient.UserName))
        {
            message.DateRead = DateTime.UtcNow;
        }
        else
        {
            var connections = await _presenceTracker.GetConnectionsForUser(recipient.UserName);
            // if there are any connections, we know the recipient is online but not in the same message group
            if(connections != null) 
            {
                await _presenceHub.Clients.Clients(connections)
                .SendAsync("NewMessageReceived", new {username = sender.UserName, knownAs = sender.KnownAs});
            }
        }

        _messageRepository.AddMessage(message);
        if (await _messageRepository.SaveAllAsync())
        {
            await Clients.Group(groupName).SendAsync("NewMessage", _mapper.Map<MessageDto>(message));
        }
    }

    // handeling adding user to a group
    private async Task<Group> AddToGroup(string groupName)
    {
        var group = await _messageRepository.GetMessageGroup(groupName);
        var connection = new Connection(Context.ConnectionId, Context.User.GetUsername());
        if (group == null)
        {
            group = new Group(groupName);
            _messageRepository.AddGroup(group);
        }

        group.Connections.Add(connection);

        if(await _messageRepository.SaveAllAsync())
        {
            return group;
        }

        throw new HubException("Failed to join group");
    }

    // handeling removing user from a group
    private async Task<Group> RemoveFromMessageGroup()
    {
        var group = await _messageRepository.GetGroupForConnections(Context.ConnectionId);
        var connection = group.Connections.FirstOrDefault(x => x.ConnectionId == Context.ConnectionId);
        _messageRepository.RemoveConnection(connection);
        if(await _messageRepository.SaveAllAsync()) return group;
        throw new HubException("Failed to remove from group");
    }

    // to ensure that the group name is in alphabetic order
    private string GetGroupName(string caller, string other)
    {
        var stringCompare = string.CompareOrdinal(caller, other) < 0;
        return stringCompare ? $"{caller}-{other}" : $"{other}-{caller}";
    }
}
}