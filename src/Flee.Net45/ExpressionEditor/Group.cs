using System;
using System.Collections.Generic;

namespace Flee.ExpressionEditor
{
    public class Group : Item
    {
        internal static string AND = "And";
        internal static string OR = "Or";
        internal static string NOT_PREFIX = "Not";
        internal static string NOT_AND = NOT_PREFIX + AND;
        internal static string NOT_OR = NOT_PREFIX + OR;

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

        internal bool CanFlattenGroups()
        {
            bool canFlatten = operation == AND;

            foreach (var item in items)
            {
                if (item is Group group)
                {
                    canFlatten &= group.CanFlattenGroups();
                }
            }

            return canFlatten;
        }

        internal void FlattenGroups()
        {
            for (int i = 0; i < items.Count; i++)
            {
                var group = items[i] as Group;
                if (group != null)
                {
                    group.FlattenGroups();
                    items.RemoveAt(i);
                    foreach (var item in group.items)
                    {
                        items.Add(item);
                    }
                }
            }
        }

        internal void TryFlattening()
        {
            if (CanFlattenGroups())
            {
                FlattenGroups();
            }
            else
            {
                foreach (var item in items)
                {
                    if (item is Group group)
                    {
                        group.TryFlattening();
                    }
                }
            }
        }
    }
}
