using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Vue.Net
{
    public abstract class VueModel
    {
        public static readonly Dictionary<string, string> LifeCycleMethodsNameMapping = new Dictionary<string, string>
        {
            { nameof(beforeCreated), "beforeCreated" },
            { nameof(created), "created" },
            { nameof(beforeMount), "beforeMount" },
            { nameof(mounted), "mounted" },
            { nameof(beforeUpdate), "beforeUpdate" },
            { nameof(updated), "updated" },
            { nameof(beforeDestroy), "beforeDestroy" },
            { nameof(destroyed), "destroyed" },
        };

        protected virtual void beforeCreated() { }

        protected virtual void created() { }

        protected virtual void beforeMount() { }

        protected virtual void mounted() { }

        protected virtual void beforeUpdate() { }

        protected virtual void updated() { }

        protected virtual void beforeDestroy() { }

        protected virtual void destroyed() { }
    }
}
