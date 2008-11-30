using System;
using System.Web;
using System.Web.Caching;
using N2.Persistence;
using N2.Web.UI;

namespace N2.Web
{
	public class CachingUrlParserDecorator : IUrlParser
	{
		readonly IUrlParser inner;
		readonly IPersister persister;
		TimeSpan slidingExpiration = TimeSpan.FromHours(1);
				
		public CachingUrlParserDecorator(IUrlParser inner, IPersister persister)
		{
			this.inner = inner;
			this.persister = persister;
		}

		public event EventHandler<PageNotFoundEventArgs> PageNotFound;

		public ContentItem StartPage
		{
			get { return inner.StartPage; }
		}

		public ContentItem CurrentPage
		{
			get { return inner.CurrentPage; }
		}
		public TimeSpan SlidingExpiration
		{
			get { return slidingExpiration; }
			set { slidingExpiration = value; }
		}

		public string BuildUrl(ContentItem item)
		{
			return inner.BuildUrl(item);
		}

		public bool IsRootOrStartPage(ContentItem item)
		{
			return inner.IsRootOrStartPage(item);
		}

		public TemplateData ResolveTemplate(Url url)
		{
			string key = string.Intern(url.ToString().ToLowerInvariant());

			TemplateData data = HttpRuntime.Cache[key] as TemplateData;
			if(data == null)
			{
				data = inner.ResolveTemplate(url);
				if(data.ID != 0)
					HttpRuntime.Cache.Add(key, data.Detach(), new ContentCacheDependency(persister), Cache.NoAbsoluteExpiration, SlidingExpiration, CacheItemPriority.Normal, null);
			}
			else
			{
				data = data.Attach(persister);
			}
			return data;
		}

		public ContentItem Parse(string url)
		{
			return inner.Parse(url);
		}
	}
}