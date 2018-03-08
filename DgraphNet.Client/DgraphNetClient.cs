using DgraphNet.Client.Proto;
using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using static DgraphNet.Client.Proto.Dgraph;

namespace DgraphNet.Client
{
    /// <summary>
    /// Implementation of a DgraphClient using grpc.
    /// <para/>Queries, mutations, and most other types of admin tasks can be run from the client.
    /// </summary>
    public class DgraphNetClient
    {
        static readonly object _lock = new object();

        IList<DgraphClient> _clients;
        int? _deadlineSecs;
        LinRead _linRead;

        private LinRead GetLinReadCopy()
        {
            Monitor.Enter(_lock);
            LinRead lr = new LinRead(_linRead);
            Monitor.Exit(_lock);

            return lr;
        }

        /// <summary>
        /// Creates a new Dgraph for interacting with a Dgraph store. 
        /// <para/>A single client is thread safe.
        /// </summary>
        /// <param name="clients">One or more synchronous grpc clients. Can contain connections to multiple servers in a cluster.</param>
        public DgraphNetClient(IList<DgraphClient> clients)
        {
            _clients = clients;
            _linRead = new LinRead();
        }

        /// <summary>
        /// Creates a new Dgraph for interacting with a Dgraph store, with the the specified deadline. 
        /// <para>A single client is thread safe.</para>
        /// </summary>
        /// <param name="clients">One or more synchronous grpc clients. Can contain connections to multiple servers in a cluster.</param>
        /// <param name="deadlineSecs">Deadline specified in secs, after which the client will timeout.</param>
        public DgraphNetClient(IList<DgraphClient> clients, int deadlineSecs)
            : this(clients)
        {
            _deadlineSecs = deadlineSecs;
        }

        /// <summary>
        /// Creates a new <see cref="Transaction"/> object. 
        /// <para/>A transaction lifecycle is as follows: 
        /// <para/>- Created using <see cref="DgraphNetClient.NewTransaction"/>
        /// <para/>- Various <see cref="Transaction.Query(string)"/> and <see cref="Transaction.Mutate(Mutation)"/> calls made.
        /// <para/>- Commit using <see cref="Transaction.Commit"/> or Discard using <see cref="Transaction.Discard"/>. If any    
        /// mutations have been made, It's important that at least one of these methods is called to clean
        /// up resources. Discard is a no-op if Commit has already been called, so it's safe to call it
        /// after Commit.
        /// </summary>
        /// <returns>a new Transaction object</returns>
        public Transaction NewTransaction()
        {
            return new Transaction(this);
        }

        /// <summary>
        /// Alter can be used to perform the following operations, by setting the right fields in the
        /// protocol buffer <see cref="Operation"/> object.
        /// <para/>- Modify a schema.
        /// <para/>- Drop predicate.
        /// <para/>- Drop the database.
        /// </summary>
        /// <param name="op">a protocol buffer Operation object representing the operation being performed.</param>
        public void Alter(Operation op)
        {
            DgraphClient client = AnyClient();
            client.Alter(op);
        }

        /// <summary>
        /// Sets the edges corresponding to predicates on the node with the given uid for deletion. This
        /// function returns a new <see cref="Mutation"/> object with the edges set. It is the caller's responsibility to
        /// run the mutation by calling <see cref="Transaction.Mutate(Mutation)"/>.
        /// </summary>
        /// <param name="mu">Mutation to add edges to</param>
        /// <param name="uid">uid uid of the node</param>
        /// <param name="predicates">predicates predicates of the edges to remove</param>
        /// <returns>a new Mutation object with the edges set</returns>
        public static Mutation DeleteEdges(Mutation mu, String uid, params string[] predicates)
        {
            Mutation b = new Mutation(mu);

            foreach (var predicate in predicates)
            {
                b.Del.Add(new NQuad()
                {
                    Subject = uid,
                    Predicate = predicate,
                    ObjectValue = new Value { DefaultVal = "_STAR_ALL" }
                });
            }
            return b;
        }

        private DgraphClient AnyClient()
        {
            Random rand = new Random();
            DgraphClient client = _clients[rand.Next(0, _clients.Count)];

            return client;
        }

        public static LinRead MergeLinReads(LinRead dst, LinRead src)
        {
            LinRead result = new LinRead(dst);

            foreach (var entry in src.Ids)
            {
                if (dst.Ids.TryGetValue(entry.Key, out ulong dstValue))
                {
                    if (dstValue < entry.Value)
                    {
                        result.Ids[entry.Key] = entry.Value;
                    }
                }
                else
                {
                    result.Ids.Add(entry.Key, entry.Value);
                }
            }
            return result;
        }

        public class Transaction : IDisposable
        {
            DgraphNetClient _client;
            TxnContext _context;
            bool _finished;
            bool _mutated;

            internal Transaction(DgraphNetClient client)
            {
                _client = client;
                _context = new TxnContext
                {
                    LinRead = _client.GetLinReadCopy()
                };
            }

            /// <summary>
            /// Sends a query to one of the connected dgraph instances. If no mutations need to be made in
            /// the same transaction, it's convenient to chain the method: 
            /// <code>client.NewTransaction().QueryWithVars(...)</code>.
            /// </summary>
            /// <param name="query">Query in GraphQL+-</param>
            /// <param name="vars">variables referred to in the QueryWithVars.</param>
            /// <returns>a Response protocol buffer object.</returns>
            public Response QueryWithVars(String query, IDictionary<string, string> vars)
            {
                Request request = new Request()
                {
                    Query = query,
                    StartTs = _context.StartTs,
                    LinRead = _context.LinRead
                };

                request.Vars.Add(vars);

                DgraphClient client = _client.AnyClient();
                Response response = client.Query(request);

                MergeContext(response.Txn);

                return response;
            }

            /// <summary>
            /// Calls <see cref="Transaction.QueryWithVars(string, IDictionary{string, string})"/> with an empty vars map.
            /// </summary>
            /// <param name="query">Query in GraphQL+-</param>
            /// <returns>a Response protocol buffer object</returns>
            public Response Query(String query)
            {
                return QueryWithVars(query, new Dictionary<string, string>());
            }

            /// <summary>
            /// Allows data stored on dgraph instances to be modified. The fields in Mutation come in pairs,
            /// set and delete. Mutations can either be encoded as JSON or as RDFs.
            /// 
            /// <para/>If <see cref="Mutation.CommitNow"/> is set, then this call will result in the transaction being committed. 
            /// In this case, an explicit call to <see cref="Transaction.Commit"/> doesn't need to subsequently be made.
            /// 
            /// </summary>
            /// <param name="mutation">a Mutation protocol buffer object representing the mutation.</param>
            /// <returns>an Assigned protocol buffer object. Its call will result in the transaction being committed. In this case, an explicit call to Transaction#commit doesn't need to subsequently be made.</returns>
            public Assigned Mutate(Mutation mutation)
            {
                if (_finished)
                {
                    throw new TxnFinishedException();
                }

                Mutation request = new Mutation(mutation)
                {
                    StartTs = _context.StartTs
                };

                DgraphClient client = _client.AnyClient();
                Assigned ag;

                try
                {
                    ag = client.Mutate(request);
                    _mutated = true;

                    if (mutation.CommitNow)
                    {
                        _finished = true;
                    }

                    MergeContext(ag.Context);

                    return ag;
                }
                catch (Exception ex)
                {
                    try
                    {
                        // Since a mutation error occurred, the txn should no longer be used
                        // (some mutations could have applied but not others, but we don't know
                        // which ones).  Discarding the transaction enforces that the user
                        // cannot use the txn further.
                        Discard();
                    }
                    finally
                    {
                        CheckAndThrowException(ex);
                    }
                }

                return null;
            }

            /// <summary>
            /// Commits any mutations that have been made in the transaction. Once Commit has been called,
            /// the lifespan of the transaction is complete.
            /// 
            /// <para/>Errors could be thrown for various reasons. Notably, a <see cref="RpcException"/> could be
            /// thrown if transactions that modify the same data are being run concurrently. It's up to the
            /// user to decide if they wish to retry. In this case, the user should create a new transaction.
            /// </summary>
            public void Commit()
            {
                if (_finished)
                {
                    throw new TxnFinishedException();
                }

                _finished = true;

                if (!_mutated)
                {
                    return;
                }

                DgraphClient client = _client.AnyClient();

                try
                {
                    client.CommitOrAbort(_context);
                }
                catch (Exception ex)
                {
                    CheckAndThrowException(ex);
                }
            }

            /// <summary>
            /// Cleans up the resources associated with an uncommitted <see cref="Transaction"/> that contains mutations.
            /// It is a no-op on transactions that have already been committed or don't contain mutations.
            /// 
            /// <para/>In some cases, the transaction can't be discarded, e.g. the grpc connection is
            /// unavailable. In these cases, the server will eventually do the transaction clean up.
            /// </summary>
            public void Discard()
            {
                if (_finished)
                {
                    return;
                }
                _finished = true;

                if (!_mutated)
                {
                    return;
                }

                _context = new TxnContext(_context)
                {
                    Aborted = true
                };

                DgraphClient client = _client.AnyClient();

                client.CommitOrAbort(_context);
            }

            private void MergeContext(TxnContext src)
            {
                TxnContext result = new TxnContext(_context);

                LinRead lr = MergeLinReads(_context.LinRead, src.LinRead);
                result.LinRead = lr;

                Monitor.Enter(_lock);

                lr = MergeLinReads(_client._linRead, lr);
                _client._linRead = lr;

                Monitor.Exit(_lock);

                if (_context.StartTs == 0)
                {
                    result.StartTs = src.StartTs;
                }
                else if (_context.StartTs != src.StartTs)
                {
                    _context = result;
                    throw new DgraphException("StartTs mismatch");
                }

                result.Keys.Add(src.Keys);

                _context = result;
            }

            // Check if Txn has been aborted and throw a TxnConflictException,
            // otherwise throw the original exception.
            private void CheckAndThrowException(Exception ex)
            {
                if (ex is Rpc​Exception rpcEx)
                {
                    StatusCode code = rpcEx.Status.StatusCode;
                    string desc = rpcEx.Status.Detail;

                    if (code == StatusCode.Aborted || code == StatusCode.FailedPrecondition)
                    {
                        throw new TxnConflictException(desc);
                    }
                }

                throw ex;
            }

            public void Dispose()
            {
                Discard();
            }
        }
    }
}
