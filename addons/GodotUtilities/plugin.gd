@tool
extends EditorPlugin

const AUTOLOADS: Dictionary = {
	"SaveManager": "res://addons/GodotUtilities/src/save_system/SaveManager.cs",
	"AudioManager": "res://addons/GodotUtilities/src/audio/AudioManager.cs",
	"ParticlesManager": "res://addons/GodotUtilities/src/particles/ParticlesManager.cs",
	"UIManager": "res://addons/GodotUtilities/src/ui_management/UIManager.cs",
	"InventoryManager": "res://addons/GodotUtilities/src/inventory/InventoryManager.cs",
	"PhysicsQuery2D": "res://addons/GodotUtilities/src/PhysicsQuery2D.cs",
}

const SETTINGS: Dictionary = {
	"godot_utilities/audio/sfx_folder": "",
	"godot_utilities/audio/music_folder": "",
	"godot_utilities/audio/ambience_folder": "",
	"godot_utilities/audio/sfx_pool_trim_cooldown": 5.0,
	
	"godot_utilities/particles/particles_folder": "",
	
	"godot_utilities/ui/panels_folder": "",
	"godot_utilities/ui/hud_panels_folder": "",
	
	
	"godot_utilities/inventory/items_folder": "",
	"godot_utilities/inventory/player_inventory_path": "",
	"godot_utilities/inventory/hotbar_inventory_path": "",
	"godot_utilities/inventory/loot_item_scene_path": "",
	"godot_utilities/inventory/item_preview_scene_path": "",
}

# Keys from before subcategory restructure — removed on plugin enable to avoid
# flat and nested keys coexisting under godot_utilities/ in Project Settings.
const LEGACY_SETTINGS: Array[String] = [
	"godot_utilities/inventory_items_folder",
	"godot_utilities/inventory_player_inventory_path",
	"godot_utilities/inventory_hotbar_inventory_path",
	"godot_utilities/inventory_loot_item_scene_path",
	"godot_utilities/inventory_item_preview_scene_path",
	"godot_utilities/sfx_folder",
	"godot_utilities/music_folder",
	"godot_utilities/ambience_folder",
	"godot_utilities/particles_folder",
	"godot_utilities/ui_panels_folder",
	"godot_utilities/ui_hud_panels_folder",
	"godot_utilities/audio_sfx_pool_trim_cooldown",
]

const ID_GENERATOR = preload("res://addons/GodotUtilities/generator/id_generator.gd")

var _menu: PopupMenu

func _enter_tree() -> void:
	_migrate_legacy_settings()
	_add_settings()
	ProjectSettings.save()

	_menu = PopupMenu.new()
	_menu.add_item("Generate IDs", 0)
	_menu.add_item("Add Autoloads", 1)
	_menu.add_separator()
	_menu.add_item("Remove Settings", 2)
	_menu.id_pressed.connect(_on_menu_item_pressed)

	add_tool_submenu_item("GodotUtilities", _menu)

func _exit_tree() -> void:
	remove_tool_menu_item("GodotUtilities")
	ProjectSettings.save()
	print("GodotUtilities: plugin disabled. Generated ID files remain intact.")

func _on_menu_item_pressed(id: int) -> void:
	match id:
		0: ID_GENERATOR.generate_all()
		1: _add_autoload_singletons()
		2: _remove_settings()

func _on_generate_all() -> void:
	ID_GENERATOR.generate_all()

func _migrate_legacy_settings() -> void:
	var migrated := false
	for key in LEGACY_SETTINGS:
		if ProjectSettings.has_setting(key):
			ProjectSettings.set_setting(key, null)
			migrated = true
	if migrated:
		print("GodotUtilities: migrated legacy settings to subcategories.")

func _add_settings() -> void:
	for setting: String in SETTINGS.keys():
		var value = SETTINGS[setting]
		var type := typeof(value)
		
		if not ProjectSettings.has_setting(setting):
			ProjectSettings.set_setting(setting, value)
		
		var hint := PROPERTY_HINT_NONE
		var hint_string := ""
		if setting.ends_with("_folder"):
			hint = PROPERTY_HINT_DIR
		elif setting.ends_with("_path"):
			hint = PROPERTY_HINT_FILE
			hint_string = "*.tres"

		ProjectSettings.add_property_info({
			"name": setting,
			"type": type,
			"hint": hint,
			"hint_string": hint_string,
		})

func _remove_settings() -> void:
	for setting: String in SETTINGS.keys():
		if ProjectSettings.has_setting(setting):
			ProjectSettings.set_setting(setting, null)
	ProjectSettings.save()

func _add_autoload_singletons() -> void:
	for name: String in AUTOLOADS.keys():
		if not ProjectSettings.has_setting("autoload/" + name):
			add_autoload_singleton(name, AUTOLOADS[name])
