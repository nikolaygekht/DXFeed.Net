# DXFeed Messages

The package allows to create DXFeed CometD request and parse DXFeed CometD responses into C# objects. 

**Requests:**

```mermaid
classDiagram

class IDXFeedMessage
<<interface>> IDXFeedMessage

class DXFeedMessage

class DXFeedMessageAuthorize

class DXFeedMessageHeartbeat

DXFeedMessage --|> IDXFeedMessage : implements

DXFeedMessage <|-- DXFeedMessageAuthorize
DXFeedMessage <|-- DXFeedMessageHeartbeat
DXFeedMessage <|-- DXFeedMessageSubscribeForQuotes
```

**Responses:**

```mermaid
classDiagram

class IDXFeedResponse
<<interface>> IDXFeedResponse

class DXFeedResponseFactory

class DXFeedResponse

class DXFeedResponseAuthorize

class DXFeedResponseHeartbeat

DXFeedResponse --|> IDXFeedResponse : implements

DXFeedResponseFactory ..> IDXFeedResponse : creates

DXFeedResponseAuthorize --|> DXFeedResponse

DXFeedResponseHeartbeat --|> DXFeedResponse

DXFeedResponseQuote --|> DXFeedResponse

DXFeedResponseCandle --|> DXFeedResponse
```

```mermaid
classDiagram

DXFeedResponseAuthorize *-- "0..1" DXFeedAdvice


DXFeedResponseQuote *-- "1..*" Quote

DXFeedResponseCandle *-- "1..*" Candle

```
