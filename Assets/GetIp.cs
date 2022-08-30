using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class GetIp : MonoBehaviour
{
    public string textValue;
    public Text textElement;

    public static string GetLocalIPAddress()
    {
        var host = System.Net.Dns.GetHostEntry(System.Net.Dns.GetHostName());
        foreach (var ip in host.AddressList)
        {
            if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
            {
                return ip.ToString();
            }
        }

        throw new System.Exception("No network adapters with an IPv4 address in the system!");
    }

    // Start is called before the first frame update
    void Start()
    {
        textValue = GetLocalIPAddress();
        textElement.text = textValue;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
