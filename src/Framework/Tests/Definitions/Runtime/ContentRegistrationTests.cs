using System.Linq;
using N2.Definitions;
using N2.Definitions.Runtime;
using N2.Definitions.Static;
using N2.Details;
using N2.Web.Mvc.Html;
using N2.Web.UI;
using NUnit.Framework;

namespace N2.Tests.Definitions
{
	[TestFixture]
	public class ContentRegistrationTests
	{
		ContentRegistration registration;
		ItemDefinition sourceDefinition;

		class EmptyItem : ContentItem
		{
		}

		[SetUp]
		public void SetUp()
		{
			registration = new ContentRegistration();
			registration.ContentType = typeof(EmptyItem);
			sourceDefinition = new DefinitionMap().GetOrCreateDefinition(registration.ContentType);
		}

		[Test]
		public void RegisteringCheckBox_AddsEditableCheckBox_ToDefinition()
		{
			registration.CheckBox("Visible", "Show the page in navigation");

			var definition = registration.AppendDefinition(sourceDefinition);

			var editable = (EditableCheckBoxAttribute)definition.Editables.Single();
			Assert.That(editable, Is.InstanceOf<EditableCheckBoxAttribute>());
			Assert.That(editable.Name, Is.EqualTo("Visible"));
			Assert.That(editable.Title, Is.EqualTo(""));
			Assert.That(editable.CheckBoxText, Is.EqualTo("Show the page in navigation"));
		}

		[Test]
		public void RegisteringDateRange_AddsEditableDateRange_ToDefinition()
		{
			registration.DateRange("From", "To", "Opening hours");

			var definition = registration.AppendDefinition(sourceDefinition);

			var editable = (WithEditableDateRangeAttribute)definition.Editables.Single();
			Assert.That(editable, Is.InstanceOf<WithEditableDateRangeAttribute>());
			Assert.That(editable.Name, Is.EqualTo("From"));
			Assert.That(editable.NameEndRange, Is.EqualTo("To"));
			Assert.That(editable.Title, Is.EqualTo("Opening hours"));
		}

		[Test]
		public void RegisteringTitle_AddsEditableTitle_ToDefinition()
		{
			registration.Title("The name of the page");

			var definition = registration.AppendDefinition(sourceDefinition);

			var editable = definition.Editables.Single();
			Assert.That(editable, Is.InstanceOf<WithEditableTitleAttribute>());
			Assert.That(editable.Name, Is.EqualTo("Title"));
			Assert.That(editable.Title, Is.EqualTo("The name of the page"));
		}

		[Test]
		public void Container()
		{
			registration.Tab("Content", "Primary content");

			var definition = registration.AppendDefinition(sourceDefinition);

			var container = definition.Containers.Single();
			Assert.That(container, Is.InstanceOf<TabContainerAttribute>());
			Assert.That(container.Name, Is.EqualTo("Content"));
			Assert.That(((TabContainerAttribute)container).TabText, Is.EqualTo("Primary content"));
		}
	}
}