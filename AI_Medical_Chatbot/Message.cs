using System;
namespace AI_Medical_Chatbot
{
	public class Message
	{
		public int SenderID { get; set; }
		public int ReceiverID { get; set; }
		public string Text { get; set; }
		public DateTime Time { get; set; }

		public Message(string text, int senderID, int receiverID)
		{
			Text = text;
			SenderID = senderID;
			ReceiverID = receiverID;
			Time = DateTime.Now;
		}
		
		public void Sender()
		{
			// if the senderid = 0, then the sender is the chatbot
			if (SenderID == 0)
			{
				Console.WriteLine("Chatbot: " + Text);
			}
			else
			{
				Console.WriteLine(Text);
			}
		}
	}
}