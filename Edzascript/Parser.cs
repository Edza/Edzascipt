using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edzascript
{
    public class Parser
    {
        static string Indent = Environment.NewLine + "    ";

        public string Parse(string input)
        {
            string dataSegment = "";
            string codeSegment = "";

            foreach (Command command in this.GetCommands(input))
            {
                if (command.OutputTarget == OutputTarget.Code)
                    codeSegment += Parser.Indent + command.ToString();
                else
                    dataSegment += Parser.Indent + command.ToString();
            }

            string output = "";
            output += Parser.StandardPrologue();
            output += dataSegment;
            output += Parser.StandardSeperator();
            output += codeSegment;
            output += Parser.StandardEpilogue();

            return output;
        }

        IEnumerable<Command> GetCommands(string input)
        {
            string[] lines = input.Split(new [] {Environment.NewLine}, StringSplitOptions.RemoveEmptyEntries);

            foreach (var line in lines)
            {
                yield return new Command(line);
            }
        }

        enum CommandType { DefineNumber, PrintNumber };
        enum OutputTarget { Data, Code }

        class Command
        {
            public CommandType Type;
            public OutputTarget OutputTarget;
            public List<string> Parameters = new List<string>();

            public Command(string line)
            {
                string[] parts = line.Split();

                if (parts.Count() < 1)
                    throw new ArgumentException();

                this.Type = Command.GetCommandTypeByName(parts[0]);
                this.OutputTarget = Command.GetCommandTypeTarget(this.Type);

                this.Parameters.AddRange(parts.Skip(1));
            }

            static CommandType GetCommandTypeByName(string name)
            {
                switch (name)
                {
                    case "def":
                        return CommandType.DefineNumber;
                    case "out":
                        return CommandType.PrintNumber;
                    default:
                        throw new InvalidOperationException();
                }
            }

            static OutputTarget GetCommandTypeTarget(CommandType type)
            {
                switch (type)
                {
                    case CommandType.DefineNumber:
                        return OutputTarget.Data;
                    case CommandType.PrintNumber:
                        return OutputTarget.Code;
                    default:
                        throw new InvalidOperationException();
                }
            }

            public override string ToString()
            {
                switch (this.Type)
                {
                    case CommandType.DefineNumber:
                        return CommandGenerator.DefineNumber(this);
                    case CommandType.PrintNumber:
                        return CommandGenerator.PrintNumber(this);
                    default:
                        throw new InvalidOperationException();
                }
            }

           
        }

        static class CommandGenerator
        {
            public static string DefineNumber(Command command)
            {
                int output;
                if(!int.TryParse(command.Parameters[1], out output))
                    throw new InvalidOperationException();

                return command.Parameters[0] + " dd " + command.Parameters[1];
            }

            public static string PrintNumber(Command command)
            {
                return "mov eax , " + command.Parameters[0] + Parser.Indent + "print str$(eax)";
            }
        }

        static string StandardPrologue()
        {
            return 
@".586
.MODEL flat, stdcall
include \masm32\include\windows.inc
include \masm32\macros\macros.asm
include \masm32\include\masm32.inc
include \masm32\include\gdi32.inc
include \masm32\include\user32.inc
include \masm32\include\kernel32.inc
includelib\masm32\lib\masm32.lib
includelib\masm32\lib\gdi32.lib
includelib\masm32\lib\user32.lib
includelib\masm32\lib\kernel32.lib
include \masm32\include\msvcrt.inc
includelib \masm32\lib\msvcrt.lib
.data
";
        }

        static string StandardSeperator()
        {
            return 
@"

.code
main PROC
";
        }

        static string StandardEpilogue()
        {
            return
@"

mov eax, input()
ret
main ENDP
END main";
        }
    }
}
