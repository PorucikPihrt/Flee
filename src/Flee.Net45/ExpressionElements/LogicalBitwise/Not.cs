using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection.Emit;
using Flee.ExpressionEditor;
using Flee.ExpressionElements.Base;
using Flee.InternalTypes;


namespace Flee.ExpressionElements.LogicalBitwise
{
    internal class NotElement : UnaryElement
    {
        public override void Emit(FleeILGenerator ilg, IServiceProvider services)
        {
            if (object.ReferenceEquals(MyChild.ResultType, typeof(bool)))
            {
                this.EmitLogical(ilg, services);
            }
            else
            {
                MyChild.Emit(ilg, services);
                ilg.Emit(OpCodes.Not);
            }
        }

        internal override Item GetItem(IServiceProvider services)
        {
            var item = MyChild.GetItem(services);
            var group = item as Group;
            if (group != null)
            {
                group.operation = Group.NOT_PREFIX + group.operation;
            }
            return item;
        }

        private void EmitLogical(FleeILGenerator ilg, IServiceProvider services)
        {
            MyChild.Emit(ilg, services);
            ilg.Emit(OpCodes.Ldc_I4_0);
            ilg.Emit(OpCodes.Ceq);
        }

        protected override System.Type GetResultType(System.Type childType)
        {
            if (object.ReferenceEquals(childType, typeof(bool)))
            {
                return typeof(bool);
            }
            else if (Utility.IsIntegralType(childType) == true)
            {
                return childType;
            }
            else
            {
                return null;
            }
        }
    }
}
