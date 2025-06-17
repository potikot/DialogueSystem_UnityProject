# UniTalks Dialogue System

UniTalks is a modular, scriptable dialogue framework for Unity that empowers developers to build rich, reactive, and extensible conversation systems — from linear cutscenes to branching NPC logic, procedural text, and runtime-integrated logic using a clean scripting API.

## Features
- Extensible dialogue graphs with runtime and editor support
- Node-based visual editor
- In-editor dialogue testing
- Visual management of the dialogue database
- Custom scripting API to control dialogue flow via code
- Event hooks and runtime commands for gameplay integration
- Works out of the box or integrates with your existing systems

---

## Installation

### Unity Package Manager

```
https://github.com/potikot/UniTalks.git
```

<details>
  <summary> Details </summary>
  <br>

  1. Open your Unity project.
  2. Navigate to `Window` > `Package Manager`.
  3. Click the `+` button in the top left.
  4. Select `Add package from git URL...`.
  5. Enter following url: `https://github.com/potikot/UniTalks.git`.
  6. If you want to install specific version just add `#v1.0.0` to the link (`1.0.0` is version you want).
  7. Click `Add`. The Runtime Console package will be installed.

</details>

### [Version History](https://github.com/potikot/UniTalks/tags)

> [!NOTE]
> It is better to read README for your installed version of package
> Current version `1.0.0-alpha`

---

## Usage

### Quickstart

To get started:
1. Create a dialogue using the visual node editor.
2. Attach a component implementing the IDialogueView interface to a GameObject in your scene.
3. Configure the component, then use UniTalksAPI methods to start the dialogue.

```csharp
UniTalksAPI.StartDialogue("dialogue name"); // will find view on scene
UniTalksAPI.StartDialogue("dialogue name", dialogueView); // directly set view
```
This flexible approach lets you quickly prototype or integrate custom dialogue behavior.

---

### Main Entry Points

`UniTalksAPI`

UniTalksAPI is the primary runtime interface for controlling dialogue execution, data loading, and global variable storage. It provides high-level access to all major operations in the UniTalks dialogue system, including:

#### Start Dialogue
You can start a dialogue by name, data asset, or using custom controller logic.

```csharp
// Basic usage (auto-finds view in scene)
UniTalksAPI.StartDialogue("intro");

// With explicit view reference
UniTalksAPI.StartDialogue("intro", myView);

// With custom DialogueController subclass
UniTalksAPI.StartDialogue<MyCustomController>("intro", myView, constructorArgs);

// Async version (async data loading)
await UniTalksAPI.StartDialogueAsync("intro", myView);
```
Overloads support both string dialogue IDs and `DialogueData` assets.

#### Dialogue Loading
Dialogue can be lazily loaded via ID or tag group:

```csharp
// Synchronous
bool loaded = UniTalksAPI.LoadDialogue("intro");
bool groupLoaded = UniTalksAPI.LoadDialogueGroup("cutscenes");

// Async
DialogueData data = await UniTalksAPI.GetDialogueAsync("intro");
bool success = await UniTalksAPI.LoadDialogueAsync("intro");
```

#### Variable System
Interact with runtime variables (e.g., for branching, conditions, memory):

```csharp
UniTalksAPI.SetVariable("hasKey", true);

bool hasKey = UniTalksAPI.GetVariable("hasKey", false);

// Raw access
object raw = UniTalksAPI.GetRawVariable("someId");

// State inspection
bool exists = UniTalksAPI.HasVariable("npcMet");
UniTalksAPI.RemoveVariable("npcMet");
UniTalksAPI.ClearVariables();
```

#### Debug & Logging
Decorator for Unity's default logging system:

```csharp
UniTalksAPI.Log("Hello");
UniTalksAPI.LogWarning("Careful...");
UniTalksAPI.LogError("Something went wrong.");
```
Use these methods to route logs through Unity's `Debug.Log`, `Debug.LogWarning`, and `Debug.LogError`, with consistent formatting via `UniTalksAPI`.

---

### Dialogue Components Manager
Manages and initializes all core system components at both runtime and in the editor. It enables modularity by allowing you to swap or override default system modules.

Each module implements either a base class with virtual logic or a corresponding interface, allowing full extensibility. You can assign your own custom implementation like this:
```csharp
DialogueComponents.Database = new MyCustomDatabase();
EditorDialogueComponents.Database = new MyCustomEditorDatabase();
```
This design allows easy customization of core behavior without modifying the original system.

---

### Preferences
Manages global settings for both runtime and editor use.
- Handles configuration such as default components or settings of components, debug logging, database paths, and more.
- Serialized using Unity's Scriptable Objects.

---

### Database

The `DialogueDatabase` class serves as the central runtime component for managing dialogue data in UniTalks. It abstracts loading, accessing, modifying, and releasing dialogue assets, as well as their associated tags and resources.

**Core Responsibilities**
* Persistent Storage: Interacts with IDialoguePersistence (e.g., `JsonDialoguePersistence`) to load and save dialogue files asynchronously or synchronously.
* Tag Management: Indexes and retrieves dialogue names by tags using a `Dictionary<string, HashSet<string>>`.
* Resource Management: Dynamically loads AudioClip, Sprite, Texture, or other Unity Object types via `Resources.Load()` or `Resources.LoadAsync()`.
* Lifecycle Management: Tracks which dialogues are loaded into memory, and allows for dynamic releasing or reloading.

**Key Paths**
* `RootPath / RelativeRootPath`: File system paths to the root database folder.
* `DialoguesRootPath / DialoguesRelativeRootPath`: Subdirectories where dialogue folders are stored.
* `ResourcesPath`: Path used to resolve Unity Resources assets (e.g., audio/images) for dialogue playback or display.

**Initialization Flow**
The `Initialize()` method:
* Instantiates a `JsonDialoguePersistence`
* Configures directory structure
* Scans and registers existing dialogues
* Collects associated tags
* It also supports lazy loading of dialogues via `GetDialogueAsync` or `LoadDialogueAsync`.

**Dialogue Access**
* `GetDialogue(string) / GetDialogueAsync(string)`: Loads a dialogue if not already cached.
* `ContainsDialogue(string)`: Checks if a dialogue is loaded.
* `TryChangeDialogueName(old, new)`: Renames a dialogue in memory if not conflicting.

**Tag Operations**
* `LoadDialoguesByTag(tag) / LoadDialoguesByTagAsync(tag)`: Loads all dialogues sharing the given tag.
* `ReleaseDialoguesByTag(tag)`: Releases all tagged dialogues from memory.
* `Tags`: Provides a read-only dictionary of tags mapped to dialogue names.

**Resource Access**
Supports both sync and async access for Unity Object resources:

```csharp
LoadResource<AudioClip>("mySound");
await LoadResourceAsync<Sprite>("myImage");
```
Directories are configurable per type via `TryAddResourceType<T>()`.

**Editor Support**
* `EditorDialogueDatabase` provides a parallel system for the Unity Editor that enables:
* Dialogue creation (`CreateDialogue`)
* Saving/loading editor data (`SaveDialogueAsync`, `LoadDialogue`)
* Integration with Unity’s `AssetDatabase`
* Maintaining separation between editor and runtime representations (`EditorDialogueData` vs. `DialogueData`)

**Internal Structure**
* `dialogues`: Holds runtime instances of DialogueData
* `tags`: Keeps a fast index of tag-to-dialogue relationships
* `resourceDirectories`: Maps Unity object types to subfolders within Resources

**Extensibility**
The system supports:
* Custom `IDialoguePersistence` strategies (e.g., XML, remote API)
* New Unity `Object` types via resource type registration
* Custom tag-driven dialogue logic

---

### Dialogue Editor

The DialogueEditor system provides a fully custom Unity Editor window and graph interface for visually authoring dialogue trees in UniTalks. It is composed of several key components built on top of Unity's `GraphView` API and designed for extensibility, usability, and real-time synchronization with runtime dialogue data.

**Overview**
* Editor Window: `DialogueEditorWindow` inherits from `BaseUniTalksEditorWindow` and manages the visual dialogue editor lifecycle.
* Graph System: `DialogueGraphView` extends `GraphView`, rendering a node-based interface for dialogue editing.
* Node Views: `INodeView` and `NodeView<T>` define the base architecture for custom dialogue node types.
* Node Registration: `DialogueEditorWindowsManager` dynamically registers node types via reflection for runtime instantiation.

#### DialogueEditorWindow

Handles the lifecycle of a dialogue editing session.
* Dynamic Title: Updates title bar based on dialogue name.
* Graph Instantiation: Loads and binds `DialogueGraphView` with the current dialogue data.
* Floating UI: Injects additional controls (e.g., `DialogueSettingsPanel`) as floating UI elements.
* State Persistence: Saves and restores the graph position and zoom scale via `EditorDialogueData`.

Key inherited from `BaseUniTalksEditorWindow` methods:
* `OnEditorDataChanged()`: Initializes graph and settings panel.
* `SaveChanges()`: Serializes current graph state via `EditorDialogueComponents.Database.SaveDialogue`.
* `DiscardChanges()`: Marks the state as clean (TODO: full discard logic).
* `CreateGUI()`: Injects UI elements (e.g., Save button, styles).

#### BaseUniTalksEditorWindow

Abstract base for all UniTalks editor windows.
* Manages `EditorDialogueData` lifecycle
* Subscribes to runtime/editor events (`OnNameChanged`, `OnDeleted`)
* Provides hooks for initialization and cleanup

Key properties:
* `EditorData`: Provides access to the full dialogue editing context.
* `DialogueName`: Returns current dialogue name.
* `OnDestroy()`: Unsubscribes from delegates to prevent leaks.

#### DialogueGraphView
Custom Unity `GraphView` used to render and manipulate dialogue nodes visually.

Key responsibilities:
* Rendering: Draws nodes, ports, edges, and background grid.
* Persistence: Loads positions from `EditorDialogueData`; saves on geometry change.
* Shortcuts: Ctrl+S triggers save operation.
* Context Menu: Dynamically generates node creation menu using registered node types.
* Edge Handling: Tracks and syncs graph edges with runtime `NodeData` via:
* * `CreateEdges()`
* * `RemoveElements()`

**DialogueEditorWindowsManager**
* Manages all windows of type `DialogueEditorWindow`
* Manages static registration of node types
* Allows dynamic mapping between NodeData and INodeView types
* Supports argument injection via AddNodeType<TD, TV>(params object[] args)
* Used by BuildContextualMenu in DialogueGraphView to instantiate nodes at runtime

#### Node System
**`INodeView` Interface**

Defines the core contract for custom dialogue node views:
* `Initialize()`: Binds node to data and graph
* `Draw()`: Constructs the node's UI
* `GetData()`: Returns `NodeData`
* `OnConnected()`, `OnDisconnected()`: Hook for edge events
* `DrawEdges()`: Optional edge drawing logic

**`NodeView<T>`**

Generic base class for creating typed node views.

Responsibilities:
* Binds `EditorNodeData` to runtime `NodeData`
* Renders inputs/outputs, fields, and editor controls
* Tracks position changes (`OnGeometryChanged`)
* Detaches lifecycle callbacks on remove (`OnDetachFromPanel`)

Built-in UI elements include:
* Title bar
* Speaker field and dropdown
* Audio clip field
* Custom command list
* Dynamic output choices (if applicable)

---

### Variables

The Variables module provides a flexible system to store, retrieve, and parse runtime variables in a key-value fashion, designed to be used within the UniTalks dialogue system or any other runtime logic requiring dynamic variable substitution.

#### VariablesStore

VariablesStore is a class managing a collection of variables stored as string keys mapped to objects. It supports generic setting and getting, allowing any type-safe data to be stored and retrieved.

Key features:
* `Set<T>(string key, T value)` <br>
Stores or overwrites a variable with the specified key and generic type value.
* `Get<T>(string key, T defaultValue = default)` <br>
Retrieves a variable by key casted to type T. If the variable is not found or cannot be cast to T, returns the provided defaultValue.
* `GetRaw(string key)` <br>
Returns the raw object stored under the key without casting, or null if not found.
* `Has(string key)` <br>
Checks if a variable with the given key exists.
* `Remove(string key)` <br>
Removes a variable by key, returning true if removed, otherwise false.
* `Clear()` <br>
Clears all stored variables.

Example:
```csharp
VariablesStore store = new VariablesStore();
store.Set("playerHealth", 100);
int health = store.Get<int>("playerHealth", 0); // returns 100
```

#### VariablesParser

`VariablesParser` is a static utility class that processes text strings, replacing variable placeholders wrapped in curly braces `{}` with their corresponding values retrieved via `UniTalksAPI.GetRawVariable`.

Functionality:
* Scans input text for patterns like `{variableName}`.
* Looks up each `variableName` in the variable storage.
* Replaces the placeholder with the string representation of the variable value.
* If a variable is missing, replaces with a descriptive error placeholder: `<variable 'variableName' not found>`.

Example:
```csharp
string dialogueLine = "Hello, {playerName}! Your score is {score}.";
string parsedLine = VariablesParser.Parse(dialogueLine);
// If playerName = "Alex" and score = 250, parsedLine will be:
// "Hello, Alex! Your score is 250."
```

---

### Commands

#### Static

You can register static methods and fields with `CommandAttribute`. Basically command appear in Console as `<ClassName>.<(Method/Field)Name> <arg1> <arg2> ...`. Attribute contains `Name` and `IncludeTypeName` fields:

- `Name` replaces method/field name.
- `IncludeTypeName` determines whether the class name will be included in the command (`true` - will be included, `false` - will not).

<details open>
<summary> Attribute usage example </summary>
<br>

```csharp
using PotikotTools.Commands;

public class ExampleClass
{
    [Command(Name = "field")] // will add command 'field <int>'
    private static int exampleField;

    [Command] // will add command 'ExampleClass.ExampleMethod <int> <string>'
    public static void ExampleMethod1(int value, string message)
    {
        exampleField = value;
        Debug.Log($"{exampleField}. {message}");
    }

    [Command(IncludeTypeName = false)] // will add command 'ExampleMethod2'
    public static void ExampleMethod2() { }
}
```

</details>

#### Non Static

Also, you can register commands through this API:

```csharp
CommandHandler.Register(string commandName, Action callback);
CommandHandler.Register(ICommandInfo commandInfo);
```

All commands should implement `ICommandInfo` interface. Default command types is `MethodCommandInfo`, `FieldCommandInfo` and `ActionCommandInfo`. And also you can add your own type of command by implementing interface in your class.

<details>
<summary> Example implementation </summary>
<br>

```csharp
using System;
using System.Reflection;

public class FieldCommandInfo : ICommandInfo
{
    // 'HintText' cache
    private string _hintText;
    // 'ParameterTypes' cache
    private Type[] _parameterTypes;

    // Name. Should not contain spaces
    public string Name { get; private set; }
    // Target object
    public object Context { get; private set; }
    // Field info
    public FieldInfo FieldInfo { get; private set; }

    // Hint in console
    public string HintText
    {
        get
        {
            if (!string.IsNullOrEmpty(_hintText))
                return _hintText;

            _hintText = $"{Name} {ParameterTypes[0].Name}";
            return _hintText;
        }
    }

    // Field type
    public Type[] ParameterTypes => _parameterTypes ??= new Type[1] { FieldInfo.FieldType };

    // Validation check
    public bool IsValid => FieldInfo != null && !string.IsNullOrEmpty(Name);

    // Constructor
    public FieldCommandInfo(string name, FieldInfo fieldInfo, object context = null)
    {
        Name = name.Replace(' ', '_');
        FieldInfo = fieldInfo;
        Context = context;
    }

    // Invoke command
    public void Invoke(object[] parameters)
    {
        FieldInfo.SetValue(Context, parameters[0]);
    }
}
```

</details>

