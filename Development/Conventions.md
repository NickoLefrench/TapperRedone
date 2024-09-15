# Coding Conventions

In no particular order to start.

## Namespace and Object naming

All scripts made for the game should be in the namespace FMS.TapperRedone.[folder path from /Scripts].
For example, a script at path _/Scripts/Characters_ should be in the namespace _FMS.TapperRedone.Characters_.
Eventually, we may look at moving some scripts to a custom package, with a parent namespace like FMS.Utilities.

Scripts should not have "Script" in the name.
Script names should ideally be descriptive, but not too simple, so that there is minimal risk of clashing with other classes,
including those in System or UnityEngine.
