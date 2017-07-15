// <auto-generated/>
using System;

namespace Telegram.Api.TL
{
	public partial class TLMessageActionChannelMigrateFrom : TLMessageActionBase 
	{
		public String Title { get; set; }
		public Int32 ChatId { get; set; }

		public TLMessageActionChannelMigrateFrom() { }
		public TLMessageActionChannelMigrateFrom(TLBinaryReader from)
		{
			Read(from);
		}

		public override TLType TypeId { get { return TLType.MessageActionChannelMigrateFrom; } }

		public override void Read(TLBinaryReader from)
		{
			Title = from.ReadString();
			ChatId = from.ReadInt32();
		}

		public override void Write(TLBinaryWriter to)
		{
			to.Write(0xB055EAEE);
			to.Write(Title);
			to.Write(ChatId);
		}
	}
}