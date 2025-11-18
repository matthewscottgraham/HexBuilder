using System;
using System.Collections.Generic;

namespace App.Utils
{
    public static class PredefinedAssemblyUtils
    {
        enum AssemblyType
        {
            AssemblyCSharp,
            AssemblyCSharpEditor,
            AssemblyCSharpEditorFirstPass,
            AssemblyCSharpFirstPass,
            Game,
            App
        }

        static AssemblyType? GetAssemblyType(string assemblyName)
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

        static void AddTypesFromAssembly(Type[] assembly, ICollection<Type> types, Type interfaceType)
        {
            if (assembly == null) return;
            for (var i = 0; i < assembly.Length; i++)
            {
                var type = assembly[i];
                if (type != interfaceType && interfaceType.IsAssignableFrom(type)) {types.Add(type);}
            }
        }
        
        public static List<Type> GetTypes(Type interfaceType)
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            var assemblyTypes = new Dictionary<AssemblyType, Type[]>();
            var types = new List<Type>();
            for (var i = 0; i < assemblies.Length; i++)
            {
                var assemblyType = GetAssemblyType(assemblies[i].GetName().Name);
                if (assemblyType != null) assemblyTypes.Add((AssemblyType)assemblyType, assemblies[i].GetTypes());
            }
            
            AddTypesFromAssembly(assemblyTypes[AssemblyType.Game], types, interfaceType);
            AddTypesFromAssembly(assemblyTypes[AssemblyType.App], types, interfaceType);
            //AddTypesFromAssembly(assemblyTypes[AssemblyType.AssemblyCSharp], types, interfaceType);
            //AddTypesFromAssembly(assemblyTypes[AssemblyType.AssemblyCSharpFirstPass], types, interfaceType);
            
            return types;
        }
    }
}
