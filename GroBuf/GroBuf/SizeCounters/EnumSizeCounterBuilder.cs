using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using GroBuf.DataMembersExtracters;

namespace GroBuf.SizeCounters
{
    internal class EnumSizeCounterBuilder : SizeCounterBuilderBase
    {
        public EnumSizeCounterBuilder(Type type)
            : base(type)
        {
            if(!Type.IsEnum) throw new InvalidOperationException("Enum expected but was '" + Type + "'");
        }

        protected override void BuildConstantsInternal(SizeCounterConstantsBuilderContext context)
        {
            context.SetFields(Type, new[]
                {
                    new KeyValuePair<string, Type>("hashCodes_" + Type.Name + "_" + Guid.NewGuid(), typeof(ulong[])),
                    new KeyValuePair<string, Type>("values_" + Type.Name + "_" + Guid.NewGuid(), typeof(int[])),
                });
        }

        protected override void CountSizeNotEmpty(SizeCounterMethodBuilderContext context)
        {
            var table = BuildHashCodesTable();
            var hashCodes = table.Key;
            var values = table.Value;
            var hashCodesField = context.Context.InitConstField(Type, 0, hashCodes);
            var valuesField = context.Context.InitConstField(Type, 1, values);

            var il = context.Il;
            context.LoadField(valuesField); // stack: [values]
            context.LoadObj(); // stack: [values, obj]
            il.Ldc_I4(values.Length); // stack: [values, obj, values.Length]
            il.Rem(true); // stack: [values, obj % values.Length]
            il.Ldelem(typeof(int)); // stack: [values[obj % values.Length]]
            context.LoadObj(); // stack: [values[obj % values.Length], obj]
            il.Ceq(); // stack: [values[obj % values.Length] == obj]
            var countAsIntLabel = il.DefineLabel("countAsInt");
            il.Brfalse(countAsIntLabel); // if(values[obj % values.Length] != obj) goto countAsInt
            context.LoadField(hashCodesField); // stack: [hashCodes]
            context.LoadObj(); // stack: [hashCodes, obj]
            il.Ldc_I4(hashCodes.Length); // stack: [hashCodes, obj, hashCodes.Length]
            il.Rem(true); // stack: [hashCodes, obj % hashCodes.Length]
            il.Ldelem(typeof(long)); // stack: [hashCodes[obj % hashCodes.Length] = hashCode]
            il.Brfalse(countAsIntLabel); // if(hashCode == 0) goto countAsInt;
            il.Ldc_I4(9); // stack: [9]
            il.Ret(); // return 9;
            il.MarkLabel(countAsIntLabel);
            il.Ldc_I4(5); // stack: [5]
        }

        protected override bool IsReference { get { return false; } }

        private KeyValuePair<ulong[], int[]> BuildHashCodesTable()
        {
            var fields = Type.GetFields(BindingFlags.Public | BindingFlags.Static);
            var values = fields.Select(field => (int)Enum.Parse(Type, field.Name)).ToArray();
            var uniqueValues = new HashSet<int>(values).ToArray();
            var nameHashes = GroBufHelpers.CalcHashesAndCheck(fields.Select(DataMember.Create));
            var hashSet = new HashSet<uint>();
            for(var x = (uint)values.Length;; ++x)
            {
                hashSet.Clear();
                var ok = true;
                foreach(var value in uniqueValues)
                {
                    var item = (uint)(value % x);
                    if(hashSet.Contains(item))
                    {
                        ok = false;
                        break;
                    }
                    hashSet.Add(item);
                }
                if(!ok) continue;
                var hashCodes = new ulong[x];
                var valuez = new int[x];
                for(var i = 0; i < x; ++i)
                    valuez[i] = -1;
                for(var i = 0; i < values.Length; i++)
                {
                    var value = values[i];
                    var index = (int)(value % x);
                    hashCodes[index] = nameHashes[i];
                    valuez[index] = value;
                }
                return new KeyValuePair<ulong[], int[]>(hashCodes, valuez);
            }
        }
    }
}