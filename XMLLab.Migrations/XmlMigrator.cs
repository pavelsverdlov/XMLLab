using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XMLLab.Migrations {
    public class XmlMigrator {
        readonly XmlContext context;
        readonly IEnumerable<XmlMigration> migrations;

        public XmlMigrator(XmlContext context, IEnumerable<XmlMigration> migrations) {
            this.context = context;
            this.migrations = migrations;
        }

        public void Migrate() {
            foreach (var m in migrations.OrderBy(x=>x.ApplicableVersion)) {
                if (context.Version == m.ApplicableVersion) {
                    m.Migrate(context);
                }
            }
        }



    }
}
