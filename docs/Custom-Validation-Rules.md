At times you might need to define a validation rule that SpecBind doesn't support out of the box. Creation of a rule is fairly simple and the SpecBind framework gives you several options:

### Bare Metal: *IValidationComparer* Interface

All validation rules derive from the *IValidationComparer* interface. This interface is made up of the following:

| Field Type | Name | Description |  
|------------|------|-------------|   
| Property | IsDefault | Defines whether this rule should be used if it doesn't match any other rules. This should be false by default.|  
| Property | RuleKeys | The rule values to match from the table that this rule processes. These should be lower case, without spaces, and not contain any filler word like "is", "the" etc. |  
| Method | Compare | Performs the comparison, passing in the property, expected value and actual value. |  

Implementing this interface will make the rule get picked up by the framework. This can be used for complicated scenarios, but is not recommended.

### Basic Implementation: *ValidationComparerBase* Abstract Class

The *ValidationComparerBase* class provides a simple implementation of the *IValidationComparer* interface. It sets *IsDefault* to false and expects a base constructor with any rule keys passed in. The Compare method is abstract so you can implement this to match your rule.

An extremely simple comparison is provided here to demonstrate how to create a rule from this base class:

```C#
namespace Examples
{
    using SpecBind.Pages;

    /// <summary>
    /// A validation comparer to see if the value contains "Foo".
    /// </summary>
    public class FooComparer : ValidationComparerBase
    {
        public FooComparer()
            : base("hasfoo")
        {
        }

        public override bool Compare(IPropertyData property, string expectedValue, string actualValue)
        {
            return actualValue != null && actualValue.Contains("foo");
        }
    }
}
```

The validation table would look like:

```Cucumber
| Field       | Rule    | Value |
| My Property | has foo |       |
```

### Value Comparer: *ValueComparerBase* Abstract Class

The *ValueComparerBase* abstract class is designed to process the defined data types listed above using native value comparisons instead of strings. The advantage here is you can use the power of .NET instead of parsing everything yourself. This class contains a number of virtual methods to parse each data type. To support a given data type simply override the "Compare" method for that data type and insert your own implementation. The example below demonstrates how you could compare absolute values for numeric types.

```C#
namespace Examples
{
    using System;

    /// <summary>
    /// A comparer that checks for numeric absloute value matching.
    /// </summary>
    public class AbsComparer : ValueComparerBase
    {
        public AbsComparer ()
            : base("abs")
        {
        }

        /// <summary>
        /// Compares the double values according to the rule.
        /// </summary>
        /// <param name="expected">The expected value.</param>
        /// <param name="actual">The actual value.</param>
        /// <returns><c>true</c> if the value passes the check, <c>false</c> otherwise.</returns>
        protected override bool Compare(double expected, double actual)
        {
            return Math.Abs(actual) == Math.Abs(expected);
        }

        /// <summary>
        /// Compares the integer values according to the rule.
        /// </summary>
        /// <param name="expected">The expected value.</param>
        /// <param name="actual">The actual value.</param>
        /// <returns><c>true</c> if the value passes the check, <c>false</c> otherwise.</returns>
        protected override bool Compare(int expected, int actual)
        {
            return Math.Abs(actual) == Math.Abs(expected);
        }
    }
}
```