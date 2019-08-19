using System;
using Flee.ExpressionEditor;
using Flee.PublicTypes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Flee.Expression.Tests
{
    [TestClass]
    public class ParseExpressionTest
    {
        #region Groupy a přílohy

        [TestMethod]
        public void Parse_NotAndGroup_Test()
        {
            var exp = "NOT (Zadost.NezadanaPriloha(\"50574d8389b94e6d9024eb2967565dc7\") AND _337fc4e428e84e0492db0af1f8af4458 <> null)";

            var context = new ExpressionContext();
            var zadostExpr = new ZadostExpression();
            context.Imports.ImportBuiltinTypes();
            context.Variables.Add("Zadost", zadostExpr);
            context.Variables.ResolveVariableType += Variables_ResolveVariableTypeString;

            var result = context.ParseGeneric<bool>(exp);

            Assert.AreEqual("group", result.type);
            var group = (Group)result;
            Assert.AreEqual("NotAnd", group.operation);
            Assert.AreEqual(2, group.items.Count);

            var firstItem = (Condition)group.items[0];
            Assert.AreEqual("50574d8389b94e6d9024eb2967565dc7", firstItem.field);
            Assert.AreEqual("NotSpecified", firstItem.@operator);
            Assert.IsNull(firstItem.value);
            Assert.IsNull(firstItem.secondField);

            var secondItem = (Condition)group.items[1];
            Assert.AreEqual("337fc4e428e84e0492db0af1f8af4458", secondItem.field);
            Assert.AreEqual("IsNotBlank", secondItem.@operator);
            Assert.IsNull(secondItem.value);
            Assert.IsNull(secondItem.secondField);
        }

        [TestMethod]
        public void Parse_ZadanaPriloha_Test()
        {
            var exp = "Zadost.ZadanaPriloha(\"50574d8389b94e6d9024eb2967565dc7\")";

            var context = new ExpressionContext();
            var zadostExpr = new ZadostExpression();
            context.Imports.ImportBuiltinTypes();
            context.Variables.Add("Zadost", zadostExpr);
            context.Variables.ResolveVariableType += Variables_ResolveVariableTypeString;

            var result = context.ParseGeneric<bool>(exp);

            var firstItem = (Condition)result;
            Assert.AreEqual("50574d8389b94e6d9024eb2967565dc7", firstItem.field);
            Assert.AreEqual("Specified", firstItem.@operator);
            Assert.IsNull(firstItem.value);
            Assert.IsNull(firstItem.secondField);
        }

        [TestMethod]
        public void Parse_TrippleAnd_Test()
        {
            var exp = "IsNotBlank(_50574d8389b94e6d9024eb2967565dc7) AND IsNotBlank(_337fc4e428e84e0492db0af1f8af4458) AND IsNotBlank(_61eaff6e5c394bb5846a7c92c01d6358)";

            var context = new ExpressionContext();
            var zadostExpr = new ZadostExpression();
            context.Imports.ImportBuiltinTypes();
            context.Imports.AddType(typeof(ExpressionFunctions));
            context.Variables.ResolveVariableType += Variables_ResolveVariableTypeString;

            var result = context.ParseGeneric<bool>(exp);

            Assert.AreEqual("group", result.type);
            var group = (Group)result;
            Assert.AreEqual("And", group.operation);
            Assert.AreEqual(3, group.items.Count);
        }

        [TestMethod]
        public void Parse_OrAndTrippleAnd_Test()
        {
            var exp = @"(MatchReGroups(_9c20e9d02aba469cb130202576517d2c;""^\\d{2}(\\d{2})"";_6939ea0ddc304dbf9a28ccd2bc11779a;""^\\d+[.\\s]+(\\d+)[.\\s]+\\d+$"") AND MatchReGroups(_9c20e9d02aba469cb130202576517d2c;""^\\d{4}(\\d{2})"";_6939ea0ddc304dbf9a28ccd2bc11779a;""^(\\d+)"") AND MatchReGroups(_9c20e9d02aba469cb130202576517d2c;""^(\\d{2})"";_6939ea0ddc304dbf9a28ccd2bc11779a;""(\\d{2})$"")) OR (IsBlank(_6939ea0ddc304dbf9a28ccd2bc11779a) AND IsBlank(_9c20e9d02aba469cb130202576517d2c))";

            var context = new ExpressionContext();
            var zadostExpr = new ZadostExpression();
            context.Imports.ImportBuiltinTypes();
            context.Imports.AddType(typeof(ExpressionFunctions));
            context.Variables.ResolveVariableType += Variables_ResolveVariableTypeString;

            var result = context.ParseGeneric<bool>(exp);

            Assert.AreEqual("group", result.type);
            var groupOr = (Group)result;
            Assert.AreEqual("Or", groupOr.operation);
            Assert.AreEqual(2, groupOr.items.Count);

            var groupAnd1 = (Group)groupOr.items[0];
            var groupAnd2 = (Group)groupOr.items[1];

            Assert.AreEqual("And", groupAnd1.operation);
            Assert.AreEqual("And", groupAnd2.operation);

            foreach (var item in groupAnd1.items)
            {
                Assert.IsInstanceOfType(item, typeof(Condition));
            }
            foreach (var item in groupAnd2.items)
            {
                Assert.IsInstanceOfType(item, typeof(Condition));
            }
        }

        #endregion

        #region String

        [TestMethod]
        public void Parse_StringContainsField_Test()
        {
            var exp = "Contains(_337fc4e428e84e0492db0af1f8af4458; _50574d8389b94e6d9024eb2967565dc7)";

            var context = new ExpressionContext();
            var zadostExpr = new ZadostExpression();
            context.Imports.ImportBuiltinTypes();
            context.Imports.AddType(typeof(ExpressionFunctions));
            context.Variables.ResolveVariableType += Variables_ResolveVariableTypeString;

            var result = context.ParseGeneric<bool>(exp);

            var firstItem = (Condition)result;
            Assert.AreEqual("337fc4e428e84e0492db0af1f8af4458", firstItem.field);
            Assert.AreEqual("Contains", firstItem.@operator);
            Assert.IsNull(firstItem.value);
            Assert.AreEqual("50574d8389b94e6d9024eb2967565dc7", firstItem.secondField);
        }

        [TestMethod]
        public void Parse_StringDoesNotContainValue_Test()
        {
            var exp = "NotContains(_337fc4e428e84e0492db0af1f8af4458; \".cz\")";

            var context = new ExpressionContext();
            var zadostExpr = new ZadostExpression();
            context.Imports.ImportBuiltinTypes();
            context.Imports.AddType(typeof(ExpressionFunctions));
            context.Variables.ResolveVariableType += Variables_ResolveVariableTypeString;

            var result = context.ParseGeneric<bool>(exp);

            var firstItem = (Condition)result;
            Assert.AreEqual("337fc4e428e84e0492db0af1f8af4458", firstItem.field);
            Assert.AreEqual("NotContains", firstItem.@operator);
            Assert.AreEqual(".cz", firstItem.value);
            Assert.IsNull(firstItem.secondField);
        }

        [TestMethod]
        public void Parse_StringStartsWithValue_Test()
        {
            var exp = "StartsWith(_337fc4e428e84e0492db0af1f8af4458; \"test\")";

            var context = new ExpressionContext();
            var zadostExpr = new ZadostExpression();
            context.Imports.ImportBuiltinTypes();
            context.Imports.AddType(typeof(ExpressionFunctions));
            context.Variables.ResolveVariableType += Variables_ResolveVariableTypeString;

            var result = context.ParseGeneric<bool>(exp);

            var firstItem = (Condition)result;
            Assert.AreEqual("337fc4e428e84e0492db0af1f8af4458", firstItem.field);
            Assert.AreEqual("StartsWith", firstItem.@operator);
            Assert.AreEqual("test", firstItem.value);
            Assert.IsNull(firstItem.secondField);
        }

        [TestMethod]
        public void Parse_StringEndsWithValue_Test()
        {
            var exp = "EndsWith(_337fc4e428e84e0492db0af1f8af4459; \"test2\")";

            var context = new ExpressionContext();
            var zadostExpr = new ZadostExpression();
            context.Imports.ImportBuiltinTypes();
            context.Imports.AddType(typeof(ExpressionFunctions));
            context.Variables.ResolveVariableType += Variables_ResolveVariableTypeString;

            var result = context.ParseGeneric<bool>(exp);

            var firstItem = (Condition)result;
            Assert.AreEqual("337fc4e428e84e0492db0af1f8af4459", firstItem.field);
            Assert.AreEqual("EndsWith", firstItem.@operator);
            Assert.AreEqual("test2", firstItem.value);
            Assert.IsNull(firstItem.secondField);
        }

        [TestMethod]
        public void Parse_StringEqualsValue_Test()
        {
            var exp = "_337fc4e428e84e0492db0af1f8af4459 = \"val0\"";

            var context = new ExpressionContext();
            var zadostExpr = new ZadostExpression();
            context.Imports.ImportBuiltinTypes();
            context.Imports.AddType(typeof(ExpressionFunctions));
            context.Variables.ResolveVariableType += Variables_ResolveVariableTypeString;

            var result = context.ParseGeneric<bool>(exp);

            var firstItem = (Condition)result;
            Assert.AreEqual("337fc4e428e84e0492db0af1f8af4459", firstItem.field);
            Assert.AreEqual("Equals", firstItem.@operator);
            Assert.AreEqual("val0", firstItem.value);
            Assert.IsNull(firstItem.secondField);
        }

        [TestMethod]
        public void Parse_StringDoesNotEqualField_Test()
        {
            var exp = "_337fc4e428e84e0492db0af1f8af4458 <> _50574d8389b94e6d9024eb2967565dc7";

            var context = new ExpressionContext();
            var zadostExpr = new ZadostExpression();
            context.Imports.ImportBuiltinTypes();
            context.Imports.AddType(typeof(ExpressionFunctions));
            context.Variables.ResolveVariableType += Variables_ResolveVariableTypeString;

            var result = context.ParseGeneric<bool>(exp);

            var firstItem = (Condition)result;
            Assert.AreEqual("337fc4e428e84e0492db0af1f8af4458", firstItem.field);
            Assert.AreEqual("DoesNotEqual", firstItem.@operator);
            Assert.IsNull(firstItem.value);
            Assert.AreEqual("50574d8389b94e6d9024eb2967565dc7", firstItem.secondField);
        }

        [TestMethod]
        public void Parse_StringIsBlank_Test()
        {
            var exp = "_337fc4e428e84e0492db0af1f8af4459 = null";

            var context = new ExpressionContext();
            var zadostExpr = new ZadostExpression();
            context.Imports.ImportBuiltinTypes();
            context.Imports.AddType(typeof(ExpressionFunctions));
            context.Variables.ResolveVariableType += Variables_ResolveVariableTypeString;

            var result = context.ParseGeneric<bool>(exp);

            var firstItem = (Condition)result;
            Assert.AreEqual("337fc4e428e84e0492db0af1f8af4459", firstItem.field);
            Assert.AreEqual("IsBlank", firstItem.@operator);
            Assert.IsNull(firstItem.value);
            Assert.IsNull(firstItem.secondField);
        }

        [TestMethod]
        public void Parse_StringIsNotBlank_Test()
        {
            var exp = "IsNotBlank(_337fc4e428e84e0492db0af1f8af4459)";

            var context = new ExpressionContext();
            var zadostExpr = new ZadostExpression();
            context.Imports.ImportBuiltinTypes();
            context.Imports.AddType(typeof(ExpressionFunctions));
            context.Variables.ResolveVariableType += Variables_ResolveVariableTypeString;

            var result = context.ParseGeneric<bool>(exp);

            var firstItem = (Condition)result;
            Assert.AreEqual("337fc4e428e84e0492db0af1f8af4459", firstItem.field);
            Assert.AreEqual("IsNotBlank", firstItem.@operator);
            Assert.IsNull(firstItem.value);
            Assert.IsNull(firstItem.secondField);
        }

        private void Variables_ResolveVariableTypeString(object sender, ResolveVariableTypeEventArgs e)
        {
            e.VariableType = typeof(string);
        }

        #endregion

        #region Int

        [TestMethod]
        public void Parse_IntEqualsValue_Test()
        {
            var exp = "Equals(_337fc4e428e84e0492db0af1f8af4459;42)";

            var context = new ExpressionContext();
            var zadostExpr = new ZadostExpression();
            context.Imports.ImportBuiltinTypes();
            context.Imports.AddType(typeof(ExpressionFunctions));
            context.Variables.ResolveVariableType += Variables_ResolveVariableTypeInt;

            var result = context.ParseGeneric<bool>(exp);

            var firstItem = (Condition)result;
            Assert.AreEqual("337fc4e428e84e0492db0af1f8af4459", firstItem.field);
            Assert.AreEqual("Equals", firstItem.@operator);
            Assert.AreEqual("42", firstItem.value);
            Assert.IsNull(firstItem.secondField);
        }

        [TestMethod]
        public void Parse_IntGreaterThanFieldValue_Test()
        {
            var exp = "GreaterThan(_337fc4e428e84e0492db0af1f8af4459;_61eaff6e5c394bb5846a7c92c01d6358)";

            var context = new ExpressionContext();
            var zadostExpr = new ZadostExpression();
            context.Imports.ImportBuiltinTypes();
            context.Imports.AddType(typeof(ExpressionFunctions));
            context.Variables.ResolveVariableType += Variables_ResolveVariableTypeInt;

            var result = context.ParseGeneric<bool>(exp);

            var firstItem = (Condition)result;
            Assert.AreEqual("337fc4e428e84e0492db0af1f8af4459", firstItem.field);
            Assert.AreEqual("GreaterThan", firstItem.@operator);
            Assert.IsNull(firstItem.value);
            Assert.AreEqual("61eaff6e5c394bb5846a7c92c01d6358", firstItem.secondField);
        }

        [TestMethod]
        public void Parse_IntNullableGreaterThanFieldValue_Test()
        {
            var exp = "GreaterThan(_337fc4e428e84e0492db0af1f8af4459;_61eaff6e5c394bb5846a7c92c01d6358)";

            var context = new ExpressionContext();
            var zadostExpr = new ZadostExpression();
            context.Imports.ImportBuiltinTypes();
            context.Imports.AddType(typeof(ExpressionFunctions));
            context.Variables.ResolveVariableType += Variables_ResolveVariableTypeIntNullable;

            var result = context.ParseGeneric<bool>(exp);

            var firstItem = (Condition)result;
            Assert.AreEqual("337fc4e428e84e0492db0af1f8af4459", firstItem.field);
            Assert.AreEqual("GreaterThan", firstItem.@operator);
            Assert.IsNull(firstItem.value);
            Assert.AreEqual("61eaff6e5c394bb5846a7c92c01d6358", firstItem.secondField);
        }

        private void Variables_ResolveVariableTypeInt(object sender, ResolveVariableTypeEventArgs e)
        {
            e.VariableType = typeof(int);
        }

        private void Variables_ResolveVariableTypeIntNullable(object sender, ResolveVariableTypeEventArgs e)
        {
            e.VariableType = typeof(int?);
        }

        #endregion

        #region Double

        [TestMethod]
        public void Parse_DoubleNotEqualsValue_Test()
        {
            var exp = "NotEquals(_337fc4e428e84e0492db0af1f8af4459;5,5)";

            var context = new ExpressionContext();
            var zadostExpr = new ZadostExpression();
            context.Imports.ImportBuiltinTypes();
            context.Imports.AddType(typeof(ExpressionFunctions));
            context.Variables.ResolveVariableType += Variables_ResolveVariableTypeDouble;

            var result = context.ParseGeneric<bool>(exp);

            var firstItem = (Condition)result;
            Assert.AreEqual("337fc4e428e84e0492db0af1f8af4459", firstItem.field);
            Assert.AreEqual("NotEquals", firstItem.@operator);
            Assert.AreEqual("5,5", firstItem.value);
            Assert.IsNull(firstItem.secondField);
        }

        private void Variables_ResolveVariableTypeDouble(object sender, ResolveVariableTypeEventArgs e)
        {
            e.VariableType = typeof(double);
        }

        #endregion

        #region Double

        [TestMethod]
        public void Parse_DateEqualsValue_Test()
        {
            var exp = "Equals(_337fc4e428e84e0492db0af1f8af4459;DateParse(\"1.1.2019\"))";

            var context = new ExpressionContext();
            var zadostExpr = new ZadostExpression();
            context.Imports.ImportBuiltinTypes();
            context.Imports.AddType(typeof(ExpressionFunctions));
            context.Imports.AddType(typeof(DateTime));
            context.Variables.ResolveVariableType += Variables_ResolveVariableTypeDate;

            var result = context.ParseGeneric<bool>(exp);

            var firstItem = (Condition)result;
            Assert.AreEqual("337fc4e428e84e0492db0af1f8af4459", firstItem.field);
            Assert.AreEqual("Equals", firstItem.@operator);
            Assert.AreEqual("1.1.2019", firstItem.value);
            Assert.IsNull(firstItem.secondField);
        }

        [TestMethod]
        public void Parse_DateGreaterOrEqualField_Test()
        {
            var exp = "GreaterOrEqual(_337fc4e428e84e0492db0af1f8af4459;_61eaff6e5c394bb5846a7c92c01d6358)";

            var context = new ExpressionContext();
            var zadostExpr = new ZadostExpression();
            context.Imports.ImportBuiltinTypes();
            context.Imports.AddType(typeof(ExpressionFunctions));
            context.Variables.ResolveVariableType += Variables_ResolveVariableTypeDate;

            var result = context.ParseGeneric<bool>(exp);

            var firstItem = (Condition)result;
            Assert.AreEqual("337fc4e428e84e0492db0af1f8af4459", firstItem.field);
            Assert.AreEqual("GreaterOrEqual", firstItem.@operator);
            Assert.IsNull(firstItem.value);
            Assert.AreEqual("61eaff6e5c394bb5846a7c92c01d6358", firstItem.secondField);
        }

        private void Variables_ResolveVariableTypeDate(object sender, ResolveVariableTypeEventArgs e)
        {
            e.VariableType = typeof(DateTime);
        }

        #endregion

        #region Boolean

        [TestMethod]
        public void Parse_BoolIsTrue_Test()
        {
            var exp = "IsTrue(_337fc4e428e84e0492db0af1f8af4459)";

            var context = new ExpressionContext();
            var zadostExpr = new ZadostExpression();
            context.Imports.ImportBuiltinTypes();
            context.Imports.AddType(typeof(ExpressionFunctions));
            context.Variables.ResolveVariableType += Variables_ResolveVariableTypeBool;

            var result = context.ParseGeneric<bool>(exp);

            var firstItem = (Condition)result;
            Assert.AreEqual("337fc4e428e84e0492db0af1f8af4459", firstItem.field);
            Assert.AreEqual("IsTrue", firstItem.@operator);
            Assert.IsNull(firstItem.value);
            Assert.IsNull(firstItem.secondField);
        }

        [TestMethod]
        public void Parse_NullableBoolIsTrue_Test()
        {
            var exp = "IsTrue(_337fc4e428e84e0492db0af1f8af4459)";

            var context = new ExpressionContext();
            var zadostExpr = new ZadostExpression();
            context.Imports.ImportBuiltinTypes();
            context.Imports.AddType(typeof(ExpressionFunctions));
            context.Variables.ResolveVariableType += Variables_ResolveVariableTypeNullableBool;

            var result = context.ParseGeneric<bool>(exp);

            var firstItem = (Condition)result;
            Assert.AreEqual("337fc4e428e84e0492db0af1f8af4459", firstItem.field);
            Assert.AreEqual("IsTrue", firstItem.@operator);
            Assert.IsNull(firstItem.value);
            Assert.IsNull(firstItem.secondField);
        }

        [TestMethod]
        public void Parse_BoolIsFalse_Test()
        {
            var exp = "IsFalse(_337fc4e428e84e0492db0af1f8af4459)";

            var context = new ExpressionContext();
            var zadostExpr = new ZadostExpression();
            context.Imports.ImportBuiltinTypes();
            context.Imports.AddType(typeof(ExpressionFunctions));
            context.Variables.ResolveVariableType += Variables_ResolveVariableTypeBool;

            var result = context.ParseGeneric<bool>(exp);

            var firstItem = (Condition)result;
            Assert.AreEqual("337fc4e428e84e0492db0af1f8af4459", firstItem.field);
            Assert.AreEqual("IsFalse", firstItem.@operator);
            Assert.IsNull(firstItem.value);
            Assert.IsNull(firstItem.secondField);
        }

        [TestMethod]
        public void Parse_BoolIsBlank_Test()
        {
            var exp = "IsBlank(_337fc4e428e84e0492db0af1f8af4459)";

            var context = new ExpressionContext();
            var zadostExpr = new ZadostExpression();
            context.Imports.ImportBuiltinTypes();
            context.Imports.AddType(typeof(ExpressionFunctions));
            context.Variables.ResolveVariableType += Variables_ResolveVariableTypeBool;

            var result = context.ParseGeneric<bool>(exp);

            var firstItem = (Condition)result;
            Assert.AreEqual("337fc4e428e84e0492db0af1f8af4459", firstItem.field);
            Assert.AreEqual("IsBlank", firstItem.@operator);
            Assert.IsNull(firstItem.value);
            Assert.IsNull(firstItem.secondField);
        }

        private void Variables_ResolveVariableTypeBool(object sender, ResolveVariableTypeEventArgs e)
        {
            e.VariableType = typeof(bool);
        }

        private void Variables_ResolveVariableTypeNullableBool(object sender, ResolveVariableTypeEventArgs e)
        {
            e.VariableType = typeof(bool?);
        }

        #endregion

        #region Regulární výrazy - porovnání skupin

        [TestMethod]
        public void MatchReGroups_Simple_Test()
        {
            var exp = "MatchReGroups(_4a11d5af8466473892e0998b3d4ad37a; \"a\"; \"b\"; \"c\")";

            var context = new ExpressionContext();
            var zadostExpr = new ZadostExpression();
            context.Imports.ImportBuiltinTypes();
            context.Imports.AddType(typeof(ExpressionFunctions));
            context.Variables.ResolveVariableType += Variables_ResolveVariableTypeString;

            var result = context.ParseGeneric<bool>(exp);

            var matchRe = (MatchReGroups)result;

            Assert.AreEqual("4a11d5af8466473892e0998b3d4ad37a", matchRe.field);
            Assert.AreEqual("CompareRegexGroups", matchRe.@operator);
            Assert.AreEqual("a", matchRe.re1);
            Assert.AreEqual("b", matchRe.value);
            Assert.AreEqual("c", matchRe.re2);
        }

        [TestMethod]
        public void MatchReGroups_WithRe_Test()
        {
            var exp = "MatchReGroups(_4a11d5af8466473892e0998b3d4ad37a; \"^(\\\\d{2})\"; _337fc4e428e84e0492db0af1f8af4459; \"(\\\\d{2})$\")";

            var context = new ExpressionContext();
            var zadostExpr = new ZadostExpression();
            context.Imports.ImportBuiltinTypes();
            context.Imports.AddType(typeof(ExpressionFunctions));
            context.Variables.ResolveVariableType += Variables_ResolveVariableTypeString;

            var result = context.ParseGeneric<bool>(exp);

            var matchRe = (MatchReGroups)result;

            Assert.AreEqual("4a11d5af8466473892e0998b3d4ad37a", matchRe.field);
            Assert.AreEqual("CompareRegexGroups", matchRe.@operator);
            Assert.AreEqual("^(\\d{2})", matchRe.re1);
            Assert.AreEqual("337fc4e428e84e0492db0af1f8af4459", matchRe.secondField);
            Assert.AreEqual("(\\d{2})$", matchRe.re2);
        }

        [TestMethod]
        public void MatchReGroups_Evaluate_Test()
        {
            var exp = "MatchReGroups(_4a11d5af8466473892e0998b3d4ad37a; \"a\"; _337fc4e428e84e0492db0af1f8af4459; \"b\")";

            var context = new ExpressionContext();
            var zadostExpr = new ZadostExpression();
            context.Imports.ImportBuiltinTypes();
            context.Imports.AddType(typeof(ExpressionFunctions));
            context.Variables.ResolveVariableType += ResolveVariableType_MatchReGroups_Evaluate;
            context.Variables.ResolveVariableValue += ResolveVariableValue_MatchReGroups_Evaluate;

            var method = context.CompileGeneric<bool>(exp);
            var result = method.Evaluate();

            Assert.IsTrue(result);
        }

        private void ResolveVariableValue_MatchReGroups_Evaluate(object sender, ResolveVariableValueEventArgs e)
        {
            if (e.VariableName == "_4a11d5af8466473892e0998b3d4ad37a")
            {
                e.VariableValue = "123";
            }
            else if (e.VariableName == "_337fc4e428e84e0492db0af1f8af4459")
            {
                e.VariableValue = new DateTime(1980, 1, 1);
            }
        }

        private void ResolveVariableType_MatchReGroups_Evaluate(object sender, ResolveVariableTypeEventArgs e)
        {
            if (e.VariableName == "_4a11d5af8466473892e0998b3d4ad37a")
            {
                e.VariableType = typeof(string);
            }
            else if (e.VariableName == "_337fc4e428e84e0492db0af1f8af4459")
            {
                e.VariableType = typeof(DateTime);
            }
        }

        #endregion
    }

    public class ZadostExpression
    {
        public bool ZadanaPriloha(string key)
        {
            return true;
        }

        public bool NezadanaPriloha(string key)
        {
            return !ZadanaPriloha(key);
        }
    }

    public static class ExpressionFunctions
    {
        public static bool Contains(string s1, string s2)
        {
            throw new NotImplementedException();
        }
        public static bool NotContains(string s1, string s2)
        {
            throw new NotImplementedException();
        }
        public static bool Equals(string s1, string s2)
        {
            throw new NotImplementedException();
        }
        public static bool NotEquals(string s1, string s2)
        {
            throw new NotImplementedException();
        }
        public static bool StartsWith(string s1, string s2)
        {
            throw new NotImplementedException();
        }
        public static bool EndsWith(string s1, string s2)
        {
            throw new NotImplementedException();
        }
        public static bool IsBlank(string value)
        {
            throw new NotImplementedException();
        }
        public static bool IsNotBlank(string value)
        {
            throw new NotImplementedException();
        }

        public static bool Equals(int i1, int i2)
        {
            throw new NotImplementedException();
        }
        public static bool Equals(int? i1, int i2)
        {
            throw new NotImplementedException();
        }
        public static bool Equals(int? i1, int? i2)
        {
            throw new NotImplementedException();
        }
        public static bool NotEquals(int i1, int i2)
        {
            throw new NotImplementedException();
        }
        public static bool NotEquals(int? i1, int i2)
        {
            throw new NotImplementedException();
        }
        public static bool NotEquals(int? i1, int? i2)
        {
            throw new NotImplementedException();
        }
        public static bool LessThan(int i1, int i2)
        {
            throw new NotImplementedException();
        }
        public static bool LessThan(int? i1, int i2)
        {
            throw new NotImplementedException();
        }
        public static bool LessThan(int? i1, int? i2)
        {
            throw new NotImplementedException();
        }
        public static bool LessOrEqual(int i1, int i2)
        {
            throw new NotImplementedException();
        }
        public static bool LessOrEqual(int? i1, int i2)
        {
            throw new NotImplementedException();
        }
        public static bool LessOrEqual(int? i1, int? i2)
        {
            throw new NotImplementedException();
        }
        public static bool GreaterThan(int i1, int i2)
        {
            throw new NotImplementedException();
        }
        public static bool GreaterThan(int? i1, int i2)
        {
            throw new NotImplementedException();
        }
        public static bool GreaterThan(int i1, int? i2)
        {
            throw new NotImplementedException();
        }
        public static bool GreaterThan(int? i1, int? i2)
        {
            throw new NotImplementedException();
        }
        public static bool GreaterOrEqual(int i1, int i2)
        {
            throw new NotImplementedException();
        }
        public static bool GreaterOrEqual(int? i1, int i2)
        {
            throw new NotImplementedException();
        }
        public static bool GreaterOrEqual(int? i1, int? i2)
        {
            throw new NotImplementedException();
        }
        public static bool IsBlank(int? value)
        {
            throw new NotImplementedException();
        }
        public static bool IsNotBlank(int? value)
        {
            throw new NotImplementedException();
        }

        public static bool Equals(double i1, double i2)
        {
            throw new NotImplementedException();
        }
        public static bool Equals(double? i1, double i2)
        {
            throw new NotImplementedException();
        }
        public static bool Equals(double? i1, double? i2)
        {
            throw new NotImplementedException();
        }
        public static bool NotEquals(double i1, double i2)
        {
            throw new NotImplementedException();
        }
        public static bool NotEquals(double? i1, double i2)
        {
            throw new NotImplementedException();
        }
        public static bool NotEquals(double? i1, double? i2)
        {
            throw new NotImplementedException();
        }
        public static bool LessThan(double i1, double i2)
        {
            throw new NotImplementedException();
        }
        public static bool LessThan(double? i1, double i2)
        {
            throw new NotImplementedException();
        }
        public static bool LessThan(double? i1, double? i2)
        {
            throw new NotImplementedException();
        }
        public static bool LessOrEqual(double i1, double i2)
        {
            throw new NotImplementedException();
        }
        public static bool LessOrEqual(double? i1, double i2)
        {
            throw new NotImplementedException();
        }
        public static bool LessOrEqual(double? i1, double? i2)
        {
            throw new NotImplementedException();
        }
        public static bool GreaterThan(double i1, double i2)
        {
            throw new NotImplementedException();
        }
        public static bool GreaterThan(double? i1, double i2)
        {
            throw new NotImplementedException();
        }
        public static bool GreaterThan(double? i1, double? i2)
        {
            throw new NotImplementedException();
        }
        public static bool GreaterOrEqual(double i1, double i2)
        {
            throw new NotImplementedException();
        }
        public static bool GreaterOrEqual(double? i1, double i2)
        {
            throw new NotImplementedException();
        }
        public static bool GreaterOrEqual(double? i1, double? i2)
        {
            throw new NotImplementedException();
        }
        public static bool IsBlank(double? value)
        {
            throw new NotImplementedException();
        }
        public static bool IsNotBlank(double? value)
        {
            throw new NotImplementedException();
        }

        //public static bool Equals(DateTime i1, DateTime i2)
        //{
        //    throw new NotImplementedException();
        //}
        public static bool Equals(DateTime? i1, DateTime i2)
        {
            throw new NotImplementedException();
        }
        public static bool Equals(DateTime i1, DateTime? i2)
        {
            throw new NotImplementedException();
        }
        public static bool Equals(DateTime? i1, DateTime? i2)
        {
            throw new NotImplementedException();
        }
        public static bool NotEquals(DateTime i1, DateTime i2)
        {
            throw new NotImplementedException();
        }
        public static bool NotEquals(DateTime? i1, DateTime i2)
        {
            throw new NotImplementedException();
        }
        public static bool NotEquals(DateTime? i1, DateTime? i2)
        {
            throw new NotImplementedException();
        }
        public static bool LessThan(DateTime i1, DateTime i2)
        {
            throw new NotImplementedException();
        }
        public static bool LessThan(DateTime? i1, DateTime i2)
        {
            throw new NotImplementedException();
        }
        public static bool LessThan(DateTime? i1, DateTime? i2)
        {
            throw new NotImplementedException();
        }
        public static bool LessOrEqual(DateTime i1, DateTime i2)
        {
            throw new NotImplementedException();
        }
        public static bool LessOrEqual(DateTime? i1, DateTime i2)
        {
            throw new NotImplementedException();
        }
        public static bool LessOrEqual(DateTime? i1, DateTime? i2)
        {
            throw new NotImplementedException();
        }
        public static bool GreaterThan(DateTime i1, DateTime i2)
        {
            throw new NotImplementedException();
        }
        public static bool GreaterThan(DateTime? i1, DateTime i2)
        {
            throw new NotImplementedException();
        }
        public static bool GreaterThan(DateTime? i1, DateTime? i2)
        {
            throw new NotImplementedException();
        }
        public static bool GreaterOrEqual(DateTime i1, DateTime i2)
        {
            throw new NotImplementedException();
        }
        public static bool GreaterOrEqual(DateTime? i1, DateTime i2)
        {
            throw new NotImplementedException();
        }
        public static bool GreaterOrEqual(DateTime? i1, DateTime? i2)
        {
            throw new NotImplementedException();
        }
        public static bool IsBlank(DateTime? value)
        {
            throw new NotImplementedException();
        }
        public static bool IsNotBlank(DateTime? value)
        {
            throw new NotImplementedException();
        }

        public static bool IsTrue(bool value)
        {
            throw new NotImplementedException();
        }
        public static bool IsFalse(bool value)
        {
            throw new NotImplementedException();
        }
        public static bool IsTrue(bool? value)
        {
            throw new NotImplementedException();
        }
        public static bool IsFalse(bool? value)
        {
            throw new NotImplementedException();
        }
        public static bool IsBlank(bool? value)
        {
            throw new NotImplementedException();
        }
        public static bool IsNotBlank(bool? value)
        {
            throw new NotImplementedException();
        }

        public static DateTime DateParse(string value)
        {
            return DateTime.Parse(value);
        }

        public static bool MatchReGroups(string arg1, string re1, int? arg2, string re2)
        {
            throw new NotImplementedException();
        }

        public static bool MatchReGroups(string arg1, string re1, int arg2, string re2)
        {
            throw new NotImplementedException();
        }

        public static bool MatchReGroups(string arg1, string re1, double? arg2, string re2)
        {
            throw new NotImplementedException();
        }

        public static bool MatchReGroups(string arg1, string re1, double arg2, string re2)
        {
            throw new NotImplementedException();
        }

        public static bool MatchReGroups(string arg1, string re1, DateTime? arg2, string re2)
        {
            return true;
        }

        public static bool MatchReGroups(string arg1, string re1, DateTime arg2, string re2)
        {
            return true;
        }

        public static bool MatchReGroups(string arg1, string re1, string arg2, string re2)
        {
            throw new NotImplementedException();
        }
    }
}
