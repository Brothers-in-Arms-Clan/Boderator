using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ArmaforcesMissionBot.Exceptions;
using ArmaforcesMissionBot.Features;
using ArmaforcesMissionBot.Features.Signups.Importer;
using Discord.Commands;
using FluentAssertions;
using Moq;
using Xunit;

namespace ArmaforcesMissionBot.Tests.Features.Signups.Importer {
    public class SignupImporterTests {
        [Theory]
        [InlineData("Ddd\ndsd", "\n")]
        [InlineData("Ddd\r\ndsd", "\r\n")]
        [InlineData("Ddddsd", "\n")]
        public void GetLineEndings(string message, string expectedLinebreak) {
            var lineEnding = SignupImporter.GetLineEnding(message);

            lineEnding.Should().Be(expectedLinebreak);
        }

        [Theory]
        [InlineData("Ddd\ndsds\n", new[] {"Ddd\n", "dsds\n", "\n"})]
        [InlineData("Ddd\r\ndsds\r\n", new[] {"Ddd\r\n", "dsds\r\n", "\r\n"})]
        [InlineData("", new[] {"\n"})]
        public void ReadLines(string message, string[] expectedLines) {
            var linesRead = SignupImporter.ReadLines(message);

            linesRead.Should().BeEquivalentTo(expectedLines);
        }

        [Theory]
        [InlineData(new[] {"dsdsd", "AF!hsda", "#dsds", "asdas"}, new[] { "hsda\nasdas" })]
        [InlineData(new[] {"AF!dsdsd", "AF!hsda", "#dsds", "asdas"}, new[] { "dsdsd", "hsda\nasdas" })]
        [InlineData(new[] {"AF!dsdsd", "hsda", "#AF!dsds", "asdas"}, new[] { "dsdsd\nhsda\nasdas" })]
        [InlineData(new[] {"AF!dsdsd", "", "asdas"}, new[] { "dsdsd\n\nasdas" })]
        [InlineData(new[] {"dsdsd", "hsda", "#AF!dsds", "asdas"}, new string[0])]
        public void ParseCommands(IEnumerable<string> lines, IEnumerable<string> expectedCommandsList) {
            var parsedCommands = (IEnumerable<string>) SignupImporter.ParseCommands(lines);

            parsedCommands.Should().BeEquivalentTo(expectedCommandsList);
        }

        [Theory]
        [InlineData("zrub-misje dsds", "zrub-misje")]
        [InlineData("zrub-misjeddds", "")]
        public void GetCommandName(string command, string expectedCommandName) {
            var commandName = SignupImporter.GetCommandName(command);

            commandName.Should().Be(expectedCommandName);
        }

        [Theory]
        [InlineData("zrub-misje dsadsa", "zrub-misje", "dsadsa")]
        [InlineData("zrub-misje asdasda \n\t", "zrub-misje", "asdasda")]
        public void GetParameterString(
            string command,
            string commandName,
            string expecterParameterString
        ) {
            var parameterString = SignupImporter.GetParameterString(command, commandName);

            parameterString.Should().Be(expecterParameterString);
        }

        [Fact]
        public void GetCommandInfoByName_NameEmpty_CommandInfoIsNull() {
            var signupImporter = PrepareImporter();
            
            var commandInfo = signupImporter.GetCommandInfoByName("");

            commandInfo.Should().BeNull();
        }

        [Fact]
        public async Task ProcessMessage_CommandsNotParsed_ThrowsInvalidParametersException() {
            var moduleMock = new Mock<IModule>();
            var signupImporter = PrepareImporter(module: moduleMock.Object);

            await signupImporter.ProcessMessage("");

            moduleMock.Verify(x => x.ReplyWithException<InvalidCommandParametersException>("Nie udało się odczytać komend."), Times.Once);
        }

        [Fact]
        public void ProcessCommand_CommandNotFound_ThrowsCommandNotFoundException() {
            const string commandName = "dsds ";

            var moduleMock = new Mock<IModule>();
            moduleMock.Setup(x => x.ReplyWithException<CommandNotFound>(It.IsAny<string>()))
                .ThrowsAsync(new CommandNotFound($"Nie znaleziono komendy {commandName}"));

            var signupImporter = PrepareImporter(
                module: moduleMock.Object);

            Func<Task> task = async () => await signupImporter.ProcessCommand(commandName);

            task.Should().ThrowExactly<CommandNotFound>($"Nie znaleziono komendy {commandName}");
        }

        private static SignupImporter PrepareImporter(
            ICommandContext commandContext = null,
            CommandService commandService = null,
            IServiceProvider serviceProvider = null,
            IModule module = null) {
            
            commandContext ??= new Mock<ICommandContext>().Object;
            commandService ??= new CommandService();
            serviceProvider ??= new Mock<IServiceProvider>().Object;
            module ??= new Mock<IModule>().Object;

            return new SignupImporter(
                commandContext,
                commandService,
                serviceProvider,
                module);
        }
    }
}
