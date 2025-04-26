using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using VRC.Udon;
using VRC.Udon.Common.Interfaces;
using VRC.Udon.VM;
using VRC.Udon.VM.Common;

namespace yajusense.Utils;

public static class UdonDisassembler
{
    private static readonly string
        SaveDir = Path.Combine(Directory.GetCurrentDirectory(), "yajusense/UdonDisassembler");

    public static void Disassemble(UdonBehaviour udonBehaviour, string ubName)
    {
        Disassemble(udonBehaviour, ubName, null);
    }

    public static void Disassemble(UdonBehaviour udonBehaviour, string ubName, IEnumerable<string> targetEventNames)
    {
        if (udonBehaviour == null)
        {
            Plugin.Log.LogError("UdonBehaviour is null");
            return;
        }

        try
        {
            DisassemblyContext context = CreateDisassemblyContext(udonBehaviour, ubName, targetEventNames);
            if (!context.Initialize()) return;

            ProcessBytecode(context);
            SaveDisassemblyOutput(context);
        }
        catch (Exception ex)
        {
            Plugin.Log.LogError($"Error in UdonDisassembler: {ex}");
        }
    }

    private static DisassemblyContext CreateDisassemblyContext(UdonBehaviour udonBehaviour, string ubName, IEnumerable<string> targetEventNames)
    {
        return new DisassemblyContext
        {
            UdonBehaviour = udonBehaviour,
            UbName = ubName,
            TargetEventNames = targetEventNames,
            Output = new StringBuilder(),
            Constants = new Dictionary<uint, string>()
        };
    }

    private static void ProcessBytecode(DisassemblyContext context)
    {
        IUdonProgram udonProgram = context.UdonBehaviour._program;
        Il2CppStructArray<byte> byteCode = udonProgram.ByteCode;
        IUdonSymbolTable symbolTable = udonProgram.SymbolTable;
        IUdonHeap heap = udonProgram.Heap;

        EventInfo eventInfo = PrepareEventInformation(context);

        var address = 0;
        bool isProcessingTargetEvent = !eventInfo.FilterEvents;
        var currentEventEndAddress = (uint)byteCode.Length;

        while (address < byteCode.Length)
            try
            {
                if (eventInfo.EventAddresses.Contains((uint)address))
                {
                    string eventName = eventInfo.AddressToEventName[(uint)address];

                    if (eventInfo.FilterEvents)
                    {
                        isProcessingTargetEvent = true;
                        currentEventEndAddress = eventInfo.EventBounds[(uint)address];
                    }

                    context.Output.AppendLine(".end_of_func");
                    context.Output.AppendLine($".func_{eventName}");
                }

                if (!isProcessingTargetEvent)
                {
                    address += 4;
                    continue;
                }

                if (eventInfo.FilterEvents && address >= currentEventEndAddress)
                {
                    isProcessingTargetEvent = false;
                    continue;
                }

                if (address + 4 > byteCode.Length)
                {
                    Plugin.Log.LogWarning($"Reached end of bytecode prematurely at 0x{address:X}.");
                    break;
                }

                uint opcode = SwapEndianness(BitConverter.ToUInt32(byteCode, address));
                address = ProcessOpcode(opcode, address, byteCode, heap, symbolTable, context);
            }
            catch (Exception ex)
            {
                Plugin.Log.LogError($"Failed to process bytecode at 0x{address:X}: {ex.Message}");
                break;
            }

        context.Output.AppendLine(".end");
    }


    private static EventInfo PrepareEventInformation(DisassemblyContext context)
    {
        var result = new EventInfo
        {
            EventAddresses = new HashSet<uint>(),
            AddressToEventName = new Dictionary<uint, string>(),
            FilterEvents = context.TargetEventNames != null && context.TargetEventNames.Any(),
            EventBounds = new Dictionary<uint, uint>()
        };

        HashSet<string> eventNameFilter = result.FilterEvents
            ? new HashSet<string>(context.TargetEventNames, StringComparer.OrdinalIgnoreCase)
            : null;

        Il2CppSystem.Collections.Generic.Dictionary<string, Il2CppSystem.Collections.Generic.List<uint>> eventTable = context.UdonBehaviour._eventTable;
        if (eventTable != null)
            foreach (Il2CppSystem.Collections.Generic.KeyValuePair<string, Il2CppSystem.Collections.Generic.List<uint>> entry in eventTable)
            {
                if (result.FilterEvents && !eventNameFilter.Contains(entry.key))
                    continue;

                foreach (uint addr in entry.Value)
                {
                    result.EventAddresses.Add(addr);
                    result.AddressToEventName[addr] = entry.key;
                }
            }

        if (result.FilterEvents && result.EventAddresses.Count == 0)
        {
            Plugin.Log.LogWarning($"No matching events found for the specified filter in {context.UbName}");
            return result;
        }

        if (result.FilterEvents)
        {
            List<uint> sortedAddresses = result.EventAddresses.OrderBy(a => a).ToList();
            for (var i = 0; i < sortedAddresses.Count; i++)
            {
                uint startAddr = sortedAddresses[i];
                uint endAddr = i < sortedAddresses.Count - 1
                    ? sortedAddresses[i + 1]
                    : (uint)context.UdonBehaviour._program.ByteCode.Length;

                result.EventBounds[startAddr] = endAddr;
            }
        }

        return result;
    }


    private static int ProcessOpcode(uint opcode, int address, byte[] byteCode, IUdonHeap heap, IUdonSymbolTable symbolTable, DisassemblyContext context)
    {
        switch (opcode)
        {
            case (uint)OpCode.NOP:
                Plugin.Log.LogInfo("Resolving NOP");
                context.Output.AppendLine($"0x{address:X}  NOP");
                address += 4;
                break;

            case (uint)OpCode.PUSH:
                if (address + 8 > byteCode.Length) break;

                Plugin.Log.LogInfo("Resolving PUSH");

                uint pushOffset = SwapEndianness(BitConverter.ToUInt32(byteCode, address + 4));

                string symbolName = symbolTable.GetSymbolFromAddress(pushOffset) ?? $"0x{pushOffset:X}";
                object heapValue = heap.GetHeapVariable(pushOffset);
                string symbolType = heapValue?.GetType().FullName ?? "Unknown";

                if (heapValue != null && symbolName.Contains("_const_"))
                {
                    Type runtimeType = heapValue.GetType();

                    if (runtimeType == typeof(bool) ||
                        runtimeType == typeof(int) ||
                        runtimeType == typeof(float) ||
                        runtimeType == typeof(double))
                        context.Constants[pushOffset] = heapValue.ToString();
                    else if (runtimeType == typeof(string)) context.Constants[pushOffset] = $"\"{heapValue}\"";
                }

                context.Output.AppendLine($"0x{address:X}  PUSH 0x{pushOffset:X} ({symbolName}[{symbolType}])");
                address += 8;
                break;

            case (uint)OpCode.POP:
                Plugin.Log.LogInfo("Resolving POP");
                context.Output.AppendLine($"0x{address:X}  POP");
                address += 4;
                break;

            case (uint)OpCode.JUMP_IF_FALSE:
                if (address + 8 > byteCode.Length) break;

                Plugin.Log.LogInfo("Resolving JUMP_IF_FALSE");
                uint jneOffset = SwapEndianness(BitConverter.ToUInt32(byteCode, address + 4));
                context.Output.AppendLine($"0x{address:X}  JUMP_IF_FALSE 0x{jneOffset:X}");
                address += 8;
                break;

            case (uint)OpCode.JUMP:
                if (address + 8 > byteCode.Length) break;

                Plugin.Log.LogInfo("Resolving JMP");

                uint jumpOffset = SwapEndianness(BitConverter.ToUInt32(byteCode, address + 4));
                context.Output.AppendLine($"0x{address:X}  JMP 0x{jumpOffset:X}");
                address += 8;
                break;

            case (uint)OpCode.EXTERN:
                if (address + 8 > byteCode.Length) break;

                Plugin.Log.LogInfo("Resolving EXTERN");

                uint externAddr = SwapEndianness(BitConverter.ToUInt32(byteCode, address + 4));
                object externObj = heap.GetHeapVariable(externAddr);
                var externName = $"unknown_extern@{externAddr:X}";

                if (externObj != null)
                {
                    if (externObj is UdonVM.CachedUdonExternDelegate cachedDelegate)
                        externName = cachedDelegate.externSignature ?? externName;
                    else
                        externName = externObj.ToString();
                }

                context.Output.AppendLine($"0x{address:X}  EXTERN \"{externName}\"");
                address += 8;
                break;

            case (uint)OpCode.ANNOTATION:
                if (address + 8 > byteCode.Length) break;

                Plugin.Log.LogInfo("Resolving ANNOTATION");

                uint annotationAddr = SwapEndianness(BitConverter.ToUInt32(byteCode, address + 4));
                object annotationObj = heap.GetHeapVariable(annotationAddr);
                string annotationText = annotationObj?.ToString() ?? $"unknown_annotation@{annotationAddr:X}";

                context.Output.AppendLine($"0x{address:X}  ANNOTATION \"{annotationText}\"");
                address += 8;
                break;

            case (uint)OpCode.JUMP_INDIRECT:
                if (address + 8 > byteCode.Length) break;

                Plugin.Log.LogInfo("Resolving JMP_INDIRECT");

                uint indirectAddr = SwapEndianness(BitConverter.ToUInt32(byteCode, address + 4));
                string targetSymbol = symbolTable.HasSymbolForAddress(indirectAddr)
                    ? symbolTable.GetSymbolFromAddress(indirectAddr)
                    : $"0x{indirectAddr:X}";

                context.Output.AppendLine($"0x{address:X}  JMP [{targetSymbol}]");
                address += 8;
                break;

            case (uint)OpCode.COPY:
                Plugin.Log.LogInfo("Resolving COPY");
                context.Output.AppendLine($"0x{address:X}  COPY");
                address += 4;
                break;

            default:
                Plugin.Log.LogWarning($"Unknown opcode: {opcode} at address 0x{address:X}");
                address += 4;
                break;
        }

        return address;
    }


    private static void SaveDisassemblyOutput(DisassemblyContext context)
    {
        FileUtils.EnsureDirectoryExists(SaveDir);

        var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
        bool filtered = context.TargetEventNames != null && context.TargetEventNames.Any();
        string filenameSuffix = filtered ? "_filtered" : "";
        string outputPath = Path.Combine(SaveDir, $"Disassembled_{context.UbName}{filenameSuffix}_{timestamp}.txt");

        Plugin.Log.LogInfo("Saving to file...");
        File.WriteAllText(outputPath, context.Output.ToString());

        if (context.Constants.Count > 0) SaveConstants(context, SaveDir, timestamp, filenameSuffix);

        string eventInfo = filtered
            ? $" (filtered to {context.EventAddresses.Count} events)"
            : "";
        Plugin.Log.LogInfo($"Disassembly completed for {context.UbName}{eventInfo}");
    }


    private static void SaveConstants(DisassemblyContext context, string saveDir, string timestamp, string filenameSuffix)
    {
        var constantsOutput = new StringBuilder(context.Constants.Count * 20);
        foreach (KeyValuePair<uint, string> kv in context.Constants) constantsOutput.AppendLine($"0x{kv.Key:X}: {kv.Value}");

        string constantsPath = Path.Combine(saveDir, $"Constants_{context.UbName}{filenameSuffix}_{timestamp}.txt");
        File.WriteAllText(constantsPath, constantsOutput.ToString());
    }

    private static uint SwapEndianness(uint value)
    {
        return ((value & 0x000000FF) << 24) |
               ((value & 0x0000FF00) << 8) |
               ((value & 0x00FF0000) >> 8) |
               ((value & 0xFF000000) >> 24);
    }

    private class DisassemblyContext
    {
        public UdonBehaviour UdonBehaviour { get; set; }
        public string UbName { get; set; }
        public IEnumerable<string> TargetEventNames { get; set; }
        public StringBuilder Output { get; set; }
        public Dictionary<uint, string> Constants { get; set; }
        public HashSet<uint> EventAddresses { get; } = new();

        public bool Initialize()
        {
            if (UdonBehaviour == null || UdonBehaviour._program == null)
            {
                Plugin.Log.LogError($"Invalid UdonBehaviour or program for {UbName}");
                return false;
            }

            return true;
        }
    }


    private class EventInfo
    {
        public HashSet<uint> EventAddresses { get; set; }
        public Dictionary<uint, string> AddressToEventName { get; set; }
        public bool FilterEvents { get; set; }
        public Dictionary<uint, uint> EventBounds { get; set; }
    }
}