using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerStatus : MonoBehaviour
{
    [SerializeField] private float lives;
    private float energy;
    
    private const float ENERGY_DECAY = 0.9f;
    private const float ENERGY_RESTORE = 0.5f;
    private const float ENERGY_MAX = 100;
    private const float ENERGY_MIN = 0.01f;
    PlayerController player;

    // Start is called before the first frame update
    void Start()
    {
        energy = ENERGY_MAX;
        player = this.gameObject.GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        // energy bar status changes
        
        if (player.GetDecaying() && energy >= ENERGY_MIN)
        {
            energy -= ENERGY_DECAY;
        } else if(!player.GetDecaying() && energy <= ENERGY_MAX)
        {
            energy += ENERGY_RESTORE;
        }
    }

    public void SetEnergyToMin()
    {
        energy = ENERGY_MIN;
    }

    public bool IsOutOfEnergy()
    {
        return energy <= ENERGY_MIN;
    }

    public bool IsAtMaxEnergy()
    {
        return energy >= ENERGY_MAX;
    }

    public float GetEnergy()
    {
        return energy;
    }
}
