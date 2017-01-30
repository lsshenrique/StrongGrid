﻿using Newtonsoft.Json;
using RichardSzalay.MockHttp;
using Shouldly;
using StrongGrid.Model;
using StrongGrid.UnitTests;
using System.Net;
using System.Net.Http;
using System.Threading;
using Xunit;

namespace StrongGrid.Resources.UnitTests
{
	public class ApiKeysTests
	{
		#region FIELDS

		private const string BASE_URI = "https://api.sendgrid.com";
		private const string ENDPOINT = "/api_keys";

		private const string SINGLE_API_KEY_JSON = @"{
			'api_key': 'SG.xxxxxxxx.yyyyyyyy',
			'api_key_id': 'xxxxxxxx',
			'name': 'My API Key',
			'scopes': [
				'mail.send',
				'alerts.create',
				'alerts.read'
			]
		}";
		private const string MULTIPLE_API_KEY_JSON = @"{
			'result': [
				{
					'name': 'A New Hope',
					'api_key_id': 'xxxxxxxx'
				},
				{
					'name': 'Another key',
					'api_key_id': 'yyyyyyyy'
				}
			]
		}";

		#endregion

		[Fact]
		public void Parse_json()
		{
			// Arrange

			// Act
			var result = JsonConvert.DeserializeObject<ApiKey>(SINGLE_API_KEY_JSON);

			// Assert
			result.ShouldNotBeNull();
			result.Key.ShouldBe("SG.xxxxxxxx.yyyyyyyy");
			result.KeyId.ShouldBe("xxxxxxxx");
			result.Name.ShouldBe("My API Key");
			result.Scopes.ShouldBe(new[] { "mail.send", "alerts.create", "alerts.read" });
		}

		[Fact]
		public void Create()
		{
			// Arrange
			var name = "My API Key";
			var scopes = new[] { "mail.send", "alerts.create", "alerts.read" };

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Post, ENDPOINT).Respond("application/json", SINGLE_API_KEY_JSON);

			var client = Utils.GetFluentClient(mockHttp);
			var apiKeys = new ApiKeys(client, ENDPOINT);

			// Act
			var result = apiKeys.CreateAsync(name, scopes, CancellationToken.None).Result;

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
			result.ShouldNotBeNull();
		}

		[Fact]
		public void Get()
		{
			// Arrange
			var keyId = "xxxxxxxx";

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Get, $"{ENDPOINT}/{keyId}").Respond("application/json", SINGLE_API_KEY_JSON);

			var client = Utils.GetFluentClient(mockHttp);
			var apiKeys = new ApiKeys(client, ENDPOINT);

			// Act
			var result = apiKeys.GetAsync(keyId, CancellationToken.None).Result;

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
			result.ShouldNotBeNull();
		}

		[Fact]
		public void GetAll()
		{
			// Arrange
			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Get, ENDPOINT).Respond("application/json", MULTIPLE_API_KEY_JSON);

			var client = Utils.GetFluentClient(mockHttp);
			var apiKeys = new ApiKeys(client, ENDPOINT);

			// Act
			var result = apiKeys.GetAllAsync(CancellationToken.None).Result;

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
			result.ShouldNotBeNull();
			result.Length.ShouldBe(2);
		}

		[Fact]
		public void Delete()
		{
			// Arrange
			var keyId = "xxxxxxxx";

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Delete, $"{ENDPOINT}/{keyId}").Respond(HttpStatusCode.OK);

			var client = Utils.GetFluentClient(mockHttp);
			var apiKeys = new ApiKeys(client, ENDPOINT);

			// Act
			apiKeys.DeleteAsync(keyId, CancellationToken.None).Wait(CancellationToken.None);

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
		}

		[Fact]
		public void Update_with_scopes()
		{
			// Arrange
			var keyId = "xxxxxxxx";
			var name = "My API Key";
			var scopes = new[] { "mail.send", "alerts.create", "alerts.read" };

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Put, $"{ENDPOINT}/{keyId}").Respond("application/json", SINGLE_API_KEY_JSON);

			var client = Utils.GetFluentClient(mockHttp);
			var apiKeys = new ApiKeys(client, ENDPOINT);

			// Act
			var result = apiKeys.UpdateAsync(keyId, name, scopes, CancellationToken.None).Result;

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
			result.ShouldNotBeNull();
		}

		[Fact]
		public void Update_without_scopes()
		{
			// Arrange
			var keyId = "xxxxxxxx";
			var name = "My API Key";
			var scopes = (string[])null;

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(new HttpMethod("PATCH"), $"{ENDPOINT}/{keyId}").Respond("application/json", SINGLE_API_KEY_JSON);

			var client = Utils.GetFluentClient(mockHttp);
			var apiKeys = new ApiKeys(client, ENDPOINT);

			// Act
			var result = apiKeys.UpdateAsync(keyId, name, scopes, CancellationToken.None).Result;

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
			result.ShouldNotBeNull();
		}
	}
}
