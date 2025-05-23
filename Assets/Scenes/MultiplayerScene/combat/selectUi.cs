using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Net;
using System.Text;
using System.Linq;
using UnityEngine.SceneManagement;

public class selectUi : MonoBehaviour
{
    public int isHost;
    public TMP_Text codeText;
    public TMP_InputField codeInput;
    public Button[] characterButtons;
    public Button launchButton;
    public static string dcodedCode;

    public static int selection = 0;

    const string chars36 = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";

    void Start()
    {
        isHost = MainPlay.IsHost;

        for (int i = 0; i < characterButtons.Length; i++)
        {
            int index = i;
            characterButtons[i].onClick.AddListener(() =>
            {
                selection = index;
                Debug.Log($"[SelectUI] Chosen character: {selection}");
            });
        }

        Debug.Log($"[SelectUI] Am I host? {isHost} {GetLocalIPv4()}{GenerateCode()}{DecodeCode(GenerateCode())}");
        if (isHost == 1)
        {

            codeInput.gameObject.SetActive(false);
            codeText.gameObject.SetActive(true);
            foreach (var b in characterButtons)
                b.gameObject.SetActive(true);

            codeText.text = GenerateCode();

            launchButton.onClick.AddListener(() =>
            {
                Debug.Log($"[SelectUI] Launching host with selection = {selection}");
                SceneManager.LoadScene(5);
                
            });
        }
        else
        {

            codeInput.gameObject.SetActive(true);
            codeText.gameObject.SetActive(false);
            foreach (var b in characterButtons)
                b.gameObject.SetActive(true);

            launchButton.onClick.AddListener(() =>
            {
                dcodedCode = DecodeCode(codeInput.text);
                SceneManager.LoadScene(5);
            });
        }
    }

    string GenerateCode()
    {
        string localIP = GetLocalIPv4();
        var bytes = localIP.Split('.').Select(byte.Parse).ToArray();
        uint val = ((uint)bytes[0] << 24) | ((uint)bytes[1] << 16)
                   | ((uint)bytes[2] << 8) | bytes[3];
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

    public string GetLocalIPv4()
{
return Dns.GetHostEntry(Dns.GetHostName())
.AddressList.First(
f => f.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
.ToString();
}
}
