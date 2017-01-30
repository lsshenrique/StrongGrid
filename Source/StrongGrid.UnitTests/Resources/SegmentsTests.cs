﻿using Moq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
	public class SegmentsTests
	{
		#region FIELDS

		private const string ENDPOINT = "/contactdb/segments";

		private const string SINGLE_SEGMENT_JSON = @"{
			'id': 1,
			'name': 'Last Name Miller',
			'list_id': 4,
			'conditions': [
				{
					'field': 'last_name',
					'value': 'Miller',
					'operator': 'eq',
					'and_or': ''
				},
				{
					'field': 'last_clicked',
					'value': '01/02/2015',
					'operator': 'gt',
					'and_or': 'and'
				},
				{
					'field': 'clicks.campaign_identifier',
					'value': '513',
					'operator': 'eq',
					'and_or': 'or'
				}
			]
		}";
		private const string MULTIPLE_SEGMENTS_JSON = @"{
			'segments': [
				{
					'id': 1,
					'name': 'Last Name Miller',
					'list_id': 4,
					'conditions': [
						{
							'field': 'last_name',
							'value': 'Miller',
							'operator': 'eq',
							'and_or': ''
						}
					],
					'recipient_count': 1
				}
			]
		}";

		#endregion

		[Fact]
		public void Parse_json()
		{
			// Arrange

			// Act
			var result = JsonConvert.DeserializeObject<Segment>(SINGLE_SEGMENT_JSON);

			// Assert
			result.ShouldNotBeNull();
			result.Conditions.ShouldNotBeNull();
			result.Conditions.Length.ShouldBe(3);

			result.Conditions[0].Field.ShouldBe("last_name");
			result.Conditions[0].LogicalOperator.ShouldBe(LogicalOperator.None);
			result.Conditions[0].Operator.ShouldBe(ConditionOperator.Equal);
			result.Conditions[0].Value.ShouldBe("Miller");

			result.Conditions[1].Field.ShouldBe("last_clicked");
			result.Conditions[1].LogicalOperator.ShouldBe(LogicalOperator.And);
			result.Conditions[1].Operator.ShouldBe(ConditionOperator.GreatherThan);
			result.Conditions[1].Value.ShouldBe("01/02/2015");

			result.Conditions[2].Field.ShouldBe("clicks.campaign_identifier");
			result.Conditions[2].LogicalOperator.ShouldBe(LogicalOperator.Or);
			result.Conditions[2].Operator.ShouldBe(ConditionOperator.Equal);
			result.Conditions[2].Value.ShouldBe("513");

			result.Id.ShouldBe(1);
			result.ListId.ShouldBe(4);
			result.Name.ShouldBe("Last Name Miller");
		}

		[Fact]
		public void Create()
		{
			// Arrange
			var name = "Last Name Miller";
			var listId = 4;
			var conditions = new[]
			{
				new SearchCondition
				{
					Field = "last_name",
					Value= "Miller",
					Operator = ConditionOperator.Equal,
					LogicalOperator = LogicalOperator.None
				},
				new SearchCondition
				{
					Field = "last_clicked",
					Value = "01/02/2015",
					Operator = ConditionOperator.GreatherThan,
					LogicalOperator = LogicalOperator.And
				},
				new SearchCondition
				{
					Field = "clicks.campaign_identifier",
					Value = "513",
					Operator = ConditionOperator.Equal,
					LogicalOperator = LogicalOperator.Or
				}
			};

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Post, ENDPOINT).Respond("application/json", SINGLE_SEGMENT_JSON);

			var client = Utils.GetFluentClient(mockHttp);
			var segments = new Segments(client, ENDPOINT);

			// Act
			var result = segments.CreateAsync(name, listId, conditions, CancellationToken.None).Result;

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
			mockHttp.Expect(HttpMethod.Get, ENDPOINT).Respond("application/json", MULTIPLE_SEGMENTS_JSON);

			var client = Utils.GetFluentClient(mockHttp);
			var segments = new Segments(client, ENDPOINT);

			// Act
			var result = segments.GetAllAsync(CancellationToken.None).Result;

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
			result.ShouldNotBeNull();
			result.Length.ShouldBe(1);
		}

		[Fact]
		public void Get()
		{
			// Arrange
			var segmentId = 1;

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Get, $"{ENDPOINT}/{segmentId}").Respond("application/json", SINGLE_SEGMENT_JSON);

			var client = Utils.GetFluentClient(mockHttp);
			var segments = new Segments(client, ENDPOINT);

			// Act
			var result = segments.GetAsync(segmentId, CancellationToken.None).Result;

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
			result.ShouldNotBeNull();
		}

		[Fact]
		public void Update()
		{
			// Arrange
			var segmentId = 5;
			var name = "The Millers";
			var listId = 5;
			var conditions = new[]
			{
				new SearchCondition
				{
					Field = "last_name",
					Value= "Miller",
					Operator = ConditionOperator.Equal,
					LogicalOperator = LogicalOperator.None
				}
			};

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(new HttpMethod("PATCH"), $"{ENDPOINT}/{segmentId}").Respond("application/json", SINGLE_SEGMENT_JSON);

			var client = Utils.GetFluentClient(mockHttp);
			var segments = new Segments(client, ENDPOINT);

			// Act
			var result = segments.UpdateAsync(segmentId, name, listId, conditions, CancellationToken.None).Result;

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
			result.ShouldNotBeNull();
		}

		[Fact]
		public void Delete_and_preserve_contacts()
		{
			// Arrange
			var segmentId = 1;
			var deleteContacts = false;

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Delete, $"{ENDPOINT}/{segmentId}?delete_contacts=false").Respond(HttpStatusCode.NoContent);

			var client = Utils.GetFluentClient(mockHttp);
			var segments = new Segments(client, ENDPOINT);

			// Act
			segments.DeleteAsync(segmentId, deleteContacts, CancellationToken.None).Wait(CancellationToken.None);

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
		}

		[Fact]
		public void Delete_and_delete_contacts()
		{
			// Arrange
			var segmentId = 1;
			var deleteContacts = true;

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Delete, $"{ENDPOINT}/{segmentId}?delete_contacts=true").Respond(HttpStatusCode.NoContent);

			var client = Utils.GetFluentClient(mockHttp);
			var segments = new Segments(client, ENDPOINT);

			// Act
			segments.DeleteAsync(segmentId, deleteContacts, CancellationToken.None).Wait(CancellationToken.None);

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
		}

		[Fact]
		public void GetRecipients()
		{
			// Arrange
			var segmentId = 1;
			var recordsPerPage = 25;
			var page = 1;

			var apiResponse = @"{
				'recipients': [
					{
						'created_at': 1422313607,
						'email': 'jones@example.com',
						'first_name': null,
						'id': 'YUBh',
						'last_clicked': null,
						'last_emailed': null,
						'last_name': 'Jones',
						'last_opened': null,
						'updated_at': 1422313790,
						'custom_fields': [
							{
								'id': 23,
								'name': 'pet',
								'value': 'Indiana',
								'type': 'text'
							}
						]
					}
				]
			}";

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Get, $"{ENDPOINT}/{segmentId}/recipients?page_size={recordsPerPage}&page={page}").Respond("application/json", apiResponse);

			var client = Utils.GetFluentClient(mockHttp);
			var segments = new Segments(client, ENDPOINT);

			// Act
			var result = segments.GetRecipientsAsync(segmentId, recordsPerPage, page, CancellationToken.None).Result;

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
			result.ShouldNotBeNull();
			result.Length.ShouldBe(1);
			result[0].Email.ShouldBe("jones@example.com");
		}
	}
}
