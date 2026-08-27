using Curators.Domain.SeedWork;
using Curators.Domain.Aggregates.UserAggregate;
using Curators.Domain.ValueObjects;
using System.Runtime.CompilerServices;

namespace Curators.Domain.Aggregates.ConversationAggregate;

public sealed class Conversation : Entity<ConversationId>, IAggregateRoot
{

    private readonly HashSet<UserId> _members;
    private readonly List<Message> _messages = new();

    private const int MaxAmountOfMembers = 20;
    public IReadOnlyCollection<UserId> Members => _members;
    public IReadOnlyCollection<Message> Messages => _messages.AsReadOnly();
    public bool IsArchived { get; private set; } = false;
    public string? InnerMessage { get; private set; } = null;
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }
    public DateTime? LastMessageAt { get; private set; }
    public int MessageCount => _messages.Count;

    private Conversation(HashSet<UserId> members, Message? message) : base(ConversationId.Generate())
    {
        this._members = members ?? new HashSet<UserId>();
        
        if (message is not null)
        {
            this._messages.Add(message);
        }

        this.CreatedAt = DateTime.UtcNow;
        this.UpdatedAt = DateTime.UtcNow;
    }

    public static Conversation Create(List<UserId> members, Message? message = null)
    {
        HashSet<UserId> membersSet = members.ToHashSet();
        
        if (membersSet.Count() < 2)
        {
            throw new InvalidOperationException("There should be at least 2 members in the conversation");
        }

        return new Conversation(membersSet, message);
    }

    public void SendMessage(Message message)
    {
        if (this.IsArchived)
            throw new InvalidOperationException("Current conversation is archived and new messages are no valid");

        this._messages.Add(message);
        this.LastMessageAt = DateTime.UtcNow;
    }

    public void ReplyTo(Message message, MessageId messageToReplyId)
    {
        if (this.IsArchived)
        {
            throw new InvalidOperationException("Current conversation is archived and new messages are no valid");
        }

        bool isMessageAuthorInConversation = this._members.Any(member => member == message.SenderId);

        if (!isMessageAuthorInConversation)
        {
            throw new InvalidOperationException("The author of message doesn't belong in this conversation");
        }

        Message messageToAnswer = this._messages.FirstOrDefault(m => m.Id.Equals(messageToReplyId)) 
            ?? throw new InvalidOperationException($"Message with ID {messageToReplyId} not in this conversation");

        messageToAnswer.AddReply(message.Id);
        this._messages.Add(message);
        this.LastMessageAt = DateTime.UtcNow;
    }

    public void AddParticipants(List<UserId> listOfParticipants)
    {   
        if (listOfParticipants is null || listOfParticipants.Count == 0)
            throw new ArgumentException("You must add a recipient/target users");

        var membersUnique = listOfParticipants.Distinct().ToList();
        var newMembersCount = membersUnique.Count;
        var membersCount = this._members.Count;

        bool isCapacityExceeded = (newMembersCount + membersCount) > MaxAmountOfMembers; 

        if (isCapacityExceeded)
        {
            throw new ArgumentOutOfRangeException(
                paramName: nameof(listOfParticipants),
                actualValue: listOfParticipants,
                message: $"Conversation can't hold more than {MaxAmountOfMembers} members"
            );
        }

        foreach (var id in membersUnique)
        {
            AddParticipant(id);
        }
    }

    public void RemoveParticipants(List<UserId> listOfParticipants)
    {
        if (listOfParticipants is null || listOfParticipants.Count == 0)
            throw new ArgumentException("You must add a recipient/target users");

        if (this._members.Count == 0)
        {
            throw new InvalidOperationException("There are no members in the current conversation");
        }

        foreach (var id in listOfParticipants.Distinct())
        {
            RemoveParticipant(id);
        }
    }

    public void AddParticipant(UserId user)
    {
        if (this._members.Contains(user))
        {
            return;
        }

        this._members.Add(user);
        this.UpdatedAt = DateTime.UtcNow;
    }

    public void RemoveParticipant(UserId user)
    {
        if (!this._members.Contains(user))
        {
            return;
        }
        
        this._members.Remove(user);
        this.UpdatedAt = DateTime.UtcNow;
    }



    public void Archive() => this.IsArchived = true;
}

public readonly record struct ConversationId(Guid Value)
{
    public static ConversationId Generate() => new ConversationId(Guid.NewGuid());
    //public override int GetHashCode() => base.GetHashCode();
    
}