using System;
using System.Collections.Generic;

namespace Blade.Entities
{
    public interface IEntityComponent
    {
        public void Initialize(Entity entity);
    }

}
