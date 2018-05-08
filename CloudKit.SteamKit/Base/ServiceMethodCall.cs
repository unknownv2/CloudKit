﻿using System;
using System.Collections.Generic;
using System.IO;
using ProtoBuf;
using SteamKit2.Internal;


namespace SteamKit2
{
    /// <summary>
    /// Represents a protobuf backed client message. Only contains the header information.
    /// </summary>
    public class ServiceCallProtobuf : ServiceMsgBase<MsgHdrProtoBuf>
    {
        internal ServiceCallProtobuf(int payloadReserve = 0)
            : base(payloadReserve)
        {
        }

        /// <summary>
        /// Gets a value indicating whether this client message is protobuf backed.
        /// Client messages of this type are always protobuf backed.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is protobuf backed; otherwise, <c>false</c>.
        /// </value>
        public override bool IsProto => true;
        /// <summary>
        /// Gets the network message type of this client message.
        /// </summary>
        /// <value>
        /// The network message type.
        /// </value>
        public override EMsg MsgType => Header.Msg;

        /// <summary>
        /// Gets or sets the session id for this client message.
        /// </summary>
        /// <value>
        /// The session id.
        /// </value>
        public override int SessionID
        {
            get => ProtoHeader.client_sessionid;
            set => ProtoHeader.client_sessionid = value;
        }
        /// <summary>
        /// Gets or sets the <see cref="SteamID"/> for this client message.
        /// </summary>
        /// <value>
        /// The <see cref="SteamID"/>.
        /// </value>
        public override SteamID SteamID
        {
            get => ProtoHeader.steamid;
            set => ProtoHeader.steamid = value ?? throw new ArgumentNullException(nameof(value));
        }

        /// <summary>
        /// Gets or sets the target job id for this client message.
        /// </summary>
        /// <value>
        /// The target job id.
        /// </value>
        public override JobID TargetJobID
        {
            get => ProtoHeader.jobid_target;
            set => ProtoHeader.jobid_target = value ?? throw new ArgumentNullException(nameof(value));
        }

        /// <summary>
        /// Gets or sets the source job id for this client message.
        /// </summary>
        /// <value>
        /// The source job id.
        /// </value>
        public override string TargetJobName
        {
            get => ProtoHeader.target_job_name;
            set => ProtoHeader.target_job_name = value ?? throw new ArgumentNullException(nameof(value));
        }
        /// <summary>
        /// Gets or sets the source job id for this client message.
        /// </summary>
        /// <value>
        /// The source job id.
        /// </value>
        public override JobID SourceJobID
        {
            get => ProtoHeader.jobid_source;
            set => ProtoHeader.jobid_source = value ?? throw new ArgumentNullException(nameof(value));
        }


        /// <summary>
        /// Shorthand accessor for the protobuf header.
        /// </summary>
        public CMsgProtoBufHeader ProtoHeader => Header.Proto;


        internal ServiceCallProtobuf(EMsg eMsg, int payloadReserve = 64)
            : base(payloadReserve)
        {
            // set our emsg
            Header.Msg = eMsg;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ClientMsgProtobuf"/> class.
        /// This is a recieve constructor.
        /// </summary>
        /// <param name="msg">The packet message to build this client message from.</param>
        public ServiceCallProtobuf(IPacketMsg msg)
            : this(msg.GetMsgTypeWithNullCheck(nameof(msg)))
        {
            DebugLog.Assert(msg.IsProto, "ServiceCallProtobuf", "ServiceCallProtobuf used for non-proto message!");

            Deserialize(msg.GetData());
        }


        /// <summary>
        /// Serializes this client message instance to a byte array.
        /// </summary>
        /// <exception cref="NotSupportedException">This class is for reading Protobuf messages only. If you want to create a protobuf message, use <see cref="ClientMsgProtobuf&lt;T&gt;"/>.</exception>
        public override byte[] Serialize()
        {
            throw new NotSupportedException("ServiceCallProtobuf is for reading only. Use ServiceCallProtobuf<T> for serializing messages.");
        }

        /// <summary>
        /// Initializes this client message by deserializing the specified data.
        /// </summary>
        /// <param name="data">The data representing a client message.</param>
        public override void Deserialize(byte[] data)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            using (MemoryStream ms = new MemoryStream(data))
            {
                Header.Deserialize(ms);
            }
        }
    }

    /// <summary>
    /// Represents a protobuf backed client message.
    /// </summary>
    /// <typeparam name="BodyType">The body type of this message.</typeparam>
    public sealed class ServiceCallMsgProtobuf<BodyType> : ServiceCallProtobuf
        where BodyType : IExtensible, new()
    {
        /// <summary>
        /// Gets the body structure of this message.
        /// </summary>
        public BodyType Body { get; private set; }


        /// <summary>
        /// Initializes a new instance of the <see cref="ClientMsgProtobuf&lt;BodyType&gt;"/> class.
        /// This is a client send constructor.
        /// </summary>
        /// <param name="eMsg">The network message type this client message represents.</param>
        /// <param name="payloadReserve">The number of bytes to initialize the payload capacity to.</param>
        public ServiceCallMsgProtobuf(EMsg eMsg, int payloadReserve = 64)
            : base(payloadReserve)
        {
            Body = new BodyType();

            // set our emsg
            Header.Msg = eMsg;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ClientMsgProtobuf&lt;BodyType&gt;"/> class.
        /// This a reply constructor.
        /// </summary>
        /// <param name="eMsg">The network message type this client message represents.</param>
        /// <param name="msg">The message that this instance is a reply for.</param>
        /// <param name="payloadReserve">The number of bytes to initialize the payload capacity to.</param>
        public ServiceCallMsgProtobuf(EMsg eMsg, MsgBase<MsgHdrProtoBuf> msg, int payloadReserve = 64)
            : this(eMsg, payloadReserve)
        {
            // our target is where the message came from
            Header.Proto.jobid_target = msg.Header.Proto.jobid_source;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ClientMsgProtobuf&lt;BodyType&gt;"/> class.
        /// This is a recieve constructor.
        /// </summary>
        /// <param name="msg">The packet message to build this client message from.</param>
        public ServiceCallMsgProtobuf(IPacketMsg msg)
            : this(msg.GetMsgTypeWithNullCheck(nameof(msg)))
        {
            DebugLog.Assert(msg.IsProto, "ClientMsgProtobuf<>", $"ClientMsgProtobuf<{typeof(BodyType).FullName}> used for non-proto message!");

            Deserialize(msg.GetData());
        }

        /// <summary>
        /// Serializes this client message instance to a byte array.
        /// </summary>
        /// <returns>
        /// Data representing a client message.
        /// </returns>
        public override byte[] Serialize()
        {
            using (MemoryStream ms = new MemoryStream())
            {
                Header.Serialize(ms);
                Serializer.Serialize(ms, Body);
                Payload.WriteTo(ms);

                return ms.ToArray();
            }
        }
        /// <summary>
        /// Initializes this client message by deserializing the specified data.
        /// </summary>
        /// <param name="data">The data representing a client message.</param>
        public override void Deserialize(byte[] data)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            using (MemoryStream ms = new MemoryStream(data))
            {
                Header.Deserialize(ms);
                Body = Serializer.Deserialize<BodyType>(ms);

                // the rest of the data is the payload
                int payloadOffset = (int)ms.Position;
                int payloadLen = (int)(ms.Length - ms.Position);

                Payload.Write(data, payloadOffset, payloadLen);
                Payload.Seek(0, SeekOrigin.Begin);
            }
        }
    }

    /// <summary>
    /// Represents a struct backed client message.
    /// </summary>
    /// <typeparam name="BodyType">The body type of this message.</typeparam>
    public sealed class ServiceCallMsg<BodyType> : ServiceMsgBase<ExtendedServiceCallMsgHdr>
        where BodyType : ISteamSerializableMessage, new()
    {
        /// <summary>
        /// Gets a value indicating whether this client message is protobuf backed.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is protobuf backed; otherwise, <c>false</c>.
        /// </value>
        public override bool IsProto => false;
        /// <summary>
        /// Gets the network message type of this client message.
        /// </summary>
        /// <value>
        /// The network message type.
        /// </value>
        public override EMsg MsgType => Header.Msg;

        /// <summary>
        /// Gets or sets the session id for this client message.
        /// </summary>
        /// <value>
        /// The session id.
        /// </value>
        public override int SessionID
        {
            get => Header.SessionID;
            set => Header.SessionID = value;
        }
        /// <summary>
        /// Gets or sets the <see cref="SteamID"/> for this client message.
        /// </summary>
        /// <value>
        /// The <see cref="SteamID"/>.
        /// </value>
        public override SteamID SteamID
        {
            get => Header.SteamID;
            set => Header.SteamID = value ?? throw new ArgumentNullException(nameof(value));
        }

        /// <summary>
        /// Gets or sets the target job id for this client message.
        /// </summary>
        /// <value>
        /// The target job id.
        /// </value>
        public override JobID TargetJobID
        {
            get => Header.TargetJobID;
            set => Header.TargetJobID = value ?? throw new ArgumentNullException(nameof(value));
        }
        /// <summary>
        /// Gets or sets the source job id for this client message.
        /// </summary>
        /// <value>
        /// The source job id.
        /// </value>
        public override JobID SourceJobID
        {
            get => Header.SourceJobID;
            set => Header.SourceJobID = value ?? throw new ArgumentNullException(nameof(value));
        }
        /// <summary>
        /// Gets or sets the target job id for this client message.
        /// </summary>
        /// <value>
        /// The target job id.
        /// </value>
        public override string TargetJobName
        {
            get => Header.TargetJobName;
            set => Header.TargetJobName = value ?? throw new ArgumentNullException(nameof(value));
        }

        /// <summary>
        /// Gets the body structure of this message.
        /// </summary>
        public BodyType Body { get; }


        /// <summary>
        /// Initializes a new instance of the <see cref="ClientMsg&lt;BodyType&gt;"/> class.
        /// This is a client send constructor.
        /// </summary>
        /// <param name="payloadReserve">The number of bytes to initialize the payload capacity to.</param>
        public ServiceCallMsg(int payloadReserve = 64)
            : base(payloadReserve)
        {
            Body = new BodyType();

            // assign our emsg
            Header.SetEMsg(Body.GetEMsg());
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ClientMsg&lt;BodyType&gt;"/> class.
        /// This a reply constructor.
        /// </summary>
        /// <param name="msg">The message that this instance is a reply for.</param>
        /// <param name="payloadReserve">The number of bytes to initialize the payload capacity to.</param>
        public ServiceCallMsg(MsgBase<ExtendedServiceCallMsgHdr> msg, int payloadReserve = 64)
            : this(payloadReserve)
        {
            if (msg == null)
            {
                throw new ArgumentNullException(nameof(msg));
            }

            // our target is where the message came from
            Header.TargetJobID = msg.Header.SourceJobID;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ClientMsg&lt;BodyType&gt;"/> class.
        /// This is a recieve constructor.
        /// </summary>
        /// <param name="msg">The packet message to build this client message from.</param>
        public ServiceCallMsg(IPacketMsg msg)
            : this()
        {
            if (msg == null)
            {
                throw new ArgumentNullException(nameof(msg));
            }

            DebugLog.Assert(!msg.IsProto, "ClientMsg", $"ClientMsg<{typeof(BodyType).FullName}> used for proto message!");

            Deserialize(msg.GetData());
        }

        /// <summary>
        /// Serializes this client message instance to a byte array.
        /// </summary>
        /// <returns>
        /// Data representing a client message.
        /// </returns>
        public override byte[] Serialize()
        {
            using (MemoryStream ms = new MemoryStream())
            {
                Header.Serialize(ms);
                Body.Serialize(ms);
                Payload.WriteTo(ms);

                return ms.ToArray();
            }
        }
        /// <summary>
        /// Initializes this client message by deserializing the specified data.
        /// </summary>
        /// <param name="data">The data representing a client message.</param>
        public override void Deserialize(byte[] data)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            using (MemoryStream ms = new MemoryStream(data))
            {
                Header.Deserialize(ms);
                Body.Deserialize(ms);

                // the rest of the data is the payload
                int payloadOffset = (int)ms.Position;
                int payloadLen = (int)(ms.Length - ms.Position);

                Payload.Write(data, payloadOffset, payloadLen);
                Payload.Seek(0, SeekOrigin.Begin);
            }
        }
    }
}