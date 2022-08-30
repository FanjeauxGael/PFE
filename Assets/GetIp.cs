using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Net;
using System.Net.Sockets;

public class GetIp : MonoBehaviour
{
    public string textValue;
    public Text textElement;

    public static string GetLocalIPAddress()
    {
        var host = Dns.GetHostEntry(Dns.GetHostName());
        string externalIpString = null;
        foreach (var ip in host.AddressList)
        {
            if (ip.AddressFamily == AddressFamily.InterNetwork)
            {
                externalIpString = ip.ToString();
            }
        }
        return externalIpString;
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
