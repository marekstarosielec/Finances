﻿using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace FinancesApi.Models
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum DatasetState
    {
        [EnumMember(Value = "Closed")]
        Closed,
        [EnumMember(Value = "Opened")]
        Opened,
        [EnumMember(Value = "Opening")]
        Opening,
        [EnumMember(Value = "Closing")]
        Closing,
        [EnumMember(Value = "OpeningError")]
        OpeningError,
        [EnumMember(Value = "ClosingError")]
        ClosingError,
        [EnumMember(Value = "UnknownError")]
        UnknownError
    }
}
