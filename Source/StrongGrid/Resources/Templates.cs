﻿using Newtonsoft.Json.Linq;
using Pathoschild.Http.Client;
using StrongGrid.Model;
using StrongGrid.Utilities;
using System.Threading;
using System.Threading.Tasks;

namespace StrongGrid.Resources
{
	/// <summary>
	/// Allows you to manage templates.
	/// </summary>
	/// <remarks>
	/// See https://sendgrid.com/docs/API_Reference/Web_API_v3/Transactional_Templates/templates.html
	/// </remarks>
	public class Templates
	{
		private readonly string _endpoint;
		private readonly Pathoschild.Http.Client.IClient _client;

		/// <summary>
		/// Initializes a new instance of the <see cref="Templates" /> class.
		/// </summary>
		/// <param name="client">SendGrid Web API v3 client</param>
		/// <param name="endpoint">Resource endpoint</param>
		public Templates(Pathoschild.Http.Client.IClient client, string endpoint = "/templates")
		{
			_endpoint = endpoint;
			_client = client;
		}

		/// <summary>
		/// Create a template.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The <see cref="Template" />.
		/// </returns>
		public Task<Template> CreateAsync(string name, CancellationToken cancellationToken = default(CancellationToken))
		{
			var data = new JObject
			{
				{ "name", name }
			};
			return _client
				.PostAsync(_endpoint)
				.WithJsonBody(data)
				.WithCancellationToken(cancellationToken)
				.AsSendGridObject<Template>();
		}

		/// <summary>
		/// Retrieve all templates.
		/// </summary>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// An array of <see cref="Template" />.
		/// </returns>
		public Task<Template[]> GetAllAsync(CancellationToken cancellationToken = default(CancellationToken))
		{
			return _client
				.GetAsync(_endpoint)
				.WithCancellationToken(cancellationToken)
				.AsSendGridObject<Template[]>("templates");
		}

		/// <summary>
		/// Retrieve a template.
		/// </summary>
		/// <param name="templateId">The template identifier.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The <see cref="Template" />.
		/// </returns>
		public Task<Template> GetAsync(string templateId, CancellationToken cancellationToken = default(CancellationToken))
		{
			return _client
				.GetAsync(string.Format("{0}/{1}", _endpoint, templateId))
				.WithCancellationToken(cancellationToken)
				.AsSendGridObject<Template>();
		}

		/// <summary>
		/// Update a template.
		/// </summary>
		/// <param name="templateId">The template identifier.</param>
		/// <param name="name">The name.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The <see cref="Template" />.
		/// </returns>
		public Task<Template> UpdateAsync(string templateId, string name, CancellationToken cancellationToken = default(CancellationToken))
		{
			var data = new JObject
			{
				{ "name", name }
			};
			return _client
				.PatchAsync(string.Format("{0}/{1}", _endpoint, templateId))
				.WithJsonBody(data)
				.WithCancellationToken(cancellationToken)
				.AsSendGridObject<Template>();
		}

		/// <summary>
		/// Delete a template.
		/// </summary>
		/// <param name="templateId">The template identifier.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The async task.
		/// </returns>
		public Task DeleteAsync(string templateId, CancellationToken cancellationToken = default(CancellationToken))
		{
			return _client
				.DeleteAsync(string.Format("{0}/{1}", _endpoint, templateId))
				.WithCancellationToken(cancellationToken)
				.AsResponse();
		}

		/// <summary>
		/// Create a template version.
		/// </summary>
		/// <param name="templateId">The template identifier.</param>
		/// <param name="name">The name.</param>
		/// <param name="subject">The subject.</param>
		/// <param name="htmlContent">Content of the HTML.</param>
		/// <param name="textContent">Content of the text.</param>
		/// <param name="isActive">if set to <c>true</c> [is active].</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The <see cref="TemplateVersion" />.
		/// </returns>
		public Task<TemplateVersion> CreateVersionAsync(string templateId, string name, string subject, string htmlContent, string textContent, bool isActive, CancellationToken cancellationToken = default(CancellationToken))
		{
			var data = new JObject
			{
				{ "name", name },
				{ "subject", subject },
				{ "html_content", htmlContent },
				{ "plain_content", textContent },
				{ "active", isActive ? 1 : 0 }
			};
			return _client
				.PostAsync(string.Format("{0}/{1}/versions", _endpoint, templateId))
				.WithJsonBody(data)
				.WithCancellationToken(cancellationToken)
				.AsSendGridObject<TemplateVersion>();
		}

		/// <summary>
		/// Activate a version.
		/// </summary>
		/// <param name="templateId">The template identifier.</param>
		/// <param name="versionId">The version identifier.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The <see cref="TemplateVersion" />.
		/// </returns>
		public Task<TemplateVersion> ActivateVersionAsync(string templateId, string versionId, CancellationToken cancellationToken = default(CancellationToken))
		{
			return _client
				.PostAsync(string.Format("{0}/{1}/versions/{2}/activate", _endpoint, templateId, versionId))
				.WithCancellationToken(cancellationToken)
				.AsSendGridObject<TemplateVersion>();
		}

		/// <summary>
		/// Retrieve a template version.
		/// </summary>
		/// <param name="templateId">The template identifier.</param>
		/// <param name="versionId">The version identifier.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The <see cref="TemplateVersion" />.
		/// </returns>
		public Task<TemplateVersion> GetVersionAsync(string templateId, string versionId, CancellationToken cancellationToken = default(CancellationToken))
		{
			return _client
				.GetAsync(string.Format("{0}/{1}/versions/{2}", _endpoint, templateId, versionId))
				.WithCancellationToken(cancellationToken)
				.AsSendGridObject<TemplateVersion>();
		}

		/// <summary>
		/// Update a template version.
		/// </summary>
		/// <param name="templateId">The template identifier.</param>
		/// <param name="versionId">The version identifier.</param>
		/// <param name="name">The name.</param>
		/// <param name="subject">The subject.</param>
		/// <param name="htmlContent">Content of the HTML.</param>
		/// <param name="textContent">Content of the text.</param>
		/// <param name="isActive">The is active.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The <see cref="TemplateVersion" />.
		/// </returns>
		public Task<TemplateVersion> UpdateVersionAsync(string templateId, string versionId, string name = null, string subject = null, string htmlContent = null, string textContent = null, bool? isActive = null, CancellationToken cancellationToken = default(CancellationToken))
		{
			var data = new JObject();
			if (!string.IsNullOrEmpty(name)) data.Add("name", name);
			if (!string.IsNullOrEmpty(subject)) data.Add("subject", subject);
			if (!string.IsNullOrEmpty(htmlContent)) data.Add("html_content", htmlContent);
			if (!string.IsNullOrEmpty(textContent)) data.Add("plain_content", textContent);
			if (isActive.HasValue) data.Add("active", isActive.Value ? 1 : 0);

			return _client
				.PatchAsync(string.Format("{0}/{1}/versions/{2}", _endpoint, templateId, versionId))
				.WithJsonBody(data)
				.WithCancellationToken(cancellationToken)
				.AsSendGridObject<TemplateVersion>();
		}

		/// <summary>
		/// Delete a template version.
		/// </summary>
		/// <param name="templateId">The template identifier.</param>
		/// <param name="versionId">The version identifier.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The async task.
		/// </returns>
		public Task DeleteVersionAsync(string templateId, string versionId, CancellationToken cancellationToken = default(CancellationToken))
		{
			return _client
				.DeleteAsync(string.Format("{0}/{1}/versions/{2}", _endpoint, templateId, versionId))
				.WithCancellationToken(cancellationToken)
				.AsResponse();
		}
	}
}
