using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mazesolver
{
    public class HeapNode
    {
        public int key;
        public Node value;
        public HeapNode parent;
        public HeapNode child;
        public HeapNode previous;
        public HeapNode next;
        public bool mark = false;
        public int degree = 0;

        public HeapNode(int key, Node value)
        {
            this.key = key;
            this.value = value;
            previous = next = this;
        }

        public bool issingle()
        {
            return (this == next);
        }
        public void insert(HeapNode node)
        {
            if (node != null)
            {
                next.previous = node.previous;
                node.previous.next = next;
                next = node;
                node.previous = this;
            }
        }
        public void remove()
        {
            previous.next = next;
            next.previous = previous;
            next = previous = this;
        }
        public void addchild(HeapNode node)
        {
            if (child == null)
            {
                child = node;
            }
            else
            {
                child.insert(node);
            }
            node.parent = this;
            node.mark = false;
            degree++;
        }
        public void removechild(HeapNode node)
        {
            if (node.parent != this)
            {
                throw new Exception("Cannot remove child from node that is not it's parent");
            }
            if (node.issingle())
            {
                if (child != node)
                {
                    throw new Exception("Cannot remove node that is not a child");
                }
                child = null;
            }
            else
            {
                if (child == node)
                {
                    child = node.next;
                }
                node.remove();
            }
            node.parent = null;
            node.mark = false;
            degree--;
        }
    }
}
