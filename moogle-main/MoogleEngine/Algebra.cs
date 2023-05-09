namespace MoogleEngine;

// La Clase Álgebra es para la parte de Álgebra I. No tiene utilidad en el resto del proyecto
public class Matriz
{
    private double[,] matriz;
    public Matriz(double[,] matriz) {
        this.matriz = matriz;
    }
    public Matriz(int filas, int columnas) {
        this.matriz = new double[filas, columnas];
    }
    public int Filas {get { return matriz.GetLength(0); } }
    public int Columnas {get { return matriz.GetLength(1); } }

    public double this[int i, int j] {
        get 
        {
            if (i < 0 || i >= Filas) throw new ArgumentOutOfRangeException("i");
            if (j < 0 || j >= Columnas) throw new ArgumentOutOfRangeException("j");
                return matriz[i,j];
        }
        set 
        {
            if (i < 0 || i >= Filas) throw new ArgumentOutOfRangeException("i");
            if (j < 0 || j >= Columnas) throw new ArgumentOutOfRangeException("j");
                matriz[i,j] = value;
        }
    }

    public bool Igualdad(Matriz a) { // Función para comparar dos matrices
        if (a == null) throw new ArgumentNullException("a");
        if (a.Filas == this.Filas && a.Columnas == this.Columnas) {           
            for (int i = 0; i < this.Filas; i++) { 
                for (int j = 0; j < this.Columnas; j++) { 
                    if (a[i,j] != this.matriz[i,j]){
                        return false;
                    }
                }
            }

            return true;
        } else {
            throw new InvalidOperationException("No coinciden las dimensiones de ambas matrices");
        }        
    }
    public Matriz Suma(Matriz a) { // Función para sumar dos matrices
        if (a == null) throw new ArgumentNullException("a");
        if (a.Filas == this.Filas && a.Columnas == this.Columnas) {
            Matriz result = new Matriz(this.Filas,this.Columnas);
            
            for (int i = 0; i < Filas; i++) { 
                for (int j = 0; j < Columnas; j++) {
                    result[i,j] = a[i,j] + this.matriz[i,j];
                }
            }

            return result;
        } else {
            throw new InvalidOperationException("No coinciden las dimensiones de ambas matrices");
        }        
    }
    public Matriz Mult(Matriz a) { // Función para multiplicar dos matrices
        if (a == null) throw new ArgumentNullException("a");
        if (a.Filas == this.Columnas) {
            Matriz result = new Matriz(this.Filas, a.Columnas);
            
            for (int i = 0; i < result.Filas; i++) { 
                for (int j = 0; j < result.Columnas; j++) { 
                    for (int k = 0; k < a.Filas; k++) { 
                        result[i,j] += this[i,k] * a[k,j];
                    }    
                }
            }

            return result;
        } else {
            throw new InvalidOperationException("No coinciden las columnas de la primera con las filas de la segunda");
        }
    }
    public Matriz MultEscalar(double escalar) { // Función para multiplicar una matriz por un escalar
        Matriz result = new Matriz(this.Filas,this.Columnas);
        
        for (int i = 0; i < Filas; i++) { 
            for (int j = 0; j < Columnas; j++) { 
                result[i,j] = this[i,j] * escalar;
            }
        }

        return result;        
    }

    public Matriz Transpuesta() { // Función para obtener la traspuesta de una matriz
        if(this.Filas != this.Columnas) throw new InvalidOperationException("La matriz no es cuadrada");
        Matriz result = new Matriz(this.Columnas, this.Filas);
        
        for (int i = 0; i < Filas; i++) { 
            for (int j = 0; j < Columnas; j++) { 
                result[i,j] = this[j,i];
            }
        }

        return result;    
    }
}