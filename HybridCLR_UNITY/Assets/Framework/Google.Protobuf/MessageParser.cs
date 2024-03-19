#region Copyright notice and license
// Protocol Buffers - Google's data interchange format
// Copyright 2015 Google Inc.  All rights reserved.
//
// Use of this source code is governed by a BSD-style
// license that can be found in the LICENSE file or at
// https://developers.google.com/open-source/licenses/bsd
#endregion

using System;
using System.Buffers;
using System.IO;
using System.Security;

namespace Google.Protobuf
{
    /// <summary>
    /// A general message parser, typically used by reflection-based code as all the methods
    /// return simple <see cref="IMessage"/>.
    /// </summary>
    public class MessageParser
    {
        private readonly Func<IMessage> factory;
        private protected bool DiscardUnknownFields { get; }

        //internal ExtensionRegistry Extensions { get; }

        internal MessageParser(Func<IMessage> factory, bool discardUnknownFields)//, ExtensionRegistry extensions)
        {
            this.factory = factory;
            DiscardUnknownFields = discardUnknownFields;
            //Extensions = extensions;
        }

        /// <summary>
        /// Creates a template instance ready for population.
        /// </summary>
        /// <returns>An empty message.</returns>
        internal IMessage CreateTemplate()
        {
            return factory();
        }

        /// <summary>
        /// Parses a message from a byte array.
        /// </summary>
        /// <param name="data">The byte array containing the message. Must not be null.</param>
        /// <returns>The newly parsed message.</returns>
        public IMessage ParseFrom(byte[] data)
        {
            IMessage message = factory();
            message.MergeFrom(data, DiscardUnknownFields);
            return message;
        }

        /// <summary>
        /// Parses a message from a byte array slice.
        /// </summary>
        /// <param name="data">The byte array containing the message. Must not be null.</param>
        /// <param name="offset">The offset of the slice to parse.</param>
        /// <param name="length">The length of the slice to parse.</param>
        /// <returns>The newly parsed message.</returns>
        public IMessage ParseFrom(byte[] data, int offset, int length)
        {
            IMessage message = factory();
            message.MergeFrom(data, offset, length, DiscardUnknownFields);
            return message;
        }

        /// <summary>
        /// Parses a message from the given byte string.
        /// </summary>
        /// <param name="data">The data to parse.</param>
        /// <returns>The parsed message.</returns>
        public IMessage ParseFrom(ByteString data)
        {
            IMessage message = factory();
            message.MergeFrom(data, DiscardUnknownFields);
            return message;
        }

        /// <summary>
        /// Parses a message from the given stream.
        /// </summary>
        /// <param name="input">The stream to parse.</param>
        /// <returns>The parsed message.</returns>
        public IMessage ParseFrom(Stream input)
        {
            IMessage message = factory();
            message.MergeFrom(input, DiscardUnknownFields);
            return message;
        }

        /// <summary>
        /// Parses a message from the given sequence.
        /// </summary>
        /// <param name="data">The data to parse.</param>
        /// <returns>The parsed message.</returns>
        [SecuritySafeCritical]
        public IMessage ParseFrom(ReadOnlySequence<byte> data)
        {
            IMessage message = factory();
            message.MergeFrom(data, DiscardUnknownFields);
            return message;
        }

        /// <summary>
        /// Parses a message from the given span.
        /// </summary>
        /// <param name="data">The data to parse.</param>
        /// <returns>The parsed message.</returns>
        [SecuritySafeCritical]
        public IMessage ParseFrom(ReadOnlySpan<byte> data)
        {
            IMessage message = factory();
            message.MergeFrom(data, DiscardUnknownFields);
            return message;
        }

        /// <summary>
        /// Parses a length-delimited message from the given stream.
        /// </summary>
        /// <remarks>
        /// The stream is expected to contain a length and then the data. Only the amount of data
        /// specified by the length will be consumed.
        /// </remarks>
        /// <param name="input">The stream to parse.</param>
        /// <returns>The parsed message.</returns>
        public IMessage ParseDelimitedFrom(Stream input)
        {
            IMessage message = factory();
            message.MergeDelimitedFrom(input, DiscardUnknownFields);
            return message;
        }

        /// <summary>
        /// Parses a message from the given coded input stream.
        /// </summary>
        /// <param name="input">The stream to parse.</param>
        /// <returns>The parsed message.</returns>
        public IMessage ParseFrom(CodedInputStream input)
        {
            IMessage message = factory();
            MergeFrom(message, input);
            return message;
        }

        // TODO: When we're using a C# 7.1 compiler, make this private protected.
        internal void MergeFrom(IMessage message, CodedInputStream codedInput)
        {
            bool originalDiscard = codedInput.DiscardUnknownFields;
            //ExtensionRegistry originalRegistry = codedInput.ExtensionRegistry;
            try
            {
                codedInput.DiscardUnknownFields = DiscardUnknownFields;
                //codedInput.ExtensionRegistry = Extensions;
                message.MergeFrom(codedInput);
            }
            finally
            {
                codedInput.DiscardUnknownFields = originalDiscard;
                //codedInput.ExtensionRegistry = originalRegistry;
            }
        }
    }

    /// <summary>
    /// A parser for a specific message type.
    /// </summary>
    /// <remarks>
    /// <p>
    /// This delegates most behavior to the
    /// <see cref="IMessage.MergeFrom"/> implementation within the original type, but
    /// provides convenient overloads to parse from a variety of sources.
    /// </p>
    /// <p>
    /// Most applications will never need to create their own instances of this type;
    /// instead, use the static <c>Parser</c> property of a generated message type to obtain a
    /// parser for that type.
    /// </p>
    /// </remarks>
    /// <typeparam name="T">The type of message to be parsed.</typeparam>
    public sealed class MessageParser<T> : MessageParser where T : IMessage<T>
    {
        // Implementation note: all the methods here *could* just delegate up to the base class and cast the result.
        // The current implementation avoids a virtual method call and a cast, which *may* be significant in some cases.
        // Benchmarking work is required to measure the significance - but it's only a few lines of code in any case.
        // The API wouldn't change anyway - just the implementation - so this work can be deferred.
        private readonly Func<T> factory;

        /// <summary>
        /// Creates a new parser.
        /// </summary>
        /// <remarks>
        /// The factory method is effectively an optimization over using a generic constraint
        /// to require a parameterless constructor: delegates are significantly faster to execute.
        /// </remarks>
        /// <param name="factory">Function to invoke when a new, empty message is required.</param>
        public MessageParser(Func<T> factory) : this(factory, false)
        {
        }

        internal MessageParser(Func<T> factory, bool discardUnknownFields) : base(() => factory(), discardUnknownFields)
        {
            this.factory = factory;
        }

        /// <summary>
        /// Creates a template instance ready for population.
        /// </summary>
        /// <returns>An empty message.</returns>
        internal new T CreateTemplate()
        {
            return factory();
        }

        /// <summary>
        /// Parses a message from a byte array.
        /// </summary>
        /// <param name="data">The byte array containing the message. Must not be null.</param>
        /// <returns>The newly parsed message.</returns>
        public new T ParseFrom(byte[] data)
        {
            T message = factory();
            message.MergeFrom(data, DiscardUnknownFields);
            return message;
        }

        /// <summary>
        /// Parses a message from a byte array slice.
        /// </summary>
        /// <param name="data">The byte array containing the message. Must not be null.</param>
        /// <param name="offset">The offset of the slice to parse.</param>
        /// <param name="length">The length of the slice to parse.</param>
        /// <returns>The newly parsed message.</returns>
        public new T ParseFrom(byte[] data, int offset, int length)
        {
            T message = factory();
            message.MergeFrom(data, offset, length, DiscardUnknownFields);
            return message;
        }

        /// <summary>
        /// Parses a message from the given byte string.
        /// </summary>
        /// <param name="data">The data to parse.</param>
        /// <returns>The parsed message.</returns>
        public new T ParseFrom(ByteString data)
        {
            T message = factory();
            message.MergeFrom(data, DiscardUnknownFields);
            return message;
        }

        /// <summary>
        /// Parses a message from the given stream.
        /// </summary>
        /// <param name="input">The stream to parse.</param>
        /// <returns>The parsed message.</returns>
        public new T ParseFrom(Stream input)
        {
            T message = factory();
            message.MergeFrom(input, DiscardUnknownFields);
            return message;
        }

        /// <summary>
        /// Parses a message from the given sequence.
        /// </summary>
        /// <param name="data">The data to parse.</param>
        /// <returns>The parsed message.</returns>
        [SecuritySafeCritical]
        internal new T ParseFrom(ReadOnlySequence<byte> data)
        {
            T message = factory();
            message.MergeFrom(data, DiscardUnknownFields);
            return message;
        }

        /// <summary>
        /// Parses a message from the given span.
        /// </summary>
        /// <param name="data">The data to parse.</param>
        /// <returns>The parsed message.</returns>
        [SecuritySafeCritical]
        internal new T ParseFrom(ReadOnlySpan<byte> data)
        {
            T message = factory();
            message.MergeFrom(data, DiscardUnknownFields);
            return message;
        }

        /// <summary>
        /// Parses a length-delimited message from the given stream.
        /// </summary>
        /// <remarks>
        /// The stream is expected to contain a length and then the data. Only the amount of data
        /// specified by the length will be consumed.
        /// </remarks>
        /// <param name="input">The stream to parse.</param>
        /// <returns>The parsed message.</returns>
        public new T ParseDelimitedFrom(Stream input)
        {
            T message = factory();
            message.MergeDelimitedFrom(input, DiscardUnknownFields);
            return message;
        }

        /// <summary>
        /// Parses a message from the given coded input stream.
        /// </summary>
        /// <param name="input">The stream to parse.</param>
        /// <returns>The parsed message.</returns>
        public new T ParseFrom(CodedInputStream input)
        {
            T message = factory();
            MergeFrom(message, input);
            return message;
        }
    }
}
