﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace N2.Persistence
{
	/// <summary>
	/// Provides content item specific access to data.
	/// </summary>
	public interface IContentItemRepository : IRepository<ContentItem>
	{
		/// <summary>Gets types of items below a certain item.</summary>
		/// <param name="ancestor">The root level item to include in the search.</param>
		/// <returns>An enumeration of discriminators and number of items with that discriminator.</returns>
		IEnumerable<DiscriminatorCount> FindDescendantDiscriminators(ContentItem ancestor);

		/// <summary>Finds published items below a certain ancestor of a specific type.</summary>
		/// <param name="ancestor">The ancestor whose descendants are searched.</param>
		/// <param name="discriminator">The discriminator the are filtered by.</param>
		/// <returns>An enumeration of items matching the query.</returns>
		IEnumerable<ContentItem> FindDescendants(ContentItem ancestor, string discriminator);
	}

	/// <summary>
	/// Conveys information about discriminators and number of items with that type.
	/// </summary>
	public class DiscriminatorCount
	{
		/// <summary>The discriminator</summary>
		public string Discriminator { get; set; }
		/// <summary>The number of items with that discriminator</summary>
		public int Count { get; set; }
	}
}
