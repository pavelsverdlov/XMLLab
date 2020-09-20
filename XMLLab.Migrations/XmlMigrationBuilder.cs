using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using XMLLab.Migrations.Operations;

namespace XMLLab.Migrations {
    public class XmlMigrationBuilder {

        readonly List<XmlMigrationOperation> operations;
        public XmlMigrationBuilder() {
            operations = new List<XmlMigrationOperation>();
        }

        public XmlMigrationBuilder AddElement(AddElementData data) {
            operations.Add(new AddElementMigrationOperation(data));
            return this;
        }
        public XmlMigrationBuilder ReplaceElement(ReplaceElementData data) {
            operations.Add(new ReplaceElementMigrationOperation(data));
            return this;
        }
        public XmlMigrationBuilder ConvertValue(ValueConverter converter) {
            operations.Add(new ConvertValueMigrationOperation(converter));
            return this;
        }

        public XmlMigrationBuilder AddAttribute() {
            return this;
        }
        public XmlMigrationBuilder RenameTag() {
            return this;
        }

        public IEnumerable<XmlMigrationOperation> Build() => operations;
    }
}
