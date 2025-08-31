namespace WhatsApp_GPT.Models
{
    public class MessagePayLoad
    {
        public string Object = string.Empty;
        public List<Entry> Entry = [];
    }

    public class Entry
    {
        public string Id = string.Empty;
        public List<Change> Changes = [];
    }

    public class Change
    {
        public Value Value = new();
        public List<Contact> Contacts = [];
    }

    public class Value
    {
        public string MessagingProduct = string.Empty;
        public Metadata Metadata = new();
        public List<Message> Messages = [];
    }

    public class Contact
    {
        public Profile Profile = new();
        public string WAId = string.Empty;
    }

    public class Message
    {
        public string From = string.Empty;
        public string Id = string.Empty;
        public string TimeStamp = string.Empty;
        public string Type = string.Empty;
        public MessageText Text = new();
    }

    public class MessageText
    {
        public string Body = string.Empty;
    }

    public class Profile
    {
        public string Name = string.Empty;
    }

    public class Metadata
    {
        public string DisplayPhoneNumber = string.Empty;
        public string PhoneNumberId = string.Empty;
    }
}
