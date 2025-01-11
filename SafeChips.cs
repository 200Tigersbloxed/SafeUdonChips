using System;
using System.Diagnostics.CodeAnalysis;
using UCS;
using UdonSharp;
using UnityEngine;
using VRC.SDK3.Persistence;
using VRC.SDKBase;

public class SafeChips : UdonSharpBehaviour
{
    private const string VERSION = "v1.0.2";
    
    [NotNull]
    [Tooltip("(Required) The UdonChips object to associate with this instance")]
    public UdonChips Chips;
    [Tooltip("When to run the checks for when money changes")]
    public CheckType CheckTime = CheckType.Update;
    [Tooltip("(When CheckTime is set to Interval) The amount of time in seconds to check for money changes")]
    public float IntervalRate = 3f;
    [Tooltip("How much of a difference there must be from the previous check to the current check to save money")]
    public float DifferenceTolerance = 0.001f;
    [Tooltip("(Advanced) What the key for saving money with persistence should be")]
    public string MoneyKey = "Money";
    [Tooltip("(Advanced) What the key for checking initialize status with persistence should be")]
    public string HasInitializedKey = "HasInit";
    
    private bool dontRun = true;
    private bool firstLoad;
    private bool sentWaitEvent;
    private float lastChipValue;
    private float startingMoney;

    /// <summary>
    /// The Event Message to Invoke when CheckTime is set to Scripted
    /// </summary>
    public void _RefreshChipData()
    {
        if(CheckTime != CheckType.Scripted)
        {
#if DEBUG
            Debug.LogWarning("Tried to RefreshChipData when not Scripting!");
#endif
            return;
        }
        Check();
    }

    /// <summary>
    /// The Event Message to Invoke when you want to Reset Saved Data
    /// </summary>
    public void _ResetData()
    {
        PlayerData.SetFloat(MoneyKey, startingMoney);
        Chips.money = startingMoney;
        lastChipValue = startingMoney;
#if DEBUG
        Debug.LogWarning("Reset player data!");
#endif
    }
    
    public void _IntervalWaitEvent()
    {
        // Don't run or CheckTime is not Interval or we haven't requested a sentWaitEvent
        if(dontRun || CheckTime != CheckType.Interval || !sentWaitEvent)
        {
            // Sanity Check
            if(dontRun) return;
            // If CheckTime is not Interval, but we are waiting for an event, no we are not
            if(CheckTime != CheckType.Interval && sentWaitEvent) sentWaitEvent = false;
            return;
        }
        Check();
        sentWaitEvent = false;
    }

    private void Check()
    {
        // If SafeChips couldn't load, don't do anything
        if(dontRun) return;
        // Values are not the same, don't do anything
        if(Math.Abs(lastChipValue - Chips.money) < DifferenceTolerance) return;
        // Values changed, so it's time for an update
        PlayerData.SetFloat(MoneyKey, Chips.money);
        lastChipValue = Chips.money;
#if DEBUG
        Debug.Log("Updated Money to " + lastChipValue);
#endif
    }

    private void CheckIntervalStatus()
    {
        // Don't run or CheckTime is not Interval or we already sent the wait event
        if(dontRun || CheckTime != CheckType.Interval || sentWaitEvent) return;
        SendCustomEventDelayedSeconds(nameof(_IntervalWaitEvent), IntervalRate);
        sentWaitEvent = true;
    }

    public override void OnPlayerRestored(VRCPlayerApi plr)
    {
        if(firstLoad || !plr.isLocal) return;
#if DEBUG
        Debug.Log("Loaded PlayerData!");
#endif
        if (Chips == null)
        {
            Debug.LogError("Cannot load SafeChips! No Chips object was provided.");
            return;
        }
        // Initialize Money
        startingMoney = Chips.money;
        // Initialize Data Store
        float loadedMoney = startingMoney;
        bool hasInit = PlayerData.GetBool(Networking.LocalPlayer, HasInitializedKey);
        if(hasInit)
        {
            float currentMoney = PlayerData.GetFloat(Networking.LocalPlayer, MoneyKey);
            loadedMoney = currentMoney;
            Chips.money = currentMoney;
        }
        else
        {
            PlayerData.SetFloat(MoneyKey, startingMoney);
            Chips.money = startingMoney;
            PlayerData.SetBool(HasInitializedKey, true);
#if DEBUG
            Debug.Log("Created PlayerData!");
#endif
        }
        lastChipValue = loadedMoney;
        dontRun = false;
        firstLoad = true;
#if DEBUG
        Debug.Log($"Loaded SafeChips ({VERSION}) with {loadedMoney} UdonChips!");
#endif
    }

    private void FixedUpdate()
    {
        if(CheckTime != CheckType.FixedUpdate) return;
        Check();
    }
    
    private void Update()
    {
        CheckIntervalStatus();
        if(CheckTime != CheckType.Update) return;
        Check();
    }
    
    private void LateUpdate()
    {
        if(CheckTime != CheckType.LateUpdate) return;
        Check();
    }
}

public enum CheckType
{
    FixedUpdate,
    Update,
    LateUpdate,
    Interval,
    Scripted
}