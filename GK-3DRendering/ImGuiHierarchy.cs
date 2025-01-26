using GK_3DRendering.Engine;
using GK_3DRendering.Engine.Objects;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GK_3DRendering
{
    class ImGuiHierarchy
    {
        public GameObject? Selected;

        public ImGuiHierarchy()
        {
            Selected = null;
        }

        public void RenderImGuiHierarchy(Scene scene)
        {
            ImGui.Begin("Hierarchy");
            if (ImGui.TreeNodeEx("Scene", ImGuiTreeNodeFlags.OpenOnArrow | ImGuiTreeNodeFlags.Framed | ImGuiTreeNodeFlags.DefaultOpen | ImGuiTreeNodeFlags.SpanFullWidth | ImGuiTreeNodeFlags.NoTreePushOnOpen))
            {

                uint TreeLineColor = ImGui.GetColorU32(ImGuiCol.Text);

                ImDrawListPtr drawList = ImGui.GetWindowDrawList();

                foreach (var obj in scene.GetGameObjects())
                {
                    if (obj.Parent == null)
                    {
                        ImRect imRect = ImGuiHierarchyObject(obj, drawList, TreeLineColor);
                    }
                }

            }
            if (ImGui.IsWindowHovered() && ImGui.IsMouseClicked(ImGuiMouseButton.Left) && !ImGui.IsAnyItemHovered())
            {
                Selected = null;
            }
            ImGui.End();

            
        }



        public ImRect ImGuiHierarchyObject(GameObject obj, ImDrawListPtr drawList, uint color)
        {
            ImRect imRect = new ImRect();

            float SmallOffsetX = 11.0f;
            float HorizontalOffset = 8.0f;

           
            ImGuiTreeNodeFlags flags =  ImGuiTreeNodeFlags.FramePadding | ImGuiTreeNodeFlags.SpanAvailWidth;
            if (obj == Selected)
                flags |= ImGuiTreeNodeFlags.Selected;
           
            if (obj.Childern.Count == 0)
            {
                flags |= ImGuiTreeNodeFlags.Leaf | ImGuiTreeNodeFlags.Bullet;
            }
            else
            {
                flags |= ImGuiTreeNodeFlags.OpenOnArrow;
               
            }
            var verticalLineStart = ImGui.GetCursorScreenPos();
            verticalLineStart.X += SmallOffsetX;
            var verticalLineEnd = verticalLineStart;
            bool opened = ImGui.TreeNodeEx(obj.Name, flags);
            if (ImGui.IsItemClicked())
                Selected = obj;
            if (opened)
            {
                imRect = new ImRect(ImGui.GetItemRectMin(), ImGui.GetItemRectMax());
                if (obj.Childern.Count != 0)
                {
                    verticalLineStart.Y = ImGui.GetCursorScreenPos().Y;

                    foreach (var child in obj.Childern)
                    {
                        ImRect childRect = ImGuiHierarchyObject(child, drawList, color);
                        float midpoint = (childRect.Min.Y + childRect.Max.Y) / 2f;
                        drawList.AddLine(new System.Numerics.Vector2(verticalLineStart.X, midpoint),
                                         new System.Numerics.Vector2(verticalLineStart.X + HorizontalOffset, midpoint), color);
                        verticalLineEnd.Y = midpoint;
                    }
                    drawList.AddLine(verticalLineStart, verticalLineEnd, color);
                }
                ImGui.TreePop();
            }
            return imRect;
        }
    }
}
