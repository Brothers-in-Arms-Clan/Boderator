using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ArmaforcesMissionBot.Exceptions;
using ArmaforcesMissionBot.Helpers;
using Discord.Commands;

namespace ArmaforcesMissionBot.Features.Signups.Importer
{
    public class SignupImporter
    {
        private ICommandContext Context { get; }
        private CommandService CommandService { get; }
        private IServiceProvider ServiceProvider { get; }
        private IModule Module { get; }

        public SignupImporter(
            ICommandContext context,
            CommandService commandService,
            IServiceProvider serviceProvider,
            IModule module)
        {
            Context = context;
            CommandService = commandService;
            ServiceProvider = serviceProvider;
            Module = module;
        }

        public async Task ProcessMessage(string message)
        {
            var lineBreak = GetLineEnding(message);
            var lines = ReadLines(message, lineBreak);
            var commands = ParseCommands(lines, lineBreak);

            if (!commands.Any())
                await Module.ReplyWithException<InvalidCommandParametersException>("Failed to read the command.");

            foreach (var command in commands) await ProcessCommand(command);
        }

        /// <summary>
        ///     Processes and executes given <paramref name="command" /> if everything works correctly.
        /// </summary>
        /// <returns></returns>
        public async Task ProcessCommand(string command)
        {
            // This might be just empty string sometimes
            if (string.IsNullOrEmpty(command)) return;

            var commandName = GetCommandName(command);
            var commandInfo = GetCommandInfoByName(commandName);

            if (commandInfo is null)
                await Module.ReplyWithException<CommandNotFound>($"Command not found {commandName}");

            if (!await UserCanUseCommand(commandInfo))
                await Module.ReplyWithException<NotAuthorizedException>($"You don't have the permission to use the command {commandInfo.Name}.");

            var parameterString = GetParameterString(command, commandName);

            var commandParameters = ParseCommandParameters(parameterString, commandInfo);

            await commandInfo
                .ExecuteAsync(
                    Context,
                    commandParameters,
                    commandParameters,
                    ServiceProvider);
        }

        /// <summary>
        ///     Tries to parse <paramref name="parameterString" /> to match given <paramref name="commandInfo" /> signature.<br />
        ///     Only one parameter is supported now.
        /// </summary>
        /// <returns></returns>
        public static List<object> ParseCommandParameters(string parameterString, CommandInfo commandInfo)
        {
            var parameterDateTime = commandInfo.Parameters.Count == 1 &&
                                    commandInfo.Parameters.First().Type == typeof(DateTime)
                ? DateTimeParser.ParseOrNull(parameterString)
                : null;

            var commandParameter = (object) parameterDateTime ?? parameterString;

            return new List<object> {commandParameter};
        }

        /// <summary>
        ///     Removes <paramref name="commandName" /> from beginning of given <paramref name="command" /> string.
        /// </summary>
        /// <returns></returns>
        public static string GetParameterString(string command, string commandName)
            => command.Substring(commandName.Length).Trim();

        /// <summary>
        ///     Extracts command name string from command line.
        /// </summary>
        /// <returns></returns>
        public static string GetCommandName(string command)
        {
            var possibleCommandNameLength = command.IndexOf(' ');
            return possibleCommandNameLength == -1
                ? ""
                : command.Substring(0, possibleCommandNameLength);
        }

        /// <summary>
        ///     Tries to retrieve <see cref="CommandInfo" /> of command with given <paramref name="commandName" />.
        /// </summary>
        /// <returns></returns>
        public CommandInfo GetCommandInfoByName(string commandName)
            => CommandService.Commands
                .FirstOrDefault(x => x.Name == commandName);

        /// <summary>
        ///     Checks if user imported command which should not be accessible to him.
        /// </summary>
        /// <param name="commandInfo"></param>
        /// <returns>False if user didn't meet command preconditions</returns>
        public async Task<bool> UserCanUseCommand(CommandInfo commandInfo)
        {
            var preconditions = await commandInfo.CheckPreconditionsAsync(Context, ServiceProvider);
            return preconditions.IsSuccess;
        }

        /// <summary>
        ///     Creates list of executable commands (command name + parameters) from given <paramref name="lines" />.<br />
        ///     Appends lines not beginning with "AF!" to previous commands if possible to allow multiline entries.<br />
        ///     Ignores lines starting with # or //.
        /// </summary>
        /// <returns><see cref="LinkedList{T}" /> containing commands for <seealso cref="CommandService" /></returns>
        public static LinkedList<string> ParseCommands(IEnumerable<string> lines, string lineBreak = "\n")
        {
            var loadedCommands = new LinkedList<string>();

            foreach (var line in lines)
            {
                if (line.StartsWith('#') || line.StartsWith("//")) continue;
                if (line.StartsWith("BIA!"))
                {
                    loadedCommands.AddLast(line.Substring("BIA!".Length));
                    continue;
                }

                if (!loadedCommands.Any()) continue;
                // ReSharper disable once PossibleNullReferenceException
                var previousCommand = loadedCommands.Last.Value;
                loadedCommands.RemoveLast();
                loadedCommands.AddLast(previousCommand + lineBreak + line);
            }

            return loadedCommands;
        }

        /// <summary>
        ///     Converts given <paramref name="message" /> to <see cref="IEnumerable{T}" /> of lines.
        ///     Preserves line break characters.
        /// </summary>
        /// <param name="message">Message to convert.</param>
        /// <returns><see cref="IEnumerable{T}" /> of lines.</returns>
        public static IEnumerable<string> ReadLines(string message, string lineBreak = null)
        {
            var lineEnding = string.IsNullOrEmpty(lineBreak)
                ? GetLineEnding(message)
                : lineBreak;

            return message.Split(lineEnding)
                .Select(x => x + lineEnding);
        }

        /// <summary>
        ///     Checks and returns used line endings in given string.
        /// </summary>
        /// <param name="message">Message to check</param>
        /// <returns>"\r\n" or "\n"</returns>
        public static string GetLineEnding(string message)
            => message.Contains("\r\n")
                ? "\r\n"
                : "\n";
    }
}
