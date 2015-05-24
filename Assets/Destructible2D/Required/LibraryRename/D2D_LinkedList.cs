using UnityEngine;
using System.Collections.Generic;

public class D2D_LinkedList<T>
	where T : class
{
	public class Node
	{
		public Node Prev;
		public Node Next;
		public int  Index;
		public T    Value;
	}
	
	public int Count;
	
	public List<Node> Elements = new List<Node>();
	
	public Stack<int> FreeIndices = new Stack<int>();
	
	public Node First;
	
	public Node Last;
	
	public void Clear()
	{
		Count = 0;
		First = null;
		Last  = null;
		
		FreeIndices.Clear();
		
		for (var i = Elements.Count - 1; i >= 0; i--)
		{
			FreeIndices.Push(i);
		}
	}
	
	public Node AddFirst(T newValue)
	{
		var newNode = GetNode(newValue);
		
		if (First != null)
		{
			newNode.Prev = null;
			newNode.Next = First;
			
			First.Prev = newNode;
			
			First = newNode;
		}
		else
		{
			First = newNode;
			Last  = newNode;
		}
		
		return newNode;
	}
	
	public Node AddLast(T newValue)
	{
		var newNode = GetNode(newValue);
		
		if (Last != null)
		{
			newNode.Prev = Last;
			newNode.Next = null;
			
			Last.Next = newNode;
			
			Last = newNode;
		}
		else
		{
			First = newNode;
			Last  = newNode;
		}
		
		return newNode;
	}
	
	public void Remove(Node n)
	{
		if (n == First)
		{
			First = n.Next;
		}
		
		if (n == Last)
		{
			Last = n.Prev;
		}
		
		if (n.Prev != null)
		{
			n.Prev.Next = n.Next;
		}
		
		if (n.Next != null)
		{
			n.Next.Prev = n.Prev;
		}
		
		Count -= 1;
	}
	
	private Node GetNode(T newValue)
	{
		Count += 1;
		
		if (FreeIndices.Count > 0)
		{
			var index   = FreeIndices.Pop();
			var element = Elements[index];
			
			element.Value = newValue;
			
			return element;
		}
		
		var newIndex   = Elements.Count;
		var newElement = new Node(); Elements.Add(newElement);
		
		newElement.Index = newIndex;
		newElement.Value = newValue;
		
		return newElement;
	}
}