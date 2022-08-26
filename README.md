
# Activators

All activators have 2 states: on and off.

## User Interactive Activators

- pressure plates
- levers (2 states, no timers. Just flip on interact)
- proximity (powered by trip wire and or magic)
- item placement (powered by magic)
- buttons (effectively the same thing as levers except they almost instantly turn off again)

## Internal Activators

- logical AND activator (active when **all** activators in its group are active)
- logical OR activator (active when **any** activator in its group is active)
- logical XOR activator (active when **one one** activator in its group is active)
- logical NOT activator (inverts the state of the referenced activator)
- memory activator (The memory activator will reference 2 activators. An enable and a reset activator. When the enable activator activates the memory activator also activates. The enable activator deactivating gets ignored so the memory activator stays on. Then when the reset activator activates the memory activator deactivates and stays inactive.)

## Activator Events

- OnActivate
- OnDeactivate
- OnStateChanged

# Actions

Actions can listen to and trigger on any of the activator's events.

## Stateless Actions

Stateless actions simply trigger on an event. Stateless actions are meant to be very short, like a few seconds.

- animation
- sound
- particle
- move an object with the position sync script on it x units along some axis

## Stateful Actions

Stateful actions reflect the state of a referenced activator, which means they can only ever have 2 states.

- animation with 2 states
- animation loop
- sound loop
- particle loop

# Other Scripts

- Object position sync

# Initial state

The initial state of the map should always be "everything is off". The system will then evaluate states on Start.

# Syncing

The only synced scripts are [interactive activators](#user-interactive-activators) and [stateful actions](#stateful-actions), although the stateful actions do not sync their active state since that is handled through activators already. Which means they only sync extra data, such as the current time in the loop. That also means that any stateful actions without any additional state besides their on off state do not require any syncing.

# Dependencies

- `OnBuildUtil`. Since the file should not be in this folder it is not a `.cs` file in here - it is a `.txt`. Simply copy the contents of [](OnBuildUtil.txt) to `Assets/JanSharp/Common/OnBuildUtil.cs`.

# Internals

Due to UdonSharp (`0.20.3`) limitations activators consist of a lot of copy pasted code. I can't think of anything we can do about that, we simply can't use base classes. Not being able to use base classes or interfaces also causes almost all inter script interactions to use reflection which increases the chance of error. It's all not great but we work with what we got. At least the majority of the editor code can be deduplicated.
