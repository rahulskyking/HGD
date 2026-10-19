using HireGameDev.Domain.Entities;

namespace HireGameDev.Infrastructure.Persistence;

/// <summary>
/// The skills master list (spec §17: 8 categories, 161 skills) that <see cref="SkillSeeder"/>
/// keeps in the database. Edit this list to add a skill or move one to another category;
/// never rename an entry here — a renamed name seeds as a new skill and strands the old one
/// (renames/merges are an admin feature, Step 12). Category names come from
/// <see cref="SkillCategory"/> so code that singles one out spells it the same way.
/// </summary>
public static class SkillCatalog
{
    public static IReadOnlyList<(string Name, string Category)> Entries { get; } =
    [
        .. In(SkillCategory.Engines,
            "Unity", "Unreal Engine 5", "Unreal Engine 4", "Godot", "Cocos2d-x", "Cocos Creator",
            "CryEngine", "GameMaker", "Construct 3", "Phaser", "PlayCanvas", "Defold",
            "Roblox Studio", "RPG Maker", "Source 2", "Custom / In-house Engine"),

        .. In(SkillCategory.Languages,
            "C#", "C++", "C", "Python", "Java", "Kotlin", "Swift", "Objective-C", "JavaScript",
            "TypeScript", "Lua", "Rust", "Go", "HLSL", "GLSL", "GDScript", "Haxe", "SQL"),

        .. In(SkillCategory.ArtTools,
            "Blender", "Autodesk Maya", "3ds Max", "ZBrush", "Substance 3D Painter",
            "Substance 3D Designer", "Adobe Photoshop", "Adobe Illustrator", "Adobe After Effects",
            "Houdini", "Spine 2D", "Marmoset Toolbag", "Marvelous Designer", "Quixel Megascans",
            "SpeedTree", "Krita", "Aseprite", "Procreate"),

        .. In(SkillCategory.DesignAndAudioTools,
            "FMOD", "Wwise", "Unreal Blueprints", "Twine", "Ink", "Articy:draft", "Machinations",
            "Figma", "Miro", "Jira", "Confluence", "Trello", "Notion", "Reaper", "Ableton Live",
            "Pro Tools", "Audacity"),

        .. In(SkillCategory.DevOpsAndInfrastructure,
            "Git", "Perforce", "Plastic SCM", "Jenkins", "GitHub Actions", "Docker", "AWS",
            "Google Cloud", "Microsoft Azure", "Firebase", "PlayFab", "Photon", "Mirror", "Nakama",
            "Unity Gaming Services", "Steamworks"),

        .. In(SkillCategory.Genres,
            "Mobile Casual", "Hyper-Casual", "Puzzle", "Match-3", "Idle & Clicker", "RPG",
            "Action RPG", "MMORPG", "FPS", "Third-Person Shooter", "Battle Royale", "MOBA",
            "Roguelike", "Platformer", "Real-Time Strategy", "Turn-Based Strategy", "Simulation",
            "Racing", "Sports", "Fighting", "Horror", "Survival", "Open World",
            "Narrative Adventure", "Visual Novel", "Casino", "Card & Board Games",
            "EdTech & Serious Games"),

        .. In(SkillCategory.Platforms,
            "Android", "iOS", "PC / Steam", "macOS", "Linux", "PlayStation 5", "PlayStation 4",
            "Xbox Series X|S", "Xbox One", "Nintendo Switch", "WebGL / HTML5", "Meta Quest",
            "PlayStation VR2"),

        .. In(SkillCategory.Roles,
            "Gameplay Programming", "Engine Programming", "Graphics & Rendering Programming",
            "Tools Programming", "Network & Multiplayer Programming", "AI Programming",
            "Physics Programming", "UI Programming", "Audio Programming",
            "Backend & LiveOps Engineering", "Mobile Game Development", "Technical Art", "VFX",
            "2D Art", "3D Modelling", "Character Art", "Environment Art", "Concept Art",
            "Animation", "Rigging", "UI/UX Design", "Game Design", "Level Design",
            "Narrative Design", "Systems Design", "Economy & Monetization Design", "Sound Design",
            "Music Composition", "QA Testing", "QA Automation", "Production & Project Management",
            "Product Management", "Data Analytics", "Community Management", "Localization"),
    ];

    private static IEnumerable<(string Name, string Category)> In(string category, params string[] names) =>
        names.Select(name => (name, category));
}
