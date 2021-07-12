using System;
using System.Collections.Generic;
using WebChat.DAL.Domain;
using WebChat.Models;

namespace WebChat.ViewModels
{
    [Serializable]
    public class ChatViewModel
    {
        public Chat NewChat { get; set; }
        public IReadOnlyCollection<Chat> AllChat { get; set; }
        public List<Messаge> ChatMessаges { get; set; }
        public string CurrentChat { get; set; }
        public string ThisUserEmail { get; set; }
        public HashSet<ChatBot> Bots { get; set; }
        public List<CheckLiveBackUser> CheckLiveBackUsers { get; set; }
    }
}
