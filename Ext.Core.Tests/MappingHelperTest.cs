using System;
using System.Collections.Generic;
using System.Linq;
using Ext.Core.Mapping;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Win32;
using Omu.ValueInjecter;

namespace Ext.Core.Tests
{
    [TestClass]
    public class MappingHelperTest
    {
        #region Nested classes

        public class TestSource
        {
            public int Id { get; set; }
            public ContainerSource DataContainer { get; set; }
            public List<ContainerSource> ListOfContainers { get; set; }
        }

        public class TestDestination
        {
            public int Id { get; set; }
            public ContainerSource DataContainer { get; set; }
            public List<ContainerDestination> ListOfContainers { get; set; }
        }

        public class ContainerSource
        {
            public int Number { get; set; }            
            public string Text { get; set; }
            public DateTime DateTime { get; set; }
            public decimal Fraction { get; set; }
        }

        public class ContainerDestination
        {
            public int Number { get; set; }
            public string Text { get; set; }
            public double OhterNumber { get; set; }
        }

        public class SourceWithPublicGetAndSet
        {
            public string SecretText { get; set; }
        }

        public class DestinationWithPrivateGetAndPublicSet
        {
            public string SecretText { private get; set; }

            public string GetSecretTest()
            {
                return this.SecretText;
            }
        }

        public class ContainerWithStirngInt
        {
            public string Number { get; set; }
            public string Text { get; set; }
        }

        public class ContainerWithIntInt
        {
            public int Number { get; set; }
            public string Text { get; set; }
        }

        public class SourceWithDict
        {
            public Dictionary<int, string> Dictionary { get; set; }
        }

        public class TargetWithDict
        {
            public Dictionary<int, string> Dictionary { get; set; }
        }

        public class SourceWithCircularReference
        {
            public SourceChildWithCircularReference Child { get; set; }
            public List<SourceChildWithCircularReference> Children { get; set; }

            public SourceWithCircularReference()
            {
                Children = new List<SourceChildWithCircularReference>();
            }
        }

        public class SourceChildWithCircularReference
        {
            public SourceWithCircularReference Source { get; set; }
        }

        public class TargetForCircularReference
        {
            public TargetChildForCircularReference Child { get; set; }
            public List<TargetChildForCircularReference> Children { get; set; }

            public TargetForCircularReference()
            {
                Children = new List<TargetChildForCircularReference>();
            }
        }

        public class TargetChildForCircularReference
        {
            public TargetForCircularReference Source { get; set; }
        }

        public class SourceWithFlattenedProperties
        {
            public SourceWithFlattenedPropertiesChild User { get; set; }
        }

        public class SourceWithFlattenedPropertiesChild
        {
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public SourceWithFlattenedPropertiesChildChild Address { get; set; }
        }

        public class SourceWithFlattenedPropertiesChildChild
        {
            public string Country { get; set; }
        }

        public class TargetWithFlattenedProperties
        {
            public string UserFirstName { get; set; }
            public string UserLastName { get; set; }
            public string UserAddressCountry { get; set; }
        }

        #endregion

        [TestMethod]
        public void should_map_with_cloning()
        {
            var source = new TestSource();
            source.Id = 22;
            source.DataContainer = new ContainerSource() { DateTime = DateTime.Now, Number = 33, Text = "Clone me" };

            source.ListOfContainers = new List<ContainerSource>();

            source.ListOfContainers.Add(new ContainerSource() { DateTime = DateTime.Now, Number = 201, Text = "Item 1" });
            source.ListOfContainers.Add(new ContainerSource() { DateTime = DateTime.Now, Number = 202, Text = "Item 2" });
            source.ListOfContainers.Add(new ContainerSource() { DateTime = DateTime.Now, Number = 203, Text = "Item 3" });


            var destination = new TestDestination();

            MappingHelper.Map(source, destination, new MappingOptions().WithMappingLevel(2));

            Assert.AreEqual(destination.Id, source.Id);
            Assert.AreEqual(destination.DataContainer.Number, source.DataContainer.Number);
            Assert.AreEqual(destination.ListOfContainers.Count, destination.ListOfContainers.Count);
            Assert.AreEqual(destination.ListOfContainers.Last().Number, source.ListOfContainers.Last().Number);
        }

        [TestMethod]
        public void should_map_when_property_has_private_get_public_set()
        {
            var source = new DestinationWithPrivateGetAndPublicSet();
            source.SecretText = "MySecret";

            var destination = new DestinationWithPrivateGetAndPublicSet();

            MappingHelper.Map(source, destination);

            Assert.AreEqual(destination.GetSecretTest(), "MySecret");
        }

        [TestMethod]
        public void should_map_properties_with_string_to_int_default_converter()
        {
            var source = new ContainerWithStirngInt {Number = "12", Text = "twelve"};
            var destination = new ContainerWithIntInt {Number = -1, Text = "default"};

            MappingHelper.Map(source, destination);

            Assert.AreEqual(destination.Number, 12);
            Assert.AreEqual(destination.Text, source.Text);
        }

        [TestMethod]
        public void should_work_with_dictionaries()
        {
            var sourse = new SourceWithDict();
            sourse.Dictionary = new Dictionary<int, string>();

            sourse.Dictionary.Add(1, "One");
            sourse.Dictionary.Add(2, "Two");
            sourse.Dictionary.Add(3, "Threee");

            var target = new TargetWithDict();

            MappingHelper.Map(sourse, target);

            Assert.AreSame(sourse.Dictionary[2], target.Dictionary[2]);
        }

        [TestMethod]
        public void should_handle_circular_references()
        {
            var source = new SourceWithCircularReference();
            var child = new SourceChildWithCircularReference();

            source.Child = child;
            child.Source = source;
            source.Children.Add(child);

            //MappingHelper.Map(source, new TargetForCircularReference());

            var target = new TargetForCircularReference();
            target.InjectFrom<FlatLoopValueInjection>(source);

            Assert.IsNotNull(target);
        }

        [TestMethod]
        public void should_flatten_object()
        {
            var source = new SourceWithFlattenedProperties();
            source.User = new SourceWithFlattenedPropertiesChild() {FirstName = "Yegor", LastName = "Sytnyk"};
            source.User.Address = new SourceWithFlattenedPropertiesChildChild(){Country = "Ukraine"};
            var target = new TargetWithFlattenedProperties();

            target.InjectFrom<FlatLoopValueInjection>(source);

            var mappingHelper = new MappingHelper();

            var test = mappingHelper.GetPropertiesWithFlattenedNested(source, true);

            Assert.IsNotNull(test);
        }

        [Ignore]
        [TestMethod]
        public void performance_test()
        {
            int iterations = 1000000;

            var containerSource = new ContainerSource();
            containerSource.Number = 359;
            containerSource.DateTime = DateTime.Now;
            containerSource.Text = "Bla-bla-blah";
            containerSource.Fraction = 0.56m;

            var containerDestination = new ContainerDestination();

            var start = DateTime.Now;

            for (int i = 0; i < iterations; i++)
            {
                MappingHelper.Map(containerSource, containerDestination);
            }

            var end = DateTime.Now;

            var time = (end - start).TotalSeconds;

            Assert.IsTrue(time < 7);
        }

        [Ignore]
        [TestMethod]
        public void performance_test_compare()
        {
            int iterations = 1000000;

            var containerSource = new ContainerSource();
            containerSource.Number = 359;
            containerSource.DateTime = DateTime.Now;
            containerSource.Text = "Bla-bla-blah";
            containerSource.Fraction = 0.56m;

            var containerDestination = new ContainerDestination();

            var start = DateTime.Now;

            for (int i = 0; i < iterations; i++)
            {
                containerDestination.InjectFrom(containerSource);
            }

            var end = DateTime.Now;

            var time = (end - start).TotalSeconds;

            Assert.IsTrue(time < 6);
        }
    }
}
