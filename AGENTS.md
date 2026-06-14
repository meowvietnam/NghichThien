# AGENTS.md

This file provides guidance to Codex (Codex.ai/code) when working with code in this repository.

## Project Overview

This is a Unity-based cultivation/xianxia RPG game (觅长生 - "Seeking Immortality") with turn-based combat, character progression, and alchemy systems. The game features elemental mechanics (Five Elements: Kim/Metal, Mộc/Wood, Thủy/Water, Hỏa/Fire, Thổ/Earth), skill systems, and networked multiplayer.

**Company**: yusuiInc  
**Unity Version**: 2022.3.x or later (check ProjectSettings/ProjectVersion.txt)  
**Target Platform**: Mobile (Android/iOS) with landscape orientation

## Core Architecture

### Manager Pattern
The game uses a centralized manager architecture with persistent singleton managers:

- **GameManager** (`Assets/Scripts/GameManager.cs`) - Core game state, scene management, map/faction conversions, character data coordination
- **UIManager** (`Assets/Scripts/UIManager.cs`) - UI panel management and transitions
- **CombatManager** (`Assets/Scripts/Combat/CombatManager.cs`) - Turn-based combat orchestration, buff/debuff tracking, elemental interactions
- **MapManager** (`Assets/Scripts/AllMaps/MapManager.cs`) - World map navigation, node graph traversal, player movement
- **NetworkManager** (`Assets/Scripts/Network/NetworkManager.cs`) - TCP client connection, authentication, server communication
- **NpcManager** (`Assets/Scripts/AllMaps/NpcManager.cs`) - NPC interactions and dialogue

### Event System
The codebase uses an Observer pattern for decoupled communication via `EventList` constants:
- Combat events: `EventList.Combat.NEW_TURN_COMBAT`, `EventList.Combat.CHARACTER_TAKE_DAME`, etc.
- Gameplay events: `EventList.GamePlay.UPDATE_PLAYER_STATS`, `EventList.GamePlay.LOAD_SCENE`, etc.
- Network events: `EventList.Network.USE_DAN`, `EventList.Network.LEARN_SKILL`, etc.

Subscribe with `Observer.AddObserver(eventName, callback)` and notify with `Observer.Notify(eventName, params)`.

### Character Stats System
Character statistics use **SecureInt** and **SecureFloat** types (anti-cheat wrappers) throughout:
- **CharacterStatsSecure** (`Assets/Scripts/CharacterStatsSecure.cs`) - Persistent character data (HP, elemental affinities, level, equipment, skills)
- **CharacterCombat** (`Assets/Scripts/Combat/CharacterCombat.cs`) - Runtime combat state with temporary stats, buffs/debuffs, elemental energy (Linh Khí)
- Always use `SecureInt`/`SecureFloat` for numeric stats, never raw `int`/`float`

### Elemental System (Five Elements)
The **ELinhKhi** enum defines elemental types:
```csharp
enum ELinhKhi { None, Kim, Moc, Thuy, Hoa, Tho, Ma }
```

Key elemental relationships defined in `CombatManager`:
- **TuongSinh** (Generation): Mộc→Hỏa→Thổ→Kim→Thủy→Mộc
- **linhKhiLienKich** (Combo chain): tracks previous element for combo attacks

Combat revolves around accumulating and spending elemental energy to cast skills.

## Key Systems

### Combat System (`Assets/Scripts/Combat/`)
Turn-based combat with:
- **Skills**: Two types - **ThanThong** (神通, divine abilities) and **CongPhap** (功法, cultivation techniques)
  - ThanThong: `Assets/Scripts/Skill/ScriptThanThong/` - Active abilities requiring elemental energy
  - CongPhap: `Assets/Scripts/Skill/ScriptCongPhap/` - Passive/active techniques with level progression
- **Buffs/Debuffs**: Extensive `EBuffAndDeBuff` enum (100+ status effects) in `BaseSkill.cs`
- **Ngo Dao** (悟道, Enlightenment): Passive combat modifiers in `Assets/Scripts/AllMaps/NgoDao/`
- Damage calculation considers elemental interactions, buffs, equipment, and Ngo Dao effects

### Alchemy System (`Assets/Scripts/LuyenDan/`)
Pill crafting and consumption:
- **ETypeDan**: DanCombat (combat pills), DanBuff (stat buffs), DanDotPha (breakthrough pills), DanOther
- **EDanBuffType**: 40+ pill types affecting stats, elements, and progression
- Pills stored in inventory and consumed via network calls

### Cultivation/Progression (`Assets/Scripts/LuyenKhi/`)
Equipment and artifact system:
- Equipment types: weapons (vũ khí), armor (phòng cụ), accessories (trang sức)
- Artifacts with special effects
- Tier/Quality system defined in `Base/CoreConfig.cs`

### Map & Navigation (`Assets/Scripts/AllMaps/`)
Node-based world map:
- **Map** base class with **Stronghold** subclass for combat encounters
- Graph-based navigation between nodes
- Player movement tracked via `idNode` in character stats
- Scene mapping: `GameManager.dictMapTypeWithSceneName` (e.g., S1=TrucSonTong, S3=KimHongKiemPhai)

### Network Layer (`Assets/Scripts/Network/`)
Custom TCP client with state machine:
- **Client** class with state pattern (UnauthenticatedClient → AuthState → AllMaps state, etc.)
- Server communication via `Network.dll` plugin
- Character data synced from server to `Client.Instance.CharacterData`

## Data Management

### ScriptableObjects
Game data stored as ScriptableObjects (inherit from `BaseData`):
- **LuyenDanResources** - Pill definitions
- **LuyenKhiResources** - Equipment/artifact definitions
- **ComboSkillResource** - Enemy/NPC skill loadouts
- **StepConfig** - Cultivation realm progression

### Binary Resources
Exported data in `Assets/Exports/*.bin`:
- `LuyenDanResources.bin` - Pill data
- `LuyenKhiResources.bin` - Equipment data
- `maps.bin` - Map configurations

### Database
SQLite database via `Assets/Scripts/Database/LocalDb.cs` (using SQLite plugin in `Assets/Plugins/SQLite/`)

## Third-Party Dependencies

- **Odin Inspector** (`Sirenix.OdinInspector`) - Enhanced Unity inspector attributes
- **DOTween** - Animation/tweening library
- **Spine** - 2D skeletal animation (character sprites)
- **Newtonsoft.Json** - JSON serialization
- **SQLite** - Local database
- **LongDHModules** (`Assets/LongDHModules/`) - Custom utility modules:
  - **AntiCheat**: SecureInt/SecureFloat (XOR-encrypted values to prevent memory editing)
  - **Observer**: Event system implementation
  - **ObjectPool**: Object pooling for performance optimization

## Development Workflow

### Building
Open the project in Unity Editor. Use Unity's standard build process:
- **File → Build Settings** to configure platform
- Ensure all scenes in `Assets/Scenes/` are added to build
- Mobile builds require platform-specific SDKs (Android SDK, Xcode for iOS)

### Testing
- **Play Mode**: Test in Unity Editor via Play button
- Combat testing: Load combat scenes directly or use `PrepareCombat` flow
- Network testing: Requires running server (see `NetworkManager` host/port configuration)

### Scene Structure
- **Start** - Character creation/login
- **AllMaps** - Main world map (always loaded as base scene)
- **S1-S29** - Individual location scenes (loaded additively)
- **Combat scenes** - Turn-based battle arenas

## Code Conventions

### Naming
- **Vietnamese terms**: Many game concepts use Vietnamese (e.g., `ThanThong`, `CongPhap`, `NgoDao`, `LuyenDan`)
- **Enums**: Prefix with `E` (e.g., `ELinhKhi`, `EMonPhai`, `EBuffAndDeBuff`)
- **Managers**: Suffix with `Manager`
- **UI classes**: Suffix with `UI` or `Panel`

### Security
- **Always use SecureInt/SecureFloat** for gameplay-critical numeric values (HP, damage, stats, currency)
- Never expose raw values in network communication
- Validate server responses before applying to local state

### Performance
- Use object pooling via `Pool.Spawn()` and `Pool.Despawn()` for frequently instantiated objects (damage text, VFX, UI elements)
- Combat effects use pooled prefabs to avoid GC pressure

### Event-Driven Design
- Prefer Observer pattern over direct coupling between systems
- Subscribe to events in `InitPosCallExecute()` or `Awake()`
- Clean up observers in `OnDestroy()` to prevent memory leaks

## Common Tasks

### Adding a New Skill
1. Create ScriptableObject in `Assets/Scripts/Skill/ScriptThanThong/` or `ScriptCongPhap/`
2. Inherit from `ThanThong` or `CongPhap` base class
3. Implement `Execute()` method with combat logic
4. Define `GetDesc()` for skill description
5. Set elemental requirements in `requireActives` list
6. Add skill ID to character's `thanThongOwner` or `congPhapOwner` list

### Adding a New Buff/Debuff
1. Add enum value to `EBuffAndDeBuff` in `BaseSkill.cs`
2. Create sprite icon and add to `CombatManager.buffOrDebuffIcons` list
3. Implement buff logic in relevant skill's `Execute()` method
4. Use `CharacterCombat.AddBuffOrDeBuff()` to apply
5. Handle buff effects in combat calculation methods

### Adding a New Pill
1. Create ScriptableObject inheriting from pill base class in `Assets/Scripts/LuyenDan/`
2. Set `ETypeDan` and `EDanBuffType`
3. Define effects in pill's data structure
4. Add to `LuyenDanResources` ScriptableObject
5. Export to `LuyenDanResources.bin`

### Debugging Combat
- Enable combat logs in `CombatManager`
- Check `CharacterCombat.buffAndDebuffCount` dictionary for active status effects
- Verify elemental energy (`khiKim`, `khiMoc`, etc.) values
- Use Odin Inspector's `[ShowInInspector]` attribute to expose runtime state

## Important Notes

- **Network dependency**: Many features require server connection. Offline testing is limited to combat scenarios with pre-configured data.
- **Binary resources**: Modifying ScriptableObjects requires re-exporting to `.bin` files for runtime use.
- **Scene additivity**: The game uses additive scene loading. `AllMaps` is the persistent base scene.
- **Mobile optimization**: Keep draw calls low, use sprite atlases, and profile on target devices regularly.
