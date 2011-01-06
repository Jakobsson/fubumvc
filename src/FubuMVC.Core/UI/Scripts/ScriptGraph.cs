﻿using System.Collections.Generic;
using System.Linq;
using FubuCore.Util;

namespace FubuMVC.Core.UI.Scripts
{
    public class ScriptGraph : IComparer<IScript>
    {
        private readonly Cache<string, IScriptObject> _objects = new Cache<string, IScriptObject>();
        private readonly Cache<string, ScriptSet> _sets = new Cache<string, ScriptSet>();
        private readonly List<ScriptExtension> _extenders = new List<ScriptExtension>();
        private readonly List<ScriptRule> _rules = new List<ScriptRule>();

        public ScriptGraph()
        {
            _sets.OnMissing = name => new ScriptSet{
                Name = name
            };

            _sets.OnAddition = (@set) =>
            {
                _objects[@set.Name] = @set;
            };


            _objects.OnMissing = name =>
            {
                return 
                    _objects.GetAll().FirstOrDefault(x => x.Matches(name)) 
                    ?? 
                    new Script(name);
            };

        }

        public int Compare(IScript x, IScript y)
        {
            if (x.ShouldBeAfter(y)) return 1;
            if (y.ShouldBeAfter(x)) return -1;

            return x.Name.CompareTo(y.Name);
        }

        public void Alias(string name, string alias)
        {
            _objects[name].AddAlias(alias);
        }

        public void Dependency(string dependent, string dependency)
        {
            _rules.Fill(new ScriptRule()
                        {
                            Dependency = dependency,
                            Dependent = dependent
                        });
        }

        public void Extension(string extender, string @base)
        {
            _extenders.Add(new ScriptExtension(){
                Base = @base,
                Extender = extender
            });
        }

        public void AddToSet(string setName, string name)
        {
            _sets[setName].Add(name);
        }

        public IEnumerable<IScript> GetScripts(IEnumerable<string> names)
        {
            return new ScriptGatherer(this, names).Gather();
        }

        public ScriptSet ScriptSetFor(string someName)
        {
            return _sets[someName];
        }


        // TODO -- try to find circular dependencies
        public void CompileDependencies(IScriptGraphLogger logger)
        {
            _sets.Each(set => set.FindScripts(this));
            _rules.Each(rule =>
            {
                var dependency = ObjectFor(rule.Dependency);
                var dependent = ObjectFor(rule.Dependent);

                dependent.AddDependency(dependency);
            });

            _extenders.Each(x =>
            {
                var @base = ScriptFor(x.Base);
                var extender = ScriptFor(x.Extender);

                @base.AddDependency(extender);
                extender.OrderedAfter(@base);
                @base.OrderedBefore(extender);
            });
        }

        // Find by name or by alias
        public IScriptObject ObjectFor(string name)
        {
            return _objects[name];
        }

        public IScript ScriptFor(string name)
        {
            return (IScript) ObjectFor(name);
        }

    }
}