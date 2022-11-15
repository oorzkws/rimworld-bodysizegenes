using System;
using RimWorld;
using VariedBodySizes;
using Verse;

namespace genebodysize;

public class GeneSizeGameComponent : GameComponent
{
    public GeneSizeGameComponent(Game game)
    {
        Main.Component = this;
    }
}