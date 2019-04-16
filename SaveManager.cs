using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    public Enemy[] enemies;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            save();
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            Load();
        }
    }

    public void save()
    {
        JObject jSaveGame = new JObject();

        for (int i = 0; i < enemies.Length; i++)
        {
            Enemy CurrentEnemy = enemies[i];
            JObject SerializedEnemy = CurrentEnemy.Serialize();
            jSaveGame.Add(CurrentEnemy.name, SerializedEnemy);
        }

        string filePath = Application.persistentDataPath + "/EnemiesData.sav";
        Debug.Log("Saving Enemies Data in " + filePath);
        byte[] encryptedSaveData = Encrypt(jSaveGame.ToString());
        File.WriteAllBytes(filePath,encryptedSaveData);
    }

    public void Load()
    {
        string filePath = Application.persistentDataPath + "/EnemiesData.sav";
        Debug.Log("Loading Enemies Data From " + filePath);

        byte[] decrypyedSaveData = File.ReadAllBytes(filePath);
        string jsonString = Decrypt(decrypyedSaveData);
        
        JObject jSaveGame = JObject.Parse(jsonString);

        for (int i = 0; i < enemies.Length; i++)
        {
            Enemy CurrentEnemy = enemies[i];
            string enemyjsonString = jSaveGame[CurrentEnemy.name].ToString();
            CurrentEnemy.Deserialize(enemyjsonString);
        }
    }

    byte[] _Key = { 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x010, 0x011, 0x012, 0x013, 0x014, 0x015, 0x016 };
    byte[] _InitializationVector = { 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x010, 0x011, 0x012, 0x013, 0x014, 0x015, 0x016 };

    byte[] Encrypt(string message)
    {
        AesManaged aes = new AesManaged();
        //Encrypt Strings
        ICryptoTransform encryptor =  aes.CreateEncryptor(_Key,_InitializationVector);

        MemoryStream memoryStream = new MemoryStream();
        CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write);
        StreamWriter streamWriter = new StreamWriter(cryptoStream);

        streamWriter.WriteLine(message);

        streamWriter.Close();
        cryptoStream.Close();
        memoryStream.Close();

        return memoryStream.ToArray();
    }

    string Decrypt(byte[] message)
    {
        AesManaged aes = new AesManaged();

        ICryptoTransform decryptor = aes.CreateDecryptor(_Key, _InitializationVector);

        MemoryStream memoryStream = new MemoryStream(message);
        CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);
        StreamReader streamReader = new StreamReader(cryptoStream);

        string decryptedMessage = streamReader.ReadToEnd();

        streamReader.Close();
        cryptoStream.Close();
        memoryStream.Close();

        return decryptedMessage;
    }
}
