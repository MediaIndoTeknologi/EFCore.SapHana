using System;
using System.Data.Common;
using System.Linq;
using Sap.Data.Hana;

namespace WonderKidEngine.EntityFrameworkCore.SapHana.Storage.Internal
{
    public class SapHanaConnectionSettings
    {
        public SapHanaConnectionSettings()
        {
        }

        public SapHanaConnectionSettings(DbConnection connection)
            : this(connection.ConnectionString)
        {
        }

        public SapHanaConnectionSettings(string connectionString)
        {
        }
        protected virtual bool Equals(SapHanaConnectionSettings other)
        {
            return false;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj.GetType() != this.GetType())
            {
                return false;
            }

            return Equals((SapHanaConnectionSettings)obj);
        }

        public override int GetHashCode() { return 0; }
    }
}
