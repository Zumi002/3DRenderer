using GK_3DRendering.Engine.Objects;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using GK_3DRendering.Engine.Components;
using GK_3DRendering.Engine.Components.Renderers;
using GK_3DRendering.Engine;
using System.Runtime.CompilerServices;
using System.Reflection;
using GK_3DRendering.Engine.LightSource;
using GK_3DRendering.Engine.Cameras;
using GK_3DRendering.Engine.Components.Controllers;

namespace GK_3DRendering
{
    class ImGuiInspector
    {

        public void RenderImGuiInspector(GameObject? obj)
        {
            ImGui.Begin("Inspector");
            if (obj == null)
            {
                ImGui.End();
                return;
            }

            ImGui.Text(obj.Name);

            DrawObjectProperties(obj.Transform);
            if (DrawObjectProperties(obj as ProjectionCamera))
            {
                ImGui.TreePush("0");
                if(ImGui.Button("Set as Active camera"))
                {
                    obj.Scene.SetActiveCamera((Camera)obj);
                }
                ImGui.Separator();
                DrawObjectProperties(((ProjectionCamera)obj).Controller as FPPController, false);
                DrawObjectProperties(((ProjectionCamera)obj).Controller as FollowerController, false);
                ImGui.TreePop();
            }


            if (DrawObjectProperties(obj.Renderer as MeshRenderer))
            {

                ImGui.TreePush("0");

                DrawObjectProperties(((MeshRenderer)obj.Renderer).Material, false);

                ImGui.TreePop();

            }
            if(DrawObjectProperties(obj.Renderer as FlagRenderer))
            {
                ImGui.TreePush("0");

                DrawObjectProperties(((FlagRenderer)obj.Renderer).Material, false);

                ImGui.TreePop();
            }
            if(DrawObjectProperties(obj as DirectionalLightSource))
            {
                
                if (ImGui.Button(((DirectionalLightSource)obj).animate?"Stop Animation":"Animate"))
                {
                    ((DirectionalLightSource)obj).FlipAnimate();
                }
            }
            DrawObjectProperties(obj as PointLightSource);
            DrawObjectProperties(obj as SpotLightSource);


            ImGui.End();
        }

        public bool DrawObjectProperties<T>(T obj, bool sep = true) where T : class?
        {
            if (obj == null)
                return false;

            var members = typeof(T).GetMembers(BindingFlags.Public | BindingFlags.Instance)
                         .Where(m => m is PropertyInfo || m is FieldInfo)
                         .Where(m => !m.GetCustomAttributes(typeof(InspectorIgnoreAttribute), false).Any());
            if (members.Any())
            {
                if (sep)
                    ImGui.SeparatorText("");
                if (ImGui.CollapsingHeader(typeof(T).Name, ImGuiTreeNodeFlags.Framed | ImGuiTreeNodeFlags.DefaultOpen))
                {


                    foreach (var member in members)
                    {
                        switch (member)
                        {

                            case PropertyInfo prop:
                                DrawPropertyValue(prop.Name, prop.PropertyType,
                                    () => prop.GetValue(obj),
                                    (value) => prop.SetValue(obj, value),
                                    member);
                                break;

                            case FieldInfo field:
                                DrawPropertyValue(field.Name, field.FieldType,
                                    () => field.GetValue(obj),
                                    (value) => field.SetValue(obj, value),
                                    member);
                                break;
                        }
                    }
                    return true;
                }
            }
            return false;
        }

        private void DrawPropertyValue(string label, Type type, Func<object> getter, Action<object> setter, MemberInfo memberInfo)
        {
            var method = typeof(ImGuiInspector).GetMethod(nameof(DrawInputProperty))
                .MakeGenericMethod(type);


            var value = getter();
            var args = new[] { label, value };

            if (memberInfo.GetCustomAttribute<InspectorSliderAttribute<int>>() is var attr && attr != null)
            {
                method = typeof(ImGuiInspector).GetMethod(nameof(DrawSliderProperty))
                .MakeGenericMethod(type);

                args = new[] { label, value, attr.Min, attr.Max };
            }
            if (memberInfo.GetCustomAttribute<InspectorSliderAttribute<float>>() is var attr2 && attr2 != null)
            {
                method = typeof(ImGuiInspector).GetMethod(nameof(DrawSliderProperty))
                .MakeGenericMethod(type);

                args = new[] { label, value, attr2.Min, attr2.Max };
            }
            if (memberInfo.GetCustomAttribute<InspectorColorAttribute>() is var attr3 && attr3 != null)
            {
                method = typeof(ImGuiInspector).GetMethod(nameof(DrawColorProperty))
                .MakeGenericMethod(type);

                args = new[] { label, value };
            }

            if ((bool)method.Invoke(this, args))
            {
                setter(args[1]);
            }
        }

        public bool DrawInputProperty<T>(string label, ref T value)
        {
            var type = typeof(T);

            if (type == typeof(float))
            {
                return ImGui.InputFloat(label, ref Unsafe.As<T, float>(ref value));
            }

            if (type == typeof(Vector3))
            {
                return ImGui.InputFloat3(label, ref Unsafe.As<T, Vector3>(ref value));
            }

            if (type == typeof(OpenTK.Mathematics.Vector3))
            {
                Vector3 vector3 = Vector3Converter.ToSystemNumerics(Unsafe.As<T, OpenTK.Mathematics.Vector3>(ref value));
                if (ImGui.InputFloat3(label, ref vector3))
                {
                    Unsafe.As<T, OpenTK.Mathematics.Vector3>(ref value) = Vector3Converter.ToOpenTK(vector3);
                    return true;
                }
            }

            if (type == typeof(int))
            {
                return ImGui.InputInt(label, ref Unsafe.As<T, int>(ref value));
            }
            return false;
        }

        public bool DrawSliderProperty<T>(string label, ref T value, T min, T max)
        {
            var type = typeof(T);

            if (type == typeof(float))
            {
                return ImGui.SliderFloat(label, ref Unsafe.As<T, float>(ref value), Unsafe.As<T, float>(ref min), Unsafe.As<T, float>(ref max));
            }

            if (type == typeof(int))
            {
                return ImGui.SliderInt(label, ref Unsafe.As<T, int>(ref value), Unsafe.As<T, int>(ref min), Unsafe.As<T, int>(ref max));
            }

            return false;
        }

        public bool DrawColorProperty<T>(string label, ref T value)
        {
            var type = typeof(T);

            if (type == typeof(OpenTK.Mathematics.Vector3))
            {
                Vector3 vector3 = Vector3Converter.ToSystemNumerics(Unsafe.As<T, OpenTK.Mathematics.Vector3>(ref value));
                if (ImGui.ColorEdit3(label, ref vector3))
                {
                    Unsafe.As<T, OpenTK.Mathematics.Vector3>(ref value) = Vector3Converter.ToOpenTK(vector3);
                    return true;
                }
            }

            return false;
        }

    }
}
