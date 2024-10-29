using System;
namespace AI_Medical_Chatbot
{
	public class Message
	{
		public string Text { get; set; }
        public int SenderID { get; set; }
        public int ReceiverID { get; set; }

		public Message(string text, int senderId, int receiverId)
        {
            Text = text;
            SenderID = senderId;
            ReceiverID = receiverId;
        }
		
		public void Sender()
		{
			if (SenderID == 0)
			{
				Console.WriteLine("Chatbot: " + Text);
			}
		}
	}
}