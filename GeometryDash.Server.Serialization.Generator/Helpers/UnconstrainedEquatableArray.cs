//https://github.com/CommunityToolkit/dotnet/blob/7b53ae23dfc6a7fb12d0fc058b89b6e948f48448/src/CommunityToolkit.Mvvm.SourceGenerators/Helpers/EquatableArray%7BT%7D.cs

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

//namespace CommunityToolkit.Mvvm.SourceGenerators.Helpers;
namespace GeometryDash.Server.Serialization.Generator.Helpers;

/// <summary>
/// Extensions for <see cref="UnconstrainedEquatableArray{T}"/>.
/// </summary>
public static class UnconstrainedEquatableArray
{
    /// <summary>
    /// Creates an <see cref="UnconstrainedEquatableArray{T}"/> instance from a given <see cref="ImmutableArray{T}"/>.
    /// </summary>
    /// <typeparam name="T">The type of items in the input array.</typeparam>
    /// <param name="array">The input <see cref="ImmutableArray{T}"/> instance.</param>
    /// <returns>An <see cref="UnconstrainedEquatableArray{T}"/> instance from a given <see cref="ImmutableArray{T}"/>.</returns>
    public static UnconstrainedEquatableArray<T> AsUnconstrainedEquatableArray<T>(this ImmutableArray<T> array)
    {
        return new(array);
    }

    public static UnconstrainedEquatableArray<T> ToUnconstrainedEquatableArray<T>(this IEnumerable<T> e)
        => e.ToImmutableArray().AsUnconstrainedEquatableArray();
}

/// <summary>
/// An immutable, equatable array. This is equivalent to <see cref="ImmutableArray{T}"/> but with value equality support.
/// </summary>
/// <typeparam name="T">The type of values in the array.</typeparam>
public readonly struct UnconstrainedEquatableArray<T> : IEquatable<UnconstrainedEquatableArray<T>>, IEnumerable<T>
{
    /// <summary>
    /// The underlying <typeparamref name="T"/> array.
    /// </summary>
    private readonly T[]? array;

    /// <summary>
    /// Creates a new <see cref="UnconstrainedEquatableArray{T}"/> instance.
    /// </summary>
    /// <param name="array">The input <see cref="ImmutableArray{T}"/> to wrap.</param>
    public UnconstrainedEquatableArray(ImmutableArray<T> array)
    {
        this.array = Unsafe.As<ImmutableArray<T>, T[]?>(ref array);
    }

    /// <summary>
    /// Gets a reference to an item at a specified position within the array.
    /// </summary>
    /// <param name="index">The index of the item to retrieve a reference to.</param>
    /// <returns>A reference to an item at a specified position within the array.</returns>
    public ref readonly T this[int index]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => ref AsImmutableArray().ItemRef(index);
    }

    /// <summary>
    /// Gets a value indicating whether the current array is empty.
    /// </summary>
    public bool IsEmpty
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => AsImmutableArray().IsEmpty;
    }

    /// <inheritdoc/>
    public bool Equals(UnconstrainedEquatableArray<T> array)
    {
        return //AsSpan().SequenceEqual(array.AsSpan());
            this.array.SequenceEqual(array.array);
    }

    /// <inheritdoc/>
    public override bool Equals([NotNullWhen(true)] object? obj)
    {
        return obj is UnconstrainedEquatableArray<T> array && Equals(this, array);
    }

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        if (this.array is not T[] array)
        {
            return 0;
        }

        HashCode hashCode = default;

        foreach (T item in array)
        {
            hashCode.Add(item);
        }

        return hashCode.ToHashCode();
    }

    /// <summary>
    /// Gets an <see cref="ImmutableArray{T}"/> instance from the current <see cref="UnconstrainedEquatableArray{T}"/>.
    /// </summary>
    /// <returns>The <see cref="ImmutableArray{T}"/> from the current <see cref="UnconstrainedEquatableArray{T}"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ImmutableArray<T> AsImmutableArray()
    {
        return Unsafe.As<T[]?, ImmutableArray<T>>(ref Unsafe.AsRef(in this.array));
    }

    /// <summary>
    /// Creates an <see cref="UnconstrainedEquatableArray{T}"/> instance from a given <see cref="ImmutableArray{T}"/>.
    /// </summary>
    /// <param name="array">The input <see cref="ImmutableArray{T}"/> instance.</param>
    /// <returns>An <see cref="UnconstrainedEquatableArray{T}"/> instance from a given <see cref="ImmutableArray{T}"/>.</returns>
    public static UnconstrainedEquatableArray<T> FromImmutableArray(ImmutableArray<T> array)
    {
        return new(array);
    }

    /// <summary>
    /// Returns a <see cref="ReadOnlySpan{T}"/> wrapping the current items.
    /// </summary>
    /// <returns>A <see cref="ReadOnlySpan{T}"/> wrapping the current items.</returns>
    public ReadOnlySpan<T> AsSpan()
    {
        return AsImmutableArray().AsSpan();
    }

    /// <summary>
    /// Copies the contents of this <see cref="UnconstrainedEquatableArray{T}"/> instance to a mutable array.
    /// </summary>
    /// <returns>The newly instantiated array.</returns>
    public T[] ToArray()
    {
        return AsImmutableArray().ToArray();
    }

    /// <summary>
    /// Gets an <see cref="ImmutableArray{T}.Enumerator"/> value to traverse items in the current array.
    /// </summary>
    /// <returns>An <see cref="ImmutableArray{T}.Enumerator"/> value to traverse items in the current array.</returns>
    public ImmutableArray<T>.Enumerator GetEnumerator()
    {
        return AsImmutableArray().GetEnumerator();
    }

    /// <inheritdoc/>
    IEnumerator<T> IEnumerable<T>.GetEnumerator()
    {
        return ((IEnumerable<T>)AsImmutableArray()).GetEnumerator();
    }

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable)AsImmutableArray()).GetEnumerator();
    }

    /// <summary>
    /// Implicitly converts an <see cref="ImmutableArray{T}"/> to <see cref="UnconstrainedEquatableArray{T}"/>.
    /// </summary>
    /// <returns>An <see cref="UnconstrainedEquatableArray{T}"/> instance from a given <see cref="ImmutableArray{T}"/>.</returns>
    public static implicit operator UnconstrainedEquatableArray<T>(ImmutableArray<T> array)
    {
        return FromImmutableArray(array);
    }

    /// <summary>
    /// Implicitly converts an <see cref="UnconstrainedEquatableArray{T}"/> to <see cref="ImmutableArray{T}"/>.
    /// </summary>
    /// <returns>An <see cref="ImmutableArray{T}"/> instance from a given <see cref="UnconstrainedEquatableArray{T}"/>.</returns>
    public static implicit operator ImmutableArray<T>(UnconstrainedEquatableArray<T> array)
    {
        return array.AsImmutableArray();
    }

    /// <summary>
    /// Checks whether two <see cref="UnconstrainedEquatableArray{T}"/> values are the same.
    /// </summary>
    /// <param name="left">The first <see cref="UnconstrainedEquatableArray{T}"/> value.</param>
    /// <param name="right">The second <see cref="UnconstrainedEquatableArray{T}"/> value.</param>
    /// <returns>Whether <paramref name="left"/> and <paramref name="right"/> are equal.</returns>
    public static bool operator ==(UnconstrainedEquatableArray<T> left, UnconstrainedEquatableArray<T> right)
    {
        return left.Equals(right);
    }

    /// <summary>
    /// Checks whether two <see cref="UnconstrainedEquatableArray{T}"/> values are not the same.
    /// </summary>
    /// <param name="left">The first <see cref="UnconstrainedEquatableArray{T}"/> value.</param>
    /// <param name="right">The second <see cref="UnconstrainedEquatableArray{T}"/> value.</param>
    /// <returns>Whether <paramref name="left"/> and <paramref name="right"/> are not equal.</returns>
    public static bool operator !=(UnconstrainedEquatableArray<T> left, UnconstrainedEquatableArray<T> right)
    {
        return !left.Equals(right);
    }
}
