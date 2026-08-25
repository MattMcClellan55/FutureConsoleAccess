using System.Text.RegularExpressions;

int index = 0;
List<Node> nodes = new List<Node>();
Console.WriteLine("What are you thinking of right now?");
string? thought = Console.ReadLine();
if (thought is not null) nodes.Add(new Node (thought));

while (!(string.Equals(thought, "close", StringComparison.OrdinalIgnoreCase) ||
         string.Equals(thought, "end", StringComparison.OrdinalIgnoreCase) ||
         string.Equals(thought, "quit", StringComparison.OrdinalIgnoreCase) ||
         string.Equals(thought, "stop", StringComparison.OrdinalIgnoreCase)))
{
    Console.WriteLine("Anything else on your mind?");
    thought = Console.ReadLine();
    if (string.IsNullOrEmpty(thought)) break;

    string[] command = Regex.Matches(thought, @"""[^""]*""|\w+").Cast<Match>()
        .Select(m => m.Value).ToArray();

    if (command.Length < 2)
    {
        Console.WriteLine("Please enter a valid command.");
        continue;
    }
    else if (string.Equals(command[0], "birth", StringComparison.OrdinalIgnoreCase))
    {
        if (command.Length > 3 || command[command.Length - 1][0] != '\"')
        {
            Console.WriteLine("Please enter a valid command.");
            continue;
        }
        else if (command.Length == 3)
        {
            if (Int32.TryParse(command[1], out int newIndex) && newIndex < nodes.Count) index = newIndex;
            else
            {
                Console.WriteLine("Please enter a valid index.");
                continue;
            }
        }

        nodes.Add(new Node(command[command.Length - 1].Trim('\"')));
        ParentChildLink(nodes[index], nodes[nodes.Count - 1]);
        index = nodes.Count - 1;
    }
    else if (string.Equals(command[0], "remove", StringComparison.OrdinalIgnoreCase))
    {
        if (Int32.TryParse(command[1], out int removeIndex) && removeIndex < nodes.Count)
        {
            nodes.RemoveAt(removeIndex);
            if (nodes.Count == 0) nodes.Add(new Node("[]"));
            if (index >= nodes.Count) index = nodes.Count - 1;
        }
        else
        {
            Console.WriteLine("Please enter a valid index.");
            continue;
        }
    }
    else if (string.Equals(command[0], "summarize", StringComparison.OrdinalIgnoreCase))
    {
        foreach (Node node in nodes)
        {
            // if (node.parent.Count > 0) continue;
            if (node.child.Count == 0) Console.WriteLine(node.text);

            foreach (Node child in node.child)
            {
                Console.WriteLine($"{node.text} -> {child.text}");
            }
        }
    }
}

static void ParentChildLink(Node parent, Node child)
{
    parent.child.Add(child);
    child.parent.Add(parent);
}

public struct Node
{
    public List<Node> parent; 
    public List<Node> child;
    public int tier;
    public string text;

    public Node(string text)
    {
        this.tier = 0;
        this.text = text;
        this.parent = new List<Node>();
        this.child = new List<Node>();
    }
}