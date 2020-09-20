using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace XMLLab.Migrations {
    public abstract class XmlMigrationOperation {
        public abstract string GetOperationDescription();
        public abstract bool Execute(XmlContext context);
    }
}
