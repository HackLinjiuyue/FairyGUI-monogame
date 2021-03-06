﻿using System;
using System.Collections.Generic;
using System.Drawing;
using FairyGUI.Utils;
using Microsoft.Xna.Framework;
using Color = Microsoft.Xna.Framework.Color;

namespace FairyGUI.Test.Scenes
{
    public class BasicsScene : DemoScene
    {
        private GComponent _mainView;
        private GObject _backBtn;
        private GComponent _demoContainer;
        private Controller _viewController;
        private Dictionary<string, GComponent> _demoObjects;

        public BasicsScene()
        {
            UIConfig.verticalScrollBar = "ui://Basics/ScrollBar_VT";
            UIConfig.horizontalScrollBar = "ui://Basics/ScrollBar_HZ";
            UIConfig.tooltipsWin = "ui://Basics/WindowFrame";
            UIConfig.popupMenu = "ui://Basics/PopupMenu";
            UIPackage.AddPackage("UI/Basics");

            _mainView = UIPackage.CreateObject("Basics", "Main").asCom;
            _mainView.MakeFullScreen();
            _mainView.AddRelation(GRoot.inst, RelationType.Size);
            AddChild(_mainView);

            _backBtn = _mainView.GetChild("btn_Back");
            _backBtn.visible = false;
            _backBtn.onClick.Add(onClickBack);

            _demoContainer = _mainView.GetChild("container").asCom;
            _viewController = _mainView.GetController("c1");

            _demoObjects = new Dictionary<string, GComponent>();

            int cnt = _mainView.numChildren;
            for (int i = 0; i < cnt; i++)
            {
                GObject obj = _mainView.GetChildAt(i);
                if (obj.group != null && obj.group.name == "btns")
                    obj.onClick.Add(runDemo);
            }
        }

        public override void Dispose()
        {
            base.Dispose();

            UIConfig.verticalScrollBar = "";
            UIConfig.horizontalScrollBar = "";
        }

        private void runDemo(EventContext context)
        {
            string type = ((GObject)(context.sender)).name.Substring(4);
            GComponent obj;
            if (!_demoObjects.TryGetValue(type, out obj))
            {
                obj = UIPackage.CreateObject("Basics", "Demo_" + type).asCom;
                _demoObjects[type] = obj;
            }

            _demoContainer.RemoveChildren();
            _demoContainer.AddChild(obj);
            _viewController.selectedIndex = 1;
            _backBtn.visible = true;

            switch (type)
            {
                case "Button":
                    PlayButton();
                    break;

                case "Text":
                    PlayText();
                    break;

                case "Grid":
                    PlayGrid();
                    break;

                case "Transition":
                    PlayTransition();
                    break;

                case "Window":
                    PlayWindow();
                    break;

                case "Popup":
                    PlayPopup();
                    break;

                case "Drag&Drop":
                    PlayDragDrop();
                    break;

                case "Depth":
                    PlayDepth();
                    break;

                case "ProgressBar":
                    PlayProgressBar();
                    break;
            }
        }

        private void onClickBack()
        {
            _viewController.selectedIndex = 0;
            _backBtn.visible = false;
        }

        //-----------------------------
        private void PlayButton()
        {
            GComponent obj = _demoObjects["Button"];
            obj.GetChild("n34").onClick.Add(() => { Log.Info("click button"); });
        }

        //------------------------------
        private void PlayText()
        {
            GComponent obj = _demoObjects["Text"];
            obj.GetChild("n12").asRichTextField.onClickLink.Add((EventContext context) =>
            {
                GRichTextField t = context.sender as GRichTextField;
                t.text = "[img]ui://Basics/pet[/img][color=#FF0000]You click the link[/color]：" + context.data;
            });
            obj.GetChild("n25").onClick.Add(() =>
            {
                obj.GetChild("n24").text = obj.GetChild("n22").text;
            });
        }

        //------------------------------
        private void PlayGrid()
        {
           
        }

        //------------------------------
        private void PlayTransition()
        {
            GComponent obj = _demoObjects["Transition"];
            obj.GetChild("n2").asCom.GetTransition("t0").Play(int.MaxValue, 0, null);
            obj.GetChild("n3").asCom.GetTransition("peng").Play(int.MaxValue, 0, null);

            obj.onAddedToStage.Add(() =>
            {
                obj.GetChild("n2").asCom.GetTransition("t0").Stop();
                obj.GetChild("n3").asCom.GetTransition("peng").Stop();
            });
        }

        //------------------------------
        private Window _winA;
        private Window _winB;
        private void PlayWindow()
        {
            GComponent obj = _demoObjects["Window"];
            obj.GetChild("n0").onClick.Add(() =>
            {
                if (_winA == null)
                    _winA = new Window1();
                _winA.Show();
            });

            obj.GetChild("n1").onClick.Add(() =>
            {
                if (_winB == null)
                    _winB = new Window2();
                _winB.Show();
            });
        }

        //------------------------------
        private PopupMenu _pm;
        private GComponent _popupCom;
        private void PlayPopup()
        {
            if (_pm == null)
            {
                _pm = new PopupMenu();
                _pm.AddItem("Item 1", __clickMenu);
                _pm.AddItem("Item 2", __clickMenu);
                _pm.AddItem("Item 3", __clickMenu);
                _pm.AddItem("Item 4", __clickMenu);
            }

            if (_popupCom == null)
            {
                _popupCom = UIPackage.CreateObject("Basics", "Component12").asCom;
                _popupCom.Center();
            }
            GComponent obj = _demoObjects["Popup"];
            obj.GetChild("n0").onClick.Add((EventContext context) =>
            {
                _pm.Show((GObject)context.sender, true);
            });

            obj.GetChild("n1").onClick.Add(() =>
            {
                GRoot.inst.ShowPopup(_popupCom);
            });


            obj.onRightClick.Add(() =>
            {
                _pm.Show();
            });
        }

        private void __clickMenu(EventContext context)
        {
            GObject itemObject = (GObject)context.data;
            Log.Info("click " + itemObject.text);
        }

        //------------------------------
        Vector2 startPos;
        private void PlayDepth()
        {
            GComponent obj = _demoObjects["Depth"];
            GComponent testContainer = obj.GetChild("n22").asCom;
            GObject fixedObj = testContainer.GetChild("n0");
            fixedObj.sortingOrder = 100;
            fixedObj.draggable = true;

            int numChildren = testContainer.numChildren;
            int i = 0;
            while (i < numChildren)
            {
                GObject child = testContainer.GetChildAt(i);
                if (child != fixedObj)
                {
                    testContainer.RemoveChildAt(i);
                    numChildren--;
                }
                else
                    i++;
            }
            startPos = new Vector2(fixedObj.x, fixedObj.y);

            obj.GetChild("btn0").onClick.Add(() =>
            {
                GGraph graph = new GGraph();
                startPos.X += 10;
                startPos.Y += 10;
                graph.xy = startPos;
                graph.DrawRect(150, 150, 1, Color.Black, Color.Red);
                obj.GetChild("n22").asCom.AddChild(graph);
            });

            obj.GetChild("btn1").onClick.Add(() =>
            {
                GGraph graph = new GGraph();
                startPos.X += 10;
                startPos.Y += 10;
                graph.xy = startPos;
                graph.DrawRect(150, 150, 1, Color.Black, Color.Green);
                graph.sortingOrder = 200;
                obj.GetChild("n22").asCom.AddChild(graph);
            });
        }

        //------------------------------
        private void PlayDragDrop()
        {
            GComponent obj = _demoObjects["Drag&Drop"];
            obj.GetChild("a").draggable = true;

            GButton b = obj.GetChild("b").asButton;
            b.draggable = true;
            b.onDragStart.Add((EventContext context) =>
            {
                //Cancel the original dragging, and start a new one with a agent.
                context.PreventDefault();

                DragDropManager.inst.StartDrag(b, b.icon, b.icon, (int)context.data);
            });

            GButton c = obj.GetChild("c").asButton;
            c.icon = null;
            c.onDrop.Add((EventContext context) =>
            {
                c.icon = (string)context.data;
            });

            GObject bounds = obj.GetChild("n7");
            RectangleF rect = bounds.TransformRect(new RectangleF(0, 0, bounds.width, bounds.height), GRoot.inst);

            //---!!Because at this time the container is on the right side of the stage and beginning to move to left(transition), so we need to caculate the final position
            rect.X -= obj.parent.x;
            //----

            GButton d = obj.GetChild("d").asButton;
            d.draggable = true;
            d.dragBounds = rect;
        }

        //------------------------------
        private void PlayProgressBar()
        {
            GComponent obj = _demoObjects["ProgressBar"];
            Timers.inst.Add(0.001f, 0, __playProgress);
            obj.onRemovedFromStage.Add(() => { Timers.inst.Remove(__playProgress); });
        }

        void __playProgress(object param)
        {
            GComponent obj = _demoObjects["ProgressBar"];
            int cnt = obj.numChildren;
            for (int i = 0; i < cnt; i++)
            {
                GProgressBar child = obj.GetChildAt(i) as GProgressBar;
                if (child != null)
                {

                    child.value += 1;
                    if (child.value > child.max)
                        child.value = 0;
                }
            }
        }
    }
}