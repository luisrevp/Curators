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
    public DateTime? LastMessageAt { get; private set; }
    public int MessageCount => _messages.Count;



    private Conversation(HashSet<UserId> members, Message? message) : base(ConversationId.Generate())
    {
        this._members = members ?? new HashSet<UserId>();
        
        if (message is not null)
        {
            this._messages.Add(message);
        }
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
            throw new InvalidOperationException("Current conversation is archived and new messages are no valid");

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
        if (listOfParticipants is null)
            return;
        
        if (listOfParticipants.Count() == 0)
            throw new ArgumentException("You must add a recipient/target users");

        var membersUnique = listOfParticipants.Distinct();

        bool isCapacityExceeded = (membersUnique.Count() + this._members.Count()) > MaxAmountOfMembers; 

        if (isCapacityExceeded)
        {
            throw new ArgumentException($"Conversation can't hold more than {MaxAmountOfMembers} members");
        }

        foreach (var id in membersUnique)
        {
            if (this._members.Add(id))
            {
                Console.WriteLine($"Member with ID {id} added successfuly");
            }
            else
            {
                Console.WriteLine($"Member with ID {id} skipped for idempotency");
            }
        }
    }

    public void RemoveParticipant(UserId user)
    {
        if (this._members.Contains(user))
        {
            this._members.Remove(user);
            return;
        }

        Console.WriteLine($"Couldn't remove user with ID {user}");
    }

    public void ArchiveChat() => this.IsArchived = true;
}

public readonly record struct ConversationId(Guid Value)
{
    public static ConversationId Generate() => new ConversationId(Guid.NewGuid());
    //public override int GetHashCode() => base.GetHashCode();
    
}