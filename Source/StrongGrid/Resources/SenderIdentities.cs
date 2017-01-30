﻿using Newtonsoft.Json.Linq;
using Pathoschild.Http.Client;
using StrongGrid.Model;
using StrongGrid.Utilities;
using System.Threading;
using System.Threading.Tasks;

namespace StrongGrid.Resources
{
	/// <summary>
	/// Allows you to create and manage sender identities for Marketing Campaigns.
	/// </summary>
	/// <remarks>
	/// See https://sendgrid.com/docs/API_Reference/Web_API_v3/Marketing_Campaigns/sender_identities.html
	/// </remarks>
	public class SenderIdentities
	{
		private readonly string _endpoint;
		private readonly Pathoschild.Http.Client.IClient _client;

		/// <summary>
		/// Initializes a new instance of the <see cref="SenderIdentities" /> class.
		/// </summary>
		/// <param name="client">SendGrid Web API v3 client</param>
		/// <param name="endpoint">Resource endpoint</param>
		public SenderIdentities(Pathoschild.Http.Client.IClient client, string endpoint = "/senders")
		{
			_endpoint = endpoint;
			_client = client;
		}

		/// <summary>
		/// Creates the asynchronous.
		/// </summary>
		/// <param name="nickname">The nickname.</param>
		/// <param name="from">From.</param>
		/// <param name="replyTo">The reply to.</param>
		/// <param name="address1">The address1.</param>
		/// <param name="address2">The address2.</param>
		/// <param name="city">The city.</param>
		/// <param name="state">The state.</param>
		/// <param name="zip">The zip.</param>
		/// <param name="country">The country.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The <see cref="SenderIdentity" />.
		/// </returns>
		public Task<SenderIdentity> CreateAsync(string nickname, MailAddress from, MailAddress replyTo, string address1, string address2, string city, string state, string zip, string country, CancellationToken cancellationToken = default(CancellationToken))
		{
			var data = CreateJObjectForSenderIdentity(nickname, from, replyTo, address1, address2, city, state, zip, country);

			return _client
				.PostAsync(_endpoint)
				.WithJsonBody(data)
				.WithCancellationToken(cancellationToken)
				.AsSendGridObject<SenderIdentity>();
		}

		/// <summary>
		/// Gets all asynchronous.
		/// </summary>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// An array of <see cref="SenderIdentity" />.
		/// </returns>
		public Task<SenderIdentity[]> GetAllAsync(CancellationToken cancellationToken = default(CancellationToken))
		{
			return _client
				.GetAsync(_endpoint)
				.WithCancellationToken(cancellationToken)
				.AsSendGridObject<SenderIdentity[]>();
		}

		/// <summary>
		/// Gets the asynchronous.
		/// </summary>
		/// <param name="senderId">The sender identifier.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The <see cref="SenderIdentity" />.
		/// </returns>
		public Task<SenderIdentity> GetAsync(long senderId, CancellationToken cancellationToken = default(CancellationToken))
		{
			var endpoint = $"{_endpoint}/{senderId}";
			return _client
				.GetAsync(endpoint)
				.WithCancellationToken(cancellationToken)
				.AsSendGridObject<SenderIdentity>();
		}

		/// <summary>
		/// Updates the asynchronous.
		/// </summary>
		/// <param name="senderId">The sender identifier.</param>
		/// <param name="nickname">The nickname.</param>
		/// <param name="from">From.</param>
		/// <param name="replyTo">The reply to.</param>
		/// <param name="address1">The address1.</param>
		/// <param name="address2">The address2.</param>
		/// <param name="city">The city.</param>
		/// <param name="state">The state.</param>
		/// <param name="zip">The zip.</param>
		/// <param name="country">The country.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The <see cref="SenderIdentity" />.
		/// </returns>
		public Task<SenderIdentity> UpdateAsync(long senderId, string nickname = null, MailAddress from = null, MailAddress replyTo = null, string address1 = null, string address2 = null, string city = null, string state = null, string zip = null, string country = null, CancellationToken cancellationToken = default(CancellationToken))
		{
			var endpoint = $"{_endpoint}/{senderId}";
			var data = CreateJObjectForSenderIdentity(nickname, from, replyTo, address1, address2, city, state, zip, country);

			return _client
				.PatchAsync(endpoint)
				.WithCancellationToken(cancellationToken)
				.AsSendGridObject<SenderIdentity>();
		}

		/// <summary>
		/// Deletes the asynchronous.
		/// </summary>
		/// <param name="senderId">The sender identifier.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The async task.
		/// </returns>
		public Task DeleteAsync(long senderId, CancellationToken cancellationToken = default(CancellationToken))
		{
			var endpoint = $"{_endpoint}/{senderId}";
			return _client
				.DeleteAsync(endpoint)
				.WithCancellationToken(cancellationToken)
				.AsResponse();
		}

		/// <summary>
		/// Resends the verification.
		/// </summary>
		/// <param name="senderId">The sender identifier.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The async task.
		/// </returns>
		public Task ResendVerification(long senderId, CancellationToken cancellationToken = default(CancellationToken))
		{
			var endpoint = $"{_endpoint}/{senderId}/resend_verification";
			return _client
				.PostAsync(endpoint)
				.WithCancellationToken(cancellationToken)
				.AsResponse();
		}

		private static JObject CreateJObjectForSenderIdentity(string nickname = null, MailAddress from = null, MailAddress replyTo = null, string address1 = null, string address2 = null, string city = null, string state = null, string zip = null, string country = null)
		{
			var result = new JObject();
			if (!string.IsNullOrEmpty(nickname)) result.Add("nickname", nickname);
			if (from != null) result.Add("from", JToken.FromObject(from));
			if (replyTo != null) result.Add("reply_to", JToken.FromObject(replyTo));
			if (!string.IsNullOrEmpty(address1)) result.Add("address", address1);
			if (!string.IsNullOrEmpty(address2)) result.Add("address2", address2);
			if (!string.IsNullOrEmpty(city)) result.Add("city", city);
			if (!string.IsNullOrEmpty(state)) result.Add("state", state);
			if (!string.IsNullOrEmpty(zip)) result.Add("zip", zip);
			if (!string.IsNullOrEmpty(country)) result.Add("country", country);
			return result;
		}
	}
}
