using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ibricks_mqtt_broker.Model.DeviceState;

namespace ibricks_mqtt_broker.Model;

[Table("Cellos")]
public class Cello
{
    [Required]
    public required string Ip { get; init; }
    
    [Required]
    public required string Description { get; init; }
    
    [Key]
    public required string Mac { get; init; }
    
    public IbricksHardwareInfo? HardwareInfo { get; set; }

    [Required]
    public Dictionary<int, RelayState> RelayStates { get; } = new();
    
    [Required]
    public Dictionary<int, DimmerState> DimmerStates { get; } = new();
    
    [Required]
    public Dictionary<int, MeteoState> MeteoStates { get; } = new();
    
    [Required]
    public Dictionary<int, ClimateState> ClimateStates { get; } = new();
    
    [Required]
    public Dictionary<int, CoverState> CoverStates { get; } = new();
}