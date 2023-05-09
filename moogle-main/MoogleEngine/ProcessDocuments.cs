namespace MoogleEngine;

public static class ProcessDocuments
{
    //Array con las direcciones de los documentos 
    private static string[] docs { get; set; }
    // Diccionario con los nombres de los documentos y las palabras sin repetir por documentos con la cantidad de veces que aparecen 
    // en el documento
    public static Dictionary<string, Dictionary<string,int>> documentWords { get; set; }
    // Diccionario con los nombres de cada documento con las palabras del documento
    public static Dictionary<string, string[]> allWords { get; set; }
    // Diccionario con los nombres de cada documento con el texto normalizado del documento 
    public static Dictionary<string, string> files { get; set; }
    // Diccionario con los nombres de cada documento con el texto original del documento 
    public static Dictionary<string, string> fileswhitoutnormalize { get; set; }

    // Método para cargar la base de documentos
    public static void LoadDocuments(string url = "Content")
    {
        docs = Directory.GetFiles(Path.Join("..", url), "", SearchOption.AllDirectories); 

        documentWords = new Dictionary<string, Dictionary<string,int>>();
        allWords = new Dictionary<string, string[]>();

        string[] wordsdocument = new string[]{};
        files = new Dictionary<string, string>();
        fileswhitoutnormalize = new Dictionary<string, string>();

        for (int i = 0; i < docs.Length; i++) {

            //Nombre del documento sin .txt
            string title = Path.GetFileNameWithoutExtension(docs[i]);
            //Nombre del documento
            string fileName = Path.GetFileName(docs[i]);
            //Separar el nombre del documento para verificar despues si es .txt
            string[] fileNameArray = fileName.Split('.');

            if (fileNameArray.Length > 1 && fileNameArray[fileNameArray.Length -1].ToLower() == "txt") { //Verificar que el documento es .txt
                // Normalizar los documentos con la función Documents.Normalize
                string text = Documents.Normalize(File.ReadAllText(docs[i])); 
                // Separar las palabras del documento en cada posición del array  
                wordsdocument = text.Split(" ", StringSplitOptions.RemoveEmptyEntries); 

                // Llenar los diccionarios
                files.Add(title, text);
                fileswhitoutnormalize.Add(title, File.ReadAllText(docs[i]));
                allWords.Add(title, wordsdocument);

                // Diccionario que contiene cada palabra con la cantidad de veces que aparece en el documento
                Dictionary<string,int> repeat = new Dictionary<string, int>();
                
                foreach (string word in wordsdocument) { // Ciclo para llenar el diccionario
                    if (repeat.ContainsKey(word)) {
                        repeat[word] += 1;
                    } else {
                        repeat.Add(word, 1);
                    }
                }
                
                documentWords.Add(title, repeat);
            }
        }
        
    }
}