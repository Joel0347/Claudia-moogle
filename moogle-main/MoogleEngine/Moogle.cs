using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Cryptography;
namespace MoogleEngine;

public static class Moogle
{
    public static TimeSpan SearchTime = new TimeSpan(); // Inicialización del cronómetro
   
    public static int Busquedas { get; set; } // Cantidad de coincidencias con la búsqueda

    public static List<float> wordIDF { get; set; } // Lista de IDF de las palabras de la query

    // Método principal del Moogle!
    public static SearchResult Query(string query) {
        // Inicio del cronómetro
        DateTime begin = DateTime.Now;
        // Normalización de la query para comenzar la búsqueda
        query = Documents.Normalize(query);
        // Convertir la query en un array con cada palabra
        string[] arrQueryTemp = query.Split(" ", StringSplitOptions.RemoveEmptyEntries);
        // Array con las palabras de la query sin repetir
        string[] arrQuery = arrQueryTemp.Union(arrQueryTemp).ToArray();

        // Diccionario que contiene las palabras de la query y cuantas veces se repiten
        Dictionary<string,int> repeat = new Dictionary<string, int>();
                
        // Relacionar cada palabra con la cantidad de veces que aparece en la query
        foreach (string word in arrQueryTemp) {
            if (repeat.ContainsKey(word)) {
                repeat[word] += 1;
            } else {
                repeat.Add(word, 1);
            }
        }

        wordIDF = new List<float>();

        if (arrQuery.Length != 0) { // Verificar que la búsqueda no sea nula
            // Asignar en una variable local una variable de la clase ProcessDocuments
            Dictionary<string, string[]> words = ProcessDocuments.allWords; 
            
            // Lista con los arrays por palabra de la query con los valores de TF-IDF por documentos
            List <float[]> TF_IDFs = new List<float[]>(); 
                    
            for (int i = 0; i < arrQuery.Length; i++) { // Ciclo para llenar la lista TF_IDFs 
                int k = 0;
                // Diccionario que relaciona el valor de TF-IDF por palabra de la query por documentos
                Dictionary<string, Dictionary<string,float>> DicTemp = Matrix.QueryTF_IDF(arrQuery[i]); 
                // Llenar la lista en el orden de las palabras de la query
                wordIDF.Add(Matrix.IDFValue);
                // Array con los valores de TF-IDF de una palabra de la query
                float[] TF_IDF = new float[words.Keys.Count];

                foreach (string title in words.Keys) { // Ciclo para llenar TF_IDF 
                    if (DicTemp.Keys.Contains(title)) { // Si contiene el documento entonces devuelve el valor de TF-IDF
                        TF_IDF[k] = DicTemp[title][arrQuery[i]]; 
                    } 
                    k += 1;                             
                }
                // Finalmente se añade este array a la lista, quedando en correspondencia con la posición de la palabra de la query
                TF_IDFs.Add(TF_IDF);
            }

            float[] suma = new float[words.Keys.Count]; // Array para obtener el score final por documento
            string[] titles = words.Keys.ToArray(); // Array con los títulos de los documentos para facilitar el proceso
            float[] max = new float[words.Keys.Count]; // Array para determinar la palabra con mas IDF por documento
            // Array con los índices correspondientes a las palabras de la query con mayor IDF por documento
            int[] indexquery = new int[words.Keys.Count]; 
            
            // Cantidad que se le va a restar al score del documento por cada palabra de la query que no aparezca
            int resta = 10 * arrQueryTemp.Length; 
            for (int i = 0; i < TF_IDFs.Count; i++) { // Itera por cada palabra de la query
                for (int j = 0; j < TF_IDFs[i].Length; j++) { // Itera por cada documento
                    if (TF_IDFs[i][j] != 0) { // Verificar si la palabra esta en el documento
                        // Si está se suma el valor de TF-IDF multiplicado por la cantidad de veces que aparece en la query
                        suma[j] += TF_IDFs[i][j] * repeat[arrQuery[i]]; 
                        
                        if (wordIDF[i] > max[j]) { // Verificar cuál es la palabra con mayor valor de IDF en el documento
                            max[j] = wordIDF[i]; // Se actualiza el valor máximo
                            indexquery[j] = i; // y la posición i de la palabra de la query con ese valor
                        }                        
                    } else { 
                        suma[j] -= resta;
                    }
                }
            }

            // Diccionario con los valores del score por documentos que tengan el mismo score
            Dictionary<float, List<string>> scoreDocuments = new Dictionary<float, List<string>>();

            for (int i = 0; i < suma.Length; i++) { // Llenar el scoreDocuments
                if (!scoreDocuments.Keys.Contains(suma[i])) scoreDocuments[suma[i]] = new List<string>();
                scoreDocuments[suma[i]].Add(titles[i]);
            }

            Array.Sort(suma); // Ordenar el array con todos los scores de menor a mayor
            float[] score = new float[suma.Length]; // Array para imprimir con los score de los documentos que coincidan con la búsqueda

            int cantdocs = 0; // Cantidad de documentos que coinciden con la búsqueda 
            for (int i = 0; i < score.Length; i++) {
                if (suma[(suma.Length - 1) - i] != -resta * arrQuery.Length){ 
                    score[i] = suma[(suma.Length - 1) - i]; // Ordenar de mayor a menor los score 
                    cantdocs += 1;
                } else { // A patir de la posición donde el valor sea -resta * arrQuery.Length no hay coincidencias con la búsqueda
                    break;
                }
            }

            // Cambiar el tamaño al score con los documentos que coincidieron con la búsqueda
            Array.Resize(ref score, cantdocs); 
            Busquedas = cantdocs; 

            SearchItem[] items = new SearchItem[score.Length]; // Array de tipo SearchItem para devolver los resultados de la búsqueda
            score = score.Union(score).ToArray();

            // Asignar en una variable local el texto normalizado por documento
            Dictionary<string, string> Snippet = ProcessDocuments.files; 
            // Asignar en una variable local el texto sin normalizar por documento
            Dictionary<string, string> SnippetR = ProcessDocuments.fileswhitoutnormalize; 
            // Array con el snippet por documento
            string[] snippet = new string[items.Length];

            // Diccionario con el título del documento y la palabra con mayor valor de IDF en ese documento
            Dictionary<string, string> WordMostImportant = new Dictionary<string, string>(); 

            for (int i = 0; i < words.Keys.Count; i++) { // Ciclo para llenar el diccionario
                WordMostImportant.Add(titles[i], arrQuery[indexquery[i]]);
            }          
            
            int p = 0;
            for (int i = 0; i < score.Length; i++) { // Iterar cada valor del score por documento
                // Si el score se repite, aquí se iteran todos los documentos que coincidan en score
                foreach (string title in scoreDocuments[score[i]]) { 
                    if (Snippet.Keys.Contains(title)) {
                        // Función de la clase Documents para crear el snippet por documento
                        snippet[p] = Documents.Snippet(Snippet[title], SnippetR[title], WordMostImportant[title]);
                        p += 1;
                    }
                    
                }
            }    

            if (score.Length == 0) { // Si no hubo coincidencias 
                Array.Resize(ref items, 1);
                items[0] = new SearchItem("Búsqueda no encontrada", "intenta algo más", 0);
            
            } else { // Llenar el array items con las coincidencias, su snippet y el score por documento
                int k = 0;
                for (int i = 0; i < score.Length; i++) {
                    foreach (string title in scoreDocuments[score[i]]) {
                        items[k] = new SearchItem(title, snippet[k], score[i]);
                        k += 1;
                    }
                }
            } 
            // Fin del cronómetro
            SearchTime = DateTime.Now - begin;
                    
            // Retorno de las búsquedas
            return new SearchResult(items, query);
        } else { // Si la búsqueda es vacía 
            SearchItem[] items = new SearchItem[1];
            items[0] = new SearchItem("Búsqueda inválida", "intenta algo más", 0);
            return new SearchResult(items, query);
        }        
    }
}