using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace XMLLab.Migrations {
    public abstract class XmlMigration {
        public Version ApplicableVersion => from;

        readonly XmlMigrationBuilder upBuilder;
        readonly XmlMigrationBuilder downBuilder;
        readonly Version from;
        readonly Version to;

        protected XmlMigration(Version from, Version to) {
            if(from == to) {
                throw MigrationException.GetInvalidMigrationVersions(from, to);
            }
            upBuilder = new XmlMigrationBuilder();
            downBuilder = new XmlMigrationBuilder();
            this.from = from;
            this.to = to;
        }

        public void Migrate(XmlContext context) {
            Up(upBuilder);
            Down(downBuilder);

            var upOperations = upBuilder.Build();
            var downOperations = downBuilder.Build();

            Exception exception = null;
            try {
                foreach (var op in upOperations) {
                    if (!op.Execute(context)) {
                        exception = MigrationException.GetOperationFailed(op.GetOperationDescription());
                    }
                }
            } catch (Exception ex) {
                exception = ex;
            }

            if (exception != null) {
                foreach (var op in downOperations) {
                    if (op.Execute(context)) {

                    }
                }

                throw exception;
            }

            context.SetVersion(to);
        }

        protected abstract void Down([NotNull] XmlMigrationBuilder migrationBuilder);
        protected abstract void Up([NotNull] XmlMigrationBuilder migrationBuilder);
    }
}
