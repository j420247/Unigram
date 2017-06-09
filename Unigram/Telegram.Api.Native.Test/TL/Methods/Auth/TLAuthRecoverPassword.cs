// <auto-generated/>
using System;
using Telegram.Api.Native.TL;

namespace Telegram.Api.TL.Methods.Auth
{
	/// <summary>
	/// RCP method auth.recoverPassword.
	/// Returns <see cref="Telegram.Api.TL.TLAuthAuthorization"/>
	/// </summary>
	public partial class TLAuthRecoverPassword : TLObject
	{
		public String Code { get; set; }

		public TLAuthRecoverPassword() { }
		public TLAuthRecoverPassword(TLBinaryReader from)
		{
			Read(from);
		}

		public override TLType TypeId { get { return TLType.AuthRecoverPassword; } }

		public override void Read(TLBinaryReader from)
		{
			Code = from.ReadString();
		}

		public override void Write(TLBinaryWriter to)
		{
			to.WriteString(Code ?? string.Empty);
		}
	}
}