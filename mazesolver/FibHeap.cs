using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mazesolver
{
    public class FibHeap
    {
        HeapNode minnode;
        public int count = 0;
        int maxdegree = 0;

        public bool isempty()
        {
            return count == 0;
        }
        public void insert(HeapNode node)
        {
            count++;
            this._insertnode(node);

        }

        public void _insertnode(HeapNode node)
        {
            if (minnode == null)
            {
                minnode = node;
            }
            else
            {
                minnode.insert(node);
                if (node.key < minnode.key)
                {
                    minnode = node;
                }
            }
        }

        public HeapNode minimum()
        {
            if (minnode == null)
            {
                throw new Exception("Cannot return minimum of empty heap");
            }
            return minnode;
        }

        public void merge(FibHeap heap)
        {
            this.minnode.insert(heap.minnode);
            if (minnode == null || (heap.minnode != null && heap.minnode.key < minnode.key))
            {
                minnode = heap.minnode;
            }
            count += heap.count;
        }

        public void removeminimum()
        {
            if (minnode == null)
            {
                throw new Exception("Cannot remove from empty heap");
            }
            count--;
            if (minnode.child != null)
            {
                HeapNode c = minnode.child;
                while (true)
                {
                    c.parent = null;
                    c = c.next;
                    if (c == minnode.child)
                    {
                        break;
                    }
                }
                minnode.child = null;
                minnode.insert(c);
            }
            if (minnode.next == minnode)
            {
                if (count != 0)
                {
                    throw new Exception("Heap error: Expected 0 keys. Count is " + count);
                }
                minnode = null;
                return;
            }
            int logsize = 100;
            HeapNode[] degreeroots = new HeapNode[logsize];
            maxdegree = 0;
            HeapNode currentpointer = minnode.next;
            while (true)
            {
                int currentdegree = currentpointer.degree;
                HeapNode current = currentpointer;
                currentpointer = currentpointer.next;
                while (degreeroots[currentdegree] != null)
                {
                    HeapNode other = degreeroots[currentdegree];
                    if (current.key > other.key)
                    {
                        HeapNode temp = other;
                        other = current;
                        current = temp;
                    }
                    other.remove();
                    current.addchild(other);
                    degreeroots[currentdegree] = null;
                    currentdegree++;
                }
                degreeroots[currentdegree] = current;
                if (currentpointer == minnode)
                {
                    break;
                }
            }
            minnode = null;
            int newmaxdegree = 0;

            for (int i = 0; i < logsize; i++)
            {
                if (degreeroots[i] != null)
                {
                    degreeroots[i].next = degreeroots[i].previous = degreeroots[i];
                    this._insertnode(degreeroots[i]);
                    if (i > newmaxdegree)
                    {
                        newmaxdegree = i;
                    }
                }
            }
            maxdegree = newmaxdegree;
        }

        public void decreasekey(HeapNode node, int newkey)
        {
            if (newkey > node.key)
            {
                throw new Exception("Cannot decrease key to a greater value");
            }
            else if (newkey == node.key)
            {
                return;
            }
            node.key = newkey;
            HeapNode parent = node.parent;
            if (parent == null)
            {
                if (newkey < minnode.key)
                {
                    minnode = node;
                }
                return;
            }
            else if (parent.key <= newkey)
            {
                return;
            }
            while (true)
            {
                parent.removechild(node);
                _insertnode(node);
                if (parent.parent == null)
                {
                    break;
                }
                else if (parent.mark == false)
                {
                    parent.mark = true;
                    break;
                }
                else
                {
                    node = parent;
                    parent = parent.parent;
                    continue;
                }
            }
        }
    }
}
