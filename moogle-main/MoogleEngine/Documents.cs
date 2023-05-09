using System;
using System.Text.RegularExpressions;

namespace MoogleEngine;

public class Documents
{
    // Función para normalizar los textos y hacer mas cómoda la búsqueda evadiendo errores de ortografía y símbolos innecesarios 
    public static string Normalize(string text)
    {
        text = text.ToLower();

        text = text.Replace("á", "a");
        text = text.Replace("é", "e");
        text = text.Replace("í", "i");
        text = text.Replace("ó", "o");
        text = text.Replace("ú", "u");

        text = Regex.Replace(text, @"[^a-zñ0-9]", " ");

        return text;
    }

    public static string Snippet(string text, string textwhitoutnormalize, string word) {
        //text texto normalizado
        //textwhitoutnormalize texto sin normalizar 
        //word palabra con mayor IDF en el texto

        string? snippet = null;

        int index = text.IndexOf(word); // Índice de la primera ocurrencia de la secuencia de caracteres de la palabra
        while (snippet == null) {
            if ((index == 0 
                || text.Substring(index - 1, 1) == " "
                )
                && (index + word.Length == text.Length
                || text.Substring(index + word.Length, 1) == " ")) // Verificar que sea exactamente la palabra
                {
                if (index < 150) { // Imprime los primeros caracteres hasta la primera ocurrencia de la palabra
                    snippet = textwhitoutnormalize.Substring(0, index);
                } else if (index > 150) { // Imprime 150 caracteres antes de la primera ocurrencia de la palabra hasta esta
                    snippet = textwhitoutnormalize.Substring(index - 150, 150);
                }
                // Concatena la otra mitad del snippet
                if (((text.Length - 1) - index) < 150) { // Imprime desde la palabra hasta el final del documento
                    snippet += textwhitoutnormalize.Substring(index);
                } else if (((text.Length - 1) - index > 150)) { // Imprime desde la palabra y 150 caracteres después  
                    snippet += textwhitoutnormalize.Substring(index, 150);
                }
            } else { // Si llega aquí es porque no era exactamente la palabra y se le indica que continúe hasta la próxima ocurrencia
                index = text.IndexOf(word, index + 1);
            }
        }

        return snippet;
    }
}