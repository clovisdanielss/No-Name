using System;
using System.Collections.Generic;
namespace Graph_Things
{
	public class Combinatory
	{
		private List<int[]> combinations;
		public Combinatory ()
		{
			combinations = new List<int[]> ();
		}

		public List<int[]> GetCombinations(){
			return combinations;
		}

		// Parametros Iniciais n -1,k -1,a[k],m=0;h=k-1
		public void LexicographicCombination(int n, int k, int[] a,int m, int h){
			// Como Conta a partir do 0, entao n = [0  ate n - 1] e nao n = [1 ate n].
			for (int j = 1; j < k + 1; j++) {
				if(k+j-h < k + 1)
					a [k + j - h] = m + j;
			}
			combinations.Add(TransformInterval (a));
			if (m < n - h) {
				h = 0;
				h += 1;
				m = a [k + 1 - h];
			}
			else {
				h += 1;
				m = a [k + 1 - h];
			}
			if (a [1] != n - k + 1){
				LexicographicCombination (n, k, a, m, h);
			}

		}

		public void PrintCombinations(){
			foreach(int[] v in combinations){
				foreach(int e in v){
					Console.Write (e.ToString () + ", ");
				}
				Console.WriteLine ();
			}
		}

		// Transforma o vetor 1-n em 0-n.
		private int[] TransformInterval(int [] vect){
			int[] result = new int[vect.Length - 1];
			for (int i = 0; i < result.Length; i++)
				result [i] = vect [i + 1] - 1;
			return result;
		}
			
	}
}

