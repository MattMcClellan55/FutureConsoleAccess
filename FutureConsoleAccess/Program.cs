using System.Text.RegularExpressions;
using System.Xml.Linq;

Dictionary<string, Node> nodes = new Dictionary<string, Node>();
Console.WriteLine("What are you thinking of right now?");
string? thought = Console.ReadLine();

if (string.IsNullOrEmpty(thought)) thought = "[]";
string index = thought;
nodes.Add(index, new Node(thought));

do
{
    Console.WriteLine("Anything else on your mind?");
    thought = Console.ReadLine();
    if (string.IsNullOrEmpty(thought)) break;
    thought = thought.Replace('\'', '\"');

    string[] command = Regex.Matches(thought, @"""[^""]*""|\w+").Cast<Match>()
        .Select(m => m.Value).ToArray();

    if (command.Length < 2)
    {
        Console.WriteLine("Please enter a valid command.");
        continue;
    }
    else if (string.Equals(command[0], "adopt", StringComparison.OrdinalIgnoreCase))
    {
        if (command.Length != 3)
        {
            Console.WriteLine("Please enter a valid command.");
            continue;
        }
        else if (command[1][0] != '\"' || !nodes.ContainsKey(command[1].Trim('\"')) || 
            command[2][0] != '\"' || !nodes.ContainsKey(command[2].Trim('\"')))
        {
            Console.WriteLine("Please enter valid indexes.");
            continue;
        }

        ParentChildLink(nodes[command[1].Trim('\"')], nodes[command[2].Trim('\"')]);
    }
    else if (string.Equals(command[0], "birth", StringComparison.OrdinalIgnoreCase))
    {
        if (command.Length > 3 || command[command.Length - 1][0] != '\"')
        {
            Console.WriteLine("Please enter a valid command.");
            continue;
        }
        else if (nodes.ContainsKey(command[command.Length - 1].Trim('\"')))
        {
            Console.WriteLine("This entry already exists.");
            continue;
        }
        else if (command.Length == 3)
        {
            if (command[1][0] != '\"' || !nodes.ContainsKey(command[1].Trim('\"')))
            {
                Console.WriteLine("Please enter a valid index.");
                continue;
            }
            index = command[1].Trim('\"');
        }

        nodes.Add(command[command.Length - 1].Trim('\"'), new Node(command[command.Length - 1].Trim('\"')));
        ParentChildLink(nodes[index], nodes[command[command.Length - 1].Trim('\"')]);
    }
    else if (string.Equals(command[0], "create", StringComparison.OrdinalIgnoreCase))
    {
        if (command.Length > 2 || command[command.Length - 1][0] != '\"')
        {
            Console.WriteLine("Please enter a valid command.");
            continue;
        }
        else if (nodes.ContainsKey(command[command.Length - 1].Trim('\"')))
        {
            Console.WriteLine("This entry already exists.");
            continue;
        }

        nodes.Add(command[command.Length - 1].Trim('\"'), new Node(command[command.Length - 1].Trim('\"')));
        index = command[command.Length - 1].Trim('\"');
    }
    else if (string.Equals(command[0], "proxy", StringComparison.OrdinalIgnoreCase))
    {
        if (command.Length != 4)
        {
            Console.WriteLine("Please enter a valid command.");
            continue;
        }
        else if (command[1][0] != '\"' || !nodes.ContainsKey(command[1].Trim('\"')) ||
            command[2][0] != '\"' || !nodes.ContainsKey(command[2].Trim('\"')) || 
            command[3][0] != '\"' || nodes.ContainsKey(command[3].Trim('\"')))
        {
            Console.WriteLine("Please enter valid indexes.");
            continue;
        }
        else if (!nodes[command[1].Trim('\"')].child.Any(c => c.key == command[2].Trim('\"')))
        {
            Console.WriteLine("There is no in-between to proxy from here.");
            continue;
        }

        nodes[command[1].Trim('\"')].child.RemoveAll(p => p.key == command[2].Trim('\"'));
        nodes[command[2].Trim('\"')].parent.RemoveAll(p => p.key == command[1].Trim('\"'));

        nodes.Add(command[3].Trim('\"'), new Node(command[3].Trim('\"')));
        index = command[3].Trim('\"');

        ParentChildLink(nodes[command[1].Trim('\"')], nodes[command[3].Trim('\"')]);
        ParentChildLink(nodes[command[3].Trim('\"')], nodes[command[2].Trim('\"')]);
    }
    else if (string.Equals(command[0], "rename", StringComparison.OrdinalIgnoreCase))
    {
        if (command.Length != 3 || command[1][0] != '\"' || command[2][0] != '\"')
        {
            Console.WriteLine("Please enter a valid command.");
            continue;
        } 
        else if (!nodes.ContainsKey(command[1].Trim('\"')) || nodes.ContainsKey(command[2].Trim('\"')))
        {
            Console.WriteLine("Please enter valid indexes.");
            continue;
        }

        if (nodes.TryGetValue(command[1].Trim('\"'), out Node? value))
        {
            nodes.Remove(command[1].Trim('\"'));
            nodes.Add(command[2].Trim('\"'), value);
            if (index == command[1].Trim('\"')) index = command[2].Trim('\"');
        }
    }
    else if (string.Equals(command[0], "remove", StringComparison.OrdinalIgnoreCase))
    {
        if (command.Length > 2 || command[1][0] != '\"')
        {
            Console.WriteLine("Please enter a valid command.");
            continue;
        }
        else if (!nodes.ContainsKey(command[1].Trim('\"')))
        {
            Console.WriteLine("Please enter a valid index.");
            continue;
        }

        nodes.Remove(command[1].Trim('\"'));
        if (nodes.Count == 0) nodes.Add("[]", new Node("[]"));
        if (!nodes.ContainsKey(index)) index = nodes.Keys.First();

        foreach (Node node in nodes.Values.ToList())
        {
            node.parent.RemoveAll(p => p.key == command[1].Trim('\"'));
            node.child.RemoveAll(c => c.key == command[1].Trim('\"'));
        }
    }
    else if (string.Equals(command[0], "summarize", StringComparison.OrdinalIgnoreCase))
    {
        foreach (Node node in nodes.Values.ToList())
        {
            if (node.parent.Count == 0 && node.child.Count == 0) Console.WriteLine(node.key);

            foreach (Node child in node.child)
            {
                Console.WriteLine($"{node.key} -> {child.key}");
            }
        }
    }
}
while (!(string.Equals(thought, "close", StringComparison.OrdinalIgnoreCase) ||
         string.Equals(thought, "end", StringComparison.OrdinalIgnoreCase) ||
         string.Equals(thought, "quit", StringComparison.OrdinalIgnoreCase) ||
         string.Equals(thought, "stop", StringComparison.OrdinalIgnoreCase)));

static void ParentChildLink(Node parent, Node child)
{
    parent.child.Add(child);
    child.parent.Add(parent);
}

public class Node
{
    public List<Node> parent; 
    public List<Node> child;
    public string key;
    public string value;

    public Node(string key)
    {
        this.key = key;
        this.value = string.Empty;
        this.parent = new List<Node>();
        this.child = new List<Node>();
    }
}