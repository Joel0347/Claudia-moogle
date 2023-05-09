using System.Collections.Specialized;
namespace MoogleEngine;

public class Matrix
{
    // Diccionario con los nombres de los documentos y la palabra de la query que se recibe como parámetro con su TF-IDF
    public static Dictionary<string, Dictionary<string,float>> WordTF_IDF { get; set; }
    // Valor de IDF por palabra de la query
    public static float IDFValue { get; set; }
    public static float TF(int repeat, int allWords) { //Función para calcular el TF de cada palabra por documento
        return (float)repeat / (float)allWords;
    }

    public static float IDF(int allFiles, float contains) { //Función para calcular el IDF de cada palabra
        return (float)Math.Log10(allFiles / contains);
    }
    // Función que devuelve un diccionario que relaciona el valor de TF-IDF por palabra de la query que se recibe como parámetro 
    // por documentos
    public static Dictionary<string, Dictionary<string,float>> QueryTF_IDF(string query){ 
        // Asignar en variables locales variables de la clase ProcessDocuments
        Dictionary<string, string[]> words = ProcessDocuments.allWords; 
        Dictionary<string, Dictionary<string,int>> documents = ProcessDocuments.documentWords;
        // Cantidad total de documentos
        int allFiles = documents.Keys.Count;

        // Diccionario por palabra de la query que se recibe como parámetro con los documentos en los que aparece y la cantidad de 
        // veces que lo hace 
        Dictionary<string, Dictionary<string,int>> reduceDocuments = new Dictionary<string, Dictionary<string, int>>();
        // Diccionario con la palabra de la query y su valor de TF-IDF por documento
        Dictionary<string,float> TF_IDF;

        WordTF_IDF = new Dictionary<string, Dictionary<string, float>>();

        // Cantidad de documentos en los que aparece la palabra
        float count = 0;
        foreach (string document in documents.Keys) {
            if (documents[document].ContainsKey(query)) {
                count += 1;
                Dictionary<string,int> word = new Dictionary<string, int>();
                word.Add(query, documents[document][query]);
                reduceDocuments.Add(document, word);
            } 
        }

        IDFValue = Matrix.IDF(allFiles, count);

        foreach (string key in reduceDocuments.Keys) { // Iterar los documentos en los que aparece la palabra para calcular su TF-IDF
            TF_IDF = new Dictionary<string,float>();
            int total = words[key].Length; // Cantidad de palabras del documento
            // Valor de TF la palabra en cada documento
            float TFValue = TF(reduceDocuments[key][query], total);
            TFValue *= IDFValue; // Calcular el TF-IDF
            TF_IDF.Add(query, TFValue);

            WordTF_IDF.Add(key, TF_IDF); 
        }   

        return WordTF_IDF;  
    }
}