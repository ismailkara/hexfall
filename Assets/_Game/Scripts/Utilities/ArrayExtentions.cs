
using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public static class ArrayExtentions {
	public static T GetLast<T> (this List<T> list) {
		return list[list.Count - 1];
	}
	
	public static T GetReverse<T> (this List<T> list, int index) {
		return list[list.Count - 1 - index];
	}

	public static void Fresh <T> (this List<T> list) {
		if (list == null) {
			list = new List<T>();
		} else {
			list.Clear();
		}
	}

	public static void LogNull<T> (this T obj) {
		Debug.Log(obj.GetType().ToString() + " - " + (obj == null));
	}

	public static T GetRandom <T> (this List<T> array)
	{
		return array[Random.Range(0, array.Count)];
		
	}
	
	public static T GetRandom <T> (this List<T> array, T exclude)
	{
		T result = array[Random.Range(0, array.Count)];
		int first, second;
		first = array.IndexOf(result);
		second = array.IndexOf(exclude);
		if (first == second)
		{
			return GetRandom(array, exclude);
		}
		else
		{
			return result;

		}
	}
	
	
	
	public static T GetRandom <T> (this T[] array)
	{
		int random = Random.Range(0, array.Length);
		return array[random];
	}

	// public static void Scramble<T>(this List<T> list, int seed = -1)
	// {
	// 	RandomJava rnd = seed == -1 ? new RandomJava((ulong)(DateTime.Now.Hour * DateTime.Now.Second * DateTime.Now.Millisecond)) : new RandomJava((ulong)seed);
	// 	for (int t = 0; t < list.Count; t++)
	// 	{
	// 		T tmp = list[t];
	// 		int r = rnd.NextInt(list.Count);
	// 		list[t] = list[r];
	// 		list[r] = tmp;
	// 	}
	// }
	
	public static void MakeReverse<T>(this List<T> list)
	{
		List<T> temp = new List<T>();

		foreach (var item in list)
		{
			temp.Add(item);
		}
		
		list.Clear();


		while (temp.Count > 0)
		{
			T tempItem = temp.GetLast();
			temp.Remove(tempItem);
			list.Add(tempItem);
		}
	}

	public static string CutFromEnd(this string original, int count)
	{
		if (original.Length == 0)
		{
			return original;
		}
		return original.Substring(0, original.Length - count);
	}
}
