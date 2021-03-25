using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Assets.Scripts
{
    public class JsonIgnoreResolver : DefaultContractResolver
    {
        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            JsonProperty prop = base.CreateProperty(member, memberSerialization);
            if (member.GetCustomAttribute<JsonIgnoreAttribute>() != null)
                prop.Ignored = true;

            return prop;
        }
    }
}
