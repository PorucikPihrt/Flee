using System;
using Flee.ExpressionElements.Base;
using Flee.ExpressionElements.Literals;
using Flee.ExpressionElements.Literals.Integral;
using Flee.ExpressionElements.Literals.Real;
using Flee.ExpressionElements.MemberElements;

namespace Flee.ExpressionEditor
{
    public class Condition : Item
    {
        public override string type
        {
            get { return "condition"; }
        }

        public string field { get; set; }

        public string @operator { get; set; }

        public string @value { get; set; }

        public string secondField { get; set; }

        internal void InitField(ExpressionElement arg)
        {
            var arg0 = ((InvocationListElement)arg).Tail;
            field = ((IdentifierElement)arg0).MemberName.Replace("_", "");
        }

        internal void InitStringValueOrSecondField(ExpressionElement arg)
        {
            if (arg is StringLiteralElement)
            {
                @value = ((StringLiteralElement)arg).Value;
            }
            else if (arg is InvocationListElement)
            {
                var invocation = ((InvocationListElement)arg).Tail;
                secondField = ((IdentifierElement)invocation).MemberName.Replace("_", "");
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        internal void InitValueOrSecondField(ExpressionElement arg)
        {
            if (arg is Int32LiteralElement)
            {
                @value = ((Int32LiteralElement)arg).Value.ToString();
            }
            else if (arg is DoubleLiteralElement)
            {
                value = ((DoubleLiteralElement)arg).Value.ToString();
            }
            else if (arg is DateTimeLiteralElement)
            {
                value = ((DateTimeLiteralElement)arg).Value.ToString();
            }
            else if (arg is BooleanLiteralElement)
            {
                value = ((BooleanLiteralElement)arg).Value.ToString();
            }
            else if (arg is InvocationListElement)
            {
                var tail = ((InvocationListElement)arg).Tail;
                if (tail is IdentifierElement)
                {
                    secondField = ((IdentifierElement)tail).MemberName.Replace("_", "");
                }
                else if (tail is FunctionCallElement)
                {
                    var funcCall = (FunctionCallElement)tail;
                    if (funcCall.MemberName == "DateParse")
                    {
                        InitStringValueOrSecondField(funcCall.Arguments[0]);
                    }
                }
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        internal void InitBoolOperator(ExpressionElement arg)
        {
            if (arg is BooleanLiteralElement)
            {
                var boolArg = (BooleanLiteralElement)arg;
                @operator = (boolArg.Value) ? "isTrue" : "isFalse";
            }
        }
    }
}
