using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using TestHelper;
using MultiLineString;

namespace MultiLineString.Test
{
    [TestClass]
    public class UnitTest : CodeFixVerifier
    {

        //No diagnostics expected to show up
        [TestMethod]
        public void TestDiagnostic1()
        {
            var test = @"";

            VerifyCSharpDiagnostic(test);
        }

        [TestMethod]
        public void TestDiagnostic2()
        {
            var test = @"
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Diagnostics;

    namespace ConsoleApplication1
    {
        class TypeName
        {
            string query = @""SELECT foo, bar
FROM table
WHERE id = 42"";
        }
    }";
            var expected = new DiagnosticResult
            {
                Id = "MultiLineString",
                Message = "Convert to single line literal",
                Severity = DiagnosticSeverity.Hidden,
                Locations =
                    new[] {
                            new DiagnosticResultLocation("Test0.cs", 13, 28)
                        }
            };

            VerifyCSharpDiagnostic(test, expected);
        }

        [TestMethod]
        public void TestFix2()
        {
            var test = @"
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Diagnostics;

    namespace ConsoleApplication1
    {
        class TypeName
        {
            string query = @""SELECT foo, bar
    FROM table
    WHERE id = 42"";
        }
    }";
            var fixtest = @"
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Diagnostics;

    namespace ConsoleApplication1
    {
        class TypeName
        {
            string query = ""SELECT foo, bar FROM table WHERE id = 42 "";
        }
    }";
            VerifyCSharpFix(test, fixtest);
        }


        [TestMethod]
        public void TestDiagnostic3()
        {
            var test = @"
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Diagnostics;

    namespace ConsoleApplication1
    {
        class TypeName
        {
string FRAGMENT_PREAMBLE = @""using System;
            using System.Collections.Generic;
            using System.Linq;
            using System.Text;
            using System.Threading.Tasks;"";
        }
    }";
            var expected = new DiagnosticResult
            {
                Id = "MultiLineString",
                Message = "Convert to single line literal",
                Severity = DiagnosticSeverity.Hidden,
                Locations =
                    new[] {
                            new DiagnosticResultLocation("Test0.cs", 13, 28)
                        }
            };

            VerifyCSharpDiagnostic(test, expected);
        }

        [TestMethod]
        public void TestFix3()
        {
            var test = @"
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Diagnostics;

    namespace ConsoleApplication1
    {
        class TypeName
        {
string FRAGMENT_PREAMBLE = @""using System;
            using System.Collections.Generic;
            using System.Linq;
            using System.Text;
            using System.Threading.Tasks;"";
        }
    }";
            var fixtest = @"
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Diagnostics;

    namespace ConsoleApplication1
    {
        class TypeName
        {
string FRAGMENT_PREAMBLE = ""using System; using System.Collections.Generic; using System.Linq; using System.Text; using System.Threading.Tasks; "";
        }
    }";
            VerifyCSharpFix(test, fixtest);
        }

        [TestMethod]
        public void TestDiagnostic4()
        {
            var test = @"
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Diagnostics;

    namespace ConsoleApplication1
    {
        class TypeName
        {
            string query = @""SELECT foo, bar FROM table WHERE id = 42"";
        }
    }";
            VerifyCSharpDiagnostic(test);
        }

        [TestMethod]
        public void TestDiagnostic5()
        {
            var test = @"
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Diagnostics;

    namespace ConsoleApplication1
    {
        class TypeName
        {
            string query = @""SELECT foo, bar \n FROM table \n WHERE id = 42"";
        }
    }";
            VerifyCSharpDiagnostic(test);
        }

        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new MultiLineStringCodeFixProvider();
        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new MultiLineStringAnalyzer();
        }
    }
}
