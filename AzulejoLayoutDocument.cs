using Newtonsoft.Json;

public class AzulejoLayoutDocument
{
    [JsonProperty("azulejo")]
    public AzulejoInfo Azulejo { get; set; }

    [JsonProperty("payload")]
    public AzulejoPayload Payload { get; set; }
}

public class AzulejoInfo
{
    [JsonProperty("version")]
    public string Version { get; set; }
}

public class AzulejoPayload
{
    [JsonProperty("playground")]
    public PlaygroundInfo Playground { get; set; }

    [JsonProperty("reference")]
    public ReferenceInfo Reference { get; set; }

    [JsonProperty("components")]
    public List<ComponentInfo> Components { get; set; }

    [JsonProperty("transforms")]
    public List<ComponentTransform> Transforms { get; set; }

    [JsonProperty("links")]
    public List<ComponentLink> Links { get; set; }
}

public class PlaygroundInfo
{
    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("version")]
    public string Version { get; set; }

    [JsonProperty("description")]
    public string Description { get; set; }
}

public class ReferenceInfo
{
    [JsonProperty("systemResolution")]
    public IntSize2D SystemResolution { get; set; }

    [JsonProperty("gameResolution")]
    public DoubleSize2D GameResolution { get; set; }

    [JsonProperty("viewportScale")]
    public double ViewportScale { get; set; }

    [JsonProperty("canvasPadding")]
    public CanvasPadding CanvasPadding { get; set; }

    [JsonProperty("gridSize")]
    public IntSize2D GridSize { get; set; }

    [JsonProperty("usingGrid")]
    public bool UsingGrid { get; set; }

    [JsonProperty("usingAutoAlign")]
    public bool UsingAutoAlign { get; set; }

    [JsonProperty("magneticThreshold")]
    public int MagneticThreshold { get; set; }

    [JsonProperty("adjacencyThreshold")]
    public int AdjacencyThreshold { get; set; }

    [JsonProperty("ApplicationScale")]
    public int ApplicationScale { get; set; }

    [JsonProperty("ConsoleApplicationScale")]
    public int ConsoleApplicationScale { get; set; }
}

public class DoubleSize2D
{
    [JsonProperty("width")]
    public double Width { get; set; }

    [JsonProperty("height")]
    public double Height { get; set; }
}

public class IntSize2D
{
    [JsonProperty("width")]
    public int Width { get; set; }

    [JsonProperty("height")]
    public int Height { get; set; }
}

public class CanvasPadding
{
    [JsonProperty("left")]
    public int Left { get; set; }

    [JsonProperty("right")]
    public int Right { get; set; }

    [JsonProperty("top")]
    public int Top { get; set; }

    [JsonProperty("bottom")]
    public int Bottom { get; set; }
}

public class ComponentInfo
{
    [JsonProperty("key")]
    public int Key { get; set; }

    [JsonProperty("version")]
    public string Version { get; set; }

    [JsonProperty("description")]
    public string Description { get; set; }

    [JsonProperty("support")]
    public ComponentSupport Support { get; set; }
}

public class ComponentSupport
{
    [JsonProperty("translation")]
    public BoolAxis3 Translation { get; set; }

    [JsonProperty("scaling")]
    public BoolAxis3 Scaling { get; set; }

    [JsonProperty("zOrdering")]
    public bool ZOrdering { get; set; }

    [JsonProperty("linking")]
    public bool Linking { get; set; }
}

public class BoolAxis3
{
    [JsonProperty("x")]
    public bool X { get; set; }

    [JsonProperty("y")]
    public bool Y { get; set; }

    [JsonProperty("z")]
    public bool Z { get; set; }
}

public class ComponentTransform
{
    [JsonProperty("componentKey")]
    public int ComponentKey { get; set; }

    [JsonProperty("desiredSize")]
    public IntSize2D DesiredSize { get; set; }

    [JsonProperty("align")]
    public string Align { get; set; }

    [JsonProperty("translate")]
    public Vector3D Translate { get; set; }

    [JsonProperty("scale")]
    public Vector3D Scale { get; set; }

    [JsonProperty("zOrder")]
    public int ZOrder { get; set; }
}

public class Vector3D
{
    [JsonProperty("x")]
    public double X { get; set; }

    [JsonProperty("y")]
    public double Y { get; set; }

    [JsonProperty("z")]
    public double Z { get; set; }
}

public sealed class ComponentLink
{
    [JsonProperty("componentKey")]
    public int ComponentKey { get; set; }

    [JsonProperty("linkWith")]
    public List<int> LinkWith { get; set; } = new();
}