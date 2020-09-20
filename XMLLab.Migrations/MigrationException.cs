using System;

namespace XMLLab.Migrations {
    public class MigrationException : Exception {
        public MigrationException(string message) : base(message) {
        }

        public static MigrationException GetOperationFailed(string descr) =>
            new MigrationException($"Operation failed. {descr}");

        internal static Exception GetInvalidMigrationVersions(Version from, Version to) =>
            new MigrationException($"Invalid migration versions. {from} -> {to}");
    }
}
