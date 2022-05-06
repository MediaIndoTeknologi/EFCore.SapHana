using System;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace WonderKidEngine.EntityFrameworkCore.SapHana.Storage.Internal
{
    public class BytesToDateTimeConverter : ValueConverter<byte[], DateTime>
    {
        private static readonly NumberToBytesConverter<long> _longToBytes
            = new NumberToBytesConverter<long>();

        public BytesToDateTimeConverter()
            : base(
                v => DateTime.FromBinary((long)_longToBytes.ConvertFromProvider(v)),
                v => (byte[])_longToBytes.ConvertToProvider(v.ToBinary()))
        {
        }
    }
}
