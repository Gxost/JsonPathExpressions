# JSONPath expressions

This project allows to create, parse, analyze and modify JSONPath expressions.

## What is JSONPath

JSONPath expressions allow to specify JSON elements to manipulate. For details see [JSONPath - XPath for JSON](https://goessner.net/articles/JsonPath/).

Since JSONPath behaviour is not standardized different implementations work in different ways. See [JSONPath Comparison](https://cburgmer.github.io/json-path-comparison/) for details. For any nuances current project refers to how [Json.NET](https://www.newtonsoft.com/json) works with JSONPath.

## Features

### Create and modify expressions

You can create JSONPath expressions from scratch or parse existing ones:

```csharp
var expr1 = new JsonPathExpression("$.a.b.c");
var expr2 = new JsonPathExpression(new JsonPathElement[]{
    new JsonPatheRootElement(),
    new JsonPathPropertyElement("a"),
    new JsonPathPropertyElement("b"),
    new JsonPathPropertyElement("c")
    });
var expr3 = JsonPathExpression.Builder.Root().Property("a").Property("b").Property("c").Build();
bool equal1and2 = expr1.Equals(expr2); // returns true
bool equal1and3 = expr1.Equals(expr3); // returns true
```

You can modify existing expressions:

```csharp
var expr = new JsonPathExpression("$.a.b.c");
string parent = expr.RemoveLast().ToString(); // returns "$.a.b"
string child = expr.Append(new JsonPathArrayIndexElement(42)).ToString(); // returns "$.a.b.c[42]"
```

### Analyze expressions

#### Check if path is absolute

```csharp
bool isAbsolute = new JsonPathExpression("$.a.b.c").IsAbsolute; // returns true because expression starts with root object
```

#### Check if path is strict

Strict JSONPath expression points at exactly one JSON element. This check does not count expressions and slice addressing last array element (`[-1:]`).

```csharp
bool isStrict = new JsonPathExpression("$.a[42].b[7:8]").IsStrict; // returns true because expression addresses exactly one element
isStrict = new JsonPathExpression("$.a[*].b[7]").IsStrict; // returns false because expression potentially addresses multiple elements
```

#### Work with parent/child relations

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

The method returns `bool?` because for some expression elements it's not easy or possible to check if they exactly match. In this case `null` means "neither true nor false":

```csharp
var expr1 = new JsonPathExpression("$.a.*.c[-1:]"); // ends with last array element
var expr2 = new JsonPathExpression("$.a.b.c[42]");
bool? matches = expr1.Matches(expr2); // returns null because it's not possible to check if 42 is the last index in the array
```

## Details

### Absolute/relative JSONPath expressions

JSONPath expressions are divided into absolute (starting with "$") and relative (not starting with "$"). To check if JSONPath expression is absolute IsAbsolute property is used:

```csharp
var absolute = new JsonPathExpression("$.a.b.c");
var relative = new JsonPathExpression("a.b.c");
bool isAbsoluteAbsolute = absolute.IsAbsolute; // returns true
bool isRelativeAbsolute = relative.IsAbsolute; // returns false
```

There are also derived classes ensuring expression type:

```csharp
var absolute = new AbsoluteJsonPathExpression("$.a.b.c"); // always starts with "$"
var relative = new RelativeJsonPathExpression("a.b.c"); // always doesn't start with "$"
```

Any JSONPath expression can be converted to absolute:

```csharp
AbsoluteJsonPathExpression expr1 = new JsonPathExpression("$.a.b.c").ToAbsolute();
AbsoluteJsonPathExpression expr2 = new JsonPathExpression("a.b.c").ToAbsolute();
bool equals = expr1.Equals(expr2); // returns true
```

### JSONPath elements

JSONPath expression consists of elements. Most of them are not restricted in usage but there are two special elements:
- `JsonPathRootElement` - first element of an absolute JSONPath expression;
- `JsonPathRecursiveDescentElement` - recursive descent (`..`), containing another non-special element (because `..` is always folowed by another element).

For creation or modification of JSONPath expressions `JsonPathRecursiveDescentElement` is handy because it forces user to use another JSONPath element:

```csharp
var expr = new JsonPathExpression(new JsonPathElement[]{
    new JsonPathRootElement(),
    new JsonPathRecursiveDescentElement(new JsonPathPropertyElement("a")) // because recursive descent must be followed by another element it accepts that element as parameter
    }); // string presentation: "$..a"
```

For JSONPath elements analysis `JsonPathRecursiveDescentElement` may cause problems because it's a special element that should be treated in a special way. To address this issue and simplify checks following `JsonPathElement` extension methods were added:
- check if element has certain type or is a recursive descent applied to element with certain type:
  - `IsOfType(JsonPathElementType type)`;
  - `IsOfType(params JsonPathElementType[] types)`;
  - `IsOfTypeInRange(JsonPathElementType firstType, JsonPathElementType lastType)`;
- `GetUnderlyingElement()` - get element which is not recursive descent (for resursive descent `JsonPathRecursiveDescentElement.AppliedToElement` is returned);
- cast element to certain type (if called on recursive descent `JsonPathRecursiveDescentElement.AppliedToElement` may be cast if `TJsonPathElement` is not `JsonPathRecursiveDescentElement`):
  - `CastTo<TJsonPathElement>()` - throws `InvalidCastException` if failed to cast;
  - `As<TJsonPathElement>()` - returns `null` if failed to cast.

#### Examples

Check expression for allowed elements:

```csharp
var expr = new JsonPathExpression("$.*..a[*]");
bool isAllowedExpression = expr.Elements.All(x => x.IsOfType(
    JsonPathElementType.Root,
    JsonPathElementType.Property,
    JsonPathElementType.AnyProperty,
    JsonPathElementType.AnyArrayIndex
    )); // returns true
```

Check if expression contains elements related to arrays:

```csharp
var expr = new JsonPathExpression("$.*..a[*]");
bool worksWithArrays = expr.Elements.Any(x => x.IsOfTypeInRange(JsonPathElementType.ArrayIndex, JsonPathElementType.FilterExpression)); // returns true
```

Note: JSONPath element types may be divided into ranges:
- `JsonPathElementType.Root` .. `JsonPathElementType.RecursiveDescent` - special elements;
- `JsonPathElementType.Property` .. `JsonPathElementType.PropertyList` - properties;
- `JsonPathElementType.ArrayIndex` .. `JsonPathElementType.ArraySlice` - array indexes and array slice;
- `JsonPathElementType.Expression` .. `JsonPathElementType.FilterExpression` - expressions (related to arrays).

Get property name:

```csharp
var expr = new JsonPathExpression("$.*..a");
string propertyName = expr.LastElement.CastTo<JsonPathPropertyElement>().Name; // returns "a"
```

### Normalization

Some meanings may be expressed using different JSONPath elements. Examples:
- `[:]` and `[*]`;
- `[0:42]` and `[:42]`;
- `[0::2]` and `[::2]`;
- `[7:7]` and `[:0]`.

This can make analysis of expressions harder. To simplify it JSONPath expressions can be brought to normalized form:

```csharp
string expr1 = new JsonPathExpression("$.a[:].b[0:42]").GetNormalized().ToString(); // returns "$.a[*].b[:42]"
```

During normalization array index list element and property list element are broken down to array index element and property element if needed. This may be helpful when an expression is constructed from scratch:

```csharp
var expr1 = new JsonPathExpression(new JsonPathElement[]{
    new JsonPathRootElement(),
    new JsonPathPropertyListElement(new []{ "a" }),
    new JsonPathArrayIndexListElement(new [] { 42 })
    }).GetNormalized();
var expr2 = new JsonPathExpression(new JsonPathElement[]{
    new JsonPathRootElement(),
    new JsonPathPropertyElement("a"),
    new JsonPathArrayIndexElement(42)
    });
bool equals = expr1.Equals(expr2); // returns true
```

### Search for matching expressions

`JsonPathExpressionMatchingSet<TJsonPathExpression>` allows to find matching expressions for a given JSONPath expression. See example:

```csharp
var expr1 = new JsonPathExpression("$.a.*.c[*]");
var expr2 = new JsonPathExpression("$.*.b.c[:]");
var expr3 = new JsonPathExpression("$.*.b.c[1::2]");

var matchingSet = new JsonPathExpressionMatchingSet<JsonPathExpression>();
matchingSet.Add(expr1);
matchingSet.Add(expr2);
matchingSet.Add(expr3);
bool matched = matchingSet.Matches(new JsonPathExpression("$.a.b.c[42]"), out var matchedBy); // matchedBy contains expr1 and expr2 because they match "$.a.b.c[42]"
```