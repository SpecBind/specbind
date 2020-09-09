Entering data into a website is simple with SpecBind. The following step is used to define data to be entered:

| Verb | Action |
|------|--------|
| Given | I entered data \<data table\> |
| When, Then | I enter data \<data table\> |

The \<data table\> argument is a table that defines two columns *Field* and *Value*. The *Field* column defines the field name to locate as a [[property|Page Model Properties]] and the *Value* column is the value to enter. It is converted to the correct value by the framework, and will throw an error if the value cannot be converted. The fields are set in the order defined by in the table. 
 
As with other property locators, SpecBind normalizes the name so that you can make the table more readable. For instance if your property name is "FirstName" you can enter the value "First Name" in the table and it will locate the correct property.

#### Example ####

```Cucumber
When I enter data
     | Field    | Value       |
     | My Field | Hello World |
```

##  Special Characters
In some cases you may need to enter data that matches the format of a token `{{TAB}}`. To do so, simply double the curly braces and the content will be treated as entered.
 