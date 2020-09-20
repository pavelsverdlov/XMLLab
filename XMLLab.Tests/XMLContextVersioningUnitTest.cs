using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

using XMLLab.Migrations;

using Xunit;

namespace XMLLab.Tests {
    public class XMLContextVersioningUnitTest {
        class M2 : XmlMigration {
            public M2() : base(new Version(0, 0, 0, 1), new Version(0, 0, 0, 2)) {
            }
            protected override void Down([NotNull] XmlMigrationBuilder migrationBuilder) {
            }
            protected override void Up([NotNull] XmlMigrationBuilder migrationBuilder) {
            }
        }
        class M3 : XmlMigration {
            public M3() : base(new Version(0, 0, 0, 2), new Version(0, 0, 0, 3)) {
            }
            protected override void Down([NotNull] XmlMigrationBuilder migrationBuilder) {

            }

            protected override void Up([NotNull] XmlMigrationBuilder migrationBuilder) {
            }
        }
        [Fact]
        public void VERSION_UP() {
            var xml = "﻿<?xml version=\"1.0\" encoding=\"utf-8\"?>\r\n<Root version=\"0.0.0.1\"></Root>";
            var context = new XmlContext(xml);
            var migrator = new XmlMigrator(context, new XmlMigration[] { new M2(), new M3() });
            migrator.Migrate();

            var text = context.SerializeText();
            var newcontext = new XmlContext(text);

            Assert.Equal(new Version(0, 0, 0, 3), newcontext.Version);
        }
    }
}
