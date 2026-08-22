using Curators.Domain.Aggregates.UserAggregate;
using Curators.Domain.Enums;
using Curators.Domain.SeedWork;
using Curators.Domain.ValueObjects;
using System.Net.Mail;

namespace Curators.Domain.Aggregates.ConversationAggregate;

public sealed class Message : Entity<MessageId>
{
    private const int MaxAmountOfAttachments = 6;
    private const long MaximumAttachmentSize = 5120;
    private const int MaxAmountOfMinutesForEdit = 20;
    private const int MessageMaxLength = 500;
    private static readonly List<string> AvailableFormats = 
        ["pdf", "docx", "txt", "xlsx", "xls", "csv", "jpg", "jpeg", "png", "mp3", "m4a", "mp4", "mov"];

    public UserId SenderId { get; }
    public string Content { get; private set; }
    public DateTime SentDate { get; } = default!;
    public DateTime? ModifiedDate { get; private set; } = null!;
    public List<MediaItem>? Attachments { get; private set; }
    public bool IsDeleted { get; private set; } = false;
    public MessageStatus Status { get; private set; }

    public readonly List<MessageId> Replies = new();
    private Message(UserId senderId, string content, DateTime SentSate, List<MediaItem>? attachment, MessageStatus status) 
        : base(MessageId.Generate())
    {
        this.SenderId = senderId;
        this.Content = content;
        this.SentDate = SentSate;
        this.Attachments = attachment;
        this.Status = status;
    }

    public static Message Create(UserId senderId, string content, List<MediaItem>? attachments = null)
    {
        CheckIfMessageContentIsValid(content);

        if (attachments is not null)
        {
            AddAttachments(attachments);
        }

        return new Message(senderId, content, DateTime.Now, attachments, MessageStatus.Sent);
    }

    public void Delete()
    {
        if (this?.Attachments?.Count > 0)
        {
            this.Attachments.Clear();
        }

        this.Content = "The message has been deleted by sender";
        this.ModifiedDate = DateTime.Now;
    }

    public void EditContent(string newMessageContent, List<MediaItem>? attachments = null)
    {
        var currentTime = DateTime.Now;
        var timeDifferences = currentTime - SentDate;
        if (timeDifferences.Minutes > MaxAmountOfMinutesForEdit)
            throw new ArgumentException($"Message can't be edited past {MaxAmountOfMinutesForEdit} minutes");

        CheckIfMessageContentIsValid(newMessageContent);

        if (attachments is not null)
        {
            AddAttachments(attachments);
        }

        this.Content = newMessageContent;
        this.ModifiedDate = DateTime.Now;
    }

    public static void AddAttachments(List<MediaItem> attachments)
    {
        if (attachments.Count > 0)
            throw new InvalidOperationException("You must provide at least 1 attachment");

        string invalidFiles = "Couldn't upload the following files: ";
        for (int i = 0; i <  attachments.Count; i++)
        {
            MediaItem attachment = attachments[i];
            bool isAttachmentValid = IsAttachmentValid(attachment); 
            if (isAttachmentValid)
            {
                invalidFiles += $"{attachment.FileName}.{attachment.Metadata.Format}, ";
                attachments.RemoveAt(i);
            }
        }
    }

    public void AddReply(MessageId message) => this.Replies.Add(message);
    public void MarkAsSent() => this.Status = MessageStatus.Sent;
    public void MarkAsDelivered() => this.Status = MessageStatus.Delivered;
    public void MarkAsRead() => this.Status = MessageStatus.Read;


    #region private methods
    private static void CheckIfMessageContentIsValid(string content)
    {
        if (string.IsNullOrEmpty(content))
            throw new ArgumentException("Message can't be empty");

        if (content.Length > MessageMaxLength)
            throw new InvalidOperationException($"Message shouldn't surpass the {MessageMaxLength} characters");
    }

    private static bool IsAttachmentValid(MediaItem attachment) =>
        !AvailableFormats.Contains(attachment.Metadata.Format) && attachment.Metadata.SizeInBytes < MaximumAttachmentSize;
    #endregion
}

public readonly record struct MessageId(Guid value)
{
    public static MessageId Generate() => new MessageId(Guid.NewGuid());
}