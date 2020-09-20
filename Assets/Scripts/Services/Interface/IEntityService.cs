﻿using Assets.Scripts.Characters;
using Assets.Scripts.Services.Events;
using System.Collections.Generic;

namespace Assets.Scripts.Services.Interface
{
    public interface IEntityService : IService
    {
        event OnUnRegister<Entity> OnUnRegister;

        void Register(Entity entity);
        void UnRegister(Entity entity);
        int Count();
        int Count<T>() where T : Entity;
        HashSet<Entity> GetAll();
        HashSet<T> GetAll<T>() where T : Entity;
        HashSet<Entity> GetAllExcept(Entity entity);
    }
}
