# JsonNet.GenericTypes
A generic type resolver for Json.Net to easily deserialize interfaces.

# Usage
Just add this somewhere in your code. 
Then when you use JsonConvert, you'll be able to deserialize interfaces. 

```csharp
	JsonConvert.DefaultSettings = () => new JsonSerializerSettings
    {
        Converters = new List<JsonConverter> { new GenericTypeResolver() }
    };
```