using System;
using System.Collections.Generic;
using FubuMVC.Core.Registration.ObjectGraph;

namespace FubuMVC.Core.Registration
{
    public interface IServiceRegistry
    {
        void SetServiceIfNone<TService, TImplementation>() where TImplementation : TService;
        void SetServiceIfNone<TService>(TService value);
        ObjectDef SetServiceIfNone(Type interfaceType, Type concreteType);

        ObjectDef AddService<TService, TImplementation>() where TImplementation : TService;
        ObjectDef AddService<TService>(Type implementation);
        void ReplaceService<TService, TImplementation>() where TImplementation : TService;
        void ReplaceService<TService>(TService value);
        void AddService<TService>(TService value);
        ObjectDef DefaultServiceFor<TService>();
        void Each(Action<Type, ObjectDef> action);
        IEnumerable<T> FindAllValues<T>();

        void ClearAll<T>();
        IEnumerable<ObjectDef> ServicesFor<TService>();
        void FillType(Type interfaceType, Type concreteType);
        void AddService(Type type, ObjectDef objectDef);
    }
}