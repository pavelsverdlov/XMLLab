using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Xml.Serialization;

using WPFLab;

using XMLLab.Migrations;
using XMLLab.Migrations.Operations;

using Xunit;

namespace XMLLab.Tests {
    public class XMLMigrationsUnitTest {
        class AddNewField : XmlMigration {
            public AddNewField() : base(new Version(0, 0, 0, 1), new Version(0, 0, 0, 2)) {
            }

            protected override void Down([NotNull] XmlMigrationBuilder migrationBuilder) {

            }

            protected override void Up([NotNull] XmlMigrationBuilder migrationBuilder) {
                migrationBuilder.AddElement(new AddElementData(nameof(AddNewFieldSomeClass.NewField), "new value",
                    nameof(AddNewFieldXmlTest.SomeClass)));
            }
        }
        public struct AddNewFieldSomeClass {
            public string SomeField { get; set; }
            public string NewField { get; set; }
        }
        public struct AddNewFieldXmlTest {
            public AddNewFieldSomeClass SomeClass { get; set; }
        }

        [Fact]
        public void ADD_NEW_FIELD_TEST() {
            var xml =
@"<?xml version='1.0' encoding='utf-8'?>
<AddNewFieldXmlTest version='0.0.0.1'>
    <SomeClass>
        <SomeField>Value</SomeField>
     </SomeClass>
</AddNewFieldXmlTest>";
            XmlMigration addNewField = new AddNewField();

            var context = new XmlContext(xml);

            var migrator = new XmlMigrator(context, new[] { addNewField });

            migrator.Migrate();

            var s = new XmlSerializer(typeof(AddNewFieldXmlTest));
            var obj = (AddNewFieldXmlTest)s.Deserialize(context.Serialize());

            Assert.Equal("new value", obj.SomeClass.NewField);
            Assert.Equal("Value", obj.SomeClass.SomeField);
        }


        public struct ReplaceFieldSomeClass {
            public string NewField { get; set; }
            public string SomeField { get; set; }
        }
        public struct ReplaceFieldXmlRoot {
            public ReplaceFieldSomeClass SomeClass { get; set; }
        }
        class ReplaceField : XmlMigration {
            public ReplaceField() : base(new Version(0, 0, 0, 1), new Version(0, 0, 0, 2)) {
            }
            protected override void Down([NotNull] XmlMigrationBuilder migrationBuilder) {

            }

            protected override void Up([NotNull] XmlMigrationBuilder migrationBuilder) {
                migrationBuilder.ReplaceElement(
                    new ReplaceElementData(
                        nameof(ReplaceFieldSomeClass.NewField), "new value",
                        nameof(ReplaceFieldSomeClass.SomeField),
                        nameof(ReplaceFieldXmlRoot.SomeClass)));
            }
        }

        [Fact]
        public void REPLACE_FIELD_TEST() {
            var xml =
@"<?xml version='1.0' encoding='utf-8'?>
<ReplaceFieldXmlRoot version='0.0.0.1'>
    <SomeClass>
        <SomeField>Value</SomeField>
     </SomeClass>
</ReplaceFieldXmlRoot>";

            var migration = new ReplaceField();

            var context = new XmlContext(xml);

            var migrator = new XmlMigrator(context, new[] { migration });

            migrator.Migrate();

            var obj = context.DeserializeAs<ReplaceFieldXmlRoot>();

            Assert.Equal("new value", obj.SomeClass.NewField);
            Assert.Null(obj.SomeClass.SomeField);
        }


        public struct ConvertValueClass {
            public float Field { get; set; }
        }
        public struct ConvertValueXmlRoot {
            public ConvertValueClass Class { get; set; }
        }
        class ConvertValue : XmlMigration {
            public ConvertValue() : base(new Version(0, 0, 0, 1), new Version(0, 0, 0, 2)) {
            }
            protected override void Down([NotNull] XmlMigrationBuilder migrationBuilder) {

            }

            protected override void Up([NotNull] XmlMigrationBuilder migrationBuilder) {
                migrationBuilder.ConvertValue(new ValueConverter(
                        nameof(ConvertValueClass.Field),
                        nameof(ConvertValueXmlRoot.Class),
                        Converter
                        ));
            }

            object Converter(string val) {
                var parts = val.Split(' ');
                var last = parts.Last();
                if(!float.TryParse(last, out var result)) {
                    result = -1;
                }
                return result;
            }
        }

        [Fact]
        public void CONVERT_VALUE() {
            var xml =
@"<?xml version='1.0' encoding='utf-8'?>
<ConvertValueXmlRoot version='0.0.0.1'>
    <Class>
        <Field>Convert this text to 3.1415</Field>
     </Class>
</ConvertValueXmlRoot>";

            var migration = new ConvertValue();

            var context = new XmlContext(xml);

            var migrator = new XmlMigrator(context, new[] { migration });

            migrator.Migrate();

            var obj = context.DeserializeAs<ConvertValueXmlRoot>();

            Assert.Equal(3.1415f, obj.Class.Field);
        }
    }

}
