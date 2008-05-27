using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

[assembly: WebResource("N2.Resources.jquery-1.2.5.min.js", "text/javascript")]

namespace N2.Resources
{
	/// <summary>
	/// Methods to register styles and javascripts.
	/// </summary>
	public class Register
	{
		/// <summary>Register an embedded style sheet reference in the page's header.</summary>
		/// <param name="page">The page onto which to register the style sheet.</param>
		/// <param name="type">The type whose assembly contains the embedded style sheet.</param>
		/// <param name="resourceName">The name of the embedded resource.</param>
		public static void StyleSheet(Page page, Type type, string resourceName)
		{
			StyleSheet(page, page.ClientScript.GetWebResourceUrl(type, resourceName), Media.All);
		}

		public static void Parts(Page page)
		{

		}

		/// <summary>Register a style sheet reference in the page's header.</summary>
		/// <param name="page">The page onto which to register the style sheet.</param>
		/// <param name="resourceUrl">The url to the style sheet to register.</param>
		public static void StyleSheet(Page page, string resourceUrl)
		{
			StyleSheet(page, resourceUrl, Media.All);
		}

		/// <summary>Register a style sheet reference in the page's header with media type.</summary>
		/// <param name="page">The page onto which to register the style sheet.</param>
		/// <param name="resourceUrl">The url to the style sheet to register.</param>
		/// <param name="media">The media type to assign, e.g. print.</param>
		public static void StyleSheet(Page page, string resourceUrl, Media media)
		{
			resourceUrl = Utility.ToAbsolute(resourceUrl);

			if (page.Items[resourceUrl] == null)
			{
				PlaceHolder holder = GetPlaceHolder(page);

				HtmlLink link = new HtmlLink();
				link.Href = resourceUrl;
				link.Attributes["type"] = "text/css";
				link.Attributes["media"] = media.ToString().ToLower();
				link.Attributes["rel"] = "stylesheet";
				holder.Controls.Add(link);

				page.Items[resourceUrl] = null;
			}
		}

		/// <summary>Register an embedded javascript resource reference in the page header.</summary>
		/// <param name="page">The page in whose header to register the javascript.</param>
		/// <param name="type">The type in whose assembly the javascript is embedded.</param>
		/// <param name="resourceName">The name of the embedded resource.</param>
		public static void JavaScript(Page page, Type type, string resourceName)
		{
			JavaScript(page, page.ClientScript.GetWebResourceUrl(type, resourceName));
		}

		/// <summary>Register an embedded javascript resource reference in the page header with options.</summary>
		/// <param name="page">The page in whose header to register the javascript.</param>
		/// <param name="type">The type in whose assembly the javascript is embedded.</param>
		/// <param name="resourceName">The name of the embedded resource.</param>
		/// <param name="options">Options flag.</param>
		public static void JavaScript(Page page, Type type, string resourceName, ScriptOptions options)
		{
			JavaScript(page, page.ClientScript.GetWebResourceUrl(type, resourceName), options);
		}

		/// <summary>Registers a script block on a page.</summary>
		/// <param name="page">The page onto which to added the script.</param>
		/// <param name="script">The script to add.</param>
		/// <param name="position">Where to add the script.</param>
		/// <param name="options">Script registration options.</param>
		public static void JavaScript(Page page, string script, ScriptPosition position, ScriptOptions options)
		{
			if (position == ScriptPosition.Header)
			{
				JavaScript(page, script, options);
			}
			else if (position == ScriptPosition.Bottom)
			{
				string key = script.GetHashCode().ToString();
				if (Is(options, ScriptOptions.None))
					page.ClientScript.RegisterClientScriptBlock(typeof (Register), key, script);
				else if (Is(options, ScriptOptions.ScriptTags))
					page.ClientScript.RegisterClientScriptBlock(typeof (Register), key, script, true);
				else if (Is(options, ScriptOptions.DocumentReady))
				{
					JQuery(page);
					page.ClientScript.RegisterClientScriptBlock(typeof (Register), key, EmbedDocumentReady(script), true);
				}
				else if (Is(options, ScriptOptions.Include))
					page.ClientScript.RegisterClientScriptInclude(key, script);
				else
					throw new ArgumentException("options");
			}
			else
				throw new ArgumentException("position");
		}

		private static string EmbedDocumentReady(string script)
		{
			return "$(document).ready(function(){" + script + "});";
		}

		public static void JavaScript(Page page, string script, ScriptOptions options)
		{
			if (page.Items[script] == null)
			{
				PlaceHolder holder = GetPlaceHolder(page);

				if (Is(options, ScriptOptions.Include))
				{
					AddScriptInclude(page, script, holder, Is(options, ScriptOptions.Prioritize));
				}
				else if (Is(options, ScriptOptions.None))
				{
					holder.Page.Items[script] = AddString(script, holder, Is(options, ScriptOptions.Prioritize));
				}
				else
				{
					Script scriptHolder = GetScriptHolder(page);
					if (Is(options, ScriptOptions.ScriptTags))
					{
						holder.Page.Items[script] = AddString(script + Environment.NewLine, scriptHolder, Is(options, ScriptOptions.Prioritize));
					}
					else if (Is(options, ScriptOptions.DocumentReady))
					{
						JQuery(page);
						holder.Page.Items[script] = AddString(EmbedDocumentReady(script) + Environment.NewLine, scriptHolder, Is(options, ScriptOptions.Prioritize));
					}
				}
			}
		}

		private class Script : Control
		{
			protected override void Render(HtmlTextWriter writer)
			{
				writer.Write("<script type='text/javascript'>");
				writer.Write(Environment.NewLine);
				writer.Write("<!--");
				writer.Write(Environment.NewLine);
				base.Render(writer);
				writer.Write(Environment.NewLine);
				writer.Write("//-->");
				writer.Write(Environment.NewLine);
				writer.Write("</script>");
			}
		}

		//private class DocumentReady : Control
		//{
		//    protected override void Render(HtmlTextWriter writer)
		//    {
		//        writer.Write("$(document).ready(function(){");
		//        base.Render(writer);
		//        writer.Write("});");
		//    }
		//}

		private static Literal AddString(string script, Control holder, bool priority)
		{
			Literal l = new Literal();
			l.Text = script;
			if(priority)
				holder.Controls.AddAt(0, l);
			else 
				holder.Controls.Add(l);
			return l;
		}

		private static void AddScriptInclude(Page page, string resourceUrl, Control holder, bool priority)
		{
			HtmlGenericControl script = new HtmlGenericControl("script");
			page.Items[resourceUrl] = script;

			resourceUrl = Utility.ToAbsolute(resourceUrl);

			script.Attributes["src"] = resourceUrl;
			script.Attributes["type"] = "text/javascript";
			if(priority)
				holder.Controls.AddAt(0, script);
			else
				holder.Controls.Add(script);
		}

		/// <summary>Registers a script reference in the page's header.</summary>
		/// <param name="page">The page onto which to register the javascript.</param>
		/// <param name="resourceUrl">The url to the javascript to register.</param>
		public static void JavaScript(Page page, string resourceUrl)
		{
			JavaScript(page, resourceUrl, ScriptOptions.Include);
		}

		public static void JQuery(Page page)
		{
#if DEBUG
			JavaScript(page, Utility.ToAbsolute("~/Edit/Js/jquery-1.2.5.js"), ScriptOptions.Prioritize | ScriptOptions.Include);
#else
			JavaScript(page, typeof(Register), "N2.Resources.jquery-1.2.5.min.js", ScriptOptions.Prioritize | ScriptOptions.Include);
#endif
		}

		private static Script GetScriptHolder(Page page)
		{
			PlaceHolder holder = GetPlaceHolder(page);
			Script scripts = page.Items["N2.Resources.scripts"] as Script;
			if (scripts == null)
			{
				page.Items["N2.Resources.scripts"] = scripts = new Script();
				holder.Controls.Add(scripts);
			}
			return scripts;
		}

		private static PlaceHolder GetPlaceHolder(Page page)
		{
			PlaceHolder holder = page.Items["N2.Resources.holder"] as PlaceHolder;
			if (holder == null)
			{
				if (page.Header == null)
					throw new N2Exception("Couldn't find the page header. The register command needs the tag <header runat='server'> somewhere in the page template, master page or a user control.");

				page.Items["N2.Resources.holder"] = holder = new PlaceHolder();
				if (page.Header.Controls.Count > 0)
					page.Header.Controls.AddAt(1, holder);
				else
					page.Header.Controls.Add(holder);
			}
			return holder;
		}

		#region TabPanel
		private static readonly string tabPanelFormat = @"$('{0}').n2tabs(null,location.hash);";

		public static void TabPanel(Page page, string selector, bool registerTabCss)
		{
			if (page.Items["N2.Resources.TabPanel"] == null)
			{
				JQuery(page);
				JavaScript(page, typeof(Register), "N2.Resources.TabPanel.js");
				string script = string.Format(tabPanelFormat, selector);
				JavaScript(page, script, ScriptOptions.DocumentReady);
				page.Items["N2.Resources.TabPanel"] = new object();
				if (registerTabCss)
				{
					StyleSheet(page, typeof(Register), "N2.Resources.TabPanel.css");
				}
			}
		}

		public static void TabPanel(Page page, string selector)
		{
			TabPanel(page, selector, true);
		}

		private static bool Is(ScriptOptions options, ScriptOptions expectedOption)
		{
			return (options & expectedOption) == expectedOption;
		}
		#endregion
	}
}