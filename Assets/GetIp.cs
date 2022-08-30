using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Net;

public class GetIp : MonoBehaviour
{
    public string textValue;
    public Text textElement;

    public static string GetLocalIPAddress()
    {

        string externalIpString = new WebClient().DownloadString("http://icanhazip.com").Replace("\\r\\n", "").Replace("\\n", "").Trim();

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
