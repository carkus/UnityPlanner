
using System;
using System.Collections;
using System.Collections.Generic;

using JsonToDataContract;

public interface IParser<T> {

   List<T> ParseNode(JsonNode node);

}