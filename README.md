# 🗺️ Territory Expansion Mod (领土扩展模组)

**Current Features:**

- 🌍 **Dynamic Territory Visualization**  
  "Automatically colors map tiles based on **faction control**, clearly showing the territorial boundaries of each faction."

- 🏘️ **Real-Time Updates**  
  "Territory regions are **updated dynamically** when settlements are added or removed."

- 🧱 **Modular System Architecture**  
  "Introduces a new `Territory` class for map tiles and a `TerritorySettlement` class for settlements, laying the foundation for extended functionality."

- 🛠️ **Expansion-Ready**  
  "Includes a `TerritoryManager` interface designed to support **future gameplay mechanics** (e.g., taxation, lordship, border friction).  
  > *Note: Currently, no extra gameplay mechanics are implemented.*"
  
## What is **Influence** in VC_Territory?

**Influence** is a numeric value representing a settlement’s power or control in the world. Every settlement in the mod has an **Influence** score that denotes how far its faction’s reach extends on the world map. In essence, Influence measures a faction’s territorial strength emanating from that settlement. It is used by the mod’s territory system to determine which faction controls each map tile and how borders are drawn between factions.

**Purpose:** Influence serves as the foundation for current and future gameplay mechanics around territory. Currently, its main purpose is to **visualize faction territories** on the world map (each tile colored by its controlling faction) and to allow basic mechanics like territory-based rewards or penalties.

## How Influence Affects Gameplay

- **Territory Expansion:** Influence spreads from each settlement to nearby tiles, diminishing over distance. Tiles track influence values from multiple settlements and assign ownership to the one with the highest influence. Factors such as terrain and biome affect propagation efficiency.

- **Territorial Boundaries:** Borders dynamically update when influence values change. A growing settlement may expand its territory, while a declining one might lose control of border tiles.

- **Gameplay Events:**
  - **Quests**: Successful completion of certain quests modifies the influence of involved factions.
  - **Colony Wealth**: Influence can scale with colony wealth if enabled, increasing the player’s territorial reach.
  - **Taxation**: Settlements generate silver based on the number of tiles they control.

## Key Classes and API

### `TerritorySettlement` – Settlement Influence

- `float Influence`
  - Current influence value.

- `void SetInfluence(float inf)`
  - Sets a new influence value and marks the settlement for recalculation.

- `bool isDirty`
  - Flag for update.

- `void Update_LongTerm_Tick()`
  - Triggers influence recalculation or extra actions every 60,000 ticks, regardless of isDirty status.

### `Territory` – Tile Control and Influence

- `Dictionary<int, float> influencesDict`
  - Maps settlement IDs to their influence on this tile.

- `Settlement settlement`
  - The settlement that currently owns this tile.

- `void Notify_Influence_Add(Settlement settle, float influence)`
  - Adds or updates influence for a settlement and propagates it.

- `void _Broadcast(Settlement settle, float newInfluence)`
  - Recursively spreads influence to neighboring tiles.

- `void Notify_Influence_Change(Settlement settle, float newInfluence)`
  - Updates existing influence value without spreading.

- `void Notify_Influence_Remove(Settlement settle)`
  - Removes a settlement’s influence.

### `TerritoryManager` – Global Territory Manager

- `static List<TerritorySettlement> territorySettlementList`
- `static List<Territory> territories`

- `void InfluenceInit()`
  - Initializes all settlement influences.

- `static void Notify_Influence_Add(Settlement settle)`
  - Begins influence propagation from a new settlement.

- `static void Notify_Influence_Remove(Settlement settle)`
  - Removes a settlement and clears its territory.

- `static void Notify_Influence_Change(TerritorySettlement ts)`
  - Recalculates tile control for a changed settlement.

- `static void SelectSettlement(Territory t)`
  - Determines which faction owns a tile.

- `void TerritoryManagerLongTermTick()`
  - Performs long-term updates every 60,000 ticks.
  - Invokes `LongTermTick()` on all `ITickableComponent` entries in `extraComponents`.
  - Calls `Update_LongTerm_Tick()` on all `TerritorySettlement` instances.
  - Calls `Update_LongTerm_Tick()` on all `Territory` tiles.


### `InfluenceHelper` – Influence Modifying Utilities

- `static void AddInfluence(this TerritorySettlement ts, float amount)`
  - Changes influence and marks for update.

- `static void AddInfluenceToFaction(Faction faction, float amount)`
  - Adjusts influence for a random settlement of a faction.

### `QuestPart_ChangeInfluence`

- Adds/removes influence as part of a quest reward or penalty.

- Called automatically at quest cleanup based on success or failure.

## Influence Configuration

- **InfluenceFactorDef**
  - XML config with `minInfluence`, `maxDistance`, biome/terrain/faction multipliers.

- **Mod Settings**
  - Players can adjust propagation rules and enable/disable systems like taxation and wealth-based influence growth.

- **Extension Points**
  - `extraComponents` support per-tile or per-settlement mod extensions.

## Summary

Influence determines territory control in VC_Territory. It propagates from settlements, affects tile ownership, and integrates with quests, colony wealth, and taxation. The system is built with mod extensibility in mind, allowing developers to add diplomacy, expansion, and other mechanics atop this framework.

