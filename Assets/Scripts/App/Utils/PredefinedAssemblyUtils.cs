using System;
using System.Collections.Generic;

namespace App.Utils
{
    public static class PredefinedAssemblyUtils
    {
        private enum AssemblyType
        {
            AssemblyCSharp,
            AssemblyCSharpEditor,
            AssemblyCSharpEditorFirstPass,
            AssemblyCSharpFirstPass,
            Game,
            App
        }

        private static AssemblyType? GetAssemblyType(string assemblyName)
        {
            return assemblyName switch
            {
                "AssemblyCSharp" => AssemblyType.AssemblyCSharp,
                "AssemblyCSharpEditor" => AssemblyType.AssemblyCSharpEditor,
                "AssemblyCSharpEditorFirstPass" => AssemblyType.AssemblyCSharpEditorFirstPass,
                "AssemblyCSharpFirstPass" => AssemblyType.AssemblyCSharpFirstPass,
                "Game" => AssemblyType.Game,
                "App" => AssemblyType.App,
                _ => null
            };
        }

        private static void AddTypesFromAssembly(Type[] assembly, ICollection<Type> types, Type interfaceType)
        {
            if (assembly == null) return;
            foreach (var type in assembly)
            {
                if (type != interfaceType && interfaceType.IsAssignableFrom(type))
                {
                    types.Add(type);
                }
            }
        }
        
        public static List<Type> GetTypes(Type interfaceType)
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            var assemblyTypes = new Dictionary<AssemblyType, Type[]>();
            var types = new List<Type>();
            foreach (var t in assemblies)
            {
                var assemblyType = GetAssemblyType(t.GetName().Name);
                if (assemblyType != null) assemblyTypes.Add((AssemblyType)assemblyType, t.GetTypes());
            }
            
            AddTypesFromAssembly(assemblyTypes[AssemblyType.Game], types, interfaceType);
            AddTypesFromAssembly(assemblyTypes[AssemblyType.App], types, interfaceType);
            //AddTypesFromAssembly(assemblyTypes[AssemblyType.AssemblyCSharp], types, interfaceType);
            //AddTypesFromAssembly(assemblyTypes[AssemblyType.AssemblyCSharpFirstPass], types, interfaceType);
            
            return types;
        }
    }
}
