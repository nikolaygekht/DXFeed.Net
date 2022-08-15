# CometD Message package

The message package defines structure of tree-form name-value dictionaries to keep content of CometD messages. 

The structure is quite self-explaining.

```mermaid
classDiagram

class IMessageElement
<<interface>> IMessageElement

class IMessageElementObject
<<interface>> IMessageElementObject

class IMessageElementArray
<<interface>> IMessageElementArray

class IMessageElementNull
<<interface>> IMessageElementNull

class IMessageElementInteger
<<interface>> IMessageElementInteger

class IMessageElementLong
<<interface>> IMessageElementLong

class IMessageElementDouble
<<interface>> IMessageElementDouble

class IMessageElementBoolean
<<interface>> IMessageElementBoolean

class IMessageElementString
<<interface>> IMessageElementString

IMessageElementObject --|> IMessageElement
IMessageElementArray --|> IMessageElement
IMessageElementNull --|> IMessageElement
IMessageElementInteger --|> IMessageElement
IMessageElementLong --|> IMessageElement
IMessageElementDouble --|> IMessageElement
IMessageElementBoolean --|> IMessageElement
IMessageElementString --|> IMessageElement

IMessageElementArray *-- IMessageElement : contains(index -> value)

IMessageElementObject *-- IMessageElement : contains(name -> value)
```

Each interface has an implementation. 

There is also an extension `MessageElementExtension` that simplifies access to primitive values (e.g. integer, string) and performs 
data type conversion if needed.