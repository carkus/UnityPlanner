using UnityEngine;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace JsonToDataContract
{
    public class JsonNode
    {
        public bool IsUserDefinedType { get; private set; }
        public Type ElementType { get; private set; }
        public string UserDefinedTypeName { get; private set; }
        public int ArrayRank { get; private set; }
        public IDictionary<string, JsonNode> Members { get; private set; }

        private JsonNode Parent { get; set; }
        public JsonNode Self { 
            get { 
                return this; 
            }
        }

        private JsonNode(Type elementType, int arrayRank)
        {
            this.Members = new Dictionary<string, JsonNode>();
            this.IsUserDefinedType = false;
            this.ElementType = elementType;
            this.ArrayRank = arrayRank;
        }

        private JsonNode(string userDefinedTypeName, int arrayRank, IDictionary<string, JsonNode> members)
        {
            this.IsUserDefinedType = true;
            this.UserDefinedTypeName = userDefinedTypeName;
            this.ArrayRank = arrayRank;
            this.Members = members;
        }

        public static JsonNode ParseJTokenToNode(JToken root, string rootTypeName)
        {
            if (root == null || root.Type == JTokenType.Null)
            {
                return new JsonNode(null, 0);
            }
            else
            {
                switch (root.Type)
                {
                    case JTokenType.Boolean:
                        return new JsonNode(typeof(bool), 0);
                    case JTokenType.String:
                        return new JsonNode(typeof(string), 0);
                    case JTokenType.Float:
                        return new JsonNode(typeof(double), 0);
                    case JTokenType.Integer:
                        return new JsonNode(GetClrIntegerType(root.ToString()), 0);
                    case JTokenType.Object:
                        return ParseJObject((JObject)root, rootTypeName);
                    case JTokenType.Array:
                        return ParseJArray((JArray)root, rootTypeName);
                    default:
                        throw new ArgumentException("Cannot work with JSON token of type " + root.Type);
                }
            }
        }

        public List<string> GetAncestors()
        {
            List<string> result = new List<string>();
            JsonNode temp = this;
            while (temp != null)
            {
                if (temp.Parent != null)
                {
                    result.Insert(0, temp.UserDefinedTypeName);
                }

                temp = temp.Parent;
            }

            result.Insert(0, "<<root>>");
            return result;
        }

        private static JsonNode ParseJArray(JArray root, string rootTypeName)
        {
            if (root.Count == 0)
            {
                return new JsonNode(null, 1);
            }

            JsonNode first = ParseJTokenToNode(root[0], rootTypeName);
            
            for (int i = 1; i < root.Count; i++)
            {
                JsonNode next = ParseJTokenToNode(root[i], rootTypeName);
                JsonNode mergedType;
                if (first.CanMerge(next, out mergedType))
                {
                    first = mergedType;
                }
                else
                {
                    throw new ArgumentException(string.Format("Cannot merge array elements {0} ({1}) and {2} ({3})",
                        0, root[0], i, root[i]));
                }
            }

            if (first.IsUserDefinedType)
            {
                return new JsonNode(first.UserDefinedTypeName, first.ArrayRank + 1, first.Members);
            }
            else
            {
                return new JsonNode(first.ElementType, first.ArrayRank + 1);
            }
        }

        private static JsonNode ParseJObject(JObject root, string rootTypeName)
        {
            Dictionary<string, JsonNode> fields = new Dictionary<string, JsonNode>();
            foreach (JProperty property in root.Properties())
            {
                JsonNode fieldType = ParseJTokenToNode(property.Value, property.Name);
                fields.Add(property.Name, fieldType);
            }

            JsonNode result = new JsonNode(rootTypeName, 0, fields);

            foreach (var field in fields.Values)
            {
                field.Parent = result;
            }

            return result;
        }

        private bool CanMerge(JsonNode other, out JsonNode mergedType)
        {
            mergedType = null;

            if (this.ArrayRank != other.ArrayRank)
            {
                return false;
            }

            if (this.IsUserDefinedType != other.IsUserDefinedType)
            {
                return false;
            }

            if (this.CanMergeInto(other, out mergedType))
            {
                return true;
            }

            if (other.CanMergeInto(this, out mergedType))
            {
                return true;
            }

            return false;
        }

        private bool CanMergeInto(JsonNode other, out JsonNode mergedType)
        {
            if (this.IsUserDefinedType)
            {
                return this.CanMergeIntoUserDefinedType(other, out mergedType);
            }
            else
            {
                return this.CanMergeIntoPrimitiveType(other, out mergedType);
            }
        }

        private bool CanMergeIntoUserDefinedType(JsonNode other, out JsonNode mergedType)
        {
            bool sameAsThis = true;
            mergedType = null;
            Dictionary<string, JsonNode> members = new Dictionary<string, JsonNode>();
            foreach (var memberName in this.Members.Keys.Union(other.Members.Keys))
            {
                if (this.Members.ContainsKey(memberName) && other.Members.ContainsKey(memberName))
                {
                    JsonNode member1 = this.Members[memberName];
                    JsonNode member2 = other.Members[memberName];
                    JsonNode merged;
                    if (!member1.CanMerge(member2, out merged))
                    {
                        return false;
                    }
                    else
                    {
                        if (merged != member1)
                        {
                            sameAsThis = false;
                        }

                        members.Add(memberName, merged);
                    }
                }
                else if (this.Members.ContainsKey(memberName))
                {
                    members.Add(memberName, this.Members[memberName]);
                }
                else
                {
                    sameAsThis = false;
                    members.Add(memberName, other.Members[memberName]);
                }
            }

            if (sameAsThis)
            {
                mergedType = this;
            }
            else
            {
                mergedType = new JsonNode(this.UserDefinedTypeName, this.ArrayRank, members);
            }

            return true;
        }

        private bool CanMergeIntoPrimitiveType(JsonNode other, out JsonNode mergedType)
        {
            
            if (this.ElementType == other.ElementType)
            {
                mergedType = this;
                return true;
            }


            bool isThisNullable = this.IsNullableType();
            bool isOtherNullable = other.IsNullableType();

            Type thisElementType = this.ElementType != null && this.ElementType.IsGenericType && this.ElementType.GetGenericTypeDefinition() == typeof(Nullable<>) ?
                this.ElementType.GetGenericArguments()[0] : this.ElementType;

            Type otherElementType = other.ElementType != null && other.ElementType.IsGenericType && other.ElementType.GetGenericTypeDefinition() == typeof(Nullable<>) ?
                other.ElementType.GetGenericArguments()[0] : other.ElementType;

            if (thisElementType == otherElementType)
            {
                // one nullable, the other not
                if (isThisNullable)
                {
                    mergedType = this;
                }
                else
                {
                    mergedType = other;
                }

                return true;
            }

            if (thisElementType == null)
            {
                if (isOtherNullable || otherElementType == typeof(string))
                {
                    mergedType = other;
                    return true;
                }

                mergedType = new JsonNode(typeof(Nullable<>).MakeGenericType(otherElementType), this.ArrayRank);
                return true;
            }

            if (otherElementType == null)
            {
                if (isThisNullable || thisElementType == typeof(string))
                {
                    mergedType = this;
                    return true;
                }

                mergedType = new JsonNode(typeof(Nullable<>).MakeGenericType(thisElementType), this.ArrayRank);
                return true;
            }

            // Number coercions
            if (this.ElementType == typeof(int))
            {
                if (other.ElementType == typeof(long) || other.ElementType == typeof(double))
                {
                    mergedType = other;
                    if (!mergedType.IsNullableType() && isThisNullable)
                    {
                        mergedType = new JsonNode(typeof(Nullable<>).MakeGenericType(mergedType.ElementType), mergedType.ArrayRank);
                    }

                    return true;
                }
            }
            else if (this.ElementType == typeof(long))
            {
                if (other.ElementType == typeof(double))
                {
                    mergedType = other;
                    if (!mergedType.IsNullableType() && isThisNullable)
                    {
                        mergedType = new JsonNode(typeof(Nullable<>).MakeGenericType(mergedType.ElementType), mergedType.ArrayRank);
                    }

                    return true;
                }
            }

            mergedType = null;
            return false;
        }

        private static Type GetClrIntegerType(string value)
        {
            int temp;
            if (int.TryParse(value, NumberStyles.None, CultureInfo.InvariantCulture, out temp))
            {
                return typeof(int);
            }

            long temp2;
            if (long.TryParse(value, NumberStyles.None, CultureInfo.InvariantCulture, out temp2))
            {
                return typeof(long);
            }

            // treat it as a double, may lose precision but at least we have a value
            return typeof(double);
        }        

        private bool IsNullableType()
        {
            return !this.IsUserDefinedType &&
                ((this.ElementType == null) ||
                    (this.ElementType.IsGenericType &&
                    this.ElementType.GetGenericTypeDefinition() == typeof(Nullable<>)));
        }
    }
}