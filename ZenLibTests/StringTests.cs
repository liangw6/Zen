// <copyright file="StringTests.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace ZenLib.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Numerics;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.Z3;
    using ZenLib;
    using static ZenLib.Language;
    using static ZenLib.Tests.TestHelper;

    /// <summary>
    /// Tests for the string type.
    /// </summary>
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class StringTests
    {
        /// <summary>
        /// Test string conversions.
        /// </summary>
        [TestMethod]
        [DataRow("")]
        [DataRow("AaBb")]
        [DataRow("\t")]
        [DataRow("\r")]
        [DataRow("\n")]
        [DataRow("\a")]
        [DataRow("\b")]
        [DataRow("\v")]
        [DataRow("\f")]
        [DataRow("\t")]
        [DataRow("\"")]
        [DataRow("\\")]
        [DataRow("\\x5C\\x6E")]
        [DataRow("endline\n")]
        [DataRow("\x01\\x01")]
        public void TestStringConversions(string s)
        {
            var context = new Context();
            var toz3 = CommonUtilities.ConvertCSharpStringToZ3(s);
            var tocs = CommonUtilities.ConvertZ3StringToCSharp(context.MkString(toz3).ToString());
            Assert.AreEqual(s, tocs);
        }

        /// <summary>
        /// Test string conversions.
        /// </summary>
        [TestMethod]
        public void TestStringConversionsRandom()
        {
            for (int i = 0; i < 1000; i++)
            {
                string s = RandomString();
                var context = new Context();
                var toz3 = CommonUtilities.ConvertCSharpStringToZ3(s);
                var tocs = CommonUtilities.ConvertZ3StringToCSharp(context.MkString(toz3).ToString());
                Assert.AreEqual(s, tocs);
            }
        }

        /// <summary>
        /// Test concatenation with empty string.
        /// </summary>
        [TestMethod]
        public void TestConcatIdentity()
        {
            CheckValid<string>(x => x + "" == x);
        }

        /// <summary>
        /// Test string concatenation is associative.
        /// </summary>
        [TestMethod]
        public void TestConcatAssociative()
        {
            CheckValid<string, string, string>((x, y, z) => (x + y) + z == x + (y + z));
        }

        /// <summary>
        /// Test concat twice.
        /// </summary>
        [TestMethod]
        public void TestConcatTwice()
        {
            CheckAgreement<string>(x => x + x == "hello");
        }

        /// <summary>
        /// Test endswith agreement.
        /// </summary>
        [TestMethod]
        public void TestEndsWithAgreement()
        {
            RandomStrings(sub =>
            {
                CheckAgreement<string>(s => s.EndsWith(sub));
            });
        }

        /// <summary>
        /// Test startswith agreement.
        /// </summary>
        [TestMethod]
        public void TestStartsWithAgreement()
        {
            RandomStrings(sub =>
            {
                CheckAgreement<string>(s => s.StartsWith(sub));
            });
        }

        /// <summary>
        /// Test contains agreement.
        /// </summary>
        [TestMethod]
        public void TestContainsAgreement()
        {
            RandomStrings(sub =>
            {
                CheckAgreement<string>(s => s.Contains(sub));
            });
        }

        /// <summary>
        /// Test startswith and replace interactions.
        /// </summary>
        [TestMethod]
        public void TestReplaceStartsWith()
        {
            RandomStrings(sub =>
            {
                if (sub != "")
                {
                    CheckValid<string, string>((s1, s2) =>
                        Implies(s1.StartsWith(sub), s1.ReplaceFirst(sub, s2).StartsWith(s2)));
                }
            });
        }

        /// <summary>
        /// Test that a string contains its substring.
        /// </summary>
        [TestMethod]
        public void TestContainsSubstring()
        {
            RandomBytes(offset =>
            RandomBytes(length =>
                CheckValid<string>(s => s.Contains(s.Substring(new BigInteger(offset), new BigInteger(length))))));
        }

        /// <summary>
        /// Test substring agreement.
        /// </summary>
        [TestMethod]
        public void TestSubstringAgreement()
        {
            var offset = new BigInteger(RandomByte());
            var length = new BigInteger(RandomByte());
            CheckAgreement<string>(s => s.Substring(offset, length).EndsWith("ab"));
        }

        /// <summary>
        /// Test length agreement.
        /// </summary>
        [TestMethod]
        public void TestLengthAgreement()
        {
            CheckAgreement<string>(s => s.Length() == new BigInteger(0));
            CheckAgreement<string>(s => s.Length() == new BigInteger(1));
            CheckAgreement<string>(s => s.Length() == new BigInteger(2));
            CheckAgreement<string>(s => s.Length() == new BigInteger(3));
            CheckAgreement<string>(s => s.Length() == new BigInteger(4));
            CheckAgreement<string>(s => s.Length() == new BigInteger(5));
            CheckAgreement<string>(s => s.Length() == new BigInteger(10));
        }

        /// <summary>
        /// Test concatenating multiple values is solved correctly.
        /// </summary>
        [TestMethod]
        public void TestConcatMultipleValues()
        {
            var f1 = Function<string, string, bool>((w, x) => w + x == "hello");
            var f2 = Function<string, string, string, bool>((w, x, y) => w + x + y == "hello");
            var f3 = Function<string, string, string, string, bool>((w, x, y, z) => w + x + y + z == "hello");
            var r1 = f1.Find((i1, i2, o) => o);
            var r2 = f2.Find((i1, i2, i3, o) => o);
            var r3 = f3.Find((i1, i2, i3, i4, o) => o);

            Assert.IsTrue(r1.HasValue);
            Assert.IsTrue(r2.HasValue);
            Assert.IsTrue(r3.HasValue);

            string s1 = r1.Value.Item1 + r1.Value.Item2;
            string s2 = r2.Value.Item1 + r2.Value.Item2 + r2.Value.Item3;
            string s3 = r3.Value.Item1 + r3.Value.Item2 + r3.Value.Item3 + r3.Value.Item4;
            Assert.AreEqual("hello", s1);
            Assert.AreEqual("hello", s2);
            Assert.AreEqual("hello", s3);

            Assert.IsTrue(f1.Evaluate("he", "llo"));
            Assert.IsFalse(f1.Evaluate("he", "ello"));

            Assert.IsTrue(f2.Evaluate("h", "e", "llo"));
            Assert.IsFalse(f2.Evaluate("hell", "l", "o"));

            Assert.IsTrue(f3.Evaluate("h", "e", "ll", "o"));
            Assert.IsFalse(f3.Evaluate("hel", "l", "l", "o"));
        }

        /// <summary>
        /// Test startswith evaluation.
        /// </summary>
        [TestMethod]
        [DataRow("brown cow", "cow", false)]
        [DataRow("brown cow", "ow", false)]
        [DataRow("brown cow", "brown", true)]
        [DataRow("brown cow", "bro", true)]
        [DataRow("quick fox", "", true)]
        [DataRow("quick fox", "uick", false)]
        public void TestStartsWithEvaluation(string s, string sub, bool expected)
        {
            var f = Function<string, string, bool>((s1, s2) => s1.StartsWith(s2));
            Assert.AreEqual(expected, f.Evaluate(s, sub));
            f.Compile();
            Assert.AreEqual(expected, f.Evaluate(s, sub));
        }

        /// <summary>
        /// Test endswith evaluation.
        /// </summary>
        [TestMethod]
        [DataRow("brown cow", "cow", true)]
        [DataRow("brown cow", "ow", true)]
        [DataRow("brown cow", "brown", false)]
        [DataRow("quick fox", "", true)]
        public void TestEndsWithEvaluation(string s, string sub, bool expected)
        {
            var f = Function<string, string, bool>((s1, s2) => s1.EndsWith(s2));
            Assert.AreEqual(expected, f.Evaluate(s, sub));
            f.Compile();
            Assert.AreEqual(expected, f.Evaluate(s, sub));
        }

        /// <summary>
        /// Test contains evaluation.
        /// </summary>
        [TestMethod]
        [DataRow("brown cow", "cow", true)]
        [DataRow("brown cow", "brown", true)]
        [DataRow("hello", "ell", true)]
        [DataRow("hello", "", true)]
        [DataRow("hello", "b", false)]
        public void TestContainsEvaluation(string s, string sub, bool expected)
        {
            var f = Function<string, string, bool>((s1, s2) => s1.Contains(s2));
            Assert.AreEqual(expected, f.Evaluate(s, sub));
            f.Compile();
            Assert.AreEqual(expected, f.Evaluate(s, sub));
        }

        /// <summary>
        /// Test string replace first.
        /// </summary>
        [TestMethod]
        [DataRow("brown cow", "cow", "fox", "brown fox")]
        [DataRow("aabbcc", "b", "d", "aadbcc")]
        [DataRow("hello", "ll", "rrr", "herrro")]
        [DataRow("hello", "", " abc", "hello abc")]
        [DataRow("abc", "b", "", "ac")]
        [DataRow("abcd", "e", "f", "abcd")]
        public void TestReplaceFirst(string s, string sub, string replace, string expected)
        {
            Assert.AreEqual(expected, CommonUtilities.ReplaceFirst(s, sub, replace));
        }

        /// <summary>
        /// Test contains replace first.
        /// </summary>
        [TestMethod]
        [DataRow("brown cow", "cow", "fox", "brown fox")]
        [DataRow("aabbcc", "b", "d", "aadbcc")]
        [DataRow("hello", "ll", "rrr", "herrro")]
        [DataRow("hello", "", " abc", "hello abc")]
        [DataRow("abc", "b", "", "ac")]
        public void TestReplaceEvaluation(string s, string sub, string replace, string expected)
        {
            var f = Function<string, string>(s => s.ReplaceFirst(sub, replace));
            Assert.AreEqual(expected, f.Evaluate(s));
            f.Compile();
            Assert.AreEqual(expected, f.Evaluate(s));
        }

        /// <summary>
        /// Test substring evaluation.
        /// </summary>
        [TestMethod]
        [DataRow("hello", 0, 3, "hel")]
        [DataRow("hello", 1, 3, "ell")]
        [DataRow("hello", 10, 3, "")]
        [DataRow("hello", 0, 20, "hello")]
        public void TestSubstringEvaluation(string s, int offset, int length, string expected)
        {
            var f = Function<string, string>(s => s.Substring(new BigInteger(offset), new BigInteger(length)));
            Assert.AreEqual(expected, f.Evaluate(s));
            f.Compile();
            Assert.AreEqual(expected, f.Evaluate(s));
        }

        /// <summary>
        /// Test string at evaluation.
        /// </summary>
        [TestMethod]
        [DataRow("abcde", 0, "a")]
        [DataRow("abcde", 1, "b")]
        [DataRow("abcde", 2, "c")]
        [DataRow("abcde", 3, "d")]
        [DataRow("abcde", 4, "e")]
        [DataRow("abcde", 5, "")]
        [DataRow("", 0, "")]
        [DataRow("", 1, "")]
        [DataRow("", 2, "")]
        public void TestAtEvaluation(string s, int index, string expected)
        {
            var f = Function<string, BigInteger, string>((s, idx) => s.At(idx));
            Assert.AreEqual(expected, f.Evaluate(s, (ushort)index));
            f.Compile();
            Assert.AreEqual(expected, f.Evaluate(s, (ushort)index));

            if (expected != "")
            {
                var x = f.Find((str, idx, o) => And(str == s, o == expected));
                Assert.AreEqual(x.Value.Item2, new BigInteger(index));
            }
        }

        /// <summary>
        /// Test string length evaluation.
        /// </summary>
        [TestMethod]
        [DataRow("", 0)]
        [DataRow("a", 1)]
        [DataRow("ab", 2)]
        [DataRow("abc", 3)]
        [DataRow("abcd", 4)]
        [DataRow("abcde", 5)]
        [DataRow("\x01\x02", 2)]
        public void TestLengthEvaluation(string s, int expected)
        {
            var f = Function<string, BigInteger>(s => s.Length());
            Assert.AreEqual(new BigInteger(expected), f.Evaluate(s));
            f.Compile();
            Assert.AreEqual(new BigInteger(expected), f.Evaluate(s));
        }

        /// <summary>
        /// Test string indexof evaluation.
        /// </summary>
        [TestMethod]
        [DataRow("abcda", "", 0, 0)]
        [DataRow("abcda", "", 3, 3)]
        [DataRow("abcda", "", 10, -1)]
        [DataRow("abcda", "a", 0, 0)]
        [DataRow("abcda", "a", 1, 4)]
        [DataRow("abcda", "b", 0, 1)]
        [DataRow("abcda", "b", 1, 1)]
        [DataRow("abcda", "b", 2, -1)]
        [DataRow("abcda", "e", 0, -1)]
        public void TestIndexOfEvaluation(string s, string sub, int offset, int expected)
        {
            var f = Function<string, BigInteger>(s => s.IndexOf(sub, new BigInteger(offset)));
            Assert.AreEqual((short)expected, f.Evaluate(s));
            f.Compile();
            Assert.AreEqual((short)expected, f.Evaluate(s));
        }

        /// <summary>
        /// Test indexof find.
        /// </summary>
        [TestMethod]
        public void TestIndexOfFind()
        {
            var f = Function<string, BigInteger>(s => s.IndexOf("a", new BigInteger(0)));
            var input = f.Find((s, o) => o == new BigInteger(5));
            Assert.AreEqual((short)5, f.Evaluate(input.Value));
        }

        /// <summary>
        /// Test multiple operations.
        /// </summary>
        [TestMethod]
        public void TestMultipleOperations()
        {
            var f = Function<string, bool>(s =>
            {
                var c = s.At(new BigInteger(3));
                var s2 = s.Substring(new BigInteger(5), new BigInteger(2));
                return And(s.StartsWith("a"), c == "b", s2 == "cd", s.Length() == new BigInteger(10));
            });

            var x = f.Find((i, o) => o);
            Assert.IsTrue(x.HasValue);
            Assert.IsTrue(f.Evaluate(x.Value));
        }

        /// <summary>
        /// Test at is substring.
        /// </summary>
        [TestMethod]
        public void TestAtIsSubstring()
        {
            CheckValid<string>(s => s.At(new BigInteger(0)) == s.Substring(new BigInteger(0), new BigInteger(1)));
            CheckValid<string>(s => s.At(new BigInteger(1)) == s.Substring(new BigInteger(1), new BigInteger(1)));
            CheckValid<string>(s => s.At(new BigInteger(2)) == s.Substring(new BigInteger(2), new BigInteger(1)));
            CheckValid<string>(s => s.At(new BigInteger(3)) == s.Substring(new BigInteger(3), new BigInteger(1)));
            CheckValid<string>(s => s.At(new BigInteger(4)) == s.Substring(new BigInteger(4), new BigInteger(1)));
        }

        /// <summary>
        /// Test endswith implies contains.
        /// </summary>
        [TestMethod]
        public void TestEndsWithContains()
        {
            RandomStrings(sub => CheckValid<string>(s => Implies(s.EndsWith(sub), s.Contains(sub))));
        }

        /// <summary>
        /// Test startswith implies contains.
        /// </summary>
        [TestMethod]
        public void TestStartsWithContains()
        {
            RandomStrings(sub => CheckValid<string>(s => Implies(s.StartsWith(sub), s.Contains(sub))));
        }

        /// <summary>
        /// Test equality for composite types.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ZenException))]
        public void TestStringEqualityCompositeException1()
        {
            CheckAgreement<IList<string>, IList<string>>((l1, l2) => l1 == l2);
        }

        /// <summary>
        /// Test invalid null string literal.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestInvalidStringLiteralNull()
        {
            Constant<string>(null);
        }

        /// <summary>
        /// Test invalid string literal.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestInvalidStringLiteral2()
        {
            char c = (char)960; // greek pi
            Constant(c.ToString());
        }

        /// <summary>
        /// Test equality for composite types.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ZenException))]
        public void TestStringEqualityCompositeException2()
        {
            CheckAgreement<IDictionary<string, string>, IDictionary<string, string>>((l1, l2) => l1 == l2);
        }

        /// <summary>
        /// Test equality for composite types.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ZenException))]
        public void TestStringEqualityCompositeException3()
        {
            CheckAgreement<Option<IList<string>>, Option<IList<string>>>((l1, l2) => l1 == l2);
        }

        /// <summary>
        /// Test equality for composite types.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ZenException))]
        public void TestStringEqualityCompositeException4()
        {
            CheckAgreement<(string, IList<string>), (string, IList<string>)>((l1, l2) => l1 == l2);
        }

        /// <summary>
        /// Test an exception is thrown for non-strings.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestConcatException()
        {
            _ = EmptyList<string>() + EmptyList<string>();
        }

        /// <summary>
        /// Test that DD backend does not work with strings.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ZenException))]
        public void TestDiagramBackendException1()
        {
            var f = Function<string, bool>(s => s == "a");
            f.Find((x, y) => y, backend: ModelChecking.Backend.DecisionDiagrams);
        }

        /// <summary>
        /// Test that DD backend does not work with strings.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ZenException))]
        public void TestDiagramBackendException2()
        {
            var f = Function<string, string>(s => s + "a");
            f.Find((x, y) => x == y, backend: ModelChecking.Backend.DecisionDiagrams);
        }

        /// <summary>
        /// Test that DD backend does not work with strings.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ZenException))]
        public void TestDiagramBackendException3()
        {
            var f = Function<string, string>(s => s + "a");
            f.Transformer();
        }
    }
}
