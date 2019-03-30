using System;
using System.Collections;
using System.Collections.Generic;

using JsonToDataContract;

namespace Config
{

    public static class Constants
    {

        //JPDSON root key Domain files
        public const string JSON_NODELABEL_TYPES = "types";
        public const string JSON_NODELABEL_PREDICATES = "predicates";
        public const string JSON_NODELABEL_OPERATORS = "operations";

        //JPDSON root key Problem files
        public const string JSON_NODELABEL_OBJECTS = "objects";
        public const string JSON_NODELABEL_STATEINIT = "init";
        public const string JSON_NODELABEL_STATEGOAL = "goal";


    }

}