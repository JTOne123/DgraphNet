# Dgraph Client for .NET

A minimal implementation for a Dgraph client for .NET, using [grpc].

[grpc]: https://grpc.io/

This client follows the [Dgraph Go client][goclient] closely.

[goclient]: https://github.com/dgraph-io/dgraph/tree/master/client

Before using this client, we highly recommend that you go through [docs.dgraph.io],
and understand how to run and work with Dgraph.

[docs.dgraph.io]:https://docs.dgraph.io

## Table of Contents
- [Download](#download)
- [Quickstart](#quickstart)
- [Using the Client](#using-the-client)
  * [Create the client](#create-the-client)
  * [Alter the database](#alter-the-database)
  * [Create a transaction](#create-a-transaction)
  * [Run a mutation](#run-a-mutation)
  * [Run a query](#run-a-query)
  * [Commit a transaction](#commit-a-transaction)
- [Development](#development)
  * [Building the source](#building-the-source)
  * [Code Style](#code-style)
  * [Running unit tests](#running-unit-tests)

## Download
TO DO

## Quickstart
Build and run the [DgraphNet.Client.Sample] project in the `Samples` folder, which
contains an end-to-end example of using the DgraphNet client. Follow the
instructions in the README of that project.

## Using the Client

### Create the client
a `DgraphNetClient` object can be initialised by passing it a list of `DgraphClient`
clients. Connecting to multiple Dgraph servers in the same cluster allows for better
distribution of workload.

The following code snippet shows just one connection.

```csharp
Channel channel = new Channel("localhost:9080", ChannelCredentials.Insecure);
DgraphClient stub = new DgraphClient(_channel);
DgraphNetClient client = new DgraphNetClient(new[] { stub });
```

Alternatively, you can specify a deadline (in seconds) after which the client will time out when making 
requests to the server.

```csharp
DgraphNetClient = new DgraphNetClient(new[] { stub }, 60); // 1 min timeout
```

### Alter the database

To set the schema, create an `Operation` object, set the schema and pass it to
`DgraphClient#Alter` method.

```csharp
string schema = "name: string @index(exact) .";
Operation op = new Operation { Schema = schema };
client.Alter(op);
```

`Operation` contains other fields as well, including drop predicate and
drop all. Drop all is useful if you wish to discard all the data, and start from
a clean slate, without bringing the instance down.

```csharp
// Drop all data including schema from the dgraph instance. This is useful
// for small examples such as this, since it puts dgraph into a clean
// state.
client.Alter(new Operation { DropAll = true });
```

### Create a transaction

To create a transaction, call `DgraphNetClient#NewTransaction()` method, which returns a
new `Transaction` object. This operation incurs no network overhead.

It is a good practise to use the transaction in a `using` block: `Transaction#Dispose` method will call `Transaction#Discard`.
Calling `Transaction#Discard()` after `Transaction#Commit()` is a no-op
and you can call `Discard()` multiple times with no additional side-effects.

```csharp
using(Transaction txn = client.NewTransaction()) 
{
  txn.Commit();
  txn.Discard(); // Commit() was already called, it is a no-op.
}
```

### Run a mutation
`Transaction#Mutate` runs a mutation. It takes in a `Mutation` object,
which provides two main ways to set data: JSON and RDF N-Quad. You can choose
whichever way is convenient.

We're going to use JSON. First we define a `Person` class to represent a person.
This data will be seralized into JSON.

```csharp
class Person {
  public string Name { get; set; }
  public Person() {}
}
```

Next, we initialise a `Person` object, serialize it and use it in `Mutation` object.

```csharp
using(Transaction txn = client.NewTransaction()) 
{
  // Create data
  Person p = new Person();
  p.Name = "Alice";

  // Serialize it with Newtonsoft.Json
  var json = JsonConvert.SerializeObject(p);

  // Run mutation
  Mutation mu = new Mutation { SetJson = ByteString.CopyFromUtf8(json) }
  txn.mutate(mu);

  txn.Commit();
}
```

Sometimes, you only want to commit mutation, without querying anything further.
In such cases, you can use a `CommitNow` field in `Mutation` object to
indicate that the mutation must be immediately committed.

### Run a query
You can run a query by calling `Transaction#Query()`. You will need to pass in a GraphQL+-
query string, and a map (optional, could be empty) of any variables that you might want to
set in the query.

The response would contain a `Json` field, which has the JSON encoded result. You will need
to decode it before you can do anything useful with it.

Let's run the following query:

```
query all($a: string) {
  all(func: eq(name, $a)) {
            name
  }
}
```

First we must create a `People` class that will help us deserialize the JSON result:

```csharp
class People {
  public List<Person> All { get; set; }
  public People() {}
}
```

Then we run the query, deserialize the result and print it out:

```csharp
// Query
string query =
"query all($a: string){\n" +
"  all(func: eq(name, $a)) {\n" +
"    name\n" +
"  }\n" +
"}\n";

IDictionary<string, string> vars = new Dictionary<string, string> 
{ 
  { "$a", "Alice" }
};

Response res = client.NewTransaction().QueryWithVars(query, vars);

// Deserialize
People ppl = JsonConvert.DeserializeObject<People>(res.Json.ToStringUtf8());

// Print results
Console.WriteLine($"people found: {ppl.All.Count}");

foreach(var p in ppl.All) 
{
  Console.WriteLine(p.name);
}

```
This should print:

```
people found: 1
Alice
```

### Commit a transaction
A transaction can be committed using the `Transaction#Commit()` method. If your transaction
consisted solely of calls to `Transaction#Query()`, and no calls to `Transaction#Mutate()`,
then calling `Transaction#Commit()` is not necessary.

An error will be returned if other transactions running concurrently modify the same data that was
modified in this transaction. It is up to the user to retry transactions when they fail.

```csharp
Transaction txn = client.NewTransaction();

try {
  // ...
  // Perform any number of queries and mutations
  // ...
  // and finally...
  txn.Commit()
} catch (TxnConflictException ex) {
   // Retry or handle exception.
} finally {
   // Clean up. Calling this after txn.Commit() is a no-op
   // and hence safe.
   txn.Discard();
}
```

## Development

### Building the source
TO DO

### Code Style
TO DO

### Running unit tests
TO DO
