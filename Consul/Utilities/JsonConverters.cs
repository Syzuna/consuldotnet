﻿// -----------------------------------------------------------------------
//  <copyright file="JsonConverters.cs" company="PlayFab Inc">
//    Copyright 2015 PlayFab Inc.
//
//    Licensed under the Apache License, Version 2.0 (the "License");
//    you may not use this file except in compliance with the License.
//    You may obtain a copy of the License at
//
//        http://www.apache.org/licenses/LICENSE-2.0
//
//    Unless required by applicable law or agreed to in writing, software
//    distributed under the License is distributed on an "AS IS" BASIS,
//    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//    See the License for the specific language governing permissions and
//    limitations under the License.
//  </copyright>
// -----------------------------------------------------------------------

using System;
using System.Reflection;
using System.Linq;
using System.Collections.Generic;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Consul
{
    public class NanoSecTimespanConverter : JsonConverter<TimeSpan?>
    {
        public override TimeSpan? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TryGetUInt64(out var ulongValue))
                return Extensions.FromGoDuration(ulongValue.ToString());
            if (reader.TryGetDouble(out var doubleValue))
                return Extensions.FromGoDuration(doubleValue.ToString(CultureInfo.InvariantCulture));

            return TimeSpan.Zero;
        }

        public override void Write(Utf8JsonWriter writer, TimeSpan? value, JsonSerializerOptions options)
        {
            if (value.HasValue)
                writer.WriteNumberValue((long)value.Value.TotalMilliseconds * 1000000);
        }
    }

    public class DurationTimespanConverter : JsonConverter<TimeSpan?>
    {
        public override TimeSpan? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return Extensions.FromGoDuration(reader.GetString());
        }

        public override void Write(Utf8JsonWriter writer, TimeSpan? value, JsonSerializerOptions options)
        {
            if (value.HasValue)
                writer.WriteStringValue(value.Value.ToGoDuration());
        }
    }

    //public class KVPairConverter : JsonConverter
    //{
    //    static Lazy<string[]> objProps = new Lazy<string[]>(() => typeof(KVPair).GetRuntimeProperties().Select(p => p.Name).ToArray());

    //    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
    //        JsonSerializer serializer)
    //    {
    //        KVPair result = new KVPair();
    //        while (reader.Read())
    //        {
    //            if (reader.TokenType == JsonToken.StartObject) { continue; }
    //            if (reader.TokenType == JsonToken.EndObject) { return result; }
    //            if (reader.TokenType == JsonToken.PropertyName)
    //            {
    //                string jsonPropName = reader.Value.ToString();
    //                var propName = objProps.Value.FirstOrDefault(p => p.Equals(jsonPropName, StringComparison.OrdinalIgnoreCase));
    //                if (propName != null)
    //                {
    //                    PropertyInfo pi = result.GetType().GetRuntimeProperty(propName);

    //                    if (jsonPropName.Equals("Flags", StringComparison.OrdinalIgnoreCase))
    //                    {
    //                        if (!string.IsNullOrEmpty(reader.ReadAsString()))
    //                        {
    //                            var val = Convert.ToUInt64(reader.Value);
    //                            pi.SetValue(result, val, null);
    //                        }
    //                    }
    //                    else if (jsonPropName.Equals("Value", StringComparison.OrdinalIgnoreCase))
    //                    {
    //                        if (!string.IsNullOrEmpty(reader.ReadAsString()))
    //                        {
    //                            var val = Convert.FromBase64String(reader.Value.ToString());
    //                            pi.SetValue(result, val, null);
    //                        }
    //                    }
    //                    else
    //                    {
    //                        if (reader.Read())
    //                        {
    //                            var convertedValue = Convert.ChangeType(reader.Value, pi.PropertyType);
    //                            pi.SetValue(result, convertedValue, null);
    //                        }
    //                    }
    //                }
    //            }
    //        }
    //        return result;
    //    }

    //    public override bool CanConvert(Type objectType)
    //    {
    //        if (objectType == typeof(KVPair))
    //        {
    //            return true;
    //        }
    //        return false;
    //    }

    //    public override bool CanWrite
    //    {
    //        get
    //        {
    //            return false;
    //        }
    //    }
    //}
}