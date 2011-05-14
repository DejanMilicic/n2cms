﻿using System.Collections.Generic;

namespace N2.Collections
{
	public interface IContentList<T> : IList<T>, INamedList<T>, IPageableList<T> where T : class, INameable
	{
	}
}
