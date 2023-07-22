
# Activator Action System

Create interactive worlds using activators and actions in the unity editor. Activators are things triggered usually by some user action, they can then chain into logical activators and ultimately activators can trigger actions.

Since the system was created within 3 days with a specific goal in mind - creating a dungeon with traps and basic puzzles - the system is fairly limited, however whatever you do with it you at the very least don't have to think about syncing, it should _just work_ (so long as you don't disable scripts part of this system). There is technically the option for you to create your own activators and actions, however there are several things to consider when doing so and I've currently not taken the time to document said things.

Due to the nature of how the system works - each activator and action being an individual component - it likely does not scale well. You can attempt to give the game objects somewhat descriptive names, but at the end of the day it requires a lot of clicking around in the hierarchy to understand what some logic setup might be doing.

With these downsides of the system in mind and you decide that this system is not what you're looking for, I'll point you to [CyanTrigger](https://github.com/CyanLaser/CyanTrigger) which is a more generic system. I've never used it personally so I cannot judge it, but purely going by its description it sounds more powerful. The issue with scalability may be a bit lessened, however since it is also working inside of the editor it will also never scale as well as code in text form. Something to keep in mind when making big and complex systems.

# Important

- Do not disable scripts part of this system, it would break syncing. For toggle-able interacts use the InteractProxy from the JanSharp Common package
- The initial state of the world is "everything is off". [Read more](#initial-state)

# Samples

This packages includes one sample which includes animation controller bases you could use for actions that require animations. To get the sample, in unity on the menu bar go to `Window => Package manager => In Project (top left) => Action Activator System => Samples`.

Note the use of `Write Defaults` in the animation states, I recommend reading the [documentation](https://docs.unity3d.com/2019.4/Documentation/Manual/class-State.html) for it to understand what it does. The reason why the loop toggle animation controller is using it on the default state is to reset the state to default when it gets deactivated, otherwise it may freeze somewhere in the middle. However that may be desired behaviour, in which case you can uncheck the checkbox. In the case for the one time animation it should always be enabled on the default state, at least in all cases I can think of. For every other state it is disabled.

Technically we **shouldn't use Write Defaults at all**, see [here](https://creators.vrchat.com/avatars/#write-defaults-on-states), although that page is specifically for avatars so I'm not even sure if it applies to worlds. Even still, the argument that it makes the animations more maintainable is very reasonable. In this case, since the sample animations are so tiny and I'm not actually providing sample animations, only the controller, I can't really provide you with "reset" animations. If you want to follow the workflow of always having Write Defaults disabled you'll have to add a reset animation that's a single frame long that "animates" the properties that get modified by the other animation in the controller to their default value, so both frame 0 and 1 in said reset animation have the same values. I'd definitely recommend doing this for bigger animation controllers. (Do note that I'm no expert with animations, but I believe what I just described does work.)

# Activators

All activators have 2 states: on and off.

## User Interactive Activators

- ToggleActivator (levers)
- ButtonActivator (DM buttons, maybe something else)
- PlayerTriggerActivator (pressure plates, trip wire and proximity triggers)
- ItemTriggerActivator (item slots)

## Logical Activators

- LogicalANDActivator (active when **all** input activators are active)
- LogicalORActivator (active when **any** input activator is active)
- LogicalXORActivator (active when **only one** input activator is active)
- LogicalNOTActivator (inverts the state of the referenced activator)
- MemoryActivator (The memory activator will reference 2 activators. An enable and a reset activator. When the enable activator activates the memory activator also activates. The enable activator deactivating gets ignored so the memory activator stays on. Then when the reset activator activates the memory activator deactivates and stays inactive.)
- ClockActivator (Emits a pulse every x seconds while the input activator is active. The input activator disabling merely pauses the clock, there is no way to reset it.)

## Activator Events

- OnActivate
- OnDeactivate
- OnStateChanged

# Actions

Actions can listen to and trigger on any of the activator's events.

## Stateless Actions

Stateless actions simply trigger on an event. Stateless actions are meant to be very short, like a few seconds.

- TriggerAnimationAction (animation with a trigger parameter. Spear wall traps might use this, though they might instead use `ToggleAnimationAction`)
- AudioAction
- ParticleAction
- MovementAction (move an object with the position sync script on it x units along some axis. Stateless because the position sync script is handling the state.)
- DropItemAction (drop one specific VRC_Pickup)

## Stateful Actions

Stateful actions reflect the state of a referenced activator, which means they can only ever have 2 states.

- ToggleAnimationAction (animation with a bool parameter - the state. For levers, trap doors)
- (animation loop) (can be done using ToggleAnimationAction)
- AudioLoopAction
- ParticleLoopAction
- ToggleGameObjectAction (doesn't actually directly reflect the state, simply toggles game objects's active state whenever the activator's state changes. technically could be done using animations but this is much simpler)

# Other Scripts

- ObjectPositionSync, required by MovementAction
- DM Toggle to show/hide DM only activators
  - `ToggleActivator` and `ButtonActivator` - use an `InteractProxy` (from the JanSharp Common package) to pass the Interact event from a locally togged object to the actual activator, since those activators must always be active
  - `PlayerTriggerActivator` and `ItemTriggerActivator` - cannot be made DM only because it breaks syncing, since that's based on local positions and trigger events

# Initial state

The initial state of the map should always be "everything is off". The system will then evaluate states on Start.

The only activator that can possibly do something right on Start is LogicalNOTActivator. Those all run Start and initialize their state which subsequently triggers any activators and actions listening to their events.

# Syncing

Synced scripts are

- ObjectPositionSync
- ToggleActivator
- ButtonActivator
- MemoryActivator

PlayerTriggerActivator and ItemTriggerActivator are synced through the position of the object entering the trigger zone. It might cause scuff but trying to work around it is like stupid difficult. And the rest is simply evaluated by every client locally.
