using FluentAssertions;
using ibricks_mqtt_broker.Model;
using ibricks_mqtt_broker.Services;
using ibricks_mqtt_broker.Services.Cello.FromCello;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace ibricks_mqtt_broker_tests;

public class IbricksMessageInterpretorTests
{
    [Fact]
    public void Test_Director_SICHG()
    {
        // Arrange
        var interpretor = new IbricksMessageInterpretor(NullLogger<IbricksMessageInterpretor>.Instance);
        var messageString = ".KISS|AF=F4CFA2DB6626|AT=0000000CLOUD|N=698|E|SICHG|T=TEMP|CH=1|U=CEL|V=21.51";

        // Act
        var message = interpretor.Interpret(messageString);

        // Assert
        message.Protocol.Should().Be(".KISS");
        message.AddressFrom.Should().Be("F4CFA2DB6626");
        message.AddressTo.Should().Be("0000000CLOUD");
        message.Nonce.Should().Be("698");
        message.Type.Should().Be(IbricksMessageType.E);
        message.Command.Should().Be(IbricksMessageCommands.SICHG);
        message.Channel.Should().Be(1);

        message.AdditionalData.Should().NotBeNull().And.HaveCount(3);
        message.AdditionalData!["T"].Should().Be("TEMP");
        message.AdditionalData!["U"].Should().Be("CEL");
        message.AdditionalData!["V"].Should().Be("21.51");
    }

    [Fact]
    public void Test_Director_SSGES()
    {
        // Arrange
        var interpretor = new IbricksMessageInterpretor(NullLogger<IbricksMessageInterpretor>.Instance);
        var messageString =
            ".KISS|AF=8CAAB5FA2BB5|AT=0000000CLOUD|N=509|E|SSGES|ST=ClickRight;Click|DEBUG=S2,12704,3134632466[3]0,192,34/27/86/118/122/126/126/121/97[4]48,96,11/13/13/11/11";

        // Act
        var message = interpretor.Interpret(messageString);

        // Assert
        message.Protocol.Should().Be(".KISS");
        message.AddressFrom.Should().Be("8CAAB5FA2BB5");
        message.AddressTo.Should().Be("0000000CLOUD");
        message.Nonce.Should().Be("509");
        message.Type.Should().Be(IbricksMessageType.E);
        message.Command.Should().Be(IbricksMessageCommands.SSGES);
        message.Channel.Should().Be(-1);

        message.AdditionalData.Should().NotBeNull().And.HaveCount(2);
        message.AdditionalData!["ST"].Should().Be("ClickRight;Click");
    }

    [Fact]
    public void Test_Director_BDCHG()
    {
        // Arrange
        var interpretor = new IbricksMessageInterpretor(NullLogger<IbricksMessageInterpretor>.Instance);
        var messageString = ".KISS|AF=8CAAB5FA2BB5|AT=0000000CLOUD|N=7|E|BDCHG|CH=1|U=CEL|V=18.00";

        // Act
        var message = interpretor.Interpret(messageString);

        // Assert
        message.Protocol.Should().Be(".KISS");
        message.AddressFrom.Should().Be("8CAAB5FA2BB5");
        message.AddressTo.Should().Be("0000000CLOUD");
        message.Nonce.Should().Be("7");
        message.Type.Should().Be(IbricksMessageType.E);
        message.Command.Should().Be(IbricksMessageCommands.BDCHG);
        message.Channel.Should().Be(1);

        message.AdditionalData.Should().NotBeNull().And.HaveCount(2);
        message.AdditionalData!["U"].Should().Be("CEL");
        message.AdditionalData!["V"].Should().Be("18.00");
    }

    [Fact]
    public void Test_Shutter_ASCHG()
    {
        // Arrange
        var interpretor = new IbricksMessageInterpretor(NullLogger<IbricksMessageInterpretor>.Instance);
        var messageString = ".KISS|AF=8CAAB5FA2BB5|AT=0000000CLOUD|N=3108|E|ASCHG|CH=1|CMD=HL|H=0.826|L=0.000";

        // Act
        var message = interpretor.Interpret(messageString);

        // Assert
        message.Protocol.Should().Be(".KISS");
        message.AddressFrom.Should().Be("8CAAB5FA2BB5");
        message.AddressTo.Should().Be("0000000CLOUD");
        message.Nonce.Should().Be("3108");
        message.Type.Should().Be(IbricksMessageType.E);
        message.Command.Should().Be(IbricksMessageCommands.ASCHG);
        message.Channel.Should().Be(1);

        message.AdditionalData.Should().NotBeNull().And.HaveCount(3);
        message.AdditionalData!["CMD"].Should().Be("HL");
        message.AdditionalData!["H"].Should().Be("0.826");
        message.AdditionalData!["L"].Should().Be("0.000");
    }

    [Fact]
    public void Test_Relay_LDCHG()
    {
        // Arrange
        var interpretor = new IbricksMessageInterpretor(NullLogger<IbricksMessageInterpretor>.Instance);
        var messageString = ".KISS|AF=8CAAB5FAABBE|AT=0000000CLOUD|N=727|E|LDCHG|CH=1|V=0.000";

        // Act
        var message = interpretor.Interpret(messageString);

        // Assert
        message.Protocol.Should().Be(".KISS");
        message.AddressFrom.Should().Be("8CAAB5FAABBE");
        message.AddressTo.Should().Be("0000000CLOUD");
        message.Nonce.Should().Be("727");
        message.Type.Should().Be(IbricksMessageType.E);
        message.Command.Should().Be(IbricksMessageCommands.LDCHG);
        message.Channel.Should().Be(1);

        message.AdditionalData.Should().NotBeNull().And.HaveCount(1);
        message.AdditionalData!["V"].Should().Be("0.000");
    }

    [Fact]
    public void Test_Relay_LRCHG()
    {
        // Arrange
        var interpretor = new IbricksMessageInterpretor(NullLogger<IbricksMessageInterpretor>.Instance);
        var messageString = ".KISS|AF=8CAAB5FA2BB5|AT=0000000CLOUD|N=474|E|LRCHG|CH=1|ST=0";

        // Act
        var message = interpretor.Interpret(messageString);

        // Assert
        message.Protocol.Should().Be(".KISS");
        message.AddressFrom.Should().Be("8CAAB5FA2BB5");
        message.AddressTo.Should().Be("0000000CLOUD");
        message.Nonce.Should().Be("474");
        message.Type.Should().Be(IbricksMessageType.E);
        message.Command.Should().Be(IbricksMessageCommands.LRCHG);
        message.Channel.Should().Be(1);

        message.AdditionalData.Should().NotBeNull().And.HaveCount(1);
        message.AdditionalData!["ST"].Should().Be("0");
    }

    [Fact]
    public void Test_IAMMASTER()
    {
        // Arrange
        var interpretor = new IbricksMessageInterpretor(NullLogger<IbricksMessageInterpretor>.Instance);
        var messageString = ".KISS|AF=8CAAB5FA2BB5|AT=0000000CLOUD|N=453|E|YHELO|IP=192.168.3.250|DESC=Buero+%2D+T1";

        // Act
        var message = interpretor.Interpret(messageString);

        // Assert
        message.Protocol.Should().Be(".KISS");
        message.AddressFrom.Should().Be("8CAAB5FA2BB5");
        message.AddressTo.Should().Be("0000000CLOUD");
        message.Nonce.Should().Be("453");
        message.Type.Should().Be(IbricksMessageType.E);
        message.Command.Should().Be(IbricksMessageCommands.YHELO);
        message.Channel.Should().Be(-1);

        message.AdditionalData.Should().NotBeNull().And.HaveCount(2);
        message.AdditionalData!["IP"].Should().Be("192.168.3.250");
        message.AdditionalData!["DESC"].Should().Be("Buero - T1");
    }

    [Fact]
    public void Test_Debug_Info()
    {
        // Arrange
        var interpretor = new IbricksMessageInterpretor(NullLogger<IbricksMessageInterpretor>.Instance);
        var messageString =
            ".KISS|AF=8CAAB5FA2BB5|AT=0000000CLOUD|N=460|E|YINFO|T=DebugInfo|V=Hardware=1R1S1H/1803;Firmware=2.2.44.PROD...";

        // Act
        var message = interpretor.Interpret(messageString);

        // Assert
        message.Protocol.Should().Be(".KISS");
        message.AddressFrom.Should().Be("8CAAB5FA2BB5");
        message.AddressTo.Should().Be("0000000CLOUD");
        message.Nonce.Should().Be("460");
        message.Type.Should().Be(IbricksMessageType.E);
        message.Command.Should().Be(IbricksMessageCommands.YINFO);
        message.Channel.Should().Be(-1);

        message.AdditionalData.Should().NotBeNull().And.HaveCount(2);
        message.AdditionalData!["T"].Should().Be("DebugInfo");
        message.AdditionalData!["V"].Should().Be("Hardware=1R1S1H/1803;Firmware=2.2.44.PROD...");
    }
}