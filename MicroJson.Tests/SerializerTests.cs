﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using NUnit.Framework;
// uncomment next line on MonoDevelop 2.8 and below
//using NUnit.Framework.SyntaxHelpers;
using System.CodeDom.Compiler;

#pragma warning disable 1591
namespace MicroJson {
  [TestFixture]
  [GeneratedCode("dummy", "dummy")]
  public class SerializerTests : TestFixtureBase {
    public class SerializeInner {
      public string b2;
    }

    [Flags]
    public enum AnEnum { Test1 = 1, Test2 = 2, Test3 = 4, Test4 = 8 };

    public class Serialize {
      [DefaultValue(4711)]
      public int I { get; set; }
      public bool B { get; set; }
      public double D { get; set; }
      public string S;
      public List<string> LS;
      public SerializeInner Inner { get; set; }
      public Dictionary<string, string> X { get; set; }
      public AnEnum E { get; set; }
      public AnEnum E2 { get; set; }
      public AnEnum[] AE { get; set; }

      public Serialize() {
        I = 4711;
      }
    }

    public class DerivedSerialize : Dictionary<string, string> {
    }

    [Test]
    public void TestDeserialization() {
      var s = @"{ ""I"": 1,
""B"": true,
""D"": 1.0,
""S"": ""Test"",
""LS"": [""a"", ""b"", ""c""],
""Inner"": { ""b2"": ""xyz"" },
""X"": { ""a"": ""A"", ""b"": ""B"" },
""E"": ""Test2, Test1"",
""E2"": 9
}";
      JsonSerializer serializer = new JsonSerializer();

      var ser = serializer.Deserialize<Serialize>(s);
      Assert.That(1, Is.EqualTo(ser.I));
      Assert.That(true, Is.EqualTo(ser.B));
      Assert.That(1.0, Is.EqualTo(ser.D));
      Assert.That("Test", Is.EqualTo(ser.S));
      Assert.That(new[] { "a", "b", "c" }, Is.EquivalentTo(ser.LS));
      Assert.That(ser.Inner != null);
      Assert.That("xyz", Is.EqualTo(ser.Inner.b2));
      Assert.That("A", Is.EqualTo(ser.X["a"]));
      Assert.That("B", Is.EqualTo(ser.X["b"]));
      Assert.That(AnEnum.Test2 | AnEnum.Test1, Is.EqualTo(ser.E));
      Assert.That(AnEnum.Test1 | AnEnum.Test4, Is.EqualTo(ser.E2));

      var s2 = @"{ ""LS"": ""a"" }";
      var d = serializer.Deserialize<Serialize>(s2);
      Assert.That(new[] { "a" }, Is.EquivalentTo(d.LS));

      var s3 = @"{ ""A"": ""a"", ""B"": ""b"", ""C"": ""c"" }";
      var derived = serializer.Deserialize<DerivedSerialize>(s3);
      Assert.That(new Dictionary<string, string> { { "A", "a" }, { "B", "b" }, { "C", "c" } }, Is.EquivalentTo(derived));
    }

    [Test]
    public void TestSerialization() {
      var o = new Serialize {
        I = 1,
        B = true,
        D = 2.3735863875686736,
        S = "Test",
        LS = new List<string>(new[] { "a", "b", "c" }),
        Inner = new SerializeInner { b2 = "xyz" },
        X = new Dictionary<string, string> { { "b", "B" }, { "a", "A" } },
        E = AnEnum.Test2 | AnEnum.Test1,
        // default value for E2
        AE = new[] { AnEnum.Test2, AnEnum.Test4 }
      };

      var s = new JsonSerializer().Serialize(o);

      Assert.That(s, Is.EqualTo(@"{""AE"":[""Test2"",""Test4""],""B"":true,""D"":2.3735863875686736,""E"":""Test1, Test2"",""I"":1,""Inner"":{""b2"":""xyz""},""LS"":[""a"",""b"",""c""],""S"":""Test"",""X"":{""a"":""A"",""b"":""B""}}"));
    }

    [Test]
    public void TestSerializerRoundTrip() {
      var s = @"{""B"":true,""D"":-1,""E"":""Test1, Test2"",""I"":-1,""Inner"":{""b2"":""xyz""},""LS"":[""a"",""b"",""c""],""S"":""Test"",""X"":{""a"":""A"",""b"":""B""}}";
      JsonSerializer serializer = new JsonSerializer();
      var ser = serializer.Deserialize<Serialize>(s);
      var s2 = serializer.Serialize(ser);
      Assert.That(s, Is.EqualTo(s2));
      var ser2 = serializer.Deserialize<Serialize>(s2);
      var s3 = serializer.Serialize(ser2);
      Assert.That(s, Is.EqualTo(s3));
    }

    [Test]
    public void TestDefaultValue() {
      var s = @"{""B"":true,""D"":-1,""E"":""Test1, Test2"",""Inner"":{""b2"":""xyz""},""LS"":[""a"",""b"",""c""],""S"":""Test"",""X"":{""a"":""A"",""b"":""B""}}";
      JsonSerializer serializer = new JsonSerializer();
      var ser = serializer.Deserialize<Serialize>(s);
      Assert.That(4711, Is.EqualTo(ser.I));
      var s2 = serializer.Serialize(ser);
      Assert.That(s, Is.EqualTo(s2));
      var ser2 = serializer.Deserialize<Serialize>(s2);
      Assert.That(4711, Is.EqualTo(ser2.I));
      var s3 = serializer.Serialize(ser2);
      Assert.That(s, Is.EqualTo(s3));
    }

    [Test]
    public void TestEmitDefaultValues() {
      var s = @"{""B"":true,""D"":-1,""E"":""Test1, Test2"",""E2"":""0"",""I"":4711,""Inner"":{""b2"":""xyz""},""LS"":[""a"",""b"",""c""],""S"":""Test"",""X"":{""a"":""A"",""b"":""B""}}";
      JsonSerializer serializer = new JsonSerializer() { EmitDefaultValues = true };
      var ser = serializer.Deserialize<Serialize>(s);
      Assert.That(ser.I, Is.EqualTo(4711));
      var s2 = serializer.Serialize(ser);
      Assert.That(s2, Is.EqualTo(s));
      var ser2 = serializer.Deserialize<Serialize>(s2);
      Assert.That(ser2.I, Is.EqualTo(4711));
      var s3 = serializer.Serialize(ser2);
      Assert.That(s3, Is.EqualTo(s));
    }

    [Test]
    public void TestEmitNulls() {
      var s = @"{""AE"":null,""B"":true,""D"":-1,""E"":""Test1, Test2"",""Inner"":{""b2"":""xyz""},""LS"":[""a"",""b"",""c""],""S"":""Test"",""X"":{""a"":""A"",""b"":""B""}}";
      JsonSerializer serializer = new JsonSerializer() { EmitNulls = true };
      var ser = serializer.Deserialize<Serialize>(s);
      Assert.That(ser.I, Is.EqualTo(4711));
      var s2 = serializer.Serialize(ser);
      Assert.That(s2, Is.EqualTo(s));
      var ser2 = serializer.Deserialize<Serialize>(s2);
      Assert.That(ser2.I, Is.EqualTo(4711));
      var s3 = serializer.Serialize(ser2);
      Assert.That(s3, Is.EqualTo(s));
    }

    [Test]
    public void TestStrings() {
      var str = "\" \\ / \b \f \n \r \t \u4711 \\n \\\" \\\\";
      JsonSerializer serializer = new JsonSerializer();
      var ser = serializer.Serialize(str);
      Assert.That(@"""\"" \\ / \b \f \n \r \t " + "\u4711" + @" \\n \\\"" \\\\""", Is.EqualTo(ser));
    }

    long convertToEpochBased(long ticks) {
      return (ticks - 621355968000000000) / 10000;
    }

    [Test]
    public void TestDateTime() {
      var str = @"""\/Date(0)\/""";
      JsonSerializer serializer = new JsonSerializer();
      var d = serializer.Deserialize<DateTime>(str);
      Assert.That(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).ToLocalTime(), Is.EqualTo(d));
      // json resolution is only milliseconds
      var now = new DateTime((DateTime.UtcNow.Ticks / 10000) * 10000, DateTimeKind.Utc);
      str = string.Format(@"""\/Date({0})\/""", convertToEpochBased(now.Ticks));
      var dnow = serializer.Deserialize<DateTime>(str);
      Assert.That(now.ToLocalTime(), Is.EqualTo(dnow));
      var x = new DateTime(1900, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
      var str2 = string.Format(@"""\/Date({0})\/""", convertToEpochBased(x.Ticks));
      Assert.That(x.ToLocalTime(), Is.EqualTo(serializer.Deserialize<DateTime>(str2)));
      AssertThrows<FormatException>(() => serializer.Deserialize<DateTime>(@"""hallo"""));
      AssertThrows<FormatException>(() => serializer.Deserialize<DateTime>(@"""\/Date(hallo)\/"""));

      Assert.That(@"""\/Date(0)\/""", Is.EqualTo(serializer.Serialize(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc))));
      Assert.That(str, Is.EqualTo(serializer.Serialize(now)));
      Assert.That(str2, Is.EqualTo(serializer.Serialize(x)));

      var n = new DateTime((DateTime.Now.Ticks / 10000) * 10000, DateTimeKind.Local);
      var n2 = serializer.Deserialize<DateTime>(serializer.Serialize(n));
      Assert.That(n, Is.EqualTo(n2));
    }

    public class Test {
      public string S { get; set; }
      public int I { get; set; }
      public List<int> L;
      public int this[int i] {
        get { return i; }
        set { }
      }
    }

    [Test]
    public void TestReadme() {
      // work around MonoTouch full-AOT limitation
      var l = new List<int>();
      l.Add(1);

      var json = @"{
                ""S"": ""Hello, world."",
                ""I"": 4711,
                ""L"": [1, 2, 3]
            }";

      var t = new JsonSerializer().Deserialize<Test>(json);

      Assert.That(t.S, Is.EqualTo("Hello, world."));
      Assert.That(4711, Is.EqualTo(t.I));
      Assert.That(new[] { 1, 2, 3 }, Is.EquivalentTo(t.L));

      var j = new JsonSerializer().Serialize(t);

      Assert.That(j, Is.EqualTo(@"{""I"":4711,""L"":[1,2,3],""S"":""Hello, world.""}"));
    }

    [Test]
    public void TestChar() {
      var json = @"""x""";
      char c = new JsonSerializer().Deserialize<char>(json);
      Assert.That(c, Is.EqualTo('x'));
      Assert.That(new JsonSerializer().Serialize('c'), Is.EqualTo(@"""c"""));
    }

    [Test]
    public void TestGuid() {
      var guid = Guid.NewGuid();
      var json = string.Format(@"""{0}""", guid);
      Guid g = new JsonSerializer().Deserialize<Guid>(json);
      Assert.That(g, Is.EqualTo(guid));
      Assert.That(new JsonSerializer().Serialize(g), Is.EqualTo(json));
    }

    [Test]
    public void TestUri() {
      var uri = new Uri(@"https://www.google.com/bla/#blub?x=1&y=2");
      var json = string.Format(@"""{0}""", uri);
      Uri u = new JsonSerializer().Deserialize<Uri>(json);
      Assert.That(u, Is.EqualTo(uri));
      Assert.That(new JsonSerializer().Serialize(u), Is.EqualTo(json));
    }

    [Test]
    public void TestEnum() {
      var e = AnEnum.Test3;
      var json = new JsonSerializer().Serialize(e);
      Assert.That(json, Is.EqualTo(@"""Test3"""));
      Assert.That(new JsonSerializer().Deserialize<AnEnum>(json), Is.EqualTo(e));
      var e4 = new JsonSerializer().Deserialize<AnEnum>("8");
      Assert.That(e4, Is.EqualTo(AnEnum.Test4));
    }

    [Test]
    public void TestArray() {
      var a = new AnEnum[] { AnEnum.Test3, AnEnum.Test1 };
      var json = new JsonSerializer().Serialize(a);
      Assert.That(json, Is.EqualTo(@"[""Test3"",""Test1""]"));
      Assert.That(new JsonSerializer().Deserialize<AnEnum[]>(json), Is.EqualTo(a));
      var a2 = new JsonSerializer().Deserialize<AnEnum[]>("[4,1]");
      Assert.That(a2, Is.EquivalentTo(a));
    }

    [Test]
    public void TestDecimal() {
      var d = 1.23456789;
      var json = new JsonSerializer().Serialize(d);
      Assert.That(json, Is.EqualTo("1.23456789"));
      Assert.That(new JsonSerializer().Deserialize<decimal>(json), Is.EqualTo(d));
      Assert.That("1.2", Is.EqualTo(new JsonSerializer().Serialize(1.2m)));
    }

    public interface ITypeInfo {
      string InterfaceProperty { get; set; }
    }

    public class BaseTypeInfo {
      public string BaseProperty { get; set; }
    }

    public class DerivedTypeInfo : BaseTypeInfo, ITypeInfo {
      public string DerivedProperty { get; set; }
      public string InterfaceProperty { get; set; }
    }

    [Test]
    public void TestTypeInfo() {
      var serializer = new JsonSerializer { UseTypeInfo = true };
      var o = new DerivedTypeInfo { BaseProperty = "base", DerivedProperty = "derived", InterfaceProperty = "interface" };
      var json = serializer.Serialize(o);
      Assert.That(json, Is.EqualTo(@"{""@type"":""DerivedTypeInfo"",""BaseProperty"":""base"",""DerivedProperty"":""derived"",""InterfaceProperty"":""interface""}"));
      var o2 = new BaseTypeInfo { BaseProperty = "base" };
      var json2 = serializer.Serialize(o2);
      Assert.That(json2, Is.EqualTo(@"{""BaseProperty"":""base""}"));

      var d = serializer.Deserialize<BaseTypeInfo>(json);
      Assert.That(d, Is.TypeOf<DerivedTypeInfo>());

      var i = serializer.Deserialize<ITypeInfo>(json);
      Assert.That(i, Is.TypeOf<DerivedTypeInfo>());
    }

    [Test]
    public void TestStaticMethods() {
      Assert.That(4711, Is.EqualTo(JsonSerializer.DeserializeObject<int>("4711")));
      Assert.That("4711", Is.EqualTo(JsonSerializer.SerializeObject(4711)));
    }

    [Test]
    public void TestMisc() {
      AssertThrows<ArgumentException>(() => new JsonSerializer().Deserialize<object>("", null));
      Assert.That(4711, Is.EqualTo(new JsonSerializer().Deserialize<int>(4711)));
      Assert.That(4711, Is.EqualTo(new JsonSerializer().Deserialize<int?>(4711.0)));
      Assert.Null(new JsonSerializer().Deserialize<object>((object)null));
      Assert.That("null", Is.EqualTo(new JsonSerializer().Serialize(null)));
      Assert.That("4711", Is.EqualTo(new JsonSerializer().Serialize((UInt64)4711)));
      Assert.That(@"""\u0001""", Is.EqualTo(new JsonSerializer().Serialize("\u0001")));
      Assert.That(@"{""x"":1}", Is.EqualTo(new JsonSerializer().Serialize(new List<KeyValuePair<string, object>> { new KeyValuePair<string, object>("x", 1) })));
    }
  }
}
#pragma warning restore 1591