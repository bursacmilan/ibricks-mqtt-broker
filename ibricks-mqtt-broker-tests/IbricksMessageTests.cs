using FluentAssertions;
using ibricks_mqtt_broker.Model;
using Xunit;

namespace ibricks_mqtt_broker_tests;

public class IbricksMessageTests
{
    private IbricksMessage _message = new()
    {
        Channel = 1,
        Command = IbricksMessageCommands.ASCHG,
        Nonce = "123",
        Protocol = "123",
        Type = IbricksMessageType.C,
        AddressFrom = "123",
        AddressTo = "123",
        AdditionalData = new Dictionary<string, string>
        {
            {
                "AF", "123"
            },
            {
                "X", "0.123"
            },
            {
                "CH", "VAL"
            },
            {
                "N", "true"
            }
        }
    };
    
    [Fact]
    public void Test_GetAdditionalOrDefault_Bool()
    {
        // Assert
        _message.GetAdditionalOrDefault<bool>(IbricksMessageParts.N).Should().BeTrue();
        _message.GetAdditionalOrDefault<bool?>(IbricksMessageParts.N).Should().BeTrue();

        _message.GetAdditionalOrDefault<bool>(IbricksMessageParts.V).Should().BeFalse();
        _message.GetAdditionalOrDefault<bool?>(IbricksMessageParts.V).Should().BeNull();
        
        ((Action) (() => _message.GetAdditionalOrDefault<bool>(IbricksMessageParts.CH))).Should().Throw<Exception>();
        ((Action) (() => _message.GetAdditionalOrDefault<bool?>(IbricksMessageParts.CH))).Should().Throw<Exception>();
    }
    
    [Fact]
    public void Test_GetAdditionalOrDefault_Int()
    {
        // Assert
        _message.GetAdditionalOrDefault<int>(IbricksMessageParts.AF).Should().Be(123);
        _message.GetAdditionalOrDefault<int?>(IbricksMessageParts.AF).Should().Be(123);

        _message.GetAdditionalOrDefault<int>(IbricksMessageParts.V).Should().Be(0);
        _message.GetAdditionalOrDefault<int?>(IbricksMessageParts.V).Should().BeNull();
        
        ((Action) (() => _message.GetAdditionalOrDefault<int>(IbricksMessageParts.CH))).Should().Throw<Exception>();
        ((Action) (() => _message.GetAdditionalOrDefault<int?>(IbricksMessageParts.CH))).Should().Throw<Exception>();
    }
    
    [Fact]
    public void Test_GetAdditionalOrDefault_Double()
    {
        // Assert
        _message.GetAdditionalOrDefault<double>(IbricksMessageParts.X).Should().Be(0.123);
        _message.GetAdditionalOrDefault<double?>(IbricksMessageParts.X).Should().Be(0.123);

        _message.GetAdditionalOrDefault<double>(IbricksMessageParts.V).Should().Be(0);
        _message.GetAdditionalOrDefault<double?>(IbricksMessageParts.V).Should().BeNull();
        
        ((Action) (() => _message.GetAdditionalOrDefault<double>(IbricksMessageParts.CH))).Should().Throw<Exception>();
        ((Action) (() => _message.GetAdditionalOrDefault<double?>(IbricksMessageParts.CH))).Should().Throw<Exception>();
    }
    
    [Fact]
    public void Test_GetAdditionalOrDefault_String()
    {
        // Assert
        _message.GetAdditionalOrDefault<string>(IbricksMessageParts.CH).Should().Be("VAL");
        _message.GetAdditionalOrDefault<string?>(IbricksMessageParts.CH).Should().Be("VAL");

        _message.GetAdditionalOrDefault<string>(IbricksMessageParts.V).Should().BeNull();
        _message.GetAdditionalOrDefault<string?>(IbricksMessageParts.V).Should().BeNull();
        
        _message.GetAdditionalOrDefault<string>(IbricksMessageParts.AF).Should().Be("123");
        _message.GetAdditional<string?>(IbricksMessageParts.AF).Should().Be("123");
    }
    
    [Fact]
    public void Test_GetAdditional_Bool()
    {
        // Assert
        _message.GetAdditional<bool>(IbricksMessageParts.N).Should().BeTrue();
        _message.GetAdditional<bool?>(IbricksMessageParts.N).Should().BeTrue();

        ((Action) (() => _message.GetAdditional<bool>(IbricksMessageParts.V))).Should().Throw<Exception>();
        ((Action) (() => _message.GetAdditional<bool?>(IbricksMessageParts.V))).Should().Throw<Exception>();
        
        ((Action) (() => _message.GetAdditional<bool>(IbricksMessageParts.CH))).Should().Throw<Exception>();
        ((Action) (() => _message.GetAdditional<bool?>(IbricksMessageParts.CH))).Should().Throw<Exception>();
    }
    
    [Fact]
    public void Test_GetAdditional_Int()
    {
        // Assert
        _message.GetAdditional<int>(IbricksMessageParts.AF).Should().Be(123);
        _message.GetAdditional<int?>(IbricksMessageParts.AF).Should().Be(123);

        ((Action) (() => _message.GetAdditional<int>(IbricksMessageParts.V))).Should().Throw<Exception>();
        ((Action) (() => _message.GetAdditional<int?>(IbricksMessageParts.V))).Should().Throw<Exception>();
        
        ((Action) (() => _message.GetAdditional<int>(IbricksMessageParts.CH))).Should().Throw<Exception>();
        ((Action) (() => _message.GetAdditional<int?>(IbricksMessageParts.CH))).Should().Throw<Exception>();
    }
    
    [Fact]
    public void Test_GetAdditional_Double()
    {
        // Assert
        _message.GetAdditional<double>(IbricksMessageParts.X).Should().Be(0.123);
        _message.GetAdditional<double?>(IbricksMessageParts.X).Should().Be(0.123);

        ((Action) (() => _message.GetAdditional<double>(IbricksMessageParts.V))).Should().Throw<Exception>();
        ((Action) (() => _message.GetAdditional<double?>(IbricksMessageParts.V))).Should().Throw<Exception>();
        
        ((Action) (() => _message.GetAdditional<double>(IbricksMessageParts.CH))).Should().Throw<Exception>();
        ((Action) (() => _message.GetAdditional<double?>(IbricksMessageParts.CH))).Should().Throw<Exception>();
    }
    
    [Fact]
    public void Test_GetAdditional_String()
    {
        // Assert
        _message.GetAdditional<string>(IbricksMessageParts.CH).Should().Be("VAL");
        _message.GetAdditional<string?>(IbricksMessageParts.CH).Should().Be("VAL");

        ((Action) (() => _message.GetAdditional<string>(IbricksMessageParts.V))).Should().Throw<Exception>();
        ((Action) (() => _message.GetAdditional<string?>(IbricksMessageParts.V))).Should().Throw<Exception>();
        
        _message.GetAdditional<string>(IbricksMessageParts.AF).Should().Be("123");
        _message.GetAdditional<string?>(IbricksMessageParts.AF).Should().Be("123");
    }
}