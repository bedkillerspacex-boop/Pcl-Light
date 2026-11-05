using System;
namespace System.Runtime.CompilerServices
{
    [AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = false)]
    internal sealed class RequiredMemberAttribute : Attribute { }

    [AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = false)]
    internal sealed class CompilerFeatureRequiredAttribute(string featureName) : Attribute
    {
        public string FeatureName { get; } = featureName;
        public bool IsOptional { get; init; }
        public const string RefStructs = "RefStructs";
        public const string RequiredMembers = "RequiredMembers";
    }
}

namespace System.Diagnostics.CodeAnalysis
{
    [AttributeUsage(AttributeTargets.Constructor, Inherited = false, AllowMultiple = false)]
    internal sealed class SetsRequiredMembersAttribute : Attribute { }

    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Method | AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    internal sealed class UnscopedRefAttribute : Attribute { }
}
