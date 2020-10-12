# JSONPath expressions

This project allows to create, parse, analyze and modify JSONPath expressions.

## What is JSONPath

JSONPath expressions allow to specify JSON elements to manipulate. For details see [JSONPath - XPath for JSON](https://goessner.net/articles/JsonPath/).

Since JSONPath behaviour is not standardized different implementations work in different ways. See [JSONPath Comparison](https://cburgmer.github.io/json-path-comparison/) for details. For any nuances current project refers to how [Json.NET](https://www.newtonsoft.com/json) works with JSONPath.

## Features

### Create and modify expressions

You can create expressions from scratch or parse existing ones.

```csharp
var expr = new JsonPathExpression("$.a.b.c");
string parent = expr.RemoveLast().ToString(); // returns "$.a.b"
string child = expr.Append(new JsonPathArrayIndexElement(42)).ToString(); // returns "$.a.b.c[42]"
```

### Analyze expressions

```csharp
var parent = new JsonPathExpression("$.a.['b']");
var child = new JsonPathExpression("$.a.b.c.d");
bool startsWith = child.StartsWith(parent); // returns true
string relative = parent.GetRelativePathTo(child).ToString(); // returns "c.d"
```

```csharp
var child1 = new JsonPathExpression("$.a.b.c.d");
var child2 = new JsonPathExpression("$.a.b.c.e");
string parent = child1.GetParentWith(child2).ToString(); // returns "$.a.b.c"
```

### Check expressions for matching

You can check if one JSONPath expression matches another one.

```csharp
var expr1 = new JsonPathExpression("$.a.*.c[:]");
var expr2 = new JsonPathExpression("$.a.b.c[42]");
bool? matches = expr1.Matches(expr2); // returns true
```

## Classes

### JsonPathExpression

Main class to work is `JsonPathExpression` which consists of JSONPath expression elements. Expression and all elements are immutable (though some properties and `ToString()` methods utilize `Lazy<T>`), so any expression modification produces a new instance.

JsonPath expressions are divided into absolute (starting with "$") and relative (not starting with "$"). While `JsonPathExpression` can contain any type of expressions there are two child classes restricting expression type available: `AbsoluteJsonPathExpression` and `RelativeJsonPathExpression`. Though almost all methods for work with JSONPath expressions work with `JsonPathExpression`, some methods accept or produce `RelativeJsonPathExpression` only.

### JsonPathExpressionMatchingSet<TJsonPathExpression>

`JsonPathExpressionMatchingSet<TJsonPathExpression>` allows to find matching expressions for a JSONPath expression. See example:

```csharp
var matchingSet = new JsonPathExpressionMatchingSet<JsonPathExpression>();
matchingSet.Add(new JsonPathExpression("$.a.*.c[*]"));
matchingSet.Add(new JsonPathExpression("$.*.b.c[:]"));
bool matched = matchingSet.Matches(new JsonPathExpression("$.a.b.c[42]"), out var matchedBy); // matchedBy contains all expressions in the matching set because all of them match "$.a.b.c[42]"
```