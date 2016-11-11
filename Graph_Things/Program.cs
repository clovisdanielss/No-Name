using System;
using System.Collections.Generic;
namespace Graph_Things
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			Console.Write ("Entre com [1] para carregar um grafo, ou com [n > 0] pra criar um grafo de tamanho n: ");
			string input = Console.ReadLine ();
			int option = Convert.ToInt16 (input);
			if (option == 1) {
				Graph g = new Graph ();
				int[] vector = new int[]{ 2, 5, 7, 9, 14, 18 };
				g.SetTerminals (vector);
				g.PrintAM ();
				DreyfusWagnerAlgorithm (g);
				/* Coisas pra exemplo... depois TIRE COMENTARIO!!
				g.GenerateCSV ();// Se estiver criando um novo grafo... Retire a chamada desse metodo.
				Console.ReadKey ();
				//g.PrintDM ();
				//g.PrintPM ();
				Graph h = EnumerateStainerTree (g);
				h.PrintMSTM ();
				Console.WriteLine ("Best Steiner Tree: Cost " + h.costMST ().ToString ());
				int[][] matrix = SteinerTree (g.GetPM (), g.GetDM (), h);
				Graph.GenerateCSV (matrix, g.GetSetOfVertices (), vector);// Se estiver criando um novo grafo... Retire a chamada desse metodo.
				Console.WriteLine ("Set of vertices: ");
				for (int line = 0; line < matrix [0].Length; line++) {
					for (int col = 0; col < matrix [0].Length; col++) {
						if (matrix [line] [col] > 0) {
							Console.Write (line.ToString () + ";");
							break;
						}
					}
				}
				Console.WriteLine ();
				for (int line = 0; line < matrix [0].Length; line++) {
					for (int col = 0; col < matrix [0].Length; col++) {
						Console.Write (matrix [line] [col].ToString () + ";");
					}
					Console.Write ("\n");
				}
				Console.Write ("\n");*/

			} else {
				Graph g = new Graph (option);
				Console.Write ("\n");
			}

		}

		public static Graph EnumerateStainerTree(Graph g){
			List<Graph> results = new List<Graph> ();
			int maxComb = g.GetTerminals ().Length - 2;
			if (maxComb < 0)
				return null;
			if (maxComb == 0) {
				Console.WriteLine ("A arvore de steiner eh o menor caminho entre os terminais!!");
				return null;
			}
			Combinatory c = new Combinatory ();
			for (int i = 0; i < maxComb; i++) {
				if (i + 2 > g.GetNotTerminals ().Length)
					break;
				c.LexicographicCombination (g.GetNotTerminals ().Length, i+1, new int[(i+1) + 1], 0, i+1);
			}

			foreach (int[] vect in c.GetCombinations()) {
				results.Add (g.SubGraph (SubSet(g.GetNotTerminals (),vect)));
			}

			results.Add (g.SubGraph (g.GetNotTerminals ()));
			results.Add (g);
			Console.WriteLine ();
			Graph result = null;
			int cost = -1;
			foreach (Graph h in results) {
//				h.PrintMSTM ();
//				Console.WriteLine ("Custo :" + h.costMST ());
				if (cost == -1) {
					cost = h.costMST ();
					result = h;
				} else if(cost > h.costMST()){
					result = h;
				}
			}
				

			return result;
		}

		// Retorna o Subconjunto do conjunto.
		public static int[] SubSet(int[] mainSet, int[] indexes){
			int[] result = new int[indexes.Length];
			for (int i = 0; i < mainSet.Length; i++) {
				for (int j = 0; j < indexes.Length; j++) {
					if (i == indexes[j]) {
						result [j] = mainSet [i];
						//Console.Write (result [j].ToString () + " " + j.ToString () + ";\n");
					}
				}
			}

			return result;
		}

		// Dados um conjunto K e seu subconjunto C, retorna todos elementos de K que não estão em C
		public static int[] SubSet_(int[] mainSet, int[] indexes){
			bool contains = false;
			int[] result = new int[mainSet.Length - indexes.Length];
			int counter = 0;
			for (int i = 0; i < mainSet.Length; i++) {
				contains = false;
				for (int j = 0; j < indexes.Length; j++) {
					if (i == indexes[j]) {
						contains = true;
						break;
					}
				}
				if (!contains) {
					result [counter] = mainSet [i];
					counter++;
				}
			}

			return result;
		}

		public static int[][] SteinerTree(int[][] PM, int[][] DM, Graph CompressedSteinerTree){
			int size = PM [0].Length;
			int[][] result = new int[size][];
			for (int i = 0; i < size; i++)
				result [i] = new int[size];
			// GetGraph returns G = [V,E]. The cell 0 returns the total number of vertices.
			for (int i = 0; i < CompressedSteinerTree.GetGraph () [0]; i++) {
				for (int j = 0; j < CompressedSteinerTree.GetGraph () [0]; j++) {
					if (CompressedSteinerTree.GetMST () [i][j] > 0) {
						int v1 = CompressedSteinerTree.GetSetOfVertices () [i];
						int v2 = CompressedSteinerTree.GetSetOfVertices () [j];
						ExpandEdge (v1, v2, PM, result,DM);
					}
				}
			}
			return result;
		}

		public static void ExpandEdge(int v1, int v2, int[][] PM, int[][] result,int [][] DM){
			if (PM [v1] [v2] == v1 || PM [v2] [v1] == v2) {
				result [v1] [v2] = DM [v1] [v2];
				result [v2] [v1] = DM [v2] [v1];
			} else {
				int v3 = PM [v1] [v2];
				ExpandEdge (v1, v3, PM, result, DM);
				ExpandEdge (v3, v2, PM, result, DM);
			}
		}

		public static void DreyfusWagnerAlgorithm(Graph g){
			List<Graph> results = new List<Graph> ();
			int[][] DM = g.GetDM ();
			int[] terminals = g.GetTerminals ();
			Combinatory comb = new Combinatory ();
			// Fazer combinacao de k pra 2.
			comb.LexicographicCombination(terminals.Length, 2, new int[3],0,2);
			foreach (int[] vect in comb.GetCombinations()) {
				Console.WriteLine ();
				int[] newSet = SubSet (terminals, vect);
				int[] removableSet = SubSet_ (g.GetSetOfVertices (), newSet);
				results.Add( g.SubGraph (removableSet));
			}
			for (int i = 2; i < terminals.Length - 1; i++) {
				foreach (Graph h in results) {
					if (h.GetSetOfVertices ().Length == i) {
						
					}
				}
			}
		}

		public static void Case1(List<Graph> list, Graph h, Graph g){
			int[] notInX = SubSet_ (g.GetSetOfVertices (), h.GetSetOfVertices ());
			for(int i =0;i < notInX.Length; i++){
				Graph aux = Unite (notInX [i], h.GetSetOfVertices (), g);
			}
		}

		public static Graph Unite(int vertex, int[] setOfVertices, Graph g){
			int[] newSet = new int[setOfVertices + 1];
			for (int i = 0; i < newSet.Length; i++) {
				if (i <= setOfVertices.Length) {
					newSet [i] = setOfVertices [i];
				} else {
					newSet [i] = vertex;
				}
			}

			return g.SubGraph (SubSet_ (g.GetSetOfVertices (), newSet));

		}

		public static bool GreaterThanTwo(int[][] matrix, int vertex){
			bool result = false;
			int counter = 0;
			foreach (int ele in matrix[vertex]) {
				if (ele > 0)
					counter++;
				if (counter >= 2) {
					result = true;
					break;
				}
			}
			return result;
		}

	}
}
