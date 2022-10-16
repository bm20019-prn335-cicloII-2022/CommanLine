using System.Diagnostics;
namespace CommandLine;

public class ArgumentParser
{
    private List<string> _nameArgument = new List<string>();
    private List<string> _nameShort = new List<string>();
    private List<string> _Descripcion = new List<string>();
    private List<Tipos> _tiposList = new List<Tipos>();
    private Dictionary<string, object> dicionario = new Dictionary<string, object>();
    private string[] Arguments = Environment.GetCommandLineArgs();
    private int getLengthArguments = 0;
    private string Descripcion;
    private string Title;
    private string Version;

    #region Constructores
    public ArgumentParser()
    {
        this.Descripcion = InfoVersion().FileDescription;
        this.Title = InfoVersion().ProductName;
        Version = InfoVersion().FileVersion;
    }
    public ArgumentParser(string descripcion)
    {
        this.Descripcion = descripcion;
        this.Title = InfoVersion().ProductName;
        Version = InfoVersion().FileVersion;
    }
    public ArgumentParser(string title,string descripcion)
    {
        this.Descripcion = descripcion;
        this.Title = title;
        Version = InfoVersion().FileVersion;
        
    }
    public ArgumentParser(string title,string descripcion,string version)
    {
        this.Descripcion = descripcion;
        this.Title = title;
        this.Version = version;
    }
    #endregion

    #region AddArguments
    
    public void AddArgument(string arg)
    {
        if (arg.Length == 0 || arg.Length<2)
            throw new NullReferenceException("El argumento debe de tener por lo menos 2 caracteres. ejemplo: --input, -i");
        if(_nameArgument.Contains(arg))
            return;
        
        _nameArgument.Add(arg);
        _nameShort.Add("");
        _Descripcion.Add("");
        _tiposList.Add(Tipos.String);
    }
    
    public void AddArgument(string arg, string descripcion="")
    {
        if (arg.Length == 0 || arg.Length<2)
            throw new NullReferenceException("El argumento debe de tener por lo menos 2 caracteres. ejemplo: --input, -i");
        if(_nameArgument.Contains(arg))
            return;
        
        _nameArgument.Add(arg);
        _nameShort.Add("");
        _Descripcion.Add(descripcion);
        _tiposList.Add(Tipos.String);

    }
    
    public void AddArgument(string arg,string shortName = "", string descripcion="")
    {
        if (arg.Length == 0 || arg.Length<2)
            throw new NullReferenceException("El argumento debe de tener por lo menos 2 caracteres. ejemplo: --input, -i");
        if(_nameArgument.Contains(arg))
            return;
        
        _nameArgument.Add(arg);
        _nameShort.Add(shortName);
        _Descripcion.Add(descripcion);
        _tiposList.Add(Tipos.String);
    }
    
    public void AddArgument(string arg,string shortName = "",string descripcion="",Tipos tipo = Tipos.String)
    {
        if (arg.Length == 0 || arg.Length<2)
            throw new NullReferenceException("El argumento debe de tener por lo menos 2 caracteres. ejemplo: --input, -i");
        if(_nameArgument.Contains(arg))
            return;
        _nameArgument.Add(arg);
        _nameShort.Add(shortName);
        _Descripcion.Add(descripcion);
        _tiposList.Add(tipo);
    }
    #endregion
    
    #region Parser
    /// <summary>
    /// Analizador de argumentos en linea de comandos.
    /// se utiliza Environment.GetCommandLineArgs() para obtener la linea de comandos
    /// </summary>
    /// <exception cref="Exception"></exception>
    /// <exception cref="ArgumentException"></exception>
    public void CommandArgumentParser()
    {
        string[] args = Arguments;
        Dictionary<string, object> dic = new Dictionary<string, object>();
        foreach (var argName in _nameArgument)
        {
            dic.Add(argName, false);
        }

        if (args.ToList().Contains("--help") || args.ToList().Contains("-h"))
        {
            ayuda();
            return;
        }
        
        for (int i = 0; i < args.Length; i++)
        {
            string argument = args[i];
            if (_nameArgument.Contains(argument) || _nameShort.Contains(argument))
            {
                int positionArg = 0;
                if(_nameArgument.Contains(argument))
                    positionArg = _nameArgument.LastIndexOf(argument);
                else if (_nameShort.Contains(argument))
                    positionArg = _nameShort.LastIndexOf(argument);
                string nameArg = _nameArgument[positionArg];
                
                if (_tiposList[positionArg] == Tipos.String)
                {
                    if (i + 1 < args.Length)
                    {
                        if(!args[i+1].Contains("-"))
                            dic[nameArg] = args[i+1];
                    }
                }else if (_tiposList[positionArg] == Tipos.Boolean)
                {
                    if(i+1 <args.Length)
                        if (!args[i + 1].Contains("-"))
                        {
                            ayuda();
                            Console.WriteLine();
                            throw new Exception("Argumento no puede recibir parametros " + args[i]+"\n");
                        }
                    dic[nameArg] = true;
                }else if (_tiposList[positionArg] == Tipos.ArrayString)
                {
                    if (i + 1 < args.Length){
                        int contador = i;
                        List<string> arg = new List<string>();
                        do
                        {
                            if (!args[contador + 1].Contains("-"))
                            {
                                arg.Add(args[contador + 1]);
                                contador++;
                            }else
                                break;
                        } while (contador+1 < args.Length);
                        dic[nameArg] = arg.ToArray();
                    }
                    else
                    {
                        throw new ArgumentException("El Argumento no ha sido satisfecho \"{0}\"", argument);
                    }
                }
            }else if (args[i].Contains('-'))
            {
                ayuda();
                Console.WriteLine();
                throw new Exception("argumento no reconocido "+ args[i]+"\n");
            }
        }
        getLengthArguments = dic.Keys.Count;
        dicionario = dic;
    }
    
    /// <summary>
    /// Analizador de argumentos en linea de comandos.
    /// se utiliza un CommandLine especifico para obtener la linea de comandos
    /// </summary>
    /// <param name="args">CommandLine</param>
    /// <exception cref="Exception">No se Esperaba un valor</exception>
    /// <exception cref="ArgumentException">Se esperaba un valor</exception>
    public void CommandArgumentParser(string[] args)
    {
        Dictionary<string, object> dic = new Dictionary<string, object>();
        foreach (var argName in _nameArgument)
        {
            dic.Add(argName, false);
        }

        if (args.ToList().Contains("--help") || args.ToList().Contains("-h"))
        {
            ayuda();
            return;
        }
        
        for (int i = 0; i < args.Length; i++)
        {
            string argument = args[i];
            if (_nameArgument.Contains(argument) || _nameShort.Contains(argument))
            {
                int positionArg = 0;
                if(_nameArgument.Contains(argument))
                    positionArg = _nameArgument.LastIndexOf(argument);
                else if (_nameShort.Contains(argument))
                    positionArg = _nameShort.LastIndexOf(argument);
                string nameArg = _nameArgument[positionArg];
                
                if (_tiposList[positionArg] == Tipos.String)
                {
                    if (i + 1 < args.Length)
                    {
                        if(!args[i+1].Contains("-"))
                            dic[nameArg] = args[i+1];
                    }
                }else if (_tiposList[positionArg] == Tipos.Boolean)
                {
                    if(i+1 <args.Length)
                        if (!args[i + 1].Contains("-"))
                        {
                            ayuda();
                            Console.WriteLine();
                            throw new Exception("Argumento no puede recibir parametros " + args[i]+"\n");
                        }
                    dic[nameArg] = true;
                }else if (_tiposList[positionArg] == Tipos.ArrayString)
                {
                    if (i + 1 < args.Length){
                        int contador = i;
                        List<string> arg = new List<string>();
                        do
                        {
                            if (!args[contador + 1].Contains("-"))
                            {
                                arg.Add(args[contador + 1]);
                                contador++;
                            }else
                                break;
                        } while (contador+1 < args.Length);
                        dic[nameArg] = arg.ToArray();
                    }
                    else
                    { 
                        //Console.WriteLine("Error en el parametro. \"{0}\"",argument);
                        throw new ArgumentException("El Argumento no ha sido satisfecho \"{0}\"", argument);
                    }
                }
            }else if (args[i].Contains('-'))
            {
                ayuda();
                Console.WriteLine();
                throw new Exception("argumento no reconocido "+ args[i]+"\n");
            }
        }
        getLengthArguments = dic.Keys.Count;
        dicionario = dic;
    }
    
    #endregion

    #region getter
    /// <summary>
    /// Obtiene el valor del argumeto
    /// </summary>
    /// <param name="key">Nombre del argumento</param>
    /// <typeparam name="T">Tipo del argumento</typeparam>
    /// <returns></returns>
    /// <exception cref="InvalidCastException">Los tipos clase son diferentes</exception>
    /// <exception cref="ArgumentException">El argumento buscado no existe</exception>
    public T GetValue<T>(string key)
    {
        if (dicionario.ContainsKey(key))
        {
            if (typeof(T).Equals(dicionario[key].GetType()))
            {
                return (T)dicionario[key];

            }
            else
                throw new InvalidCastException($"No se puede convertir \"{dicionario[key].GetType().FullName}\" en \"{typeof(T).FullName}\"");
        }
        else
        {
            throw new ArgumentException($"El argumento no existe. \"{key}\"");
        }
    }
    /// <summary>
    /// Obtiene la cantidad de argumentos
    /// </summary>
    /// <returns></returns>
    public int GetLengthArguments()
    {
        return getLengthArguments;
    }
    /// <summary>
    /// Obtiene La lista de argumentos
    /// </summary>
    /// <returns></returns>
    public List<string> GetListArguments()
    {
        return _nameArgument;
    }
    #endregion

    private List<string> examples = new List<string>();
    public void AddExamples(List<string> examples)
    {
        this.examples = examples;
    }
    
    private void ayuda()
    {
        string print = $"{Title} -- Version: {Version}\n\nDescripcion:\n{Descripcion}\n\n";
        string commands = "";
        for (int i = 0; i < _nameArgument.Count; i++)
        {
            commands += $"{_nameArgument[i]}, {_nameShort[i]}\t{_Descripcion[i]}, type: {_tiposList[i].ToString()}\n";
        }
        commands += $"\n--help, -h\tImprime la ayuda del programa";
        Console.WriteLine(print+commands);
        if (examples.Count > 0)
        {
            Console.WriteLine("Ejemplos:");
            examples.ForEach(e => Console.WriteLine(e));
        }
    }
    
    private static FileVersionInfo InfoVersion()
    {
        System.Reflection.Assembly executingAssembly = System.Reflection.Assembly.GetExecutingAssembly();
        return FileVersionInfo.GetVersionInfo(executingAssembly .Location);
    }
    /// <summary>
    /// Imprime la ayuda de la linea de comandos
    /// </summary>
    public void PrintHelp()
    {
        ayuda();
    }
}