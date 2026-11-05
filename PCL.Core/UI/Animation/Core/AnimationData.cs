using System;
using System.Collections.Generic;

namespace PCL.Core.UI.Animation.Core;

public record AnimationData<T> : IAnimationData
{
    public IAnimation Animation { get; init; } = null!;
    public IReadOnlyDictionary<int, T> Values { get; init; } = null!;
}
