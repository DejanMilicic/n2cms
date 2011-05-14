﻿using System.Linq;
using System.Web;
using N2.Collections;
using N2.Definitions;
using N2.Edit;
using N2.Edit.FileSystem.Items;
using N2.Engine;
using N2.Web;

namespace N2.Management.Content.Navigation
{
	[Service(typeof(IAjaxService))]
	public class ChildrenAjaxService : IAjaxService
	{
		private readonly Navigator navigator;
		private readonly VirtualNodeFactory nodes;
		private readonly IUrlParser urls;

		public ChildrenAjaxService(Navigator navigator, VirtualNodeFactory nodes, IUrlParser urls)
		{
			this.navigator = navigator;
			this.nodes = nodes;
			this.urls = urls;
		}

		#region IAjaxService Members

		public string Name
		{
			get { return "children"; }
		}

		public bool RequiresEditAccess
		{
			get { return true; }
		}

		/// <summary>Gets whether request's HTTP method is valid for this service.</summary>
		public bool IsValidHttpMethod(string httpMethod)
		{
			return httpMethod == "POST";
		}

		public void Handle(HttpContextBase context)
		{
			string path = context.Request["path"];
			var filter = CreateFilter(context.Request["filter"]);
			var parent = navigator.Navigate(urls.StartPage, path);
			var childItems = filter.Pipe(parent.GetChildren().Union(nodes.GetChildren(path)));
			var children = childItems.Select(c => ToJson(c)).ToArray();

			context.Response.ContentType = "application/json";
			context.Response.Write("{\"path\":\"" + Encode(parent.Path) + "\", \"children\":[" + string.Join(", ", children) + "]}");
		}

		private ItemFilter CreateFilter(string filter)
		{
			FilterHelper filterIs = Filter.Is;
			switch (filter)
			{
				case "any":
					return filterIs.All(filterIs.Not(filterIs.Part()), filterIs.Not(filterIs.Type<ISystemNode>()));
				case "parts":
					return filterIs.All(filterIs.Part(), filterIs.Not(filterIs.Type<ISystemNode>()));
				case "pages":
					return filterIs.All(filterIs.Page(), filterIs.Not(filterIs.Type<ISystemNode>()));
				case "files":
					return filterIs.Type<File>();
				case "directories":
					return filterIs.Type<AbstractDirectory>();
				case "io":
					return filterIs.Type<AbstractNode>();
				default:
					return filterIs.Anything();
			}
		}

		private static string ToJson(ContentItem c)
		{
			return string.Format("{{\"id\":\"{0}\", \"name\":\"{1}\", \"title\":\"{2}\"}}", c.ID, Encode(c.Name), Encode(c.Title));
		}

		private static string Encode(string text)
		{
			return text.Replace("\\", "\\\\").Replace("\"", "\\\"").Replace("\n", "\\n").Replace("\r", "\\r");
		}

		#endregion
	}
}