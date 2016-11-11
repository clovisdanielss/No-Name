using System;
using System.IO;
using System.Collections.Generic;

namespace Graph_Things
{
	public class Graph
	{
		private int vertices,edges;
		private int[]setOfVertices;
		private String text;
		private int[][] distanceMatrix, predMatrix,adjacenceMatrix, incidencyMatrix, mstMatrix;
		private int[] terminals,notTerminals;

		private void LoadFile(){
			Console.Write ("Entre com o nome do arquivo: ");
			string file = Console.ReadLine ();
			try{
				using(StreamReader sr = new StreamReader(file + ".txt")){
					text = sr.ReadToEnd();
				}
				
			}
			catch(Exception e){
				Console.WriteLine ("Error in load file.");
				Console.WriteLine (e.ToString ());
			}
			if (text != null) {
				string[] verticesStrings = text.Split (';');
				vertices = Convert.ToInt16 (verticesStrings [0]);
				adjacenceMatrix = new int[vertices] [];
				for (int i = 0; i < vertices; i++) {
					adjacenceMatrix [i] = new int[vertices];
				}
				int count = 1;
				for (int line = 0 ; line < vertices ; line++) {
					for (int col = 0; col < vertices ; col++) {
						if (verticesStrings [count] != "\n" && verticesStrings [count] != null) {
							adjacenceMatrix [line] [col] = Convert.ToInt16 (verticesStrings [count]);
						}
						else
							col--;
						count++;
					}
				}
				CalculateEdges ();
				CreateIM ();
				CreateDM ();
				CreateMSTM ();
			}

		}

		public Graph(int size){
			vertices = size;
			adjacenceMatrix = new int[vertices] [];
			for (int i = 0; i < vertices; i++) {
				adjacenceMatrix [i] = new int[vertices];
			}
			Random seed = new Random ();
			for (int i = 0; i < size; i++) {
				for (int j = i+1; j < size; j++) {
					int weight = Convert.ToInt16( seed.Next (9));
					if (weight == 0)
						weight = 9;
					adjacenceMatrix [i] [j] = weight;
					adjacenceMatrix [j] [i] = weight;

				}
			}

			CalculateEdges ();
			CreateIM ();
			CreateDM ();
			CreateMSTM ();

			Console.Write ("Entre com o nome do arquivo: ");
			string file = Console.ReadLine ();
			try{
				using(StreamWriter sr = new StreamWriter(file + ".txt")){
					sr.Write(vertices.ToString() + ";");
					for (int i = 0; i < size; i++) {
						for (int j = 0; j < size; j++) {
							sr.Write(adjacenceMatrix[i][j].ToString() + ";");
						}
					}
				}

			}
			catch(Exception e){
				Console.WriteLine ("Error in Create file.");
				Console.WriteLine (e.ToString ());
			}
		}

		private void CreateIM(){
			incidencyMatrix = new int[vertices][];
			for (int i = 0; i < vertices; i++)
				incidencyMatrix [i] = new int[edges];
			int edgeIndex = 0;
			for (int line = 0 ; line < vertices ; line++) {
				for (int col = line; col < vertices ; col++) {
					if (adjacenceMatrix [line][col] >= 1) {
						incidencyMatrix [line][edgeIndex] = 1;
						incidencyMatrix [col][ edgeIndex] = 1;
						edgeIndex++;
					}
				}
			}
		}

		// Set of vertices sera o do grafo do problema.
		public void GenerateCSV(){
			try{
				int count  = 0;
				using(StreamWriter sr = new StreamWriter("vertices_g.csv")){
					for (int i = 0; i < adjacenceMatrix[0].Length + 1; i++) {
						if(i == 0){
							sr.Write("Id;Label;Terminal\n");
						}
						else
							if(count < terminals.Length && terminals[count] == i - 1){
								count++;
								sr.Write((i-1).ToString() + ";" + setOfVertices[i-1].ToString() +";1" +"\n");
							}
							else
								sr.Write((i-1).ToString() + ";" + setOfVertices[i-1].ToString() +";0" +"\n");
					}
				}
			}

			catch(Exception e){
				Console.WriteLine ("Error in Create file.");
				Console.WriteLine (e.ToString ());
			}

			try{
				using(StreamWriter sr = new StreamWriter("arestas_g.csv")){
					for (int i = 0; i < adjacenceMatrix[0].Length + 1; i++) {
						if(i == 0){
							sr.Write("Source;Target;Type;Weight\n");
						}
						if(i>0)
							for(int j = i; j < adjacenceMatrix[0].Length; j++)
								if(adjacenceMatrix[i-1][j] > 0)
									sr.Write((i-1).ToString() + ";" + j.ToString() +";Undirected;"+adjacenceMatrix[i-1][j].ToString()+"\n");
					}
				}
			}

			catch(Exception e){
				Console.WriteLine ("Error in Create file.");
				Console.WriteLine (e.ToString ());
			}

		}

		public static void GenerateCSV(int[][] matrix, int[] setOfVertices, int[] terminals){
			try{
				int count  = 0;
				using(StreamWriter sr = new StreamWriter("vertices_h.csv")){
					for (int i = 0; i < matrix[0].Length + 1; i++) {
						if(i == 0){
							sr.Write("Id;Label;Terminal\n");
						}
						else
							if(count < terminals.Length && terminals[count] == i - 1){
								count++;
								sr.Write((i-1).ToString() + ";" + setOfVertices[i-1].ToString() +";1" +"\n");
							}
							else
								sr.Write((i-1).ToString() + ";" + setOfVertices[i-1].ToString() +";0" +"\n");
						}
					}
				}
				
			catch(Exception e){
				Console.WriteLine ("Error in Create file.");
				Console.WriteLine (e.ToString ());
			}
				
			try{
				using(StreamWriter sr = new StreamWriter("arestas_h.csv")){
					for (int i = 0; i < matrix[0].Length + 1; i++) {
						if(i == 0){
							sr.Write("Source;Target;Type;Weight\n");
						}
						if(i>0)
							for(int j = i; j < matrix[0].Length; j++)
								if(matrix[i-1][j] > 0)
									sr.Write((i-1).ToString() + ";" + j.ToString() +";undirected;"+matrix[i-1][j].ToString()+"\n");
					}
				}
			}

			catch(Exception e){
				Console.WriteLine ("Error in Create file.");
				Console.WriteLine (e.ToString ());
			}

		}
	

		private void CreateDM(){
			distanceMatrix = new int[vertices][];
			predMatrix = new int[vertices][];
			for (int i = 0; i < vertices; i++) {
				int[][] vec = new int[2][];
				vec = Dijkstra (i);
				distanceMatrix [i] = vec[0];
				predMatrix [i] = vec [1];
			}
		}

		private void CreateMSTM(){ // Cria a matriz da arvore geradora minima
			mstMatrix = new int[vertices] [];
			for (int i = 0; i < vertices; i++) {
				mstMatrix [i] = new int[vertices];
			}
			int [] pred = Prim();
			for (int i = 1; i < pred.Length; i++) {
				if (pred [i] != -1) {
					mstMatrix [i] [pred [i]] = distanceMatrix [i] [pred [i]];
					mstMatrix [pred [i]] [i] = distanceMatrix [pred [i]] [i];
				} else {
					mstMatrix [i] [i] = 9999;
				}
			}
		}

		private int TakeMin(List<int> queue, int[] dist){
			int lesser = 0;
			foreach (int vertex in queue) {
				lesser = vertex;
				break;
			}

			foreach (int vertex in queue) {
				if (dist [lesser] > dist [vertex])
					lesser = vertex;
			}
			return lesser;
		}
		public int EdgeWeight(int a, int b){
			if (a == -1 || b == -1) {
				return 9999;
			}else
				return adjacenceMatrix [a][ b];
		}

		private int EdgeKWeight(int a, int b){
			if (a == -1 || b == -1) {
				return 9999;
			}else
				return distanceMatrix [a][ b];
		}

		private int ExtractDictionary(Dictionary<int,int> dict){
			int lesser = 0;

			List<int> list = new List<int> (dict.Keys);

			foreach (int vertex in list) {
				lesser = vertex;
				break;
			}


			foreach (int vertex in list) {
				if (dict [lesser] > dict [vertex])
					lesser = vertex;
			}
				
			return lesser;
		}

		public Graph ()
		{
			LoadFile ();
			setOfVertices = new int[vertices];
			for (int i = 0; i < vertices; i++)
				setOfVertices [i] = i;
		}

		public Graph(int[][] am, int size){
			vertices = size - 1;
			setOfVertices = new int[size - 1];
			adjacenceMatrix = new int[size - 1][];
			for (int i = 0; i < size - 1; i++)
				adjacenceMatrix[i] = new int[size - 1 ];
			for (int i = 0; i < size - 1; i++)
				for (int j = 0; j < size - 1 ; j++) {
					if (i == 0)
						setOfVertices [j] = am [i][ j+1]; 
					adjacenceMatrix [i][ j] = am [i + 1][ j + 1];
				}
			CalculateEdges ();
			CreateIM ();
			CreateDM ();
			CreateMSTM ();
		}

		public int costMST(){
			int result = 0;
			foreach (int[] vect in mstMatrix) {
				foreach (int k in vect) {
					result += k;
				}
			}
			return result / 2;
		}

		public void PrintSetOfVertices(){
			Console.Write ("{");
			foreach(int v in setOfVertices)
				Console.Write (v.ToString()  +  ";");
			Console.WriteLine("}");
		}

		public void SetTerminals(int[] term){
			notTerminals = new int[vertices - term.Length];
			terminals = term;
			bool ad;
			int count = 0;
			for (int v = 0; v < vertices; v++) {
				ad = true;
				for (int i = 0; i < term.Length; i++)
					if (term [i] == v) {
						ad = false;
						break;
					}
				if (ad) {
					notTerminals [count] = v;
					count++;
				}
			}
		}

		public int[] GetNotTerminals(){
			return notTerminals;
		}

		public int[] GetTerminals(){
			return terminals;
		}

		public void PrintAM(){
			Console.WriteLine ("Adjacency Matrix!");

			for (int line = 0 ; line < vertices ; line++) {
				for (int col = 0; col < vertices ; col++) {
					Console.Write (adjacenceMatrix [line][col].ToString() + ";");
				}
				Console.Write ("\n");
			}
			Console.Write ("\n");
		}

		public void PrintMSTM(){
			Console.WriteLine ("Min. Spam Tree Matrix!");
			for (int line = 0 ; line < vertices ; line++) {
				for (int col = 0; col < vertices ; col++) {
					Console.Write (mstMatrix [line][col].ToString() + ";");
				}
				Console.Write ("\n");
			}
			Console.Write ("\n");
		}

		public void PrintIM(){
			Console.WriteLine ("Incidence Matrix!");
			for (int line = 0 ; line < vertices ; line++) {
				for (int col = 0; col < edges ; col++) {
					Console.Write (incidencyMatrix [line][col].ToString() + ";");
				}
				Console.Write ("\n");
			}
			Console.Write ("\n");
		}

		public void PrintDM(){
			Console.WriteLine ("Distance Matrix!");
			for (int line = 0 ; line < vertices ; line++) {
				for (int col = 0; col < vertices ; col++) {
					Console.Write (distanceMatrix [line][col].ToString() + ";");
				}
				Console.Write ("\n");
			}
			Console.Write ("\n");
		}

		public void PrintPM(){
			Console.WriteLine ("Pred Matrix!");
			for (int line = 0 ; line < vertices ; line++) {
				for (int col = 0; col < vertices ; col++) {
					Console.Write (predMatrix [line][col].ToString() + ";");
				}
				Console.Write ("\n");
			}
			Console.Write ("\n");
		}

		public Graph SubGraph(int[] removableSet){
			int subSet = vertices - removableSet.Length;
			int [][] matrix = new int[subSet + 1][];
			for(int i = 0; i < subSet + 1; i ++)
				matrix[i] = new int[subSet + 1];
			int count = 1;
			bool ad = false;
			for (int v = 0; v < vertices; v++) {
				ad = true;
				for (int i = 0; i < removableSet.Length; i++)
					if (removableSet [i] == v) {
						ad = false;
						break;
					}
				if (ad) {
					matrix [0][ count] = v;
					matrix [count][ 0] = v;
					count++;
				}
			}
			for (int line = 1 ; line < subSet + 1 ; line++) {
				for (int col = 1; col < subSet + 1 ; col++) {
					matrix [line][ col] = distanceMatrix [matrix[line][0]][matrix [0][ col]];
				}
			}

			return new Graph(matrix, subSet + 1);
		}
			
		private void CalculateEdges(){
			edges = 0;
			for (int line = 0 ; line < vertices ; line++) {
				for (int col = 0; col < vertices ; col++) {
					if (adjacenceMatrix [line][ col] >= 1)
						edges++;
				}
			}
			edges = edges / 2;
		}

		public int[][] GetAM(){
			return adjacenceMatrix;
		}
		public int[][] GetMST(){
			return mstMatrix;
		}
		public int[][] GetDM(){
			return distanceMatrix;
		}
		public int[][] GetPM(){
			return predMatrix;
		}
		public int[] GetSetOfVertices(){
			return setOfVertices;
		}


		public int[] GetGraph(){
			int[] graph = new int[2];
			graph [0] = vertices;
			graph [1] = edges;
			return graph;
		}

		public int[][] Dijkstra(int start){
			int infinite = 9999;
			List<int> queue = new List<int> ();
			int[] pred = new int[vertices];
			int[] dist = new int[vertices];
			for (int vertex = 0; vertex < pred.Length; vertex++) {
				pred [vertex] = -1;
				if (vertex != start)
					dist [vertex] = infinite;
				queue.Add (vertex);
			}
			while (queue.Count > 0) {
				int u = TakeMin (queue,dist);
				queue.Remove (u);
				int[] neighbors = ReturnNeighbors (u);
				for (int v = 0; v < neighbors.Length; v++) {
					if (neighbors [v] >= 1) {
						int aux = neighbors [v] + dist [u];
						if (aux < dist [v]) {
							dist [v] = aux;
							pred [v] = u;
						}
					}
				}
			}
			int[][] vec = new int[2][];
			vec [0] = dist;
			vec [1] = pred;
			return vec;

		}

		public int[] ReturnNeighbors(int vertex){
			return adjacenceMatrix [vertex];
		}
		public int[] ReturnKNeighbors(int vertex){
			return distanceMatrix [vertex];
		}

		public int[] Prim(){ // Só funciona para grafos conexos / Porque o VERTICE escolhido eh o 0, entao se o 0 nao estiver conexo.../ 
			Dictionary<int,int> queue = new Dictionary<int,int> ();
			List<int> explored = new List<int>();
			int[] pred = new int[vertices];
			for (int vertex = 0; vertex < pred.Length; vertex++) {
				pred [vertex] = -1;
			}
			queue.Add (0,0); // Adicionando qualquer véritice. Peso inicial 0. Dic(vertex,weight) ~~ (key,value)

			while (queue.Count > 0) {
				int v = ExtractDictionary (queue);
				queue.Remove (v);
				explored.Add (v);
				int[] neighbors = ReturnKNeighbors (v);
				for (int u = 0; u < neighbors.Length; u++) {
					if (neighbors [u] >= 1) {
						if (!explored.Contains (u) && EdgeKWeight (pred [u], u) > EdgeKWeight (v, u)) {
							if (queue.ContainsKey (u))
								queue [u] = EdgeKWeight (v, u);
							else
								queue.Add (u, EdgeKWeight (v, u));
							pred [u] = v;
						}
					}
				}
			}

/*			Console.WriteLine ("Predecessores:  ");
			for (int vertex = 0; vertex < pred.Length; vertex++) {
				Console.WriteLine ("Predecessor de " + vertex.ToString () + " eh " + pred [vertex]);
			}*/
			return pred;
		}

	}
}
	