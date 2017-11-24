using System.Collections;
using System.Drawing;

namespace PmlChecker.PmlChecker
{
    public class PoistionNode
    {
        public float x;
        public float y;
        public SizeF s;
        public PoistionNode()
        {
            x = -1;
            y = -1;
            s = new SizeF(0, 0);
        }
    }

    public class TreeNode
    {
        public string text;
        public int childCount;
        public ArrayList children;
        public PoistionNode pos;
        public int backChild;
        public int id;
        public Image nodeImage;

        public TreeNode(int id)
        {
            text = null;
            childCount = 0;
            children = new ArrayList();
            this.id = id;
            backChild = -1;
            nodeImage = null;
            pos = new PoistionNode();
        }

    }

}
