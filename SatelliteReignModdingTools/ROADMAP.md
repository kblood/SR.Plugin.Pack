# Satellite Reign Modding Tools – Improvement Roadmap

Purpose
- Elevate SatelliteReignModdingTools from a set of browsers to a unified, production-ready mod authoring suite.
- Prioritize high-impact features while keeping the codebase maintainable and compatible with existing exports from LoadCustomData Enhanced.

Guiding Principles
- One unified application: consistent UX, shared services (IO, validation, search, undo/redo).
- Readable, serializable DTOs that mirror game data as exported by LoadCustomData.
- Non-destructive editing: preview, validate, diff, and package changes.
- Build features incrementally; retain compile stability at every step.

Status Snapshot – 2025-08-07
- Completed (Phase 0 foundations)
  - IGameDataManager interface (Services/IGameDataManager.cs)
  - DataWatcher with auto-reload wiring in MainMenu (Services/DataWatcher.cs, MainMenu.cs)
  - SkillDataManager with initial DTOs (Services/SkillDataManager.cs, DTOs/SkillsDTOs.cs)
  - XmlDiffUtil utility present (Services/XmlDiffUtil.cs)
  - Centralized settings panel (SettingsForm.cs) and persisted paths via Settings.Default
  - SRInfoHelper for centralized logging (Services/SRInfoHelper.cs)
- Implemented (this update)
  - SharedToolbar control and integrated into Item, Quest, Translations, Skills
  - DiffViewerForm modal for line-based diff preview and confirmation
  - ItemBrowser: basic validation and search; Save (with diff) uses DiffViewerForm
  - QuestBrowser: validation, search, and Save (with diff)
  - TranslationsBrowser: validation, search, and Save (with diff)
  - SkillsBrowser: search and DiffViewerForm for save confirmation
- In Progress
  - Unified visual polish across browsers; migrating ad-hoc buttons into shared toolbar everywhere
  - Validation results UX (moving from MessageBox to reusable panel/dialog)
- Not Started
  - Economy & Faction Browser (read-only)
  - Mod packaging pipeline and manifest

Current Capabilities (baseline)
- Browsers: Quests, Items, Enemies, Missions, Translations, Skills (initial)
- Services: FileManager, ItemDataManager, SkillDataManager, SpawnCardManager, SpriteSerializer, SRInfoHelper, XmlDiffUtil, DataWatcher
- Formats: XML exports from LoadCustomData Enhanced (translations.xml, itemDefinitions.xml, questDefinitions.xml, icons/)

Phase 0: Stabilization and Shared Infrastructure (1–2 weeks)
Deliverables
- Unified main window (tabbed) and shared toolbar (Search, Filter, Reload, Export, Validate)
- Centralized configuration for data directory, export directory, and auto-refresh (file watcher)
- Shared services: IGameDataManager interface, ValidationResult model, Diff utilities, central logging via SRInfoHelper
- Non-breaking refactors: leave existing browsers functional; add common helpers gradually

Tasks
- Add Services/ValidationResult model + common validation helpers
- Introduce file watching and auto-refresh for XML changes [DONE]
- Implement basic Diff view for XML vs. in-memory (line-based first, object-based later)
- Add Settings panel (paths, auto-backup, auto-validate on save) [DONE]
- Create a SharedToolbar UserControl used by all browsers (hosts Search, Reload, Validate, Export)

Acceptance Criteria
- Tools launch with a unified header and shared commands
- Any browser can reload after external export with no restart [DONE]
- Validate command lists issues (missing keys, IDs out of range) without crashing (visible UI)

Phase 1: High-Impact Feature Foundations (3–5 weeks)
1) Skills & Progression Browser (read-only → editable later)
- DTOs: SerializableSkillData, SerializableAgentClassData, SerializableSkillTree, SerializableProgressionCurve [INITIAL DTOs DONE]
- Manager: SkillDataManager implementing IGameDataManager [DONE]
- UI: TreeView for dependencies, simple property grid for selected skill, XP curve preview stub
- Data: progressionData.xml (if not present, surface message and link to LoadCustomData enhancement)

2) Economy & Faction Browser (read-only → editable later)
- DTOs: SerializableATMData, SerializableBankNetworkData, SerializableFactionRelationshipData, SerializableEconomicBalance
- Managers: EconomyDataManager, FactionDataManager (IGameDataManager)
- UI: Grid for faction relationships; list for ATMs with filters; basic cost tables
- Data: economy/faction XMLs (if missing, degrade gracefully; provide validation warnings)

3) Enhanced Weapon Data (extend ItemBrowser)
- Extend in-memory model to surface ammo types, attachments, damage curves if available in itemDefinitions.xml
- Wire ItemBrowser "Combat" tab to show ammo/attachments and visualize basic damage over distance/time when data is available (fallback to empty-state)

Acceptance Criteria
- New browsers open with placeholder data or informative empty-state messages
- Managers can Load() and Validate() without runtime exceptions
- ItemBrowser shows a wired-up Combat tab with detected advanced weapon data

Phase 2: Content Creation (4–6 weeks)
- Audio & Atmosphere Browser: assign music/sfx per district, list audio zones and triggers; embed simple player for preview (optional later)
- Character Appearance Browser: wardrobe list, color palettes, icon previews; 3D preview TBD (stretch, requires Unity host or embedded viewer)
- Enhanced Weapon Browser: attachment compatibility matrix and ammo definitions

Acceptance Criteria
- Audio data round-trips through DTOs and validates basic referential integrity
- Appearance DTOs deserialize and present editable lists with validation
- Weapon attachments visualize compatibility consistency

Phase 3: World and AI Configuration (5–7 weeks)
- World Browser: district, building, location listings with security levels and tags
- AI Browser: patrol route lists and state machine summaries (visual editor is a stretch goal)

Acceptance Criteria
- World DTOs serialize/deserialize without loss; data validated (IDs, bounds, references)
- AI listings render from exported data; visual editing TBD

Phase 4: Packaging, Validation, and Community (2–3 weeks)
- Mod Package Manager: manifest, dependency tracking, compatibility validation, bundle creation
- Unified Export: one-click export of all modified systems to a package
- Diff/Compare: side-by-side comparison and changelog generation

Acceptance Criteria
- Tools can produce a self-contained mod package with manifest
- Validation reports warnings/errors before export

Cross-Cutting Enhancements
- Undo/Redo stack per browser
- Presets: save/load templates for common changes
- Search across all systems
- Performance: avoid re-parsing large XMLs unnecessarily

Dependencies on LoadCustomData (game-side mod)
- Ensure exports exist for progressionData.xml, economy/faction data, audio zones/triggers, world/districts, AI routes
- Maintain lowercase booleans and XmlSerializer-compatible structures
- Provide IDs/names that can be cross-referenced without reflection

Use of Decompiles and Game Install
- Decompiles/Assembly-CSharp: confirm type and field names for DTO mapping (e.g., SkillManager, MoneyManager, FactionManager, AudioManager, District)
- SatelliteReign/: for verifying runtime behavior and testing imports via BepInEx and LoadCustomData

Immediate Next Steps (Sprint: Aug 7–14, 2025)
- SharedToolbar: add a reusable UserControl and embed into Item/Quest/Translations/Skills browsers
- Validation UI: standard panel/modal to display results from IGameDataManager.Validate(); wire Items/Quests/Skills
- ItemBrowser Combat tab: bind ammo types, attachments, and curve stubs from DTOs; show empty-state when unavailable
- Economy & Faction (read-only): scaffold DTOs and managers; basic UI with filters and empty-state messaging
- Diff View: integrate Services/XmlDiffUtil in a simple modal for Items and Quests
- Contribution Docs: document DTO/validation conventions and folder structure

Acceptance Test Matrix (initial)
- Quests: translation key linkage, circular sub-quest detection, invalid location warnings
- Items: modifier range sanity checks, icon presence, research references
- Skills: prerequisite cycles, XP curve monotonicity, class restrictions
- Economy/Factions: matrix symmetry where required, bounds on rates, ATM coordinates in valid districts

Notes
- Keep WinForms for UI parity; avoid designer-heavy changes early—favor programmatic layouts for new browsers
- Ensure SatelliteReignModdingTools remains usable even when some XML exports are missing (progressive enhancement)
