using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace PmlChecker.PmlChecker
{
    public class CFG
    {
        // 默认字体
        private static readonly Font NodeTextFont = new Font("Verdana", 8f, FontStyle.Bold);

        // 节点大小
        private static SizeF _minimumNodeSize = new SizeF(32, 28);

        // 间隙大小
        private static Size _nodeGapping = new Size(32, 32);

        // 笔刷
        private static readonly Dictionary<string, Pen> Pens = new Dictionary<string, Pen>();

        private int _mostLeft;
        private int _mostRight;
        private int _mostDown;

        public TreeNode CfgTreeNode { get; set; }

        public CFG(TreeNode astTreeNode)
        {
            CfgTreeNode = astTreeNode;
        }

        // 创建结点
        private static Bitmap CreateNodeImage(Size size, string text, Font font)
        {
            var img = new Bitmap(size.Width, size.Height);
            using (var g = Graphics.FromImage(img))
            {
                // 画框
                g.SmoothingMode = SmoothingMode.HighQuality;
                var rcl = new Rectangle(1, 1, img.Width - 2, img.Height - 2);
                g.FillRectangle(Brushes.White, rcl);
                var linearBrush = new LinearGradientBrush(rcl, Color.LightBlue, Color.BlueViolet,
                    LinearGradientMode.ForwardDiagonal);
                g.DrawEllipse(NodeBorderPen, rcl);
                g.FillEllipse(linearBrush, rcl);
                linearBrush.Dispose();

                var sizeText = g.MeasureString(text, font);
                // 框内填字
                g.DrawString(text, font, Brushes.Black, Math.Max(0, (size.Width - sizeText.Width) / 2),
                    Math.Max(0, (size.Height - sizeText.Height) / 2));
            }
            return img;
        }

        // 连接处笔刷
        private static Pen ConnectionPen
        {
            get
            {
                const string penName = "ConnectionPen";
                if (!Pens.ContainsKey(penName))
                {
                    Pens.Add(penName,
                        new Pen(Brushes.Black, 1) {EndCap = LineCap.ArrowAnchor, StartCap = LineCap.Round});
                }
                return Pens[penName];
            }
        }

        // 节点边缘笔刷
        private static Pen NodeBorderPen
        {
            get
            {
                const string penName = "NodeBorderPen";
                if (!Pens.ContainsKey(penName))
                {
                    Pens.Add(penName, new Pen(Color.Silver, 1));
                }
                return Pens[penName];
            }
        }

        public Image Draw()
        {
            HandleNodesSize(CfgTreeNode);
            AssignAllNodesPos(CfgTreeNode, 0, 0);
            var image = DrawTree();
            return image;
        }

        // 处理所有结点大小
        private static void HandleNodesSize(TreeNode astTreeNode)
        {
            var nodeText = astTreeNode.text;
            var nodeSize = TextMeasurer.MeasureString("*" + nodeText + "*", NodeTextFont);
            nodeSize.Width = Math.Max(_minimumNodeSize.Width, nodeSize.Width);
            nodeSize.Height = Math.Max(_minimumNodeSize.Height, nodeSize.Height);
            astTreeNode.pos.s = nodeSize;
            var nodeImage = CreateNodeImage(nodeSize.ToSize(), nodeText, NodeTextFont);
            astTreeNode.nodeImage = nodeImage;
            int i;
            for (i = 0; i < astTreeNode.childCount; i++)
            {
                HandleNodesSize((TreeNode) astTreeNode.children[i]);
            }
        }

        // 分配所有子结点以根节点为（0,0）的相对坐标
        private void AssignAllNodesPos(TreeNode astTreeNode, int center, int height)
        {
            astTreeNode.pos.x = center;
            height += astTreeNode.nodeImage.Height;
            astTreeNode.pos.y = height;

            if (astTreeNode.childCount == 0)
            {
                // 记录叶子结点边界
                if (center - astTreeNode.nodeImage.Size.Width / 2 < _mostLeft)
                    _mostLeft = center - astTreeNode.nodeImage.Size.Width / 2;
                if (center + astTreeNode.nodeImage.Size.Width - astTreeNode.nodeImage.Size.Width / 2 > _mostRight)
                    _mostRight = center + astTreeNode.nodeImage.Size.Width - astTreeNode.nodeImage.Size.Width / 2;
                if (height + _nodeGapping.Height > _mostDown)
                    _mostDown = height + _nodeGapping.Height;
                return;
            }

            var childCentres = new int[astTreeNode.childCount];
            var childSizes = new Size[astTreeNode.childCount];

            int i;
            var fromLeft = 0;
            for (i = 0; i < astTreeNode.childCount; i++)
            {
                childSizes[i] = ((TreeNode) astTreeNode.children[i]).nodeImage.Size;
                childCentres[i] = fromLeft + childSizes[i].Width / 2;
                fromLeft = childCentres[i] + _nodeGapping.Width
                           + childSizes[i].Width - childSizes[i].Width / 2;
            }

            // 计算坐标
            fromLeft -= _nodeGapping.Width;
            var half = fromLeft / 2;
            var shift = center - half;

            // 记录边界
            if (shift < _mostLeft)
                _mostLeft = shift;
            if (shift + fromLeft > _mostRight)
                _mostRight = shift + fromLeft;
            if (height + _nodeGapping.Height > _mostDown)
                _mostDown = height + _nodeGapping.Height;

            // 递归计算子结点坐标
            for (i = 0; i < astTreeNode.childCount; i++)
                AssignAllNodesPos((TreeNode) astTreeNode.children[i], childCentres[i] + shift,
                    height + _nodeGapping.Height);
        }

        private Image DrawTree()
        {
            //画整个图
            var totalSize = new Size
            {
                Width = _mostRight - _mostLeft,
                Height = _mostDown
            };

            var result = new Bitmap(totalSize.Width, totalSize.Height);
            var g = Graphics.FromImage(result);
            g.SmoothingMode = SmoothingMode.HighQuality;
            g.FillRectangle(Brushes.White, new Rectangle(new Point(0, 0), totalSize));
            DrawEveryNode(CfgTreeNode, g);
            g.Dispose();
            return result;
        }

        private void DrawEveryNode(TreeNode astTreeNode, Graphics g)
        {
            int i;
            g.DrawImage(astTreeNode.nodeImage, astTreeNode.pos.x - astTreeNode.nodeImage.Width / 2 - _mostLeft,
                astTreeNode.pos.y);
            for (i = 0; i < astTreeNode.childCount; i++)
            {
                // 画子结点
                DrawEveryNode((TreeNode) astTreeNode.children[i], g);

                // 画子结点和父结点的连线
                var x1 = astTreeNode.pos.x - _mostLeft;
                var y1 = astTreeNode.pos.y + astTreeNode.nodeImage.Height;
                var x2 = ((TreeNode) (astTreeNode.children[i])).pos.x - _mostLeft;
                var y2 = ((TreeNode) (astTreeNode.children[i])).pos.y;
                var h = y2 - y1;
                var w = x1 - x2;
                var points1 = new List<PointF>
                {
                    new PointF(x1, y1),
                    new PointF(x1 - w / 6, y1 + h / 3.5f),
                    new PointF(x2 + w / 6, y2 - h / 3.5f),
                    new PointF(x2, y2),
                };
                g.DrawCurve(ConnectionPen, points1.ToArray(), 0.5f);
            }

            // 画回路的连线
            if (astTreeNode.backChild >= 0)
            {
                var backTreeNode = (TreeNode) Form1.nodes[astTreeNode.backChild];

                var x1 = astTreeNode.pos.x - _mostLeft;
                var y1 = astTreeNode.pos.y;

                var x2 = backTreeNode.pos.x - _mostLeft;
                var y2 = backTreeNode.pos.y;

                // 上面注释的是在此处添加一些自定义画返回线的规则，可以规避开其他的线
                // 目前采用的是简单的画在两个结点右侧
                x2 += backTreeNode.nodeImage.Width - backTreeNode.nodeImage.Width / 2;
                x1 += astTreeNode.nodeImage.Width - astTreeNode.nodeImage.Width / 2;
                y1 += astTreeNode.nodeImage.Height / 2;
                y2 += backTreeNode.nodeImage.Height / 2;

                var h = y2 - y1;
                var w = x1 - x2;
                var points1 = new List<PointF>
                {
                    new PointF(x1, y1),
                    new PointF(x2, y2),
                };
                g.DrawCurve(ConnectionPen, points1.ToArray(), 0.5f);
            }
        }
    }
}