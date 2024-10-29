using System.Text.RegularExpressions;

Console.Title = "Todo App";
const string path = "todos.txt";
List<Todo> todos = new List<Todo>(ParseTodos());

static void DisplayMenu()
{
    Console.WriteLine("1. Add");
    Console.WriteLine("2. Delete");
    Console.WriteLine("3. Show All");
    Console.WriteLine("4. Clear");
    Console.WriteLine("5. Exit");
    Console.Write("Enter option: ");
}

static void UpdateTodos(List<Todo> newTodos)
{
    List<string> lines = new List<string>();

    foreach (Todo todo in newTodos)
    {
        string formattedTodo = $"{{id:{todo.Id}, title:\"{todo.Title}\"}}";
        lines.Add(formattedTodo);
    }

    try
    {
        File.WriteAllLines(path, lines);
    }
    catch (UnauthorizedAccessException ex)
    {
        Console.WriteLine("Error: You do not have permission to write to this file.");
        Console.WriteLine($"Details: {ex.Message}");
    }
    catch (IOException ex)
    {
        Console.WriteLine("Error: An I/O error occurred while writing to the file.");
        Console.WriteLine($"Details: {ex.Message}");
    }
    catch (Exception ex)
    {
        Console.WriteLine("An unexpected error occurred.");
        Console.WriteLine($"Details: {ex.Message}");
    }
}

static List<Todo> ParseTodos()
{
    List<Todo> todoList = new List<Todo>();
    Regex regex = new Regex(@"\{id:(\d+), title:""(.+?)""\}");

    try
    {
        foreach (string line in File.ReadLines(path))
        {
            Match match = regex.Match(line);
            if (match.Success)
            {
                try
                {
                    int id = int.Parse(match.Groups[1].Value);
                    string title = match.Groups[2].Value;

                    Todo t = new Todo(id, title);
                    todoList.Add(t);
                }
                catch (FormatException ex)
                {
                    Console.WriteLine($"Error parsing line: '{line}'. Invalid format. Details: {ex.Message}");
                }
                catch (OverflowException ex)
                {
                    Console.WriteLine($"Error parsing line: '{line}'. ID is too large. Details: {ex.Message}");
                }
            }
            else
            {
                Console.WriteLine($"Warning: Line does not match expected format: '{line}'");
            }
        }
    }
    catch (FileNotFoundException ex)
    {
        Console.WriteLine("Error: The todos file was not found.");
        Console.WriteLine($"Details: {ex.Message}");
    }
    catch (UnauthorizedAccessException ex)
    {
        Console.WriteLine("Error: You do not have permission to read this file.");
        Console.WriteLine($"Details: {ex.Message}");
    }
    catch (IOException ex)
    {
        Console.WriteLine("Error: An I/O error occurred while reading the file.");
        Console.WriteLine($"Details: {ex.Message}");
    }
    catch (Exception ex)
    {
        Console.WriteLine("An unexpected error occurred while parsing todos.");
        Console.WriteLine($"Details: {ex.Message}");
    }

    return todoList;
}


static string getTodoTitle()
{
    string todoTitle = "";
    do
    {
        Console.Write("Todo title (0 to exit): ");
        todoTitle = Console.ReadLine();

    } while (string.IsNullOrEmpty(todoTitle));

    return todoTitle;
}
static int getTodoId()
{
    string todoIdInput = Console.ReadLine();
    int todoId;
    while (!int.TryParse(todoIdInput, out todoId))
    {
        Console.Write("Invalid id! Todo id (0 to exit): ");
        todoIdInput = Console.ReadLine();
    }

    return todoId;
}

while (true)
{
    DisplayMenu();

    bool success = int.TryParse(Console.ReadLine(), out int choice);

    if (success)
    {
        switch (choice)
        {
            case 1:
                string todoTitle = getTodoTitle();
                if (todoTitle.Equals("0"))
                {
                    Console.Clear();
                    break;
                }

                HashSet<int> existingIds = new HashSet<int>(todos.Select(t => t.Id));


                int newId = 1;
                while (existingIds.Contains(newId))
                {
                    newId++;
                }
                Todo newTodo = new Todo(newId, todoTitle);
                todos.Add(newTodo);
                UpdateTodos(todos);
                Console.Clear();
                break;

            case 2:
                Console.Write("Todo id (0 to exit): ");
                int todoId = getTodoId();

                if (todoId == 0)
                {
                    Console.Clear();
                    break;
                }
                Todo todoToRemove = todos.Find(todo => todo.Id.Equals(todoId));
                bool exit = false;
                while (todoToRemove == null)
                {
                    Console.WriteLine($"Cannot find todo with id {todoId}");
                    Console.Write("Todo id (0 to exit): ");

                    todoId = getTodoId();

                    if (todoId == 0)
                    {
                        Console.Clear();
                        exit = true;
                        break;
                    }

                    todoToRemove = todos.Find(todo => todo.Id.Equals(todoId));
                }
                if (!exit)
                {
                    todos.Remove(todoToRemove);
                    UpdateTodos(todos);
                    Console.WriteLine($"Todo with id {todoId} was successfully deleted.");
                    Thread.Sleep(1500);
                    Console.Clear();
                }

                break;

            case 3:
                Console.WriteLine("\n");
                foreach (Todo todo in todos)
                {
                    Console.WriteLine($"{todo.Id} - {todo.Title}");
                }
                Console.WriteLine("\n");
                break;
            case 4:
                Console.Clear();
                break;
            case 5:
                Console.WriteLine("Exiting...");
                Thread.Sleep(1000);
                Environment.Exit(0);
                break;
        }
    }
    else
    {
        DisplayMenu();
    }
}

class Todo
{
    private int id;
    private string title;

    public Todo(int id, string title)
    {
        Id = id;
        Title = title;
    }

    public int Id
    {
        get { return id; }
        set { id = value; }
    }

    public string Title
    {
        get { return title; }
        set
        {
            if (!string.IsNullOrEmpty(value))
                title = value;
        }
    }
}