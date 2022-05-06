﻿// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

namespace WonderKidEngine.EntityFrameworkCore.SapHana.Infrastructure
{
    public delegate string SapHanaSchemaNameTranslator(string schemaName, string objectName);

    public enum SapHanaSchemaBehavior
    {
        /// <summary>
        /// Throw an exception if a schema is being used. Any specified translator delegate will be ignored.
        /// This is the default.
        /// </summary>
        Throw,

        /// <summary>
        /// Silently ignore any schema definitions. Any specified translator delegate will be ignored.
        /// </summary>
        Ignore,

        /// <summary>
        /// Use the specified translator delegate to translate from an input schema and object name to
        /// an output object name whenever a schema is being used.
        /// </summary>
        Translate,
    }
}
