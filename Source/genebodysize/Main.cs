using System.Reflection;
using HarmonyLib;
using RimWorld;
using VariedBodySizes;
using Verse;

namespace genebodysize;
[StaticConstructorOnStartup]
public static class Main
{
    public static GeneSizeGameComponent Component;
    
    static Main()
    {
        new Harmony("mute.genebodysize").PatchAll(Assembly.GetExecutingAssembly());
    }
}