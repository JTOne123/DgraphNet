﻿// <auto-generated>
//     Generated by the protocol buffer compiler.  DO NOT EDIT!
//     source: api.proto
// </auto-generated>
// Original file comments:
//
// Copyright (C) 2017 Dgraph Labs, Inc. and Contributors
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//    http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
#pragma warning disable 1591
#region Designer generated code

using System;
using System.Threading;
using System.Threading.Tasks;
using grpc = global::Grpc.Core;

namespace DgraphNet.Client.Proto {
  /// <summary>
  /// Graph response.
  /// </summary>
  internal static partial class Dgraph
  {
    static readonly string __ServiceName = "api.Dgraph";

    static readonly grpc::Marshaller<global::DgraphNet.Client.Proto.Request> __Marshaller_Request = grpc::Marshallers.Create((arg) => global::Google.Protobuf.MessageExtensions.ToByteArray(arg), global::DgraphNet.Client.Proto.Request.Parser.ParseFrom);
    static readonly grpc::Marshaller<global::DgraphNet.Client.Proto.Response> __Marshaller_Response = grpc::Marshallers.Create((arg) => global::Google.Protobuf.MessageExtensions.ToByteArray(arg), global::DgraphNet.Client.Proto.Response.Parser.ParseFrom);
    static readonly grpc::Marshaller<global::DgraphNet.Client.Proto.Mutation> __Marshaller_Mutation = grpc::Marshallers.Create((arg) => global::Google.Protobuf.MessageExtensions.ToByteArray(arg), global::DgraphNet.Client.Proto.Mutation.Parser.ParseFrom);
    static readonly grpc::Marshaller<global::DgraphNet.Client.Proto.Assigned> __Marshaller_Assigned = grpc::Marshallers.Create((arg) => global::Google.Protobuf.MessageExtensions.ToByteArray(arg), global::DgraphNet.Client.Proto.Assigned.Parser.ParseFrom);
    static readonly grpc::Marshaller<global::DgraphNet.Client.Proto.Operation> __Marshaller_Operation = grpc::Marshallers.Create((arg) => global::Google.Protobuf.MessageExtensions.ToByteArray(arg), global::DgraphNet.Client.Proto.Operation.Parser.ParseFrom);
    static readonly grpc::Marshaller<global::DgraphNet.Client.Proto.Payload> __Marshaller_Payload = grpc::Marshallers.Create((arg) => global::Google.Protobuf.MessageExtensions.ToByteArray(arg), global::DgraphNet.Client.Proto.Payload.Parser.ParseFrom);
    static readonly grpc::Marshaller<global::DgraphNet.Client.Proto.TxnContext> __Marshaller_TxnContext = grpc::Marshallers.Create((arg) => global::Google.Protobuf.MessageExtensions.ToByteArray(arg), global::DgraphNet.Client.Proto.TxnContext.Parser.ParseFrom);
    static readonly grpc::Marshaller<global::DgraphNet.Client.Proto.Check> __Marshaller_Check = grpc::Marshallers.Create((arg) => global::Google.Protobuf.MessageExtensions.ToByteArray(arg), global::DgraphNet.Client.Proto.Check.Parser.ParseFrom);
    static readonly grpc::Marshaller<global::DgraphNet.Client.Proto.Version> __Marshaller_Version = grpc::Marshallers.Create((arg) => global::Google.Protobuf.MessageExtensions.ToByteArray(arg), global::DgraphNet.Client.Proto.Version.Parser.ParseFrom);

    static readonly grpc::Method<global::DgraphNet.Client.Proto.Request, global::DgraphNet.Client.Proto.Response> __Method_Query = new grpc::Method<global::DgraphNet.Client.Proto.Request, global::DgraphNet.Client.Proto.Response>(
        grpc::MethodType.Unary,
        __ServiceName,
        "Query",
        __Marshaller_Request,
        __Marshaller_Response);

    static readonly grpc::Method<global::DgraphNet.Client.Proto.Mutation, global::DgraphNet.Client.Proto.Assigned> __Method_Mutate = new grpc::Method<global::DgraphNet.Client.Proto.Mutation, global::DgraphNet.Client.Proto.Assigned>(
        grpc::MethodType.Unary,
        __ServiceName,
        "Mutate",
        __Marshaller_Mutation,
        __Marshaller_Assigned);

    static readonly grpc::Method<global::DgraphNet.Client.Proto.Operation, global::DgraphNet.Client.Proto.Payload> __Method_Alter = new grpc::Method<global::DgraphNet.Client.Proto.Operation, global::DgraphNet.Client.Proto.Payload>(
        grpc::MethodType.Unary,
        __ServiceName,
        "Alter",
        __Marshaller_Operation,
        __Marshaller_Payload);

    static readonly grpc::Method<global::DgraphNet.Client.Proto.TxnContext, global::DgraphNet.Client.Proto.TxnContext> __Method_CommitOrAbort = new grpc::Method<global::DgraphNet.Client.Proto.TxnContext, global::DgraphNet.Client.Proto.TxnContext>(
        grpc::MethodType.Unary,
        __ServiceName,
        "CommitOrAbort",
        __Marshaller_TxnContext,
        __Marshaller_TxnContext);

    static readonly grpc::Method<global::DgraphNet.Client.Proto.Check, global::DgraphNet.Client.Proto.Version> __Method_CheckVersion = new grpc::Method<global::DgraphNet.Client.Proto.Check, global::DgraphNet.Client.Proto.Version>(
        grpc::MethodType.Unary,
        __ServiceName,
        "CheckVersion",
        __Marshaller_Check,
        __Marshaller_Version);

    /// <summary>Service descriptor</summary>
    public static global::Google.Protobuf.Reflection.ServiceDescriptor Descriptor
    {
      get { return global::DgraphNet.Client.Proto.ApiReflection.Descriptor.Services[0]; }
    }

    /// <summary>Base class for server-side implementations of Dgraph</summary>
    public abstract partial class DgraphBase
    {
      public virtual global::System.Threading.Tasks.Task<global::DgraphNet.Client.Proto.Response> Query(global::DgraphNet.Client.Proto.Request request, grpc::ServerCallContext context)
      {
        throw new grpc::RpcException(new grpc::Status(grpc::StatusCode.Unimplemented, ""));
      }

      public virtual global::System.Threading.Tasks.Task<global::DgraphNet.Client.Proto.Assigned> Mutate(global::DgraphNet.Client.Proto.Mutation request, grpc::ServerCallContext context)
      {
        throw new grpc::RpcException(new grpc::Status(grpc::StatusCode.Unimplemented, ""));
      }

      public virtual global::System.Threading.Tasks.Task<global::DgraphNet.Client.Proto.Payload> Alter(global::DgraphNet.Client.Proto.Operation request, grpc::ServerCallContext context)
      {
        throw new grpc::RpcException(new grpc::Status(grpc::StatusCode.Unimplemented, ""));
      }

      public virtual global::System.Threading.Tasks.Task<global::DgraphNet.Client.Proto.TxnContext> CommitOrAbort(global::DgraphNet.Client.Proto.TxnContext request, grpc::ServerCallContext context)
      {
        throw new grpc::RpcException(new grpc::Status(grpc::StatusCode.Unimplemented, ""));
      }

      public virtual global::System.Threading.Tasks.Task<global::DgraphNet.Client.Proto.Version> CheckVersion(global::DgraphNet.Client.Proto.Check request, grpc::ServerCallContext context)
      {
        throw new grpc::RpcException(new grpc::Status(grpc::StatusCode.Unimplemented, ""));
      }

    }

    /// <summary>Client for Dgraph</summary>
    public partial class DgraphClient : grpc::ClientBase<DgraphClient>
    {
      /// <summary>Creates a new client for Dgraph</summary>
      /// <param name="channel">The channel to use to make remote calls.</param>
      public DgraphClient(grpc::Channel channel) : base(channel)
      {
      }
      /// <summary>Creates a new client for Dgraph that uses a custom <c>CallInvoker</c>.</summary>
      /// <param name="callInvoker">The callInvoker to use to make remote calls.</param>
      public DgraphClient(grpc::CallInvoker callInvoker) : base(callInvoker)
      {
      }
      /// <summary>Protected parameterless constructor to allow creation of test doubles.</summary>
      protected DgraphClient() : base()
      {
      }
      /// <summary>Protected constructor to allow creation of configured clients.</summary>
      /// <param name="configuration">The client configuration.</param>
      protected DgraphClient(ClientBaseConfiguration configuration) : base(configuration)
      {
      }

      public virtual global::DgraphNet.Client.Proto.Response Query(global::DgraphNet.Client.Proto.Request request, grpc::Metadata headers = null, DateTime? deadline = null, CancellationToken cancellationToken = default(CancellationToken))
      {
        return Query(request, new grpc::CallOptions(headers, deadline, cancellationToken));
      }
      public virtual global::DgraphNet.Client.Proto.Response Query(global::DgraphNet.Client.Proto.Request request, grpc::CallOptions options)
      {
        return CallInvoker.BlockingUnaryCall(__Method_Query, null, options, request);
      }
      public virtual grpc::AsyncUnaryCall<global::DgraphNet.Client.Proto.Response> QueryAsync(global::DgraphNet.Client.Proto.Request request, grpc::Metadata headers = null, DateTime? deadline = null, CancellationToken cancellationToken = default(CancellationToken))
      {
        return QueryAsync(request, new grpc::CallOptions(headers, deadline, cancellationToken));
      }
      public virtual grpc::AsyncUnaryCall<global::DgraphNet.Client.Proto.Response> QueryAsync(global::DgraphNet.Client.Proto.Request request, grpc::CallOptions options)
      {
        return CallInvoker.AsyncUnaryCall(__Method_Query, null, options, request);
      }
      public virtual global::DgraphNet.Client.Proto.Assigned Mutate(global::DgraphNet.Client.Proto.Mutation request, grpc::Metadata headers = null, DateTime? deadline = null, CancellationToken cancellationToken = default(CancellationToken))
      {
        return Mutate(request, new grpc::CallOptions(headers, deadline, cancellationToken));
      }
      public virtual global::DgraphNet.Client.Proto.Assigned Mutate(global::DgraphNet.Client.Proto.Mutation request, grpc::CallOptions options)
      {
        return CallInvoker.BlockingUnaryCall(__Method_Mutate, null, options, request);
      }
      public virtual grpc::AsyncUnaryCall<global::DgraphNet.Client.Proto.Assigned> MutateAsync(global::DgraphNet.Client.Proto.Mutation request, grpc::Metadata headers = null, DateTime? deadline = null, CancellationToken cancellationToken = default(CancellationToken))
      {
        return MutateAsync(request, new grpc::CallOptions(headers, deadline, cancellationToken));
      }
      public virtual grpc::AsyncUnaryCall<global::DgraphNet.Client.Proto.Assigned> MutateAsync(global::DgraphNet.Client.Proto.Mutation request, grpc::CallOptions options)
      {
        return CallInvoker.AsyncUnaryCall(__Method_Mutate, null, options, request);
      }
      public virtual global::DgraphNet.Client.Proto.Payload Alter(global::DgraphNet.Client.Proto.Operation request, grpc::Metadata headers = null, DateTime? deadline = null, CancellationToken cancellationToken = default(CancellationToken))
      {
        return Alter(request, new grpc::CallOptions(headers, deadline, cancellationToken));
      }
      public virtual global::DgraphNet.Client.Proto.Payload Alter(global::DgraphNet.Client.Proto.Operation request, grpc::CallOptions options)
      {
        return CallInvoker.BlockingUnaryCall(__Method_Alter, null, options, request);
      }
      public virtual grpc::AsyncUnaryCall<global::DgraphNet.Client.Proto.Payload> AlterAsync(global::DgraphNet.Client.Proto.Operation request, grpc::Metadata headers = null, DateTime? deadline = null, CancellationToken cancellationToken = default(CancellationToken))
      {
        return AlterAsync(request, new grpc::CallOptions(headers, deadline, cancellationToken));
      }
      public virtual grpc::AsyncUnaryCall<global::DgraphNet.Client.Proto.Payload> AlterAsync(global::DgraphNet.Client.Proto.Operation request, grpc::CallOptions options)
      {
        return CallInvoker.AsyncUnaryCall(__Method_Alter, null, options, request);
      }
      public virtual global::DgraphNet.Client.Proto.TxnContext CommitOrAbort(global::DgraphNet.Client.Proto.TxnContext request, grpc::Metadata headers = null, DateTime? deadline = null, CancellationToken cancellationToken = default(CancellationToken))
      {
        return CommitOrAbort(request, new grpc::CallOptions(headers, deadline, cancellationToken));
      }
      public virtual global::DgraphNet.Client.Proto.TxnContext CommitOrAbort(global::DgraphNet.Client.Proto.TxnContext request, grpc::CallOptions options)
      {
        return CallInvoker.BlockingUnaryCall(__Method_CommitOrAbort, null, options, request);
      }
      public virtual grpc::AsyncUnaryCall<global::DgraphNet.Client.Proto.TxnContext> CommitOrAbortAsync(global::DgraphNet.Client.Proto.TxnContext request, grpc::Metadata headers = null, DateTime? deadline = null, CancellationToken cancellationToken = default(CancellationToken))
      {
        return CommitOrAbortAsync(request, new grpc::CallOptions(headers, deadline, cancellationToken));
      }
      public virtual grpc::AsyncUnaryCall<global::DgraphNet.Client.Proto.TxnContext> CommitOrAbortAsync(global::DgraphNet.Client.Proto.TxnContext request, grpc::CallOptions options)
      {
        return CallInvoker.AsyncUnaryCall(__Method_CommitOrAbort, null, options, request);
      }
      public virtual global::DgraphNet.Client.Proto.Version CheckVersion(global::DgraphNet.Client.Proto.Check request, grpc::Metadata headers = null, DateTime? deadline = null, CancellationToken cancellationToken = default(CancellationToken))
      {
        return CheckVersion(request, new grpc::CallOptions(headers, deadline, cancellationToken));
      }
      public virtual global::DgraphNet.Client.Proto.Version CheckVersion(global::DgraphNet.Client.Proto.Check request, grpc::CallOptions options)
      {
        return CallInvoker.BlockingUnaryCall(__Method_CheckVersion, null, options, request);
      }
      public virtual grpc::AsyncUnaryCall<global::DgraphNet.Client.Proto.Version> CheckVersionAsync(global::DgraphNet.Client.Proto.Check request, grpc::Metadata headers = null, DateTime? deadline = null, CancellationToken cancellationToken = default(CancellationToken))
      {
        return CheckVersionAsync(request, new grpc::CallOptions(headers, deadline, cancellationToken));
      }
      public virtual grpc::AsyncUnaryCall<global::DgraphNet.Client.Proto.Version> CheckVersionAsync(global::DgraphNet.Client.Proto.Check request, grpc::CallOptions options)
      {
        return CallInvoker.AsyncUnaryCall(__Method_CheckVersion, null, options, request);
      }
      /// <summary>Creates a new instance of client from given <c>ClientBaseConfiguration</c>.</summary>
      protected override DgraphClient NewInstance(ClientBaseConfiguration configuration)
      {
        return new DgraphClient(configuration);
      }
    }

    /// <summary>Creates service definition that can be registered with a server</summary>
    /// <param name="serviceImpl">An object implementing the server-side handling logic.</param>
    public static grpc::ServerServiceDefinition BindService(DgraphBase serviceImpl)
    {
      return grpc::ServerServiceDefinition.CreateBuilder()
          .AddMethod(__Method_Query, serviceImpl.Query)
          .AddMethod(__Method_Mutate, serviceImpl.Mutate)
          .AddMethod(__Method_Alter, serviceImpl.Alter)
          .AddMethod(__Method_CommitOrAbort, serviceImpl.CommitOrAbort)
          .AddMethod(__Method_CheckVersion, serviceImpl.CheckVersion).Build();
    }

  }
}
#endregion