## Validating Fields ##

The final key part of the process is verifying that what you expect to see on the screen matches what is there. To assist with this the following step exists.

| Verb | Action |
|------|--------|
| Given | I saw \<validation table\> |
| Then| I see \<validation table\> |

The *validation table* is a SpecFlow table that consists of three columns; *Field*, *Rule* and *Value*. Similar to data entry steps the *Field* column defines the field name to locate as a [[property|Page Model Properties]] and the *Value* column is the value to check. The *Rule* column determines the type of validation to perform on the field. The *Value* column is the expected value to check for. It also drives how the expected and actual values are compared. 

The system attempts to convert the expected value to one of the following data types in the order described below. This eases matching of a given value from some of the formatting hassles. For instance if your value is a date, and you write an expectation of "1/1/2013" and your actual value is "January 1, 2013" the rule processor will determine that these values indeed match.

* Date Time (Uses standard .NET date/time formatting according to the current culture)
* Double
* Integer
* Boolean
* String (default)

Note that for numeric types, the presence of a decimal point will trigger the presence as a double. If there is no decimal point, then the number will be treated as an integer.

Using those data types, the following rules come with the framework for analysis:

| Rule | Description |
|------|-------------|
| Equals | The expected value and actual value are an exact match. |
| Does Not Equal | The expected value and actual value do not match. |
| Not Equals | Same as *Does Not Equal* |
| Not Equal  | Same as *Does Not Equal* |
| Contains | The actual value contains the expected value within it. |
| Does Not Contains | The actual value does not contain the expected value within it. |
| Starts With | The actual value starts with the expected value. |
| Ends With | The actual value ends with the expected value. |
| Greater Than | Checks numeric or date types for the actual value to be greater than the expected value. |
| Greater Than Equals | Checks numeric or date types for the actual value to be greater than or equal to the expected value. |
| Less Than | Checks numeric or date types for the actual value to be less than the expected value. |
| Less Than Equals | Checks numeric or date types for the actual value to be less than or equal to the expected value. |
| Exists | The specified field exists and is visible. |
| Does Not Exist | The specified field does not exist. |
| Enabled | The specified field is enabled for editing. |
| Is Enabled | Same as *Enabled* |
| Not Enabled | The field is not enabled for editing. |
| Disabled | Same as *Not Enabled* |
| Is Not Enabled | Same as *Not Enabled* |

**Note: Beginning in 2.1 values such as true or false provided to Enabled or Disabled rules are ignored. This means if you want to check that something is enabled use Enabled, if you want to check disabled use Disabled.**

Also note that Selenium can only check enabled or disabled state on input elements and radio boxes. For more information see this post: https://stackoverflow.com/a/26212504/90287

Data types are converted to match the field type as well as the system attempting to force a rule match on a field if possible. If the rule is incompatible with the field type a validation exception will be thrown for the step. The fields are checked in the order defined by in the table.

### Validating Lists ###

Similar to validating fields, lists or grids of items need to be validated for correct data. A similar step exists to validate a list.

 
| Verb | Action |
|------|--------|
| Given | I saw \<field name\> list \<rule\> \<validation table\> |
| Then| I see \<field name\> list \<rule\> \<validation table\> |

In this case *field name* indicates the [[property|Page Model Properties]] that represents the table and *rule* is the evaluation applied on the list. Not that while the validation table is the same as in validating fields, The field names map to the column or field values in the list. In order for the list rule to be valid, all field evaluations must succeed.

| List Rule | Description |
|-----------|-------------|
| Contains | At least one row in the list matches all the validations. |
| Equals | All of the items in the list match the validations. |
| Exists | At least one row in the list matches all the validations. |
| Does Not Contain | No rows in the list match the validations. |
| Does Not Equal | None of the rows in the list match the validations. |
| Not Exists | No rows in the list match the validations. |
| Starts With | The first row in the list matches the validations. |
| Ends With | The last row in the list matches the validations. |

Custom validation rules can also be defined for the system, for more information see [[Custom Validation Rules]].

### Validating List Counts ###

Sometimes it may be necessary to count the number of items in a list. The following step actions support this:
 
| Verb | Action |
|------|--------|
| Given | I saw \<field name\> list contains \<rule\> \<count\> items |
| Then| I see \<field name\> list contains \<rule\> \<count\> items |

In this case *field name* indicates the [[property|Page Model Properties]] that represents the table and *rule* is the evaluation applied on the list (see below). *count* is the row count value to compare to. The available evaluation rules are:

| List Rule | Description |
|-----------|-------------|
| exactly | The row count matches the *count* value exactly |
| at least| The row count is a value equal to or greater than *count* |
| at most | The row count is a value equal to or less than *count* |
