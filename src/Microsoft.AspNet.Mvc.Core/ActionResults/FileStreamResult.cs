// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNet.Http;
using Microsoft.Framework.Internal;

namespace Microsoft.AspNet.Mvc
{
    /// <summary>
    /// Represents an <see cref="ActionResult"/> that when executed will
    /// write a file from a stream to the response.
    /// </summary>
    public class FileStreamResult : FileResult
    {
        // default buffer size as defined in BufferedStream type
        private const int BufferSize = 0x1000;

        private Stream _fileStream;

        /// <summary>
        /// Creates a new <see cref="FileStreamResult"/> instance with
        /// the provided <paramref name="fileStream"/>.
        /// </summary>
        /// <param name="fileStream">The stream with the file.</param>
        public FileStreamResult([NotNull] Stream fileStream)
            : base(contentType: null)
        {
            FileStream = fileStream;
        }

        /// <summary>
        /// Creates a new <see cref="FileStreamResult"/> instance with
        /// the provided <paramref name="fileStream"/> and the
        /// provided <paramref name="contentType"/>.
        /// </summary>
        /// <param name="fileStream">The stream with the file.</param>
        /// <param name="contentType">The Content-Type header of the response.</param>
        public FileStreamResult([NotNull] Stream fileStream, string contentType)
            : base(contentType)
        {
            FileStream = fileStream;
        }

        /// <summary>
        /// Gets or sets the stream with the file that will be sent back as the response.
        /// </summary>
        public Stream FileStream
        {
            get
            {
                return _fileStream;
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value));
                }

                _fileStream = value;
            }
        }

        /// <inheritdoc />
        protected async override Task WriteFileAsync(HttpResponse response, CancellationToken cancellation)
        {
            var outputStream = response.Body;

            using (FileStream)
            {
                await FileStream.CopyToAsync(outputStream, BufferSize, cancellation);
            }
        }
    }
}