using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Flee.PublicTypes;

namespace ParsingTest
{
    class Program
    {
        static void Main(string[] args)
        {
            var strExp = "NOT (Zadost.NezadanaPriloha(\"50574d8389b94e6d9024eb2967565dc7\") AND _337fc4e428e84e0492db0af1f8af4458 <> null)";

            var context = new ExpressionContext();
            var zadostExpr = new ZadostExpression();
            context.Imports.ImportBuiltinTypes();
            context.Variables.Add("Zadost", zadostExpr);
            context.Variables.ResolveVariableType += Variables_ResolveVariableType;
            context.Variables.ResolveVariableValue += Variables_ResolveVariableValue;

            var exp = context.CompileGeneric<bool>(strExp);
            bool result = exp.Evaluate();
        }

        private static void Variables_ResolveVariableValue(object sender, ResolveVariableValueEventArgs e)
        {
            e.VariableValue = "CZ12345678";
        }

        private static void Variables_ResolveVariableType(object sender, ResolveVariableTypeEventArgs e)
        {
            e.VariableType = typeof(string);
        }
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
}
