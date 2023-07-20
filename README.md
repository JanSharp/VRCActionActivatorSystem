
# Activators

All activators have 2 states: on and off.

## User Interactive Activators

- [x] ToggleActivator (levers)
- [x] ButtonActivator (DM buttons, maybe something else)
- [x] PlayerTriggerActivator (pressure plates, trip wire and proximity triggers)
- [x] ItemTriggerActivator (item slots)

## Logical Activators

- [x] LogicalANDActivator (active when **all** input activators are active)
- [x] LogicalORActivator (active when **any** input activator is active)
- [x] LogicalXORActivator (active when **only one** input activator is active)
- [x] LogicalNOTActivator (inverts the state of the referenced activator)
- [x] MemoryActivator (The memory activator will reference 2 activators. An enable and a reset activator. When the enable activator activates the memory activator also activates. The enable activator deactivating gets ignored so the memory activator stays on. Then when the reset activator activates the memory activator deactivates and stays inactive.)
- [x] ClockActivator (Emits a pulse every x seconds while the input activator is active. The input activator disabling merely pauses the clock, there is no way to reset it.)

## Activator Events

- OnActivate
- OnDeactivate
- OnStateChanged

# Actions

Actions can listen to and trigger on any of the activator's events.

## Stateless Actions

Stateless actions simply trigger on an event. Stateless actions are meant to be very short, like a few seconds.

- [x] TriggerAnimationAction (animation with a trigger parameter. Spear wall traps might use this, though they might instead use `ToggleAnimationAction`)
- [x] AudioAction
- [x] ParticleAction
- [x] MovementAction (move an object with the position sync script on it x units along some axis. Stateless because the position sync script is handling the state.)
- [x] DropItemAction (drop one specific VRC_Pickup)

## Stateful Actions

Stateful actions reflect the state of a referenced activator, which means they can only ever have 2 states.

- [x] ToggleAnimationAction (animation with a bool parameter - the state. For levers, trap doors)
- [x] (animation loop) (can be done using ToggleAnimationAction)
- [x] AudioLoopAction
- [x] ParticleLoopAction
- [x] ToggleGameObjectAction (doesn't actually directly reflect the state, simply toggles game objects's active state whenever the activator's state changes. technically could be done using animations but this is much simpler)

# Other Scripts

- [x] ObjectPositionSync, required by MovementAction
- [x] DM Toggle to show/hide DM only activators
  - `ToggleActivator` and `ButtonActivator` - use an `InteractProxy` to pass the Interact event from a locally togged object to the actual activator which must always be active
  - `PlayerTriggerActivator` and `ItemTriggerActivator` - cannot be made DM only because it breaks syncing, since that's based on local positions and trigger events

# Initial state

The initial state of the map should always be "everything is off". The system will then evaluate states on Start.

The only activator that can possibly do something right on Start is LogicalNOTActivator. Those all run Start and initialize their state which subsequently triggers any activators and actions listening to their events.

# Syncing

Synced scripts are

- ToggleActivator
- ButtonActivator
- MemoryActivator

PlayerTriggerActivator and ItemTriggerActivator are synced through the position of the object entering the trigger zone. It might cause scuff but trying to work around it is like stupid difficult. And the rest is simply evaluated by every client locally.
