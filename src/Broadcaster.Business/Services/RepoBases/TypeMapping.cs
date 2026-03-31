using System;
using System.Reflection;

namespace Broadcaster.Business.Services.RepoBases;

internal static class TypeMapping
{
    internal static (Assembly[] assembliesScanned, IEnumerable<Type> typesFound) GetExportedTypes(ICollection<Assembly> assemblies)
    {
        IEnumerable<string> rootNamespaces = ["Broadcaster", .. assemblies.Select(a => a.GetName().Name!.Split('.')[0]).Distinct()];
        Assembly[] assembliesScanned = GetReferencedAssemblies(assemblies, rootNamespaces);

        IEnumerable<Type> typesFound = assembliesScanned.SelectMany(a => a.GetTypes()).Where(t => t is { IsNested: false, Namespace: not null }).Distinct();
        return (assembliesScanned, typesFound);
    }

    private static Assembly[] GetReferencedAssemblies(ICollection<Assembly> assemblies, IEnumerable<string> rootNamespaces)
    {
        List<Assembly> newAssemblies = assemblies.SelectMany(a => a.GetReferencedAssemblies().Select(n => { try { return Assembly.Load(n); } catch { return null; } }))
            .Where(a => a != null && rootNamespaces.Contains(a.ManifestModule.Name.Split('.')[0])).ToList()!;

        if (newAssemblies.Count != 0)
            newAssemblies.AddRange(GetReferencedAssemblies(newAssemblies, rootNamespaces));

        return [.. assemblies, .. newAssemblies];
    }
}
