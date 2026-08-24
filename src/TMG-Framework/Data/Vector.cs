/*
    Copyright 2017-2019 University of Toronto

    This file is part of TMG-Framework for XTMF2.

    TMG-Framework for XTMF2 is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    TMG-Framework for XTMF2 is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with TMG-Framework for XTMF2.  If not, see <http://www.gnu.org/licenses/>.
*/

using System.Buffers;
using static TMG.Utilities.ExceptionHelper;

namespace TMG;

/// <summary>
/// Represents a single dimension floating point data storage with a given shape.
/// </summary>
public sealed class Vector : IDisposable
{
    /// <summary>
    /// The categories that shape this vector.
    /// </summary>
    public Categories Categories { get; }

    /// <summary>
    /// The backing data for this vector.
    /// </summary>
    public Span<float> Data => _backingMemory is null ? ThrowAlreadyDisposed() : _backingMemory.Value.Span;

    private Span<float> ThrowAlreadyDisposed()
    {
        throw new ObjectDisposedException(nameof(Vector));
    }

    public void Dispose()
    {
        _backingMemory = default;
        Thread.MemoryBarrier();
        _allocator?.Dispose();
        _allocator = null;
    }

    ~Vector()
    {
        Dispose();
    }

    private Memory<float>? _backingMemory;

    private IMemoryOwner<float>? _allocator;

    /// <summary>
    /// Create a new vector given the shape of the categories.
    /// </summary>
    /// <param name="categories">The categories to shape the vector around.</param>
    public Vector(Categories categories) : this(categories, null) { }


    /// <summary>
    /// Create a new vector given the shape of the categories and an optional memory allocator.
    /// </summary>
    /// <param name="categories">The categories to shape the vector around.</param>
    /// <param name="allocator">The memory pool to use for the vector data.</param>
    public Vector(Categories categories, MemoryPool<float>? allocator)
    {
        if (categories == null)
        {
            ThrowParameterNull(nameof(categories));
        }
        Categories = categories;
        _allocator = allocator?.Rent(Categories.Count);
        _backingMemory = _allocator?.Memory ?? new float[Categories.Count];
    }

    /// <summary>
    /// Create a new vector given the shape of the given vector.
    /// This will not create a clone of the given vector.
    /// </summary>
    /// <param name="vector">The vector to use to create the shape from.</param>
    public Vector(Vector vector) : this(vector, null) { }

    /// <summary>
    /// Create a new vector given the shape of the given vector and an optional memory allocator.
    /// </summary>
    /// <param name="vector">The vector to use to create the shape from.</param>
    /// <param name="allocator">The memory pool to use for the vector data.</param>
    public Vector(Vector vector, MemoryPool<float>? allocator)
    {
        if (vector == null)
        {
            ThrowParameterNull(nameof(vector));
        }
        Categories = vector.Categories;
        _allocator = allocator?.Rent(Categories.Count);
        _backingMemory = _allocator?.Memory ?? new float[Categories.Count];
    }

    public float this[CategoryIndex sparseIndex]
    {
        get
        {
            var index = Categories.GetFlatIndex(sparseIndex);
            return index >= 0 ? Data[index] : 0.0f;
        }
        set
        {
            var index = Categories.GetFlatIndex(sparseIndex);
            if (index >= 0)
            {
                Data[index] = value;
            }
            ThrowOutOfRangeException(nameof(sparseIndex));
        }
    }

    /// <summary>
    /// The number of records contained in this vector.
    /// </summary>
    /// <returns>The number of elements.</returns>
    public int Count => _backingMemory?.Length ?? 0;
}

