using System.Collections.Generic;

namespace Flee.ExpressionEditor
{
    public class Group : Item
    {
        public Group()
        {
            items = new List<Item>();
        }

        public override string type
        {
            get { return "group"; }
        }

        public string operation { get; set; }

        public List<Item> items { get; set; }
    }
}
