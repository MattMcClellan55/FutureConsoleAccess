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

        Adopt(nodes[command[1].Trim('\"')], nodes[command[2].Trim('\"')]);
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

        Birth(command[command.Length - 1].Trim('\"'));
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

        Create(command[command.Length - 1].Trim('\"'));
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
        else if (!nodes[command[1].Trim('\"')].child.Any(c => c == command[2].Trim('\"')))
        {
            Console.WriteLine("There is no in-between to proxy from here.");
            continue;
        }

        nodes.Add(command[3].Trim('\"'), new Node(command[3].Trim('\"')));
        index = command[3].Trim('\"');
        Proxy(nodes[command[1].Trim('\"')], nodes[command[2].Trim('\"')], nodes[command[3].Trim('\"')]);
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

        Remove(command[1].Trim('\"'));
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

        Rename(command[1].Trim('\"'), command[2].Trim('\"'));
    }
    else if (string.Equals(command[0], "summarize", StringComparison.OrdinalIgnoreCase))
    {
        Summarize();
    }
}
while (!(string.Equals(thought, "close", StringComparison.OrdinalIgnoreCase) ||
         string.Equals(thought, "end", StringComparison.OrdinalIgnoreCase) ||
         string.Equals(thought, "quit", StringComparison.OrdinalIgnoreCase) ||
         string.Equals(thought, "stop", StringComparison.OrdinalIgnoreCase)));

void Adopt(Node parent, Node child)
{
    ParentChildLink(parent, child);
}

void Birth(string newKey)
{
    nodes.Add(newKey, new Node(newKey));
    ParentChildLink(nodes[index], nodes[newKey]);
}

void Create(string nodeKey)
{
    nodes.Add(nodeKey, new Node(nodeKey));
    index = nodeKey;
}

void Proxy(Node parent, Node child, Node proxy)
{
    parent.child.RemoveAll(p => p == child.key);
    child.parent.RemoveAll(p => p == parent.key);
    ParentChildLink(parent, proxy);
    ParentChildLink(proxy, child);
}
void Remove(string index)
{
    nodes.Remove(index);
    if (nodes.Count == 0) nodes.Add("[]", new Node("[]"));
    if (!nodes.ContainsKey(index)) index = nodes.Keys.First();

    foreach (Node node in nodes.Values.ToList())
    {
        node.parent.RemoveAll(p => p == index);
        node.child.RemoveAll(c => c == index);
    }
}

void Rename(string oldIndex, string newIndex)
{
    if (nodes.TryGetValue(oldIndex, out Node? value))
    {
        nodes.Remove(oldIndex);
        nodes.Add(newIndex, value);
        if (index == oldIndex) index = newIndex;
    }
}

void Summarize()
{
    foreach (Node node in nodes.Values.ToList())
    {
        if (node.parent.Count == 0 && node.child.Count == 0) Console.WriteLine(node.key);
        foreach (string childKey in node.child)
        {
            Console.WriteLine($"{node.key} -> {childKey}");
        }
    }
}

static void ParentChildLink(Node parent, Node child)
{
    parent.child.Add(child.key);
    child.parent.Add(parent.key);
}

public class Node
{
    public List<string> parent; 
    public List<string> child;
    public string key;
    public string value;

    public Node(string key)
    {
        this.key = key;
        this.value = string.Empty;
        this.parent = new List<string>();
        this.child = new List<string>();
    }
}