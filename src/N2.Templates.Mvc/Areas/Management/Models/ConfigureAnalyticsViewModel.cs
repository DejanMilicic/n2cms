﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Reimers.Google.Analytics;

namespace N2.Templates.Mvc.Areas.Management.Models
{
	public class ConfigureAnalyticsViewModel
	{
		public List<Dimension> AllDimensions { get; set; }
		public List<Metric> AllMetrics { get; set; }
		public List<Dimension> SelectedDimensions { get; set; }
		public List<Metric> SelectedMetrics { get; set; }
	}
}