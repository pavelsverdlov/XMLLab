using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace XMLLab.Migrations.Operations {
    public readonly struct AddElementData {
        public AddElementData(string fieldName, string value, string parentName) {
            FieldName = fieldName;
            Value = value;
            ParentName = parentName;
        }

        public string FieldName { get; }
        public string Value { get; }
        public string ParentName { get; }

        public override string ToString()
            => $"[FieldName={FieldName};ParentName={ParentName}]";
    }

    class AddElementMigrationOperation : XmlMigrationOperation {
        readonly AddElementData data;

        public AddElementMigrationOperation(AddElementData data) {
            this.data = data;
        }

        public override bool Execute(XmlContext context) {
            var newfield = new XElement(data.FieldName, data.Value);
            foreach (var e in context.Document.Descendants(data.ParentName)) {
                e.Add(newfield);
            }
            return true;
        }

        public override string GetOperationDescription() {
            return $"AddField{data}";
        }
    }
    public readonly struct ReplaceElementData {
        public ReplaceElementData(string fieldName, string value, string fieldNameToReplace, string parentName) {
            FieldName = fieldName;
            Value = value;
            ParentName = parentName;
            FieldNameToReplace = fieldNameToReplace;
        }

        public string FieldName { get; }
        public string Value { get; }
        public string ParentName { get; }
        public string FieldNameToReplace { get; }

        public override string ToString()
            => $"[FieldName={FieldName};ParentName={ParentName}]";
    }

    class ReplaceElementMigrationOperation : XmlMigrationOperation {
        readonly ReplaceElementData data;

        public ReplaceElementMigrationOperation(ReplaceElementData data) {
            this.data = data;
        }

        public override bool Execute(XmlContext context) {
            var newfield = new XElement(data.FieldName, data.Value);
            foreach (var e in context.Document.Descendants(data.ParentName)) {
                var found = e.Descendants(data.FieldNameToReplace).ToList();
                foreach (var node in found) {
                    node.ReplaceWith(newfield);
                }
            }
            return true;
        }

        public override string GetOperationDescription() {
            return $"ReplaceField{data}";
        }
    }

    public class ValueConverter {
        public ValueConverter(string fieldName, string parentName, Func<string, object> convert) {
            FieldName = fieldName;
            ParentName = parentName;
            this.convert = convert;
        }

        public string FieldName { get; }
        public string ParentName { get; }
        readonly Func<string, object> convert;

        public object Convert(string currentVal) => convert(currentVal);

        public override string ToString()
            => $"[FieldName={FieldName};ParentName={ParentName}]";
    }

    class ConvertValueMigrationOperation : XmlMigrationOperation {
        readonly ValueConverter converter;

        public ConvertValueMigrationOperation(ValueConverter data) {
            this.converter = data;
        }

        public override bool Execute(XmlContext context) {
            foreach (var e in context.Document.Descendants(converter.ParentName)) {
                var found = e.Descendants(converter.FieldName);
                foreach (var node in found) {

                    node.SetValue(converter.Convert(node.Value));
                    //node.ReplaceWith(node);

                }
            }

            var d = context.SerializeText();


            return true;
        }

        public override string GetOperationDescription() {
            return $"ReplaceField{converter}";
        }
    }
}
