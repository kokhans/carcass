// MIT License
//
// Copyright (c) 2022-2025 Serhii Kokhan
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

// ReSharper disable UnusedType.Global

namespace Carcass.Core;

/// <summary>
///     Provides a base class for managing both managed and unmanaged resources in a consistent manner.
///     Implements the <see cref="IDisposable" /> interface to support deterministic finalization.
/// </summary>
public abstract class Disposable : IDisposable
{
    /// <summary>
    ///     Indicates whether the object has already been disposed.
    /// </summary>
    /// <remarks>
    ///     This variable is used to prevent multiple attempts to dispose an object,
    ///     ensuring that resources are released only once during the object's lifetime.
    /// </remarks>
    private bool _disposed;

    /// <summary>
    ///     Releases the resources used by the instance. This method is called to cleanup both managed and unmanaged resources.
    /// </summary>
    /// <remarks>
    ///     This implementation ensures that resources are released properly and only once by adhering to the disposal pattern.
    /// </remarks>
    /// <exception cref="ObjectDisposedException">
    ///     Thrown when an operation is attempted on an instance that has already been disposed.
    /// </exception>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    ///     Provides a base class for implementing the <see cref="IDisposable" /> pattern.
    ///     Ensures proper cleanup of resources, including managed and unmanaged resources.
    /// </summary>
    ~Disposable()
    {
        Dispose(false);
    }

    /// <summary>
    ///     Releases resources used by the object.
    /// </summary>
    /// <param name="disposing">
    ///     A boolean value indicating whether to release managed resources (true) or unmanaged resources
    ///     only (false).
    /// </param>
    /// <exception cref="ObjectDisposedException">Thrown if the object has already been disposed.</exception>
    private void Dispose(bool disposing)
    {
        if (_disposed)
            return;

        if (disposing)
            DisposeManagedResources();

        DisposeUnmanagedResources();

        _disposed = true;
    }

    /// <summary>
    ///     Releases managed resources used by the current instance of the class.
    /// </summary>
    /// <remarks>
    ///     Override this method in derived classes to provide custom logic for releasing managed resources.
    ///     This method will be called by the <c>Dispose</c> method when cleanup is necessary.
    /// </remarks>
    /// <exception cref="ObjectDisposedException">
    ///     Thrown if the method is called on an already disposed object in an incorrect implementation.
    /// </exception>
    protected virtual void DisposeManagedResources()
    {
    }

    /// <summary>
    ///     Releases unmanaged resources associated with the object.
    ///     This method is called during the disposal process to clean up
    ///     unmanaged resources that the object might be holding.
    /// </summary>
    /// <exception cref="ObjectDisposedException">
    ///     Thrown if the method is called on an already disposed object.
    /// </exception>
    protected virtual void DisposeUnmanagedResources()
    {
    }
}