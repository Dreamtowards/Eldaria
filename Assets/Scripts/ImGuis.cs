using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ImGuiNET;
using System;
using System.IO;

public class ImGuis : MonoBehaviour
{
    public static float Luminance(Vector3 rgb)
    {
        return (rgb.x * 0.299f + rgb.y * 0.587f + rgb.z * 0.114f);
    }

    void Start()
    {
       
        //ImGuiIOPtr io = ImGui.GetIO();


        //ImFontPtr fontPtr = io.Fonts.AddFontFromFileTTF(Path.Combine(Application.streamingAssetsPath, "fonts/menlo.ttf"), 14);
        //fontPtr.ConfigData.OversampleH = 3;


        ImGuiStylePtr styl = ImGui.GetStyle();
        styl.GrabMinSize = 7;
        styl.FrameBorderSize = 0;
        styl.WindowMenuButtonPosition = ImGuiDir.Right;
        styl.DisplaySafeAreaPadding = new Vector2(0, 0);
        styl.FramePadding = new Vector2(8, 2);
        styl.ScrollbarSize = 10;
        styl.ScrollbarRounding = 2;
        styl.TabRounding = 2;

        for (int i = 0; i < (int)ImGuiCol.COUNT; ++i)
        {
            ref Vector4 c = ref styl.Colors[i];
            float f = Mathf.Max(Luminance(new(c.x, c.y, c.z)), 0.06f);
            c = new Vector4(f,f,f,c.w);
        }

        var Col = styl.Colors;
        Col[(int)ImGuiCol.HeaderHovered] = new(0.051f, 0.431f, 0.992f, 1.000f);
        Col[(int)ImGuiCol.HeaderActive] = new(0.071f, 0.388f, 0.853f, 1.000f);
        Col[(int)ImGuiCol.Header] = new(0.106f, 0.298f, 0.789f, 1.000f);  // also for Selectable.

        Col[(int)ImGuiCol.TitleBg] = new(0.082f, 0.082f, 0.082f, 0.800f);
        Col[(int)ImGuiCol.TitleBgActive] = new(0.082f, 0.082f, 0.082f, 1.000f);

        Col[(int)ImGuiCol.Tab] =
        Col[(int)ImGuiCol.TabUnfocused] = new(0, 0, 0, 0);

        Col[(int)ImGuiCol.TabActive] = new(0.26f, 0.26f, 0.26f, 1.000f);
        Col[(int)ImGuiCol.TabUnfocusedActive] =
        Col[(int)ImGuiCol.WindowBg] = new(0.176f, 0.176f, 0.176f, 1.000f); //new(0.19f, 0.19f, 0.19f, 1.0f);  // new(0.212f, 0.212f, 0.212f, 1.000f);
        Col[(int)ImGuiCol.TitleBg] =
        Col[(int)ImGuiCol.TitleBgActive] = new(0.128f, 0.128f, 0.128f, 0.940f);
    }

    void OnLayout()
    {
        ImGui.ShowDemoWindow();
    }


    void OnEnable()
    {

        ImGuiUn.Layout += OnLayout;
    }
    void OnDisable()
    {
        ImGuiUn.Layout -= OnLayout;
    }

}
