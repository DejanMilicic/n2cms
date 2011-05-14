﻿using System;
using N2.Definitions;
using N2.Definitions.Runtime;

namespace N2.Web.Mvc
{
	public class Builder<T> where T : IContainable
	{
		string PropertyName { get; set; }
		ContentRegistration Registration { get; set; }


		public Builder(string propertyName, ContentRegistration re)
		{
			this.PropertyName = propertyName;
			this.Registration = re;
		}


		public Builder<T> Configure(Action<T> configurationExpression)
		{
			if (Registration != null && configurationExpression != null)
				Registration.Configure(PropertyName, configurationExpression);

			return this;
		}
	}
}