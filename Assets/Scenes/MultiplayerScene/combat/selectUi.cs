using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using System.Net;
using System.Text;
using System.Linq;

public class CharacterSelectUI : MonoBehaviour
{
    int isHost;
    
    public CharacterSpawnManager spawnMgr;
    public TMP_Text codeText;
    public TMP_InputField codeInput;
    public TMP_Dropdown charDropdown;
    public Button launchButton;

    const string chars36 = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";

    void Start()
    {
        isHost = MainPlay.IsHost;
        if (isHost==1)
        {
            codeInput.gameObject.SetActive(false);
            codeText.gameObject.SetActive(true);
            charDropdown.gameObject.SetActive(true);

            string code = GenerateCode();
            codeText.text = code;

            launchButton.onClick.AddListener(() =>
            {
                byte idx = (byte)charDropdown.value;
                spawnMgr.SetChoiceForLocal(idx);
                var transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
                transport.SetConnectionData("0.0.0.0", 7777);
                NetworkManager.Singleton.StartHost();
            });
        }
        else
        {
            codeInput.gameObject.SetActive(true);
            codeText.gameObject.SetActive(false);
            charDropdown.gameObject.SetActive(false);

            launchButton.onClick.AddListener(() =>
            {
                string code = codeInput.text;
                string ip = DecodeCode(code);
                var transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
                transport.SetConnectionData(ip, 7777);
                NetworkManager.Singleton.StartClient();
            });
        }
    }

    string GenerateCode()
    {
        string localIP = GetLocalIPAddress();
        var bytes = localIP.Split('.').Select(byte.Parse).ToArray();
        uint val = ((uint)bytes[0] << 24) | ((uint)bytes[1] << 16) | ((uint)bytes[2] << 8) | bytes[3];
        return EncodeBase36(val, 8);
    }

    string DecodeCode(string code)
    {
        uint val = DecodeBase36(code);
        byte b0 = (byte)(val >> 24);
        byte b1 = (byte)((val >> 16) & 0xFF);
        byte b2 = (byte)((val >> 8) & 0xFF);
        byte b3 = (byte)(val & 0xFF);
        return $"{b0}.{b1}.{b2}.{b3}";
    }

    string EncodeBase36(uint val, int length)
    {
        var sb = new StringBuilder();
        while (val > 0)
        {
            sb.Append(chars36[(int)(val % 36)]);
            val /= 36;
        }
        while (sb.Length < length) sb.Append('0');
        return new string(sb.ToString().Reverse().ToArray());
    }

    uint DecodeBase36(string code)
    {
        code = code.ToUpperInvariant();
        uint val = 0;
        foreach (char c in code)
            val = val * 36 + (uint)chars36.IndexOf(c);
        return val;
    }

    string GetLocalIPAddress()
    {
        var host = Dns.GetHostEntry(Dns.GetHostName());
        foreach (var ip in host.AddressList)
            if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                return ip.ToString();
        return "127.0.0.1";
    }
}
