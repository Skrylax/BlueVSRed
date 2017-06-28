using UnityEngine;
using System.Collections;
using System.Text.RegularExpressions;
using System;
using Newtonsoft.Json;

public class MessageContainer {

    public string messageType;
    public object message;
    public double timeInSeconds;

    public MessageContainer(string messageType, object message) {
        this.messageType = messageType;
        this.message = message;
        this.timeInSeconds = GetCurrentTimeInSeconds();
    }

    double GetCurrentTimeInSeconds() {
        DateTime ms = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        double t = Math.Round( (DateTime.UtcNow - ms).TotalSeconds, 3, MidpointRounding.AwayFromZero);
        return t;
    }

    /// <summary>
    /// Convert all public variables to Json.
    /// </summary>
    /// <returns>A string with the class data in Json format.</returns>
    public string ToJson()
    {
        string json = JsonConvert.SerializeObject(this, Formatting.Indented);
        return Regex.Replace(json, "(\"(?:[^\"\\\\]|\\\\.)*\")|\\s+", "$1");
    }
}

