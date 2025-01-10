# SafeUdonChips

A feature rich plug-in for [UdonChips](https://lura.booth.pm/items/3060394) which adds configurable persistence.

![kdjfhgb](https://github.com/user-attachments/assets/a13f3ccb-5606-42d8-8575-0529345a4647)

## Setup

1. [Download](https://github.com/200Tigersbloxed/SafeUdonChips/releases/latest/download/SafeUdonChips.unitypackage) the latest release
2. Import the unitypackage into your project
3. Drag the Prefab into the Scene with the UdonChips GameObject
4. Reference the Chips object
5. *(Optional)* Configure the Behaviour

## Configuration

Below are the possible configurations and their definitions.

### Chips

> [!CAUTION]
> 
> This field is **REQUIRED** to be set. SafeChips will not run without this field.

The GameObject which contains the UdonChips UdonBehaviour.

### CheckTime

The event frame to continuously check for changes to the Money value on UdonChips.

+ [FixedUpdate](https://docs.unity3d.com/2022.3/Documentation/ScriptReference/MonoBehaviour.FixedUpdate.html)
+ [Update](https://docs.unity3d.com/2022.3/Documentation/ScriptReference/MonoBehaviour.Update.html) *(default)*
+ [LateUpdate](https://docs.unity3d.com/2022.3/Documentation/ScriptReference/MonoBehaviour.LateUpdate.html)
+ Interval
  + Checks for changes in an interval measured in seconds
+ Scripted
  + Only invokes when messaged `_RefreshChipData`

> [!IMPORTANT]
> 
> When **NOT** using `Scripted` CheckTime, you **CANNOT** invoke `_RefreshChipData` to update money.

### IntervalRate

> [!NOTE]
> 
> This configuration is only valid when CheckTime is set to Interval

The rate of when to check again for changes to the Money value on UdonChips. Measured in seconds.

### DifferenceTolerance

The tolerance amount for how different the difference of money has to be before updating again. Recommended to keep at `0.001f`, only change if your money is expected to go past 3 decimal places.

> [!WARNING]
> 
> The Advanced configurations are not meant to be messed with. Please only change these if you have a valid reason to.

### MoneyKey (Advanced)

The key to use when saving money to PlayerData.

### HasInitializedKey (Advanced)

The key to use when saving initialization checks to PlayerData.

## Multiple Chips?

In theory, this project will work fine with multiple UdonChips Behaviours as long as you can get them to work together. Be sure that there are no duplicate keys for PlayerData (see MoneyKey and HasInitializedKey configurations).

## Messages

The following section goes over messages that are available to developers who want to interface with SafeUdonChips.

+ _RefreshChipData
  + Checks for change in Money when using Scripted CheckTime
+ _ResetData
  + Resets all PlayerData and sets Money to the starting value
