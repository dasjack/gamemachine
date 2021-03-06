using System;
using System.Collections;
using  System.Collections.Generic;
using System.Linq;
using  System.Text.RegularExpressions;
using GameMachine;
using GameMachine.Core;

namespace GameMachine.Chat
{
    // Simple chat command parsing that understands the following syntax
    //  /tell <player id> <message string> -> Send message to specific player
    // /<channel name> <message string> -> Send message to channel
    // /join <channel name> -> Joins a channel
    // /leave <channel name> -> Leave a channel


    public class ChatCommands
    {

        private Messenger messenger;
        private Chatbox chatbox;
        public string groupName = "priv_" + User.Instance.username + "_group";

        public ChatCommands (Messenger _messenger, Chatbox chatbox)
        {
            messenger = _messenger;
            this.chatbox = chatbox;
        }
                       
        private string SanitizeChannelName (string str)
        {
            return Regex.Replace (str, @"\s+", "");
        }
        

        public void process (string senderId, string command)
        {
            List<string> words;
            string messageType = null;
            
            if (command.StartsWith ("/tell")) {
                command = command.Replace ("/tell", "");
                messageType = "private";
            } else if (command.StartsWith ("/join")) {
                command = command.Replace ("/join", "");
                messenger.JoinChannel (SanitizeChannelName (command));
                return;
            } else if (command.StartsWith ("/leave")) {
                command = command.Replace ("/leave", "");
                messenger.leaveChannel (SanitizeChannelName (command));
                return;
            } else if (command.StartsWith ("/channels")) {
                foreach (string channel in messenger.channelSubscriptions) {
                    messenger.messageReceived (channel);
                }
                return;
            } else if (command.StartsWith ("/group_create")) {
                messenger.JoinChannel (groupName);
                return;
            } else if (command.StartsWith ("/group")) {
                string msg = command.Replace ("/group", "");
                messenger.SendText (senderId, ChatManager.currentGroup, msg, "group");
                return;
            } else if (command.StartsWith ("/invite")) {
                command = command.Replace ("/invite", "");
                words = new List<string> (command.Split (" ".ToCharArray (), StringSplitOptions.RemoveEmptyEntries));
                string invitee = words [0];
                messenger.InviteToChannel (User.Instance.username, invitee, groupName);
                return;
            } else if (command.StartsWith ("/help")) {
                string[] commandlist = new string[] {
                    "Valid commands:\n",
                    "/tell [recipient] [message] - send private message",
                    "/join [channel] - join channel",
                    "/leave [channel] - leave channel",
                    "/channels - show channels you are subscribed to",
                    "/group_create - create group",
                    "/group [groupname] [message] - send group message",
                    "/invite [invitee] - invite to private group",
                    "/help - this message"
                };
                chatbox.AddMessage ("yellow", String.Join ("\n", commandlist));
                return;
            } else if (command.StartsWith ("/")) {
                command = command.Replace ("/", "");
                messageType = "group";
            }

            words = new List<string> (command.Split (" ".ToCharArray (), StringSplitOptions.RemoveEmptyEntries));
            string channelName = words [0];
            command = command.Replace (channelName, "");
            string message = command;
            
            string logMessage = string.Format ("channel={0} message={1} messageType={2}", channelName, message, messageType);
            Logger.Debug (logMessage);

            if (message.Length == 0) {
                chatbox.AddMessage ("red", "Invalid command");
                return;
            }

            if (messageType == "group" && !messenger.channelSubscriptions.Contains (channelName)) {
                chatbox.AddMessage ("red", "You are not subscribed to " + channelName);
                return;
            }
            
            messenger.SendText (senderId, channelName, message, messageType);
        }
    }
}
