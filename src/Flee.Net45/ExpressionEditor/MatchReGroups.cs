using Flee.ExpressionElements.Base;
using Flee.ExpressionElements.Literals;
using Flee.ExpressionElements.MemberElements;

namespace Flee.ExpressionEditor
{
    public class MatchReGroups : Condition
    {
        public MatchReGroups()
        {
        }

        internal MatchReGroups(ArgumentList arguments)
        {
            @operator = "CompareRegexGroups";
            InitField(arguments[0]);
            InitStringValueOrSecondField(arguments[2]);
            re1 = GetStringValue(arguments[1]);
            re2 = GetStringValue(arguments[3]);
        }

        public override string type
        {
            get { return "MatchReGroups"; }
        }

        public string re1 { get; set; }

        public string re2 { get; set; }

        private static string GetStringValue(ExpressionElement expressionElement)
        {
            var sle = (StringLiteralElement)expressionElement;
            return sle.Value;
        }
    }
}
