using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using WonderKidEngine.EntityFrameworkCore.SapHana.Infrastructure.Internal;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using WonderKidEngine.EntityFrameworkCore.SapHana.Infrastructure;
using WonderKidEngine.EntityFrameworkCore.SapHana.Scaffolding.Internal;

namespace WonderKidEngine.EntityFrameworkCore.SapHana.Storage.Internal
{
    // ReSharper disable PrivateFieldCanBeConvertedToLocalVariable
    public class SapHanaTypeMappingSource : RelationalTypeMappingSource
    {
        // boolean
        private readonly SapHanaBoolTypeMapping _boolean = new SapHanaBoolTypeMapping("boolean", size: 1);

        // bit
        private readonly ULongTypeMapping _bit = new ULongTypeMapping("bit", DbType.UInt64);

        // integers
        private readonly SByteTypeMapping _tinyint = new SByteTypeMapping("tinyint", DbType.SByte);
        private readonly ByteTypeMapping _utinyint = new ByteTypeMapping("tinyint unsigned", DbType.Byte);
        private readonly ShortTypeMapping _smallint = new ShortTypeMapping("smallint", DbType.Int16);
        private readonly UShortTypeMapping _usmallint = new UShortTypeMapping("smallint unsigned", DbType.UInt16);
        private readonly IntTypeMapping _int = new IntTypeMapping("int", DbType.Int32);
        private readonly UIntTypeMapping _uint = new UIntTypeMapping("int unsigned", DbType.UInt32);
        private readonly LongTypeMapping _bigint = new LongTypeMapping("bigint", DbType.Int64);
        private readonly ULongTypeMapping _ubigint = new ULongTypeMapping("bigint unsigned", DbType.UInt64);

        // decimals
        private readonly SapHanaDecimalTypeMapping _decimal = new SapHanaDecimalTypeMapping("decimal", precision: 65, scale: 30);
        private readonly SapHanaDoubleTypeMapping _double = new SapHanaDoubleTypeMapping("double", DbType.Double);
        private readonly SapHanaFloatTypeMapping _float = new SapHanaFloatTypeMapping("float", DbType.Single);

        // binary
        private readonly RelationalTypeMapping _binary = new SapHanaByteArrayTypeMapping(fixedLength: true);
        private readonly RelationalTypeMapping _varbinary = new SapHanaByteArrayTypeMapping();

        //
        // String mappings depend on the SapHanaOptions.NoBackslashEscapes setting:
        //

        private SapHanaStringTypeMapping _charUnicode;
        private SapHanaStringTypeMapping _varcharUnicode;
        private SapHanaStringTypeMapping _tinytextUnicode;
        private SapHanaStringTypeMapping _textUnicode;
        private SapHanaStringTypeMapping _mediumtextUnicode;
        private SapHanaStringTypeMapping _longtextUnicode;

        private SapHanaStringTypeMapping _nchar;
        private SapHanaStringTypeMapping _nvarchar;

        private SapHanaStringTypeMapping _enum;

        // DateTime
        private readonly SapHanaDateTypeMapping _dateDateOnly = new SapHanaDateTypeMapping("date", typeof(DateOnly));
        private readonly SapHanaDateTypeMapping _dateDateTime = new SapHanaDateTypeMapping("date", typeof(DateTime));
        private readonly SapHanaTimeTypeMapping _timeTimeOnly = new SapHanaTimeTypeMapping("time", typeof(TimeOnly));
        private readonly SapHanaTimeTypeMapping _timeTimeSpan = new SapHanaTimeTypeMapping("time", typeof(TimeSpan));
        private readonly SapHanaDateTimeTypeMapping _dateTime = new SapHanaDateTimeTypeMapping("datetime");
        private readonly SapHanaDateTimeTypeMapping _timeStamp = new SapHanaDateTimeTypeMapping("timestamp");
        private readonly SapHanaDateTimeOffsetTypeMapping _dateTimeOffset = new SapHanaDateTimeOffsetTypeMapping("datetime");
        private readonly SapHanaDateTimeOffsetTypeMapping _timeStampOffset = new SapHanaDateTimeOffsetTypeMapping("timestamp");

        private readonly RelationalTypeMapping _binaryRowVersion
            = new SapHanaDateTimeTypeMapping(
                "timestamp",
                null,
                typeof(byte[]),
                new BytesToDateTimeConverter(),
                new ByteArrayComparer());
        private readonly RelationalTypeMapping _binaryRowVersion6
            = new SapHanaDateTimeTypeMapping(
                "timestamp",
                6,
                typeof(byte[]),
                new BytesToDateTimeConverter(),
                new ByteArrayComparer());

        // guid
        private GuidTypeMapping _guid;


        // Scaffolding type mappings
        private readonly SapHanaCodeGenerationMemberAccessTypeMapping _codeGenerationMemberAccess = new SapHanaCodeGenerationMemberAccessTypeMapping();

        private Dictionary<string, RelationalTypeMapping[]> _storeTypeMappings;
        private Dictionary<Type, RelationalTypeMapping> _clrTypeMappings;
        private Dictionary<Type, RelationalTypeMapping> _scaffoldingClrTypeMappings;

        private readonly ISapHanaOptions _options;

        private bool _initialized;
        private readonly object _initializationLock = new object();

        public SapHanaTypeMappingSource(
            [NotNull] TypeMappingSourceDependencies dependencies,
            [NotNull] RelationalTypeMappingSourceDependencies relationalDependencies,
            [NotNull] ISapHanaOptions options)
            : base(dependencies, relationalDependencies)
        {
            _options = options;
        }

        private void Initialize()
        {
            //
            // String mappings depend on the SapHanaOptions.NoBackslashEscapes setting:
            //

            _charUnicode = new SapHanaStringTypeMapping("char", _options, StoreTypePostfix.Size, fixedLength: true);
            _varcharUnicode = new SapHanaStringTypeMapping("varchar", _options, StoreTypePostfix.Size);
            _tinytextUnicode = new SapHanaStringTypeMapping("tinytext", _options, StoreTypePostfix.None);
            _textUnicode = new SapHanaStringTypeMapping("text", _options, StoreTypePostfix.None);
            _mediumtextUnicode = new SapHanaStringTypeMapping("mediumtext", _options, StoreTypePostfix.None);
            _longtextUnicode = new SapHanaStringTypeMapping("longtext", _options, StoreTypePostfix.None);

            _nchar = new SapHanaStringTypeMapping("nchar", _options, StoreTypePostfix.Size, fixedLength: true);
            _nvarchar = new SapHanaStringTypeMapping("nvarchar", _options, StoreTypePostfix.Size);

            _enum = new SapHanaStringTypeMapping("enum", _options, StoreTypePostfix.None);

            _storeTypeMappings
                = new Dictionary<string, RelationalTypeMapping[]>(StringComparer.OrdinalIgnoreCase)
                {
                    // bit
                    { "boolean",                       new[] { _boolean } },

                    // integers
                    { "tinyint",                   new[] { _tinyint } },
                    { "tinyint unsigned",          new[] { _utinyint } },
                    { "smallint",                  new[] { _smallint } },
                    { "smallint unsigned",         new[] { _usmallint } },
                    { "mediumint",                 new[] { _int } },
                    { "mediumint unsigned",        new[] { _uint } },
                    { "int",                       new[] { _int } },
                    { "int unsigned",              new[] { _uint } },
                    { "integer",                   new[] { _int } },
                    { "integer unsigned",          new[] { _uint } },
                    { "bigint",                    new[] { _bigint } },
                    { "bigint unsigned",           new[] { _ubigint } },

                    // decimals
                    { "decimal",                   new[] { _decimal } },
                    { "decimal unsigned",          new[] { _decimal } }, // deprecated since 8.0.17-SapHana
                    { "numeric",                   new[] { _decimal } },
                    { "numeric unsigned",          new[] { _decimal } }, // deprecated since 8.0.17-SapHana
                    { "dec",                       new[] { _decimal } },
                    { "dec unsigned",              new[] { _decimal } }, // deprecated since 8.0.17-SapHana
                    { "fixed",                     new[] { _decimal } },
                    { "fixed unsigned",            new[] { _decimal } }, // deprecated since 8.0.17-SapHana
                    { "double",                    new[] { _double } },
                    { "double unsigned",           new[] { _double } }, // deprecated since 8.0.17-SapHana
                    { "double precision",          new[] { _double } },
                    { "double precision unsigned", new[] { _double } }, // deprecated since 8.0.17-SapHana
                    { "real",                      new[] { _double } },
                    { "real unsigned",             new[] { _double } }, // deprecated since 8.0.17-SapHana
                    { "float",                     new[] { _float } },
                    { "float unsigned",            new[] { _float } }, // deprecated since 8.0.17-SapHana

                    // binary
                    { "binary",                    new[] { _binary } },
                    { "varbinary",                 new[] { _varbinary } },
                    { "tinyblob",                  new[] { _varbinary } },
                    { "blob",                      new[] { _varbinary } },
                    { "mediumblob",                new[] { _varbinary } },
                    { "longblob",                  new[] { _varbinary } },

                    // string
                    { "char",                      new[] { _charUnicode } },
                    { "varchar",                   new[] { _varcharUnicode } },
                    { "tinytext",                  new[] { _tinytextUnicode } },
                    { "text",                      new[] { _textUnicode } },
                    { "mediumtext",                new[] { _mediumtextUnicode } },
                    { "longtext",                  new[] { _longtextUnicode } },

                    { "enum",                      new[] { _enum } },

                    { "nchar",                     new[] { _nchar } },
                    { "nvarchar",                  new[] { _nvarchar } },

                    // DateTime
                    { "date",                      new RelationalTypeMapping[] { _dateDateOnly, _dateDateTime } },
                    { "time",                      new RelationalTypeMapping[] { _timeTimeOnly, _timeTimeSpan } },
                    { "datetime",                  new RelationalTypeMapping[] { _dateTime, _dateTimeOffset } },
                    { "timestamp",                 new RelationalTypeMapping[] { _timeStamp, _timeStampOffset } },
                };

            _clrTypeMappings
                = new Dictionary<Type, RelationalTypeMapping>
                {
                    { typeof(bool),    _boolean },
	                // integers
	                { typeof(short),    _smallint },
                    { typeof(ushort),   _usmallint },
                    { typeof(int),      _int },
                    { typeof(uint),     _uint },
                    { typeof(long),     _bigint },
                    { typeof(ulong),    _ubigint },

	                // decimals
	                { typeof(decimal),  _decimal },
                    { typeof(float),    _float },
                    { typeof(double),   _double },

	                // byte / char
	                { typeof(sbyte),    _tinyint },
                    { typeof(byte),     _utinyint },

                    // datetimes
                    { typeof(DateOnly), _dateDateOnly },
                    { typeof(TimeOnly), _timeTimeOnly.Clone(_options.DefaultDataTypeMappings.ClrTimeOnlyPrecision, null) },
                    { typeof(TimeSpan), _options.DefaultDataTypeMappings.ClrTimeSpan switch
                        {
                            SapHanaTimeSpanType.Time6 => _timeTimeSpan.Clone(6, null),
                            SapHanaTimeSpanType.Time => _timeTimeSpan,
                            _ => _timeTimeSpan
                        }},
                    { typeof(DateTime), _options.DefaultDataTypeMappings.ClrDateTime switch
                        {
                            SapHanaDateTimeType.DateTime6 =>_dateTime.Clone(6, null),
                            SapHanaDateTimeType.Timestamp6 => _timeStamp.Clone(6, null),
                            SapHanaDateTimeType.Timestamp => _timeStamp,
                            _ => _dateTime,
                        }},
                    { typeof(DateTimeOffset), _options.DefaultDataTypeMappings.ClrDateTimeOffset switch
                        {
                            SapHanaDateTimeType.DateTime6 =>_dateTimeOffset.Clone(6, null),
                            SapHanaDateTimeType.Timestamp6 => _timeStampOffset.Clone(6, null),
                            SapHanaDateTimeType.Timestamp => _timeStampOffset,
                            _ => _dateTimeOffset,
                        }}
                };

            // Guid
            if (_guid != null)
            {
                _storeTypeMappings[_guid.StoreType] = new RelationalTypeMapping[]{ _guid };
                _clrTypeMappings[typeof(Guid)] = _guid;
            }

            // Type mappings that only exist to work around the limited code generation capabilites when scaffolding:
            _scaffoldingClrTypeMappings = new Dictionary<Type, RelationalTypeMapping>
            {
                { typeof(SapHanaCodeGenerationMemberAccess), _codeGenerationMemberAccess }
            };
        }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        protected override RelationalTypeMapping FindMapping(in RelationalTypeMappingInfo mappingInfo) =>
            // first, try any plugins, allowing them to override built-in mappings
            base.FindMapping(mappingInfo) ??
            FindRawMapping(mappingInfo)?.Clone(mappingInfo);

        private RelationalTypeMapping FindRawMapping(RelationalTypeMappingInfo mappingInfo)
        {
            // Use deferred initialization to support connection (string) based type mapping in
            // design time mode (scaffolder etc.).
            // This is a singleton class and therefore needs to be thread-safe.
            if (!_initialized)
            {
                lock (_initializationLock)
                {
                    if (!_initialized)
                    {
                        Initialize();
                        _initialized = true;
                    }
                }
            }

            var clrType = mappingInfo.ClrType;
            var storeTypeName = mappingInfo.StoreTypeName;
            var storeTypeNameBase = mappingInfo.StoreTypeNameBase;

            if (storeTypeName != null)
            {
                // First look for the fully qualified store type name.
                if (_storeTypeMappings.TryGetValue(storeTypeName, out var mappings))
                {
                    // We found the user-specified store type.
                    // If no CLR type was provided, we're probably scaffolding from an existing database. Take the first
                    // mapping as the default.
                    // If a CLR type was provided, look for a mapping between the store and CLR types. If none is found,
                    // fail immediately.
                    return clrType == null
                        ? mappings[0]
                        : mappings.FirstOrDefault(m => m.ClrType == clrType);
                }

                // Then look for the base store type name.
                if (_storeTypeMappings.TryGetValue(storeTypeNameBase, out mappings))
                {
                    return clrType == null
                        ? mappings[0]
                            .Clone(in mappingInfo)
                        : mappings.FirstOrDefault(m => m.ClrType == clrType)
                            ?.Clone(in mappingInfo);
                }

                // A store type name was provided, but is unknown. This could be a domain (alias) type, in which case
                // we proceed with a CLR type lookup (if the type doesn't exist at all the failure will come later).
            }

            if (clrType != null)
            {
                if (_clrTypeMappings.TryGetValue(clrType, out var mapping))
                {
                    // If needed, clone the mapping with the configured length/precision/scale
                    if (mappingInfo.Precision.HasValue)
                    {
                        if (clrType == typeof(decimal))
                        {
                            return mapping.Clone(mappingInfo.Precision.Value, mappingInfo.Scale);
                        }

                        if (clrType == typeof(DateTime) ||
                            clrType == typeof(DateTimeOffset) ||
                            clrType == typeof(TimeSpan))
                        {
                            return mapping.Clone(mappingInfo.Precision.Value, null);
                        }
                    }

                    return mapping;
                }

                if (clrType == typeof(string))
                {
                    var isFixedLength = mappingInfo.IsFixedLength == true;

                    //int? size = mappingInfo.Size ??
                    //           (mappingInfo.IsKeyOrIndex &&
                    //            _options.LimitKeyedOrIndexedStringColumnLength
                    //               // Allow to use at most half of the max key length, so at least 2 columns can fit
                    //               ? Math.Min(_options.ServerVersion.MaxKeyLength / (_options.DefaultCharSet.MaxBytesPerChar * 2), 255)
                    //               : (int?)null);
                    int? size = null;
                    // If a string column size is bigger than it can/might be, we automatically adjust it to a variable one with an
                    // unlimited size.
                    if (size > 65_553 / _options.DefaultCharSet.MaxBytesPerChar)
                    {
                        size = null;
                        isFixedLength = false;
                    }

                    mapping = isFixedLength
                        ? _charUnicode
                        : size == null
                            ? _longtextUnicode
                            : _varcharUnicode;

                    return size == null
                        ? mapping
                        : mapping.Clone($"{mapping.StoreTypeNameBase}({size})", size);
                }

                if (clrType == typeof(byte[]))
                {
                    if (mappingInfo.IsRowVersion == true)
                    {
                        return _binaryRowVersion6;

                            /*
                            _options.ServerVersion.Supports.DateTime6
                            ? _binaryRowVersion6
                            : _binaryRowVersion;
                            */
                    }

                    int? size = null;
                    /*
                    var size = mappingInfo.Size ??
                               (mappingInfo.IsKeyOrIndex
                                   ? _options.ServerVersion.MaxKeyLength
                                   : (int?)null);*/

                    return new SapHanaByteArrayTypeMapping(
                        size: size,
                        fixedLength: mappingInfo.IsFixedLength == true);
                }

                if (_scaffoldingClrTypeMappings.TryGetValue(clrType, out mapping))
                {
                    return mapping;
                }
            }

            return null;
        }

        protected override string ParseStoreTypeName(string storeTypeName, out bool? unicode, out int? size, out int? precision, out int? scale)
        {
            var storeTypeBaseName = base.ParseStoreTypeName(storeTypeName, out unicode, out size, out precision, out scale);

            if (storeTypeBaseName is not null)
            {
                // We are checking for a character set clause as part of the store type base name here, because it was common before 5.0
                // to specify charsets this way, because there were no character set specific annotations available yet.
                // Users might still use migrations generated with previous versions and just add newer migrations on top of those.
                var characterSetOccurrenceIndex = storeTypeBaseName.IndexOf("character set", StringComparison.OrdinalIgnoreCase);

                if (characterSetOccurrenceIndex < 0)
                {
                    characterSetOccurrenceIndex = storeTypeBaseName.IndexOf("charset", StringComparison.OrdinalIgnoreCase);
                }

                if (characterSetOccurrenceIndex >= 0)
                {
                    storeTypeBaseName = storeTypeBaseName[..characterSetOccurrenceIndex].TrimEnd();
                }

                if (storeTypeName.Contains("unsigned", StringComparison.OrdinalIgnoreCase))
                {
                    storeTypeBaseName += " unsigned";
                }
            }

            return storeTypeBaseName;
        }
    }
}
