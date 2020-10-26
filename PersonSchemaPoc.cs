using System;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks.Dataflow;

namespace jsondsl
{
    public enum TypeEnum
    {
        String,
        Integer,
        Object,
        Array
    }

    public abstract class Property
    {
        public Property(TypeEnum type, string description)
        {
            this.Type = type.ToString().ToLower();
            this.Description = description;
        }

        public string Description { get; set; }

        public string Type { get; }
    }

    public class StringProperty : Property
    {
        public StringProperty(string description) : base(TypeEnum.String, description)
        {
        }

        public string? DefaultValue { get; set; } = null;
    }

    public class IntegerProperty : Property
    {
        public IntegerProperty(string description) : base(TypeEnum.Integer, description)
        {
        }

        public int? Maximum { get; set; } = null;
        public int? Minimum { get; set; } = null;
        public int? DefaultValue { get; set; } = null;
    }

    public class PersonProperties
    {
        public object FirstName => new StringProperty("Enter the first name");
        public object LastName => new StringProperty("Enter the last name")
        {
            DefaultValue = "No name :=("
        };

        public object Age => new IntegerProperty("Minimum age is 12")
        {
            Minimum = 12,
            Maximum = 99
        };

        public object Hairs => new IntegerProperty("How many hairs the person has");
    }

    public abstract class Schema
    {
        public Schema(string title, TypeEnum type)
        {
            Title = title;
            Type = type.ToString().ToLower();
        }

        [JsonPropertyName("$title")] 
        public string Title { get; }

        public string Type { get; }
    }

    public class PersonSchema: Schema
    {
        public virtual object Properties => new PersonProperties();

        public PersonSchema(string title="person") : base(title, TypeEnum.Object)
        {
        }
    }

    public class Person2Properties : PersonProperties
    {
        public object PhoneNumber => new StringProperty("Phone Number");
    }

    public class Person2Schema : PersonSchema
    {
        public Person2Schema(string title = "person2") : base(title)
        {
        }

        public override object Properties => new Person2Properties();
    }

    static class Program
    {
        public static void Main(string[] args)
        {
            var schema = new Person2Schema();
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                IgnoreNullValues = true,
            };
            var jsonString = JsonSerializer.Serialize(schema, schema.GetType(), options);
            Console.WriteLine(jsonString);
        }
    }
}