using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using VRC.Udon;
using yajusense.Core;
using yajusense.UI;
using yajusense.Utils;
using Object = UnityEngine.Object;

namespace yajusense.Modules.Visual;

public class UdonInspector : BaseModule
{
    private readonly string _saveDir = Path.Combine(Directory.GetCurrentDirectory(), "yajusense", "UdonInspector");
    private readonly Dictionary<UdonBehaviour, string> _udonCache = new();
    private readonly Window _window;
    private Vector2 _scrollPos;
    private KeyValuePair<UdonBehaviour, string> _selectedUdon;

    public UdonInspector() : base("UdonInspector", "Disassemble and analyze Udon Behaviours", ModuleCategory.Visual,
        KeyCode.F10)
    {
        _window = new Window(new Rect(0, 0, 600f, 500f), "Udon Inspector");
    }

    public override void OnEnable()
    {
        try
        {
            RefreshUdonCache();
        }
        catch (Exception ex)
        {
            YjPlugin.Log.LogError($"[UdonInspector] Initialization failed: {ex}");
        }
    }

    public override void OnDisable()
    {
        _udonCache.Clear();
        _selectedUdon = default;
    }

    public override void OnGUI()
    {
        if (!VRCUtils.IsInWorld()) return;
        _window.Begin();
        {
            DrawControls();

            _scrollPos = GUILayout.BeginScrollView(_scrollPos);
            {
                DrawUdonList();
            }
            GUILayout.EndScrollView();

            DrawDisassemblyControls();
        }
        _window.End();
    }

    private void DrawControls()
    {
        if (GUILayout.Button("Refresh Udon Cache")) RefreshUdonCache();

        if (GUILayout.Button("Disassemble All")) DisassembleAll();

        if (GUILayout.Button("Dump All EventTable Func")) DumpAllEventTableFunctions();
    }

    private void DrawUdonList()
    {
        try
        {
            GUILayout.Label($"Found UdonBehaviours ({_udonCache.Count})");

            GUILayout.Space(10);
            GUILayout.BeginVertical();
            {
                foreach (var kv in _udonCache)
                {
                    var udonBehaviour = kv.Key;
                    var ubName = kv.Value;
                    var selectedUdonBehaviour = _selectedUdon.Key;

                    var isSelected = selectedUdonBehaviour == udonBehaviour;
                    var btnStyle = isSelected ? GUI.skin.button : GUI.skin.box;

                    if (GUILayout.Button(ubName, btnStyle))
                        _selectedUdon = new KeyValuePair<UdonBehaviour, string>(udonBehaviour, ubName);
                }
            }
            GUILayout.EndVertical();
        }
        catch (Exception ex)
        {
            YjPlugin.Log.LogError($"[UdonInspector] Udon list rendering failed: {ex}");
        }
    }

    private void DrawDisassemblyControls()
    {
        if (_selectedUdon.Key == null) return;

        GUILayout.Space(15);
        if (GUILayout.Button($"Disassemble {_selectedUdon.Key}"))
            UdonDisassembler.Disassemble(_selectedUdon.Key, _selectedUdon.Value);
    }

    private void DisassembleAll()
    {
        RefreshUdonCache();
        foreach (var kv in _udonCache) UdonDisassembler.Disassemble(kv.Key, kv.Value);
    }

    private void DumpAllEventTableFunctions()
    {
        foreach (var kv in _udonCache) DumpEventTableFunctions(kv.Key, kv.Value);
    }

    private void DumpEventTableFunctions(UdonBehaviour udonBehaviour, string udonName)
    {
        try
        {
            if (udonBehaviour._eventTable == null)
            {
                YjPlugin.Log.LogWarning("Selected UdonBehaviour has no event table");
                return;
            }

            var output = new StringBuilder();
            output.AppendLine($"Event Table Dump for: {_selectedUdon.Value}\n");

            uint index = 0;
            foreach (var entry in udonBehaviour._eventTable)
            {
                output.AppendLine($"Event: {entry.key}");
                output.AppendLine();
                index++;
            }

            var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            FileUtils.EnsureDirectoryExists(_saveDir);
            var savePath = Path.Combine(_saveDir, $"EventTable_{udonName}_{timestamp}.txt");
            File.WriteAllText(savePath, output.ToString());
            YjPlugin.Log.LogInfo($"Saved event table dump to: {savePath}");
        }
        catch (Exception ex)
        {
            YjPlugin.Log.LogError($"Event table dump failed: {ex}");
        }
    }

    private void RefreshUdonCache()
    {
        if (!VRCUtils.IsInWorld()) return;

        _udonCache.Clear();
        var allObjs = Object.FindObjectsOfType<GameObject>();
        foreach (var go in allObjs)
            if (go.TryGetComponent<UdonBehaviour>(out var ub))
                _udonCache.Add(ub, go.name);

        YjPlugin.Log.LogInfo($"[UdonInspector] {_udonCache.Count} UdonBehaviours found");
    }
}