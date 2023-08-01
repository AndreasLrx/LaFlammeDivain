using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 
public class PlayerSave : MonoBehaviour
{
    private float currentSpeed = 10;
    private float currentRange = 5;
    private float currentAttackSpeed = 1;
    private float currentShotSpeed = 1;
    private float currentDamage = 3;
    private float currentHealth = 20;
    

    public void Save()
    {
        string destination = Application.persistentDataPath + "/playerSave.dat";
		FileStream file;

		if(File.Exists(destination)) 
            file = File.OpenWrite(destination);
		else 
            file = File.Create(destination);

        currentAttackSpeed = PlayerController.Instance.entity.attackSpeed;
        currentDamage = PlayerController.Instance.entity.damage;
        currentHealth = PlayerController.Instance.entity.health;
        currentRange = PlayerController.Instance.entity.range;
        currentShotSpeed = PlayerController.Instance.entity.shotSpeed;
        currentSpeed = PlayerController.Instance.entity.speed;


        PlayerData data = new PlayerData(currentSpeed, currentRange, currentAttackSpeed, currentShotSpeed, currentDamage, currentHealth);

        BinaryFormatter bf = new BinaryFormatter();
        bf.Serialize(file, data);
        file.Close();
    }

    public void Load()
    {
        string destination = Application.persistentDataPath + "/playerSave.dat";
        FileStream file;

        if (File.Exists(destination)) file = File.OpenRead(destination);
        else
        {
            Debug.LogError("File not found");
            return;
        }

        BinaryFormatter bf = new BinaryFormatter();
        PlayerData data = (PlayerData)bf.Deserialize(file);
        file.Close();

        PlayerController.Instance.entity.speed = data.saveSpeed;
        PlayerController.Instance.entity.range = data.saveRange;
        PlayerController.Instance.entity.attackSpeed = data.saveAttackSpeed;
        PlayerController.Instance.entity.shotSpeed = data.saveShotSpeed;
        PlayerController.Instance.entity.damage = data.saveDamage;
        PlayerController.Instance.entity.health = data.saveHealth;
        Debug.Log("Game data loaded!");
    }
}

[System.Serializable]
class PlayerData
{
    public float saveSpeed;
    public float saveRange;
    public float saveAttackSpeed;
    public float saveShotSpeed;
    public float saveDamage;
    public float saveHealth;

    public PlayerData(float currentSpeed, float currentRange, float currentAttackSpeed, float currentShotSpeed, float currentDamage, float currentHealth)
    {
        this.saveSpeed = currentSpeed;
        this.saveRange = currentRange;
        this.saveAttackSpeed = currentAttackSpeed;
        this.saveShotSpeed = currentShotSpeed;
        this.saveDamage = currentDamage;
        this.saveHealth = currentHealth;
    }
}

